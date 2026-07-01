namespace FinanceTracker.Server.DTOs.Transactions
{
    /// <summary>
    /// DTO для отображения информации о транзакции
    /// </summary>
    public class TransactionDto
    {
        /// <summary>
        /// Уникальный идентификатор транзакции
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// Дата транзакции
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Сумма потраченных денег
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Комментарий к транзакции
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// ID статьи расхода
        /// </summary>
        public int ExpenseItemId { get; set; }

        /// <summary>
        /// Название статьи расхода
        /// </summary>
        public string ExpenseItemName { get; set; } = string.Empty;

        /// <summary>
        /// ID категории
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Название категории
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;
    }
}