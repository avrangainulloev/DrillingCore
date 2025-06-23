using DrillingCore.Application.Interfaces;
using DrillingCore.Core.Entities;
using DrillingCore.Infrastructure.Persistence;
using DrillingCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Service
{
    public class FormDeliveryService : IFormDeliveryService
    {
        private readonly IFormDeliveryRepository _repository;
        private readonly IEmailSender _emailSender;
        private readonly DrillingCoreDbContext _context;
        private readonly FormPdfBuilder _formPdfBuilder;
        private readonly IBackgroundTaskQueue _taskQueue;

        public FormDeliveryService(IFormDeliveryRepository repository, IEmailSender emailSender, DrillingCoreDbContext context , FormPdfBuilder formPdfBuilder, IBackgroundTaskQueue taskQueue)
        {
            _repository = repository;
            _emailSender = emailSender;
            _context = context;
            _formPdfBuilder = formPdfBuilder;
            _taskQueue = taskQueue;
        }

        public async Task TrySendOnFormCompleted(int formId, FormDeliveryRule rule, CancellationToken ct)
        {
            var form = await _context.ProjectForms
                .Include(p => p.Project)
                .Include(p => p.FormType)
                .Include(p => p.FormParticipants).ThenInclude(p => p.Participant).ThenInclude(u => u.User)
                .Include(p => p.FormSignatures)
                .Include(p => p.Creator)
                .Include(p => p.FormChecklistResponses).ThenInclude(r => r.ChecklistItem)
                .FirstOrDefaultAsync(p => p.Id == formId, ct);

            if (form == null) return;

          
            if (rule == null || rule.Condition != FormDeliveryCondition.AfterEachFormCompleted)
                return;

            var recipients = rule.Recipients.Select(r => r.Email).ToList();
            if (!recipients.Any()) return;

            byte[] pdfBytes;
            string fileName;
            string subject = string.IsNullOrWhiteSpace(form.UnitNumber)
                 ? $"Form Completed: {form.FormType?.Name} (No Unit Number) on {form.DateFilled:yyyy-MM-dd}."
                 : $"Form Completed: {form.FormType?.Name} for {form.UnitNumber} on {form.DateFilled:yyyy-MM-dd}.";
            string body = $"Attached is the completed form for {form.DateFilled:yyyy-MM-dd}.";

            switch (form.FormType?.Name)
            {
                case "FLHA":
                    var flhaForm = await _context.FLHAForms
                        .FirstOrDefaultAsync(f => f.ProjectFormId == form.Id, ct);
                    if (flhaForm == null) return;

                    var hazards = await _context.FLHAFormHazards
                        .Where(h => h.FLHAFormId == flhaForm.ProjectFormId)
                        .ToListAsync(ct);

                    pdfBytes = _formPdfBuilder.BuildFlhaPdf(form, flhaForm, hazards);
                    fileName = $"FLHA_{form.DateFilled:yyyyMMdd}_{form.Project?.Name}.pdf";
                    break;

                case "Drilling":
                    var drillingForm = await _context.DrillingForms
                        .FirstOrDefaultAsync(x => x.ProjectFormId == form.Id, ct);

                    if (drillingForm == null)
                        return;

                    pdfBytes = _formPdfBuilder.BuildDrillingFormPdf(form, drillingForm);
                    fileName = $"Drilling_{form.DateFilled:yyyyMMdd}_{form.Project?.Name}_{form.CrewName}.pdf";
                    break;

                default:
                    var checklistItems = await _context.ChecklistItems
                        .Where(x => x.FormTypeId == form.FormTypeId)
                        .ToListAsync(ct);

                    pdfBytes = _formPdfBuilder.BuildDrillInspectionPdf(form, checklistItems);
                    string unitPart = string.IsNullOrWhiteSpace(form.UnitNumber) ? "NoUnit" : form.UnitNumber;
                    fileName = $"Form_{form.FormType?.Name}_{unitPart}_{form.DateFilled:yyyyMMdd}.pdf";
                    //fileName = $"Form_{form.FormType?.Name}_{form.DateFilled:yyyyMMdd}.pdf";
                    break;
            }

            await _emailSender.SendEmailWithAttachmentAsync(recipients, subject, body, pdfBytes, fileName);
        }



        public async Task<bool> CheckIfAllProjectParticipantsSigned(int projectId, int formTypeId, DateOnly dateFilled, CancellationToken cancellationToken)
        {
            // 1. Загружаем всех участников проекта, активных на момент dateFilled
            var activeParticipantIds = await _context.Participants
                .Where(p => p.ProjectId == projectId &&
                           (p.EndDate == null || p.EndDate >=dateFilled))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            if (!activeParticipantIds.Any())
                return false;

            // 2. Загружаем все подписи участников по формам с указанными параметрами
            var signedParticipantIds = await _context.FormSignatures
                .Where(s =>
                    s.ProjectForm.ProjectId == projectId &&
                    s.ProjectForm.FormTypeId == formTypeId &&
                    s.ProjectForm.DateFilled == dateFilled &&
                    s.ProjectForm.Status == "Completed")
                .Select(s => s.ParticipantId)
                .Distinct()
                .ToListAsync(cancellationToken);

            // 3. Проверка: все ли участники имеют подпись
            return activeParticipantIds.All(id => signedParticipantIds.Contains(id));
        }

        public async Task TrySendOnAllParticipantsSigned(
      int projectId,
      int formTypeId,
      DateOnly dateFilled,
      FormDeliveryRule rule,
      CancellationToken cancellationToken)
        {
            var formTypeName = await _context.FormTypes
                .Where(f => f.Id == formTypeId)
                .Select(f => f.Name)
                .FirstOrDefaultAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(formTypeName)) return;

            if (formTypeName.Equals("FLHA", StringComparison.OrdinalIgnoreCase))
            {
                await HandleFlhaFormDelivery(projectId, formTypeId, dateFilled, rule, cancellationToken);
            }
            else if (formTypeName.Equals("Drilling", StringComparison.OrdinalIgnoreCase))
            {
                await HandleDrillingFormDelivery(projectId, formTypeId, dateFilled, rule, cancellationToken);
            }
            else if (formTypeName.Contains("Inspection", StringComparison.OrdinalIgnoreCase) ||
                     formTypeName.Equals("ATV/UTV", StringComparison.OrdinalIgnoreCase))
            {
                await HandleDrillInspectionDelivery(projectId, formTypeId, dateFilled, rule, cancellationToken);
            }
        }
        private async Task HandleFlhaFormDelivery(int projectId, int formTypeId,DateOnly dateFilled,FormDeliveryRule rule, CancellationToken cancellationToken)
        {
            var recipients = rule.Recipients.Select(r => r.Email).ToList();
            if (!recipients.Any()) return;

            var projectForms = await _context.ProjectForms
                .Where(f =>
                    f.ProjectId == projectId &&
                    f.FormTypeId == formTypeId &&
                    f.DateFilled == dateFilled &&
                    f.Status == "Completed")
                .Include(f => f.FormParticipants).ThenInclude(p => p.Participant).ThenInclude(u => u.User)
                .Include(f => f.FormSignatures)
                .Include(f => f.Creator)
                .ToListAsync(cancellationToken);

            if (!projectForms.Any()) return;

            var formIds = projectForms.Select(f => f.Id).ToList();

            var flhaForms = await _context.FLHAForms
                .Where(f => formIds.Contains(f.ProjectFormId))
                .ToListAsync(cancellationToken);

            var flhaFormIds = flhaForms.Select(f => f.ProjectFormId).ToList();

            var hazards = await _context.FLHAFormHazards
                .Where(h => flhaFormIds.Contains(h.FLHAFormId))
                .ToListAsync(cancellationToken);

            var combined = projectForms.Select(f =>
            {
                var flha = flhaForms.FirstOrDefault(x => x.ProjectFormId == f.Id);
                var h = hazards.Where(x => x.FLHAFormId == flha?.ProjectFormId).ToList();
                return (Form: f, Flha: flha, Hazards: h);
            }).Where(x => x.Flha != null).ToList();

            var pdf = _formPdfBuilder.BuildCombinedFlhaPdf(combined);

            var subject = $"FLHA Forms - {dateFilled:yyyy-MM-dd}";
            var filename = $"FLHA_{dateFilled:yyyyMMdd}_{projectForms.First().Project?.Name ?? "Project"}.pdf";
            var body = $"Attached are all FLHA forms completed for {dateFilled:yyyy-MM-dd}.";

            await _emailSender.SendEmailWithAttachmentAsync(recipients, subject, body, pdf, filename);
        }


        private async Task HandleDrillInspectionDelivery(int projectId,int formTypeId,DateOnly dateFilled,FormDeliveryRule rule,CancellationToken cancellationToken)
        {
            var recipients = rule.Recipients.Select(r => r.Email).ToList();
            if (!recipients.Any()) return;

            var projectForms = await _context.ProjectForms
                .Where(f =>
                    f.ProjectId == projectId &&
                    f.FormTypeId == formTypeId &&
                    f.DateFilled == dateFilled &&
                    f.Status == "Completed")
                .Include(f => f.FormParticipants).ThenInclude(p => p.Participant).ThenInclude(u => u.User)
                .Include(f => f.FormSignatures)
                .Include(f => f.FormChecklistResponses).ThenInclude(c => c.ChecklistItem)
                .Include(f => f.Creator)
                .ToListAsync(cancellationToken);

            if (!projectForms.Any()) return;

            var checklistItems = await _context.ChecklistItems
                .Where(c => c.FormTypeId == formTypeId)
                .ToListAsync(cancellationToken);

            var pdf = _formPdfBuilder.BuildCombinedDrillInspectionPdf(projectForms, checklistItems);

            var subject = $"Inspection Forms - {dateFilled:yyyy-MM-dd}";
            var filename = $"Inspection_{dateFilled:yyyyMMdd}_{projectForms.First().Project?.Name ?? "Project"}.pdf";
            var body = $"Attached are all inspection forms completed for {dateFilled:yyyy-MM-dd}.";

            await _emailSender.SendEmailWithAttachmentAsync(recipients, subject, body, pdf, filename);
        }

        private async Task HandleDrillingFormDelivery(
    int projectId,
    int formTypeId,
    DateOnly dateFilled,
    FormDeliveryRule rule,
    CancellationToken cancellationToken)
        {
            var recipients = rule.Recipients.Select(r => r.Email).ToList();
            if (!recipients.Any()) return;

            // Загружаем все ProjectForms
            var projectForms = await _context.ProjectForms
                .Where(f =>
                    f.ProjectId == projectId &&
                    f.FormTypeId == formTypeId &&
                    f.DateFilled == dateFilled &&
                    f.Status == "Completed")
                .Include(f => f.Creator)
                .Include(f => f.FormParticipants).ThenInclude(p => p.Participant).ThenInclude(u => u.User)
                .Include(f => f.FormSignatures)
                .Include(f => f.Project)
                .ToListAsync(cancellationToken);

            if (!projectForms.Any()) return;

            var formIds = projectForms.Select(f => f.Id).ToList();

            // Загружаем все DrillingForms по ID
            var drillingForms = await _context.DrillingForms
                .Where(d => formIds.Contains(d.ProjectFormId))
                .ToListAsync(cancellationToken);

            var combined = projectForms
                .Select(f => (
                    Form: f,
                    Drilling: drillingForms.FirstOrDefault(d => d.ProjectFormId == f.Id)
                ))
                .Where(x => x.Drilling != null)
                .ToList();

            if (!combined.Any()) return;

            var pdf = _formPdfBuilder.BuildCombinedDrillingPdf(combined);

            var subject = $"Drilling Forms - {dateFilled:yyyy-MM-dd}";
            var filename = $"Drilling_{dateFilled:yyyyMMdd}_{projectForms.First().Project?.Name ?? "Project"}.pdf";
            var body = $"Attached are all Drilling forms completed for {dateFilled:yyyy-MM-dd}.";

            await _emailSender.SendEmailWithAttachmentAsync(recipients, subject, body, pdf, filename);
        }

    }
}
