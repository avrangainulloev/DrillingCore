using DrillingCore.Core.Entities;
using Microsoft.AspNetCore.Hosting;
using QuestPDF.Drawing;
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


        static FormPdfBuilder()
        {
            // Один раз при первом использовании класса
            FontManager.RegisterFont(File.OpenRead(Path.Combine(AppContext.BaseDirectory, "fonts", "NotoSansSymbols-VariableFont_wght.ttf")));
        }

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
                        page.DefaultTextStyle(x => x.FontFamily("Noto Sans Symbols Thin").FontSize(11));
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

        public byte[] BuildFlhaPdf(ProjectForm form, FLHAForm flhaForm, List<FLHAFormHazard> hazards)
        {
            try
            {
                var signatureMap = form.FormSignatures
                    .ToDictionary(s => s.ParticipantId, s => s.SignatureUrl);
 

                var doc = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(40);
                        page.Size(PageSizes.A4);
                        page.DefaultTextStyle(x => x.FontFamily("Noto Sans Symbols Thin").FontSize(11));
                        page.PageColor(Colors.White);

                        page.Header().Text("Field Level Hazard Assessment (FLHA)")
                            .FontSize(18).Bold().AlignCenter();

                        page.Content().Column(col =>
                        {
                            col.Spacing(12);

                            // ℹ️ Task + Info block
                            col.Item().Border(1).Background(Colors.Grey.Lighten3).Padding(10).Column(info =>
                            {
                                info.Spacing(4);
                                info.Item().Text($"Crew Name: {form.Creator.FullName}").Bold();
                                info.Item().Text($"Date Filled: {form.DateFilled:yyyy-MM-dd}");                                
                                info.Item().Text($"Task: {flhaForm.TaskDescription}");
                            });

                            // ⚠️ Hazard blocks
                            col.Item().Column(section =>
                            {
                                section.Spacing(6);
                                section.Item().Text("Hazards").FontSize(14).Bold();

                                section.Item()
                                    .PaddingLeft(5)
                                    .Border(1)
                                    .BorderColor(Colors.Grey.Lighten1)
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(10)
                                    .Grid(grid =>
                                    {
                                        grid.Columns(2);
                                        grid.Spacing(4);

                                        foreach (var hazard in hazards)
                                        {
                                            grid.Item().Column(hz =>
                                            {
                                                hz.Item().Text($"⚠ {hazard.HazardText}").Bold();
                                                hz.Item().Text($"✔ {hazard.ControlMeasures}")
                        .FontColor(Colors.Grey.Darken2);
                                            });
                                        }
                                    });
                            });

                            // 💬 Comments block (если есть)
                            if (!string.IsNullOrWhiteSpace(form.OtherComments))
                            {
                                col.Item().PaddingTop(10).Column(c =>
                                {
                                    c.Item().Text("Other Comments").FontSize(13).Bold();
                                    c.Item().Text(form.OtherComments).Italic();
                                });
                            }
                            // ✍️ Participants + Signatures
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
                Console.WriteLine("FLHA PDF generation failed: " + ex.Message);
                throw;
            }
        }


        public byte[] BuildCombinedFlhaPdf(List<(ProjectForm Form, FLHAForm Flha, List<FLHAFormHazard> Hazards)> forms)
        {
            var doc = Document.Create(container =>
            {
                foreach (var (form, flha, hazards) in forms)
                {
                    container.Page(page =>
                    {
                        page.Margin(40);
                        page.Size(PageSizes.A4);
                        page.DefaultTextStyle(x => x.FontFamily("Noto Sans Symbols Thin").FontSize(11));
                        page.PageColor(Colors.White);

                        page.Header().Text("Field Level Hazard Assessment (FLHA)")
                            .FontSize(18).Bold().AlignCenter();

                        page.Content().Element(c => ComposeFlhaPage(c, form, flha, hazards));

                        page.Footer().AlignCenter().Text("Generated by DrillingCore System").Italic().FontSize(10);
                    });
                }
            });

            using var stream = new MemoryStream();
            doc.GeneratePdf(stream);
            return stream.ToArray();
        }

        private void ComposeFlhaPage(IContainer container, ProjectForm form, FLHAForm flha, List<FLHAFormHazard> hazards)
        {
            var signatureMap = form.FormSignatures.ToDictionary(s => s.ParticipantId, s => s.SignatureUrl);

            container.Column(col =>
            {
                col.Spacing(12);

                // ℹ️ Информация о форме
                col.Item().Border(1).Background(Colors.Grey.Lighten3).Padding(10).Column(info =>
                {
                    info.Spacing(4);
                    info.Item().Text($"Creator: {form.Creator?.FullName ?? "Unknown"}").Bold();
                    info.Item().Text($"Date Filled: {form.DateFilled:yyyy-MM-dd}");
                    info.Item().Text($"Task Description: {flha.TaskDescription}").Italic();
                });

                // ⚠️ Hazards
                col.Item().Text("Hazards").FontSize(14).Bold();

                col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten3).Padding(10).Grid(grid =>
                {
                    grid.Columns(2);
                    grid.Spacing(4);

                    foreach (var hazard in hazards)
                    {
                        grid.Item().Column(hz =>
                        {
                            hz.Item().Text($"⚠ {hazard.HazardText}").Bold();
                            hz.Item().Text($"✔ {hazard.ControlMeasures}").FontColor(Colors.Grey.Darken2);
                        });
                    }
                });

                // 💬 Comments block
                if (!string.IsNullOrWhiteSpace(form.OtherComments))
                {
                    col.Item().PaddingTop(10).Column(c =>
                    {
                        c.Item().Text("Other Comments").FontSize(13).Bold();
                        c.Item().Text(form.OtherComments).Italic();
                    });
                }

                // ✍️ Participants
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

                            grid.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Column(sig =>
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
        }


        public byte[] BuildCombinedDrillInspectionPdf(List<ProjectForm> forms, List<ChecklistItem> checklistItems)
        {
            try
            {
                var groupedChecklist = checklistItems
                    .GroupBy(i => i.GroupName ?? "General")
                    .ToList();

                var doc = Document.Create(container =>
                {
                    foreach (var form in forms)
                    {
                        var checklistMap = form.FormChecklistResponses
                            .ToDictionary(r => r.ChecklistItemId, r => r.Response ? "true" : "false");

                        var signatureMap = form.FormSignatures
                            .ToDictionary(s => s.ParticipantId, s => s.SignatureUrl);

                        container.Page(page =>
                        {
                            page.Margin(40);
                            page.Size(PageSizes.A4);
                            page.DefaultTextStyle(x => x.FontFamily("Noto Sans Symbols Thin").FontSize(11));
                            page.PageColor(Colors.White);

                            page.Header().Text(form.FormType?.Name ?? "Inspection Form")
                                .FontSize(18).Bold().AlignCenter();

                            page.Content().Column(col =>
                            {
                                col.Spacing(12);

                                // ℹ️ General info
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

                                    foreach (var group in groupedChecklist)
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

                                // 💬 Comments
                                if (!string.IsNullOrWhiteSpace(form.OtherComments))
                                {
                                    col.Item().PaddingTop(10).Column(c =>
                                    {
                                        c.Item().Text("Other Comments").FontSize(13).Bold();
                                        c.Item().Text(form.OtherComments).Italic();
                                    });
                                }

                                // ✍️ Signatures
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
                    }
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

        public byte[] BuildDrillingFormPdf(ProjectForm form, DrillingForm drillingForm)
        {
            try
            {
                var signatureMap = form.FormSignatures
                    .ToDictionary(s => s.ParticipantId, s => s.SignatureUrl);

                var doc = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(40);
                        page.Size(PageSizes.A4);
                        page.DefaultTextStyle(x => x.FontFamily("Noto Sans Symbols Thin").FontSize(11));
                        page.PageColor(Colors.White);

                        page.Header().Text("Drilling Report")
                            .FontSize(18).Bold().AlignCenter();

                        page.Content().Column(col =>
                        {
                            col.Spacing(14);

                            // 📄 Информация о проекте
                            col.Item().Border(1).Background(Colors.Grey.Lighten3).Padding(10).Column(info =>
                            {
                                info.Spacing(4);
                                info.Item().Text($"Project: {form.Project?.Name ?? "N/A"}").Bold();
                                info.Item().Text($"Date Filled: {form.DateFilled:yyyy-MM-dd}");
                                info.Item().Text($"Created By: {form.Creator?.FullName ?? "N/A"}");
                            });

                            // 📊 Бурение — отдельным блоком
                            col.Item().Border(1).BorderColor(Colors.Blue.Medium)
                                .Background(Colors.Grey.Lighten4)
                                .Padding(12).Column(stats =>
                                {
                                    stats.Spacing(6);
                                    stats.Item().Text("Drilling Summary").FontSize(14).Bold().FontColor(Colors.Blue.Medium);
                                    stats.Item().Text($"Total Wells: {drillingForm.NumberOfWells}").FontSize(12).Bold();
                                    stats.Item().Text($"Total Meters: {drillingForm.TotalMeters}").FontSize(12).Bold();
                                });

                            // 💬 Комментарии
                            if (!string.IsNullOrWhiteSpace(form.OtherComments))
                            {
                                col.Item().PaddingTop(10).Column(c =>
                                {
                                    c.Item().Text("Other Comments").FontSize(13).Bold();
                                    c.Item().Text(form.OtherComments).Italic();
                                });
                            }

                            // ✍️ Подписавшиеся
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

                                        grid.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Column(sig =>
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
                Console.WriteLine("Drilling PDF generation failed: " + ex.Message);
                throw;
            }
        }

        public byte[] BuildCombinedDrillingPdf(List<(ProjectForm Form, DrillingForm Drilling)> forms)
        {
            try
            {
                var date = forms.FirstOrDefault().Form.DateFilled.ToString("yyyy-MM-dd");
                var totalWells = forms.Sum(f => f.Drilling.NumberOfWells);
                var totalMeters = forms.Sum(f => f.Drilling.TotalMeters);

                var doc = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(40);
                        page.Size(PageSizes.A4);
                        page.DefaultTextStyle(x => x.FontFamily("Noto Sans Symbols Thin").FontSize(11));
                        page.PageColor(Colors.White);

                        page.Header().Text($"Drilling Report for {date}")
                            .FontSize(18).Bold().AlignCenter();

                        page.Content().Column(col =>
                        {
                            col.Spacing(15);

                            // 📋 Таблица бурения
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);   // #
                                    columns.RelativeColumn(3);    // Creator
                                    columns.RelativeColumn(2);    // Date
                                    columns.RelativeColumn(2);    // Wells
                                    columns.RelativeColumn(2);    // Meters
                                });

                                // 🔠 Заголовок
                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("#").Bold();
                                    header.Cell().Element(CellStyle).Text("Creator").Bold();
                                    header.Cell().Element(CellStyle).Text("Date").Bold();
                                    header.Cell().Element(CellStyle).Text("Wells").Bold();
                                    header.Cell().Element(CellStyle).Text("Meters").Bold();
                                });

                                int index = 1;
                                foreach (var (form, drilling) in forms)
                                {
                                    table.Cell().Element(CellStyle).Text(index++.ToString());
                                    table.Cell().Element(CellStyle).Text(form.Creator?.FullName ?? "-");
                                    table.Cell().Element(CellStyle).Text(form.DateFilled.ToString("yyyy-MM-dd"));
                                    table.Cell().Element(CellStyle).Text(drilling.NumberOfWells.ToString());
                                    table.Cell().Element(CellStyle).Text($"{drilling.TotalMeters:F2}");
                                }

                                // ➖ Разделитель
                                table.Cell().ColumnSpan(5).Element(CellStyle).PaddingVertical(3).BorderBottom(1).Text("");

                                // ➕ Итоговая строка с фоном
                                table.Cell().Element(cell => CellStyle(cell).Background(Colors.Grey.Lighten3)).Text("");
                                table.Cell().Element(cell => CellStyle(cell).Background(Colors.Grey.Lighten3)).Text("");
                                table.Cell().Element(cell => CellStyle(cell).Background(Colors.Grey.Lighten3)).Text("Total")
                                    .Bold().AlignRight();
                                table.Cell().Element(cell => CellStyle(cell).Background(Colors.Grey.Lighten3)).Text(totalWells.ToString()).Bold();
                                table.Cell().Element(cell => CellStyle(cell).Background(Colors.Grey.Lighten3)).Text($"{totalMeters:F2}").Bold();
                            });
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
                Console.WriteLine("Drilling Combined PDF generation failed: " + ex.Message);
                throw;
            }

            // 🧱 Стиль ячеек
            static IContainer CellStyle(IContainer container)
            {
                return container
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .PaddingVertical(4)
                    .PaddingHorizontal(6)
                    .AlignMiddle();
            }
        }


    }
}
