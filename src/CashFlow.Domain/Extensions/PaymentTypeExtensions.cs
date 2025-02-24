using CashFlow.Domain.Enums;

namespace CashFlow.Domain.Extensions
{
    public static class PaymentTypeExtensions
    {
        public static string PaymentTypeToString(this PaymentType payment)
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
    }
}
