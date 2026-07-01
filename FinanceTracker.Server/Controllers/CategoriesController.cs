using FinanceTracker.Server.Data;
using FinanceTracker.Server.DTOs.Categories;
using FinanceTracker.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Server.Controllers
{
    /// <summary>
    /// Контроллер для работы с категориями расходов
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly FinanceDbContext _context;

        /// <summary>
        /// Конструктор контроллера
        /// </summary>
        public CategoriesController(FinanceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить список всех категорий расходов
        /// </summary>
        /// <returns>Список категорий</returns>
        /// <response code="200">Возвращает список категорий</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var categories = await _context.Categories
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    MonthlyBudget = c.MonthlyBudget,
                    IsActive = c.IsActive,
                    ExpenseItemsCount = c.ExpenseItems.Count
                })
                .ToListAsync();

            return Ok(categories);
        }

        /// <summary>
        /// Получить категорию по идентификатору
        /// </summary>
        /// <param name="id">ID категории</param>
        /// <returns>Информация о категории</returns>
        /// <response code="200">Категория найдена</response>
        /// <response code="404">Категория не найдена</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await _context.Categories
                .Where(c => c.CategoryId == id)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    MonthlyBudget = c.MonthlyBudget,
                    IsActive = c.IsActive,
                    ExpenseItemsCount = c.ExpenseItems.Count
                })
                .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(new { message = $"Категория с ID {id} не найдена" });
            }

            return Ok(category);
        }

        /// <summary>
        /// Создать новую категорию расходов
        /// </summary>
        /// <param name="dto">Данные для создания категории</param>
        /// <returns>Созданная категория</returns>
        /// <response code="201">Категория успешно создана</response>
        /// <response code="400">Неверные данные</response>
        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                MonthlyBudget = dto.MonthlyBudget,
                IsActive = dto.IsActive
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var result = new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                MonthlyBudget = category.MonthlyBudget,
                IsActive = category.IsActive,
                ExpenseItemsCount = 0
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = category.CategoryId },
                result);
        }

        /// <summary>
        /// Обновить существующую категорию
        /// </summary>
        /// <param name="id">ID обновляемой категории</param>
        /// <param name="dto">Новые данные</param>
        /// <returns>Обновлённая категория</returns>
        /// <response code="200">Категория успешно обновлена</response>
        /// <response code="400">Неверные данные</response>
        /// <response code="404">Категория не найдена</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(new { message = $"Категория с ID {id} не найдена" });
            }

            category.Name = dto.Name;
            category.MonthlyBudget = dto.MonthlyBudget;
            category.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            var result = new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                MonthlyBudget = category.MonthlyBudget,
                IsActive = category.IsActive,
                ExpenseItemsCount = category.ExpenseItems.Count
            };

            return Ok(result);
        }

        /// <summary>
        /// Удалить категорию расходов
        /// </summary>
        /// <param name="id">ID удаляемой категории</param>
        /// <response code="204">Категория успешно удалена</response>
        /// <response code="404">Категория не найдена</response>
        /// <response code="409">Категория не может быть удалена (есть связанные статьи)</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .Include(c => c.ExpenseItems)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound(new { message = $"Категория с ID {id} не найдена" });
            }

            if (category.ExpenseItems.Any())
            {
                return Conflict(new
                {
                    message = "Нельзя удалить категорию, так как в ней есть статьи расходов. Сначала удалите или переместите статьи."
                });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}