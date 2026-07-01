using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Server.DTOs.ExpenseItems
{
    /// <summary>
    /// DTO для создания новой статьи расхода
    /// </summary>
    public class CreateExpenseItemDto
    {
        /// <summary>
        /// Название статьи расхода
        /// </summary>
        [Required(ErrorMessage = "Название статьи обязательно")]
        [StringLength(100, MinimumLength = 1,
            ErrorMessage = "Название должно содержать от 1 до 100 символов")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// ID категории, к которой относится статья
        /// </summary>
        [Required(ErrorMessage = "Категория обязательна")]
        [Range(1, int.MaxValue, ErrorMessage = "Неверный ID категории")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Активна ли статья
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}