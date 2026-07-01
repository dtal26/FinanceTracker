namespace FinanceTracker.Server.DTOs.Categories
{
    /// <summary>
    /// DTO для отображения информации о категории расходов
    /// </summary>
    public class CategoryDto
    {
        /// <summary>
        /// Уникальный идентификатор категории
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Название категории
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Месячный бюджет в рублях
        /// </summary>
        public decimal MonthlyBudget { get; set; }

        /// <summary>
        /// Активна ли категория
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Количество статей расходов в этой категории
        /// </summary>
        public int ExpenseItemsCount { get; set; }
    }
}