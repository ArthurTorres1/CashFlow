namespace CashFlow.Domain.Repositories.Expenses
{
    public interface IExpensesDeleteOnlyRepository
    {
        /// <summary>
        /// This function return TRUE if the deletion was successfull 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);
    }
}
