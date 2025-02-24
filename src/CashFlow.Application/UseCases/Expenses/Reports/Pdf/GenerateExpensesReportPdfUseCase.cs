
using CashFlow.Application.UseCases.Expenses.Reports.Pdf.Colors;
using CashFlow.Application.UseCases.Expenses.Reports.Pdf.Fonts;
using CashFlow.Domain.Extensions;
using CashFlow.Domain.Reports;
using CashFlow.Domain.Repositories.Expenses;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using System.Reflection;

namespace CashFlow.Application.UseCases.Expenses.Reports.Pdf
{
    public class GenerateExpensesReportPdfUseCase : IGenerateExpensesReportPdfUseCase
    {
        private const string CURRENCY_SYMBOL = "R$";
        private const int HEIGHT_ROW_EXPENSE_TABLE = 25;

        private readonly IExpensesReadOnlyRepository _repository;
        public GenerateExpensesReportPdfUseCase(IExpensesReadOnlyRepository repository)
        {
            _repository = repository;
            GlobalFontSettings.FontResolver = new ExpensesReportFontResolver();
        }


        public async Task<byte[]> Execute(DateOnly month)
        {
            var expenses = await _repository.FilterByMonth(month);
            if (expenses.Count == 0)
                return [];

            var document = CreateDocument(month);
            var page = CreatePage(document);

            CreateHeaderWithProfileFotoAndName(page);

            var totalExpenses = expenses.Sum(expense => expense.Amount);
            CreateTotalSpentSection(page, month, totalExpenses);

            foreach (var expense in expenses)
            {
                var table = CreateExpenseTable(page);

                //Inicio cabeçalho
                var row = table.AddRow();
                row.Height = HEIGHT_ROW_EXPENSE_TABLE;

                AddExpenseTitle(row.Cells[0], expense.Title);
                AddHeaderForAmount(row.Cells[3]);
                //Fim cabeçalho

                //Inicio do corpo das despesa Data, Hora, Tipo de pagamento e valor
                row = table.AddRow();
                row.Height = HEIGHT_ROW_EXPENSE_TABLE;

                row.Cells[0].AddParagraph(expense.Date.ToString("D"));
                SetStyleBaseForExpenseInformation(row.Cells[0]);
                row.Cells[0].Format.LeftIndent = 20;

                row.Cells[1].AddParagraph(expense.Date.ToString("t"));
                SetStyleBaseForExpenseInformation(row.Cells[1]);

                row.Cells[2].AddParagraph(expense.PaymentType.PaymentTypeToString());
                SetStyleBaseForExpenseInformation(row.Cells[2]);

                AddAmountForExpense(row.Cells[3], expense.Amount);

                row = table.AddRow();
                row.Height = HEIGHT_ROW_EXPENSE_TABLE;

                AddDescriptionExpense(row.Cells[0], expense.Description);


                //fim do corpo da despesa

                // Espaço entre as despesas no arquivo
                AddWhiteSpace(table);

            }

            return RederDocuments(document);
        }

        private Document CreateDocument(DateOnly month)
        {
            var document = new Document();
            document.Info.Title = $"{ResourceReportGenerationMessages.EXPENSE_FOR} {month:Y}";
            document.Info.Author = "Arthur Torres";

            var style = document.Styles["Normal"];
            style!.Font.Name = FontHelper.RALEWAY_REGULAR;
            return document;
        }

        private Section CreatePage(Document document)
        {
            var section = document.AddSection();
            section.PageSetup = document.DefaultPageSetup.Clone();
            //width 595 - height 842 tamanho de uma folha a4
            section.PageSetup.PageFormat = PageFormat.A4;
            section.PageSetup.LeftMargin = 40;
            section.PageSetup.RightMargin = 40;
            section.PageSetup.TopMargin = 40;
            section.PageSetup.BottomMargin = 40;
            return section;
        }

        private void CreateHeaderWithProfileFotoAndName(Section page)
        {
            var table = page.AddTable();
            table.AddColumn();
            table.AddColumn("300");

            var row = table.AddRow();

            var assembly = Assembly.GetExecutingAssembly();
            var directoryName = Path.GetDirectoryName(assembly.Location);
            var pathFile = Path.Combine(directoryName!, "Logo", "pdfimage.jpg");

            var image = row.Cells[0].AddImage(pathFile);

            image.Width = Unit.FromPoint(64);
            image.Height = Unit.FromPoint(64);

            row.Cells[1].AddParagraph("Bem vindo(a)");
            row.Cells[1].Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 16 };
            row.Cells[1].VerticalAlignment = MigraDoc.DocumentObjectModel.Tables.VerticalAlignment.Center;

        }

        private void CreateTotalSpentSection(Section page, DateOnly month, decimal totalExpenses)
        {
            var paragraph = page.AddParagraph();
            paragraph.Format.SpaceBefore = "40";
            paragraph.Format.SpaceAfter = "40";
            var title = string.Format(ResourceReportGenerationMessages.TOTAL_SPENT_IN, month.ToString("Y"));

            paragraph.AddFormattedText(title, new Font
            {
                Name = FontHelper.RALEWAY_REGULAR,
                Size = 15
            });

            paragraph.AddLineBreak();


            paragraph.AddFormattedText($"{totalExpenses} {CURRENCY_SYMBOL}", new Font
            {
                Name = FontHelper.WORKSANS_BLACK,
                Size = 50
            });
        }

        private Table CreateExpenseTable(Section page)
        {
            /*width 595 - height 842 tamanho de uma folha a4,
            ou seja essas colunas totalizam um width de 595 mais as margens de 40 cada lado*/
            var table = page.AddTable();
            table.AddColumn("195").Format.Alignment = ParagraphAlignment.Left;
            table.AddColumn("80").Format.Alignment = ParagraphAlignment.Center;
            table.AddColumn("120").Format.Alignment = ParagraphAlignment.Center;
            table.AddColumn("120").Format.Alignment = ParagraphAlignment.Right;

            return table;
        }

        private void AddExpenseTitle(Cell cell, string expenseTitle)
        {

            cell.AddParagraph(expenseTitle);
            cell.Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 14, Color = ColorsHelper.BLACK };
            cell.Shading.Color = ColorsHelper.RED_LIGHT;
            cell.VerticalAlignment = VerticalAlignment.Center;
            cell.Format.LeftIndent = 20;
            cell.MergeRight = 2;

        }

        private void AddHeaderForAmount(Cell cell)
        {
            cell.AddParagraph(ResourceReportGenerationMessages.AMOUNT);
            cell.Format.Font = new Font { Name = FontHelper.RALEWAY_BLACK, Size = 14, Color = ColorsHelper.WHITE };
            cell.Shading.Color = ColorsHelper.RED_DARK;
            cell.VerticalAlignment = VerticalAlignment.Center;
        }

        private void SetStyleBaseForExpenseInformation(Cell cell)
        {
            cell.Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 12, Color = ColorsHelper.BLACK };
            cell.Shading.Color = ColorsHelper.GREEN_DARK;
            cell.VerticalAlignment = VerticalAlignment.Center;
        }

        private void AddAmountForExpense(Cell cell, decimal amount)
        {
            cell.AddParagraph($"-{amount} {CURRENCY_SYMBOL}");
            cell.Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 14, Color = ColorsHelper.BLACK };
            cell.Shading.Color = ColorsHelper.WHITE;
            cell.VerticalAlignment = VerticalAlignment.Center;
        }

        private void AddDescriptionExpense(Cell cell, string expenseDescription)
        {
            if (expenseDescription is null)
            {
                cell.AddParagraph("Sem descrição");
                cell.Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 12, Color = ColorsHelper.BLACK };
                cell.Shading.Color = ColorsHelper.GREEN_LIGHT;
                cell.VerticalAlignment = VerticalAlignment.Center;
                cell.Format.LeftIndent = 20;
                cell.MergeRight = 2;
            }else{
                cell.AddParagraph(expenseDescription);
                cell.Format.Font = new Font { Name = FontHelper.WORKSANS_REGULAR, Size = 12, Color = ColorsHelper.BLACK };
                cell.Shading.Color = ColorsHelper.GREEN_LIGHT;
                cell.VerticalAlignment = VerticalAlignment.Center;
                cell.Format.LeftIndent = 20;
                cell.MergeRight = 2;
            }
        }

        private void AddWhiteSpace(Table table)
        {
            var row = table.AddRow();
            row.Height = 30;
            row.Borders.Visible = false;
        }

        private byte[] RederDocuments(Document document)
        {
            var renderer = new PdfDocumentRenderer
            {
                Document = document
            };

            renderer.RenderDocument();
            using var file = new MemoryStream();
            renderer.PdfDocument.Save(file);

            return file.ToArray();

        }
    }
}
