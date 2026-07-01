using FinanceTracker.Server.Data;
using FinanceTracker.Server.DTOs.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceTracker.Server.Models;

namespace FinanceTracker.Server.Controllers
{
    /// <summary>
    /// Контроллер для работы с транзакциями
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly FinanceDbContext _context;

        public TransactionsController(FinanceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить список всех транзакций
        /// </summary>
        /// <returns>Список всех транзакций</returns>
        /// <response code="200">Возвращает список транзакций</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAll()
        {
            var transactions = await _context.Transactions
                .Include(t => t.ExpenseItem)
                    .ThenInclude(e => e.Category)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    Date = t.Date,
                    Amount = t.Amount,
                    Comment = t.Comment,
                    ExpenseItemId = t.ExpenseItemId,
                    ExpenseItemName = t.ExpenseItem != null ? t.ExpenseItem.Name : "Неизвестно",
                    CategoryId = t.ExpenseItem != null ? t.ExpenseItem.CategoryId : 0,
                    CategoryName = t.ExpenseItem != null && t.ExpenseItem.Category != null
                        ? t.ExpenseItem.Category.Name : "Неизвестно"
                })
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            return Ok(transactions);
        }

        /// <summary>
        /// Получить транзакции за конкретный день
        /// </summary>
        /// <param name="date">Дата в формате YYYY-MM-DD</param>
        /// <returns>Список транзакций за указанный день</returns>
        /// <response code="200">Возвращает список транзакций за день</response>
        [HttpGet("date/{date}")]
        [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetByDate(DateTime date)
        {
            var transactions = await _context.Transactions
                .Include(t => t.ExpenseItem)
                    .ThenInclude(e => e.Category)
                .Where(t => t.Date.Date == date.Date)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    Date = t.Date,
                    Amount = t.Amount,
                    Comment = t.Comment,
                    ExpenseItemId = t.ExpenseItemId,
                    ExpenseItemName = t.ExpenseItem != null ? t.ExpenseItem.Name : "Неизвестно",
                    CategoryId = t.ExpenseItem != null ? t.ExpenseItem.CategoryId : 0,
                    CategoryName = t.ExpenseItem != null && t.ExpenseItem.Category != null
                        ? t.ExpenseItem.Category.Name : "Неизвестно"
                })
                .OrderBy(t => t.Date)
                .ToListAsync();

            return Ok(transactions);
        }

        /// <summary>
        /// Получить транзакции за месяц
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц (1-12)</param>
        /// <returns>Список транзакций за указанный месяц</returns>
        /// <response code="200">Возвращает список транзакций за месяц</response>
        [HttpGet("month/{year}/{month}")]
        [ProducesResponseType(typeof(IEnumerable<TransactionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TransactionDto>>> GetByMonth(int year, int month)
        {
            if (month < 1 || month > 12)
            {
                return BadRequest(new { message = "Месяц должен быть от 1 до 12" });
            }

            var transactions = await _context.Transactions
                .Include(t => t.ExpenseItem)
                    .ThenInclude(e => e.Category)
                .Where(t => t.Date.Year == year && t.Date.Month == month)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    Date = t.Date,
                    Amount = t.Amount,
                    Comment = t.Comment,
                    ExpenseItemId = t.ExpenseItemId,
                    ExpenseItemName = t.ExpenseItem != null ? t.ExpenseItem.Name : "Неизвестно",
                    CategoryId = t.ExpenseItem != null ? t.ExpenseItem.CategoryId : 0,
                    CategoryName = t.ExpenseItem != null && t.ExpenseItem.Category != null
                        ? t.ExpenseItem.Category.Name : "Неизвестно"
                })
                .OrderBy(t => t.Date)
                .ToListAsync();

            return Ok(transactions);
        }

        /// <summary>
        /// Получить сводку по дню
        /// </summary>
        /// <param name="date">Дата в формате YYYY-MM-DD</param>
        /// <returns>Сводка по дню с цветным стикером</returns>
        /// <response code="200">Возвращает сводку по дню</response>
        [HttpGet("summary/{date}")]
        [ProducesResponseType(typeof(DaySummaryDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<DaySummaryDto>> GetDaySummary(DateTime date)
        {
            var transactions = await _context.Transactions
                .Where(t => t.Date.Date == date.Date)
                .ToListAsync();

            var totalAmount = transactions.Sum(t => t.Amount);
            var transactionCount = transactions.Count;

            string sticker;
            string message;

            if (totalAmount < 500)
            {
                sticker = "green";
                message = "День прошел экономно!";
            }
            else if (totalAmount <= 2000)
            {
                sticker = "yellow";
                message = "Траты в пределах обычного.";
            }
            else
            {
                sticker = "red";
                message = "День был затратным!";
            }

            var summary = new DaySummaryDto
            {
                Date = date.Date,
                TotalAmount = totalAmount,
                Sticker = sticker,
                Message = message,
                TransactionCount = transactionCount
            };

            return Ok(summary);
        }

        /// <summary>
        /// Получить транзакцию по идентификатору
        /// </summary>
        /// <param name="id">ID транзакции</param>
        /// <returns>Информация о транзакции</returns>
        /// <response code="200">Транзакция найдена</response>
        /// <response code="404">Транзакция не найдена</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionDto>> GetById(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.ExpenseItem)
                    .ThenInclude(e => e.Category)
                .Where(t => t.TransactionId == id)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    Date = t.Date,
                    Amount = t.Amount,
                    Comment = t.Comment,
                    ExpenseItemId = t.ExpenseItemId,
                    ExpenseItemName = t.ExpenseItem != null ? t.ExpenseItem.Name : "Неизвестно",
                    CategoryId = t.ExpenseItem != null ? t.ExpenseItem.CategoryId : 0,
                    CategoryName = t.ExpenseItem != null && t.ExpenseItem.Category != null
                        ? t.ExpenseItem.Category.Name : "Неизвестно"
                })
                .FirstOrDefaultAsync();

            if (transaction == null)
            {
                return NotFound(new { message = $"Транзакция с ID {id} не найдена" });
            }

            return Ok(transaction);
        }

        /// <summary>
        /// Создать новую транзакцию
        /// </summary>
        /// <param name="dto">Данные для создания транзакции</param>
        /// <returns>Созданная транзакция</returns>
        /// <response code="201">Транзакция успешно создана</response>
        /// <response code="400">Неверные данные или превышен лимит</response>
        /// <response code="404">Статья расхода не найдена</response>
        [HttpPost]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionDto>> Create([FromBody] CreateTransactionDto dto)
        {
            // Проверяем, существует ли статья расхода
            var expenseItem = await _context.ExpenseItems
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.ExpenseItemId == dto.ExpenseItemId);

            if (expenseItem == null)
            {
                return NotFound(new { message = $"Статья расхода с ID {dto.ExpenseItemId} не найдена" });
            }

            if (!expenseItem.IsActive)
            {
                return BadRequest(new
                {
                    message = $"Нельзя создать транзакцию — статья '{expenseItem.Name}' неактивна"
                });
            }

            var dailyTotal = await _context.Transactions
                .Where(t => t.Date.Date == dto.Date.Date)
                .SumAsync(t => t.Amount);

            if (dailyTotal + dto.Amount > 1_000_000)
            {
                return BadRequest(new
                {
                    message = $"Превышен дневной лимит в 1 000 000 рублей. Текущая сумма за день: {dailyTotal} руб., добавляемая сумма: {dto.Amount} руб."
                });
            }

            var transaction = new Transaction
            {
                Date = dto.Date.Date, 
                Amount = dto.Amount,
                Comment = dto.Comment,
                ExpenseItemId = dto.ExpenseItemId
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            var result = new TransactionDto
            {
                TransactionId = transaction.TransactionId,
                Date = transaction.Date,
                Amount = transaction.Amount,
                Comment = transaction.Comment,
                ExpenseItemId = transaction.ExpenseItemId,
                ExpenseItemName = expenseItem.Name,
                CategoryId = expenseItem.CategoryId,
                CategoryName = expenseItem.Category?.Name ?? "Неизвестно"
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = transaction.TransactionId },
                result);
        }

        /// <summary>
        /// Обновить существующую транзакцию
        /// </summary>
        /// <param name="id">ID обновляемой транзакции</param>
        /// <param name="dto">Новые данные</param>
        /// <returns>Обновлённая транзакция</returns>
        /// <response code="200">Транзакция успешно обновлена</response>
        /// <response code="400">Неверные данные или статья стала неактивной</response>
        /// <response code="404">Транзакция не найдена</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionDto>> Update(int id, [FromBody] UpdateTransactionDto dto)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound(new { message = $"Транзакция с ID {id} не найдена" });
            }

            var currentExpenseItem = await _context.ExpenseItems.FindAsync(transaction.ExpenseItemId);

            if (currentExpenseItem != null && !currentExpenseItem.IsActive && dto.ExpenseItemId.HasValue)
            {
                return BadRequest(new
                {
                    message = $"Нельзя изменить статью расхода — статья '{currentExpenseItem.Name}' стала неактивной"
                });
            }

            if (dto.ExpenseItemId.HasValue && dto.ExpenseItemId.Value != transaction.ExpenseItemId)
            {
                var newExpenseItem = await _context.ExpenseItems
                    .Include(e => e.Category)
                    .FirstOrDefaultAsync(e => e.ExpenseItemId == dto.ExpenseItemId.Value);

                if (newExpenseItem == null)
                {
                    return NotFound(new { message = $"Статья расхода с ID {dto.ExpenseItemId} не найдена" });
                }

                if (!newExpenseItem.IsActive)
                {
                    return BadRequest(new
                    {
                        message = $"Нельзя выбрать неактивную статью '{newExpenseItem.Name}'"
                    });
                }

                transaction.ExpenseItemId = dto.ExpenseItemId.Value;
            }

            if (dto.Date.Date != transaction.Date || dto.Amount != transaction.Amount)
            {
                var dailyTotal = await _context.Transactions
                    .Where(t => t.Date.Date == dto.Date.Date && t.TransactionId != id)
                    .SumAsync(t => t.Amount);

                if (dailyTotal + dto.Amount > 1_000_000)
                {
                    return BadRequest(new
                    {
                        message = $"Превышен дневной лимит в 1 000 000 рублей"
                    });
                }
            }

            transaction.Date = dto.Date.Date;
            transaction.Amount = dto.Amount;
            transaction.Comment = dto.Comment;

            await _context.SaveChangesAsync();

            var updatedTransaction = await _context.Transactions
                .Include(t => t.ExpenseItem)
                    .ThenInclude(e => e.Category)
                .Where(t => t.TransactionId == id)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    Date = t.Date,
                    Amount = t.Amount,
                    Comment = t.Comment,
                    ExpenseItemId = t.ExpenseItemId,
                    ExpenseItemName = t.ExpenseItem != null ? t.ExpenseItem.Name : "Неизвестно",
                    CategoryId = t.ExpenseItem != null ? t.ExpenseItem.CategoryId : 0,
                    CategoryName = t.ExpenseItem != null && t.ExpenseItem.Category != null
                        ? t.ExpenseItem.Category.Name : "Неизвестно"
                })
                .FirstOrDefaultAsync();

            return Ok(updatedTransaction);
        }

        /// <summary>
        /// Удалить транзакцию
        /// </summary>
        /// <param name="id">ID удаляемой транзакции</param>
        /// <response code="204">Транзакция успешно удалена</response>
        /// <response code="404">Транзакция не найдена</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound(new { message = $"Транзакция с ID {id} не найдена" });
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}