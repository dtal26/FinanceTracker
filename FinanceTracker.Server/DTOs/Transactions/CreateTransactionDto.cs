using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Server.DTOs.Transactions
{
    /// <summary>
    /// DTO для создания новой транзакции
    /// </summary>
    public class CreateTransactionDto
    {
        /// <summary>
        /// Дата транзакции
        /// </summary>
        [Required(ErrorMessage = "Дата транзакции обязательна")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Сумма потраченных денег
        /// </summary>
        [Required(ErrorMessage = "Сумма обязательна")]
        [Range(0.01, 1000000, ErrorMessage = "Сумма должна быть от 0.01 до 1 000 000 рублей")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Комментарий к транзакции
        /// </summary>
        [StringLength(500, ErrorMessage = "Комментарий не должен превышать 500 символов")]
        public string? Comment { get; set; }

        /// <summary>
        /// ID статьи расхода
        /// </summary>
        [Required(ErrorMessage = "Статья расхода обязательна")]
        [Range(1, int.MaxValue, ErrorMessage = "Неверный ID статьи расхода")]
        public int ExpenseItemId { get; set; }
    }
}