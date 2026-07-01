using FinanceTracker.Server.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTracker.Server.Models
{
    /// <summary>
    /// Статья расхода
    /// </summary>
    public class ExpenseItem
    {
        /// <summary>
        /// Уникальный идентификатор статьи
        /// </summary>
        [Key]
        public int ExpenseItemId { get; set; }

        /// <summary>
        /// Название статьи расхода
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Ссылка на категорию
        /// </summary>
        [Required]
        public int CategoryId { get; set; }

        /// <summary>
        /// Навигационное свойство
        /// </summary>
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        /// <summary>
        /// Активна ли статья
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Навигационное свойство, у статьи может быть много транзакций
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}