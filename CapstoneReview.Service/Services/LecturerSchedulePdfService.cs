using CapstoneReview.Repository.Interfaces;
using CapstoneReview.Service.DTOs;
using CapstoneReview.Service.Exceptions;
using CapstoneReview.Service.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CapstoneReview.Service.Services
{
    public class LecturerSchedulePdfService : ILecturerSchedulePdfService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LecturerSchedulePdfService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<byte[]> ExportLecturerSchedulePdfAsync(int lecturerId, int reviewRound)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var lecturer = await _unitOfWork.Lecturers.GetByIdAsync(lecturerId);
            if (lecturer == null)
                throw new BusinessRuleException("Lecturer not found.");

            var scheduleItems = await BuildLecturerScheduleAsync(lecturerId, reviewRound);

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("LECTURER REVIEW SCHEDULE")
                            .Bold().FontSize(18);

                        column.Item().Text($"Lecturer: {lecturer.FullName}");
                        column.Item().Text($"Email: {lecturer.Email}");
                        column.Item().Text($"Review Round: {reviewRound}");
                        column.Item().Text($"Generated at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    });

                    page.Content().PaddingVertical(15).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(45);
                            columns.RelativeColumn(1.3f);
                            columns.RelativeColumn(1f);
                            columns.RelativeColumn(1.8f);
                            columns.RelativeColumn(2.2f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Slot").Bold();
                            header.Cell().Element(CellStyle).Text("Time").Bold();
                            header.Cell().Element(CellStyle).Text("Room").Bold();
                            header.Cell().Element(CellStyle).Text("Team").Bold();
                            header.Cell().Element(CellStyle).Text("Topic").Bold();
                        });

                        foreach (var item in scheduleItems.OrderBy(x => x.StartTime))
                        {
                            table.Cell().Element(CellStyle).Text(item.SlotId.ToString());
                            table.Cell().Element(CellStyle)
                                .Text($"{item.StartTime:yyyy-MM-dd HH:mm} - {item.EndTime:HH:mm}");
                            table.Cell().Element(CellStyle).Text(item.Room);
                            table.Cell().Element(CellStyle).Text(item.TeamName);
                            table.Cell().Element(CellStyle).Text(item.TopicTitle);
                        }

                        if (!scheduleItems.Any())
                        {
                            table.Cell().ColumnSpan(5).Element(CellStyle)
                                .Text("No schedule found for this lecturer in the selected review round.");
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Capstone Review Registration Tool - ");
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                });
            }).GeneratePdf();

            return pdfBytes;
        }

        private static QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
        {
            return container
                .PaddingVertical(6)
                .PaddingHorizontal(4)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2);
        }

        private async Task<List<LecturerSchedulePdfItem>> BuildLecturerScheduleAsync(int lecturerId, int reviewRound)
        {
            var slots = await _unitOfWork.Slots.GetSlotsByReviewRoundAsync(reviewRound);

            var result = new List<LecturerSchedulePdfItem>();

            foreach (var slot in slots)
            {
                var lecturerAssigned = slot.SlotLecturers.Any(x => x.LecturerId == lecturerId);
                if (!lecturerAssigned)
                    continue;

                foreach (var slotTopic in slot.SlotTopics)
                {
                    var topic = slotTopic.Topic;
                    if (topic == null)
                        continue;

                    result.Add(new LecturerSchedulePdfItem
                    {
                        SlotId = slot.Id,
                        ReviewRound = slot.ReviewRound,
                        Room = slot.Room,
                        StartTime = slot.StartTime,
                        EndTime = slot.EndTime,
                        TopicTitle = topic.Title,
                        TeamName = topic.Team?.TeamName ?? "-"
                    });
                }
            }

            return result;
        }
    }
}