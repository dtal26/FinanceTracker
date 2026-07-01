using FinanceTracker.Server.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTracker.Server.Models
{
    /// <summary>
    /// Транзакция
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Уникальный идентификатор транзакции
        /// </summary>
        [Key]
        public int TransactionId { get; set; }

        /// <summary>
        /// Дата транзакции
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Сумма потраченных денег
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Сумма должна быть положительной")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Комментарий к транзакции
        /// </summary>
        [MaxLength(500)]
        public string? Comment { get; set; }

        /// <summary>
        /// Ссылка на статью расхода
        /// </summary>
        [Required]
        public int ExpenseItemId { get; set; }

        /// <summary>
        /// Навигационное свойство, ссылка на статью расхода
        /// </summary>
        [ForeignKey("ExpenseItemId")]
        public ExpenseItem? ExpenseItem { get; set; }
    }
}