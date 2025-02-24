using CashFlow.Domain.Enums;
using CashFlow.Domain.Reports;
using CashFlow.Domain.Repositories.Expenses;
using ClosedXML.Excel;

namespace CashFlow.Application.UseCases.Expenses.Reports.Excel
{
    public class GenerateExpenseReportExcelUseCase : IGenerateExpenseReportExcelUseCase
    {
        private const string CURRENCY_SYMBOL = "R$";

        private readonly IExpensesReadOnlyRepository _repository;
        public GenerateExpenseReportExcelUseCase(IExpensesReadOnlyRepository repository)
        {
            _repository = repository;
        }
        public async Task<byte[]> Execute(DateOnly month)
        {
            var expenses = await _repository.FilterByMonth(month);
            if (expenses.Count == 0)
                return [];

            using var workbook = new XLWorkbook();
            workbook.Author = "ArthurTorres/CashFlow";
            workbook.Style.Font.FontSize = 12;
            workbook.Style.Font.FontName = "Times New Roman";

            //DateTime.Now.ToString("Y"); -- Serve para escolher como ele vai me retornar em string por exemplo Y = Junho, 2024, devolve mes e ano

            var worksheet = workbook.Worksheets.Add(month.ToString("Y"));

            InsertHeader(worksheet);

            //começa apartir da linha dois ja que alinha um da planilha fica o header
            var raw = 2;
            foreach (var expense in expenses)
            {
                worksheet.Cell($"A{raw}").Value = expense.Title;
                worksheet.Cell($"B{raw}").Value = expense.Description;
                worksheet.Cell($"C{raw}").Value = expense.Date;
                worksheet.Cell($"D{raw}").Value = ConvertPayment_Type(expense.PaymentType);
                worksheet.Cell($"E{raw}").Value = expense.Amount;
                worksheet.Cell($"E{raw}").Style.NumberFormat.Format = $"-{CURRENCY_SYMBOL} #,##0.00";

                raw++;
            }

            worksheet.Columns("A:E").Width = 25;

            var file = new MemoryStream();
            workbook.SaveAs(file);

            return file.ToArray();
        }

        private string ConvertPayment_Type(PaymentType payment)
        {
            return payment switch
            {
                PaymentType.Cash => "Dinheiro",
                PaymentType.CreditCard => "Cartão de crédito",
                PaymentType.DebitCard => "Cartão de débito",
                PaymentType.EletronicTransfer => "Transferência eletrônica",
                _ => string.Empty
            };
        }

        private void InsertHeader(IXLWorksheet worksheet)
        {
            worksheet.Cell("A1").Value = ResourceReportGenerationMessages.TITLE;
            worksheet.Cell("B1").Value = ResourceReportGenerationMessages.DESCRIPTION;
            worksheet.Cell("C1").Value = ResourceReportGenerationMessages.DATE;
            worksheet.Cell("D1").Value = ResourceReportGenerationMessages.PAYMENT_TYPE;
            worksheet.Cell("E1").Value = ResourceReportGenerationMessages.AMOUNT;

            worksheet.Cells("A1:E1").Style.Font.Bold = true;
            worksheet.Cells("A1:E1").Style.Fill.BackgroundColor = XLColor.FromHtml("#F5C2B6");

            worksheet.Cells("A1:D1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell("E1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

        }
    }
}
