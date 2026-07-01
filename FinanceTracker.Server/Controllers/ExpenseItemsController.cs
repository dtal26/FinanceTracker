using FinanceTracker.Server.Data;
using FinanceTracker.Server.DTOs.ExpenseItems;
using FinanceTracker.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Server.Controllers
{
    /// <summary>
    /// Контроллер для работы со статьями расходов
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseItemsController : ControllerBase
    {
        private readonly FinanceDbContext _context;

        public ExpenseItemsController(FinanceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить список всех статей расходов
        /// </summary>
        /// <returns>Список статей</returns>
        /// <response code="200">Возвращает список статей</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ExpenseItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ExpenseItemDto>>> GetAll()
        {
            var items = await _context.ExpenseItems
                .Include(e => e.Category)
                .Select(e => new ExpenseItemDto
                {
                    ExpenseItemId = e.ExpenseItemId,
                    Name = e.Name,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category != null ? e.Category.Name : "Неизвестно",
                    IsActive = e.IsActive,
                    TransactionsCount = e.Transactions.Count
                })
                .ToListAsync();

            return Ok(items);
        }

        /// <summary>
        /// Получить только активные статьи расходов
        /// </summary>
        /// <returns>Список активных статей</returns>
        /// <response code="200">Возвращает список активных статей</response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<ExpenseItemDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ExpenseItemDto>>> GetActive()
        {
            var items = await _context.ExpenseItems
                .Include(e => e.Category)
                .Where(e => e.IsActive)
                .Select(e => new ExpenseItemDto
                {
                    ExpenseItemId = e.ExpenseItemId,
                    Name = e.Name,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category != null ? e.Category.Name : "Неизвестно",
                    IsActive = e.IsActive,
                    TransactionsCount = e.Transactions.Count
                })
                .ToListAsync();

            return Ok(items);
        }

        /// <summary>
        /// Получить статью по идентификатору
        /// </summary>
        /// <param name="id">ID статьи</param>
        /// <returns>Информация о статье</returns>
        /// <response code="200">Статья найдена</response>
        /// <response code="404">Статья не найдена</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExpenseItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExpenseItemDto>> GetById(int id)
        {
            var item = await _context.ExpenseItems
                .Include(e => e.Category)
                .Where(e => e.ExpenseItemId == id)
                .Select(e => new ExpenseItemDto
                {
                    ExpenseItemId = e.ExpenseItemId,
                    Name = e.Name,
                    CategoryId = e.CategoryId,
                    CategoryName = e.Category != null ? e.Category.Name : "Неизвестно",
                    IsActive = e.IsActive,
                    TransactionsCount = e.Transactions.Count
                })
                .FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound(new { message = $"Статья с ID {id} не найдена" });
            }

            return Ok(item);
        }

        /// <summary>
        /// Создать новую статью расхода
        /// </summary>
        /// <param name="dto">Данные для создания статьи</param>
        /// <returns>Созданная статья</returns>
        /// <response code="201">Статья успешно создана</response>
        /// <response code="400">Неверные данные или неактивная категория</response>
        /// <response code="404">Категория не найдена</response>
        [HttpPost]
        [ProducesResponseType(typeof(ExpenseItemDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExpenseItemDto>> Create([FromBody] CreateExpenseItemDto dto)
        {
            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
            {
                return NotFound(new { message = $"Категория с ID {dto.CategoryId} не найдена" });
            }

            if (!category.IsActive)
            {
                return BadRequest(new
                {
                    message = $"Нельзя создать статью, категория '{category.Name}' неактивна. Сначала активируйте категорию."
                });
            }

            var item = new ExpenseItem
            {
                Name = dto.Name,
                CategoryId = dto.CategoryId,
                IsActive = dto.IsActive
            };

            _context.ExpenseItems.Add(item);
            await _context.SaveChangesAsync();

            var result = new ExpenseItemDto
            {
                ExpenseItemId = item.ExpenseItemId,
                Name = item.Name,
                CategoryId = item.CategoryId,
                CategoryName = category.Name,
                IsActive = item.IsActive,
                TransactionsCount = 0
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = item.ExpenseItemId },
                result);
        }

        /// <summary>
        /// Обновить существующую статью расхода
        /// </summary>
        /// <param name="id">ID обновляемой статьи</param>
        /// <param name="dto">Новые данные</param>
        /// <returns>Обновлённая статья</returns>
        /// <response code="200">Статья успешно обновлена</response>
        /// <response code="400">Неверные данные или неактивная категория</response>
        /// <response code="404">Статья или категория не найдена</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ExpenseItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExpenseItemDto>> Update(int id, [FromBody] UpdateExpenseItemDto dto)
        {
            var item = await _context.ExpenseItems.FindAsync(id);
            if (item == null)
            {
                return NotFound(new { message = $"Статья с ID {id} не найдена" });
            }

            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
            {
                return NotFound(new { message = $"Категория с ID {dto.CategoryId} не найдена" });
            }

            if (!category.IsActive)
            {
                return BadRequest(new
                {
                    message = $"Нельзя привязать статью к неактивной категории '{category.Name}'"
                });
            }

            item.Name = dto.Name;
            item.CategoryId = dto.CategoryId;
            item.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            var result = new ExpenseItemDto
            {
                ExpenseItemId = item.ExpenseItemId,
                Name = item.Name,
                CategoryId = item.CategoryId,
                CategoryName = category.Name,
                IsActive = item.IsActive,
                TransactionsCount = item.Transactions.Count
            };

            return Ok(result);
        }

        /// <summary>
        /// Удалить статью расхода
        /// </summary>
        /// <param name="id">ID удаляемой статьи</param>
        /// <response code="204">Статья успешно удалена</response>
        /// <response code="404">Статья не найдена</response>
        /// <response code="409">Статья не может быть удалена (есть связанные транзакции)</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.ExpenseItems
                .Include(e => e.Transactions)
                .FirstOrDefaultAsync(e => e.ExpenseItemId == id);

            if (item == null)
            {
                return NotFound(new { message = $"Статья с ID {id} не найдена" });
            }

            if (item.Transactions.Any())
            {
                return Conflict(new
                {
                    message = "Нельзя удалить статью, так как по ней есть транзакции. Сначала удалите или перенесите транзакции."
                });
            }

            _context.ExpenseItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}