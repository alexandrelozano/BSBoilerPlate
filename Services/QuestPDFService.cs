using BSBoilerPlate.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;

namespace BSBoilerPlate.Services
{
    public class QuestPDFService
    {
        private const int marginPTs = 56;
        private const string fontFamilyName = Fonts.Calibri;
        private const float fontSize = 10F;
        private const int maxCharsPerCell = 80;
        private const int minCharsPerCell = 10;

        private readonly IStringLocalizer<QuestPDFService> _localizer;

        public QuestPDFService(IStringLocalizer<QuestPDFService> localizer)
        {
            _localizer = localizer;
        }

        public MemoryStream CreatePDFTable(DataTable data, string title, bool landscape)
        {
            var memorystream = new MemoryStream();

            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(landscape ? PageSizes.A4.Landscape() : PageSizes.A4);
                    page.Margin(marginPTs, Unit.Point);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(fontSize).FontFamily(fontFamilyName).Fallback(TextStyle.Default.FontFamily("Arial")));

                    page.Header()
                        .Text(title)
                        .SemiBold().FontSize(16).FontColor(Colors.Black);

                    page.Content()
                        .PaddingVertical(5, Unit.Millimetre)
                        .Table(table =>
                        { 

                            var headers = (from dc in data.Columns.Cast<DataColumn>()
                                           select dc.ColumnName).ToList();

                            // Rough fit columns calculation
                            int tableWidth = landscape ? (int)(PageSizes.A4.Landscape().Width - marginPTs * 2) : (int)(PageSizes.A4.Width - marginPTs * 2);
                            int[] columnsWidth = new int[headers.Count];

                            for (uint c = 0; c < headers.Count; c++)
                            {
                                var cellWidth = Math.Max(minCharsPerCell, Math.Min($"{headers[(int)c]}".Length, maxCharsPerCell));

                                if (columnsWidth[c] < cellWidth)
                                    columnsWidth[c] = cellWidth;
                            }

                            foreach (DataRow row in data.Rows)
                            {
                                uint c = 0;
                                foreach (var value in row.ItemArray)
                                {
                                    var cellWidth = Math.Max(minCharsPerCell, Math.Min($"{value}".Length, maxCharsPerCell));
                                    if (columnsWidth[c] < cellWidth)
                                        columnsWidth[c] = cellWidth;
                                    c += 1;
                                }
                            }

                            int sumWidth = columnsWidth.Sum();
                            float ratio = tableWidth / (float)sumWidth;
                            for (int i = 0; i < columnsWidth.Length; i++)
                                columnsWidth[i] = (int)(columnsWidth[i] * ratio);

                            // Create columns
                            table.ColumnsDefinition(columns =>
                            {
                                for (uint c = 0; c < headers.Count; c++)
                                {
                                    columns.ConstantColumn(columnsWidth[c], Unit.Point);
                                }
                            });

                            table.Header(header =>
                            {
                                for (uint c = 0; c < headers.Count; c++)
                                {
                                    header.Cell().Row(1).Column(c + 1).Element(BlockHeader).Text(headers[(int)c]);
                                }
                            });

                            // Create rows
                            uint colIndex = 1;
                            uint rowIndex = 1;
                            foreach (DataRow row in data.Rows)
                            {
                                colIndex = 1;
                                rowIndex++;

                                foreach (var value in row.ItemArray)
                                {
                                    if (IsNumber(value))
                                        table.Cell().Row(rowIndex).Column(colIndex).Element(BlockCell).AlignRight().Text($"{value}");
                                    else
                                        table.Cell().Row(rowIndex).Column(colIndex).Element(BlockCell).AlignLeft().Text($"{value}"); ;

                                    colIndex += 1;
                                }
                            }
                        });

                    page.Footer()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Row(1).Column(1).Element(BlockFooterCell).AlignLeft().Text($"{DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")}");
                            table.Cell().Row(1).Column(2).Element(BlockFooterCell).AlignRight().Text(x =>
                            {
                                x.Span($"{_localizer["Page"]} ");
                                x.CurrentPageNumber();
                                x.Span($" {_localizer["of"]} ");
                                x.TotalPages();
                            });
                        });
                });
            })
                .GeneratePdf(memorystream);

            memorystream.Position = 0;

            return memorystream;
        }

        static IContainer BlockFooterCell(IContainer container)
        {
            return container
                .BorderTop(1)
                .Background(Colors.White)
                .Padding(1, Unit.Millimetre);
        }

        static IContainer BlockCell(IContainer container)
        {
            return container
                .Border(1)
                .Background(Colors.White)
                .Padding(1, Unit.Millimetre)
                .ShowOnce()
                .AlignMiddle();
        }

        static IContainer BlockHeader(IContainer container)
        {
            return container
                .Border(1)
                .Background(Colors.Grey.Lighten3)
                .Padding(1, Unit.Millimetre)
                .AlignCenter()
                .AlignMiddle();
        }

        static bool IsNumber(object? value)
        {
            if (value == null)
                return false;
            else
                return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
    }
}
