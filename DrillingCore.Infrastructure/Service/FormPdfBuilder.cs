using DrillingCore.Core.Entities;
using Microsoft.AspNetCore.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Service
{
    public  class FormPdfBuilder
    {
        private readonly IWebHostEnvironment _env;

        public FormPdfBuilder(IWebHostEnvironment env)
        {
            _env = env;
        }

        public byte[] BuildDrillInspectionPdf(ProjectForm form, List<ChecklistItem> checklistItems)
        {
            try
            {
                var checklistMap = form.FormChecklistResponses
                    .ToDictionary(r => r.ChecklistItemId, r => r.Response ? "true" : "false");

                var signatureMap = form.FormSignatures
                    .ToDictionary(s => s.ParticipantId, s => s.SignatureUrl);

                var checklistGroups = checklistItems
                    .GroupBy(i => i.GroupName ?? "General")
                    .ToList();

                var doc = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(40);
                        page.Size(PageSizes.A4);
                        page.DefaultTextStyle(x => x.FontSize(11));
                        page.PageColor(Colors.White);

                        page.Header().Text(form.FormType?.Name ?? "Inspection Form")
                            .FontSize(18).Bold().AlignCenter();

                        page.Content().Column(col =>
                        {
                            col.Spacing(12);

                            // ℹ️ General info block
                            col.Item().Border(1).Background(Colors.Grey.Lighten3).Padding(10).Column(info =>
                            {
                                info.Spacing(4);
                                info.Item().Text($"Crew Name: {form.CrewName}").Bold();
                                info.Item().Text($"Date Filled: {form.DateFilled:yyyy-MM-dd}");
                                info.Item().Text($"Unit Number: {form.UnitNumber}");
                            });

                            // ✅ Checklist block
                            col.Item().Column(checklistSection =>
                            {
                                checklistSection.Spacing(6);
                                checklistSection.Item().Element(e => e.PaddingLeft(5)).Text("Checklist").FontSize(14).Bold();

                                foreach (var group in checklistGroups)
                                {
                                    checklistSection.Item()
                                        .PaddingLeft(5)
                                        .Border(1)
                                        .BorderColor(Colors.Grey.Lighten1)
                                        .Background(Colors.Grey.Lighten3)
                                        .Padding(10)
                                        .Column(groupCol =>
                                        {
                                            groupCol.Spacing(5);

                                            groupCol.Item().Text(group.Key)
                                        .FontSize(13)
                                        .SemiBold()
                                        .FontColor(Colors.Blue.Medium);

                                            groupCol.Item().Grid(grid =>
                                            {
                                                grid.Columns(2);
                                                grid.Spacing(4);

                                                foreach (var item in group)
                                                {
                                                    var response = checklistMap.TryGetValue(item.Id, out var val) ? val : "false";
                                                    var isChecked = response == "true";
                                                    var check = isChecked ? "☑" : "☐";

                                                    grid.Item().Text($"{check} {item.Label}")
                                                .WrapAnywhere()
                                                .FontColor(isChecked ? Colors.Black : Colors.Grey.Darken2);
                                                }
                                            });
                                        });
                                }
                            });

                            // 💬 Comments block (без рамки, но отступ)
                            if (!string.IsNullOrWhiteSpace(form.OtherComments))
                            {
                                col.Item().PaddingTop(10).Column(c =>
                                {
                                    c.Item().Text("Other Comments").FontSize(13).Bold();
                                    c.Item().Text(form.OtherComments).Italic();
                                });
                            }

                            // ✍️ Participants & Signatures
                            if (form.FormParticipants.Any())
                            {
                                col.Item().PaddingTop(10).Text("Participants & Signatures").FontSize(14).Bold();

                                col.Item().Grid(grid =>
                                {
                                    grid.Columns(2);
                                    grid.Spacing(10);

                                    foreach (var fp in form.FormParticipants)
                                    {
                                        var name = fp.Participant?.User?.FullName ?? $"Participant #{fp.ParticipantId}";

                                        grid.Item().Border(1)
                                            .BorderColor(Colors.Grey.Lighten2)
                                            .Padding(6)
                                            .Column(sig =>
                                            {
                                                sig.Item().Text(name).Bold();

                                                if (signatureMap.TryGetValue(fp.ParticipantId, out var sigPath))
                                                {
                                                    var fullPath = Path.Combine(_env.WebRootPath, sigPath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                                                    if (File.Exists(fullPath))
                                                    {
                                                        var bytes = File.ReadAllBytes(fullPath);
                                                        sig.Item().Height(30).MaxWidth(100).Image(bytes, ImageScaling.FitHeight);
                                                    }
                                                    else
                                                    {
                                                        sig.Item().Text("[missing image]").Italic().FontColor(Colors.Grey.Darken2);
                                                    }
                                                }
                                                else
                                                {
                                                    sig.Item().Text("[no signature]").Italic().FontColor(Colors.Grey.Darken2);
                                                }
                                            });
                                    }
                                });
                            }
                        });

                        page.Footer().AlignCenter().Text("Generated by DrillingCore System").Italic().FontSize(10);
                    });
                });

                using var stream = new MemoryStream();
                doc.GeneratePdf(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("PDF generation failed: " + ex.Message);
                throw;
            }
        }


    }
}
