using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Server.DTOs.Categories
{
    /// <summary>
    /// DTO для создания новой категории расходов
    /// </summary>
    public class CreateCategoryDto
    {
        /// <summary>
        /// Название категории
        /// </summary>
        [Required(ErrorMessage = "Название категории обязательно")]
        [StringLength(100, MinimumLength = 1,
            ErrorMessage = "Название должно содержать от 1 до 100 символов")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Месячный бюджет в рублях
        /// </summary>
        [Range(0, double.MaxValue,
            ErrorMessage = "Месячный бюджет не может быть отрицательным")]
        public decimal MonthlyBudget { get; set; }

        /// <summary>
        /// Активна ли категория
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}