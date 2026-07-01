using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTracker.Server.Models
{
    /// <summary>
    /// Категория расходов
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Уникальный идентификатор категории
        /// </summary>
        [Key]
        public int CategoryId { get; set; }

        /// <summary>
        /// Название категории
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Месячный бюджет в рублях
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyBudget { get; set; }

        /// <summary>
        /// Активна ли категория
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Навигационное свойство, у категории может быть много статей расходов
        public ICollection<ExpenseItem> ExpenseItems { get; set; } = new List<ExpenseItem>();
    }
}