namespace FinanceTracker.Server.DTOs.ExpenseItems
{
    /// <summary>
    /// DTO для отображения информации о статье расхода
    /// </summary>
    public class ExpenseItemDto
    {
        /// <summary>
        /// Уникальный идентификатор статьи
        /// </summary>
        public int ExpenseItemId { get; set; }

        /// <summary>
        /// Название статьи расхода
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// ID категории, к которой относится статья
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Название категории
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Активна ли статья
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Количество транзакций по этой статье
        /// </summary>
        public int TransactionsCount { get; set; }
    }
}