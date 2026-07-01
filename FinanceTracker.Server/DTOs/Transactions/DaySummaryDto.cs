namespace FinanceTracker.Server.DTOs.Transactions
{
    /// <summary>
    /// DTO для отображения сводки по дню
    /// </summary>
    public class DaySummaryDto
    {
        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Общая сумма трат за день
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Цвет стикера
        /// </summary>
        public string Sticker { get; set; } = string.Empty;

        /// <summary>
        /// Сообщение о статусе дня
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Количество транзакций за день
        /// </summary>
        public int TransactionCount { get; set; }
    }
}