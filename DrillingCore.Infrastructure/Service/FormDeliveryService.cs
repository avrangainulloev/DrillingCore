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

        public FormDeliveryService(IFormDeliveryRepository repository, IEmailSender emailSender, DrillingCoreDbContext context , FormPdfBuilder formPdfBuilder)
        {
            _repository = repository;
            _emailSender = emailSender;
            _context = context;
            _formPdfBuilder = formPdfBuilder;
        }

        public async Task TrySendOnFormCompleted(ProjectForm form, CancellationToken ct)
        {
            var rule = await _repository.GetRuleAsync(form.ProjectId, form.FormTypeId, ct);
            if (rule == null || rule.Condition != FormDeliveryCondition.AfterEachFormCompleted)
                return;

            var recipients = rule.Recipients.Select(r => r.Email).ToList();
            if (!recipients.Any()) return;

            var checklistItems = await _context.ChecklistItems
    .Where(x => x.FormTypeId == form.FormTypeId)
    .ToListAsync();
            var pdf = _formPdfBuilder.BuildDrillInspectionPdf(form, checklistItems);
            var fileName = $"Form_{form.FormType?.Name}_{form.DateFilled:yyyyMMdd}.pdf";
            var subject = $"Form Completed: {form.FormType?.Name}";
            var body = $"Attached is the completed form for {form.DateFilled:yyyy-MM-dd}.";

            await _emailSender.SendEmailWithAttachmentAsync(recipients, subject, body, pdf, fileName);
        }
    }
}
