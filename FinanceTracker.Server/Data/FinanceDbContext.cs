using FinanceTracker.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Server.Data
{
    /// <summary>
    /// Контекст базы данных для Entity Framework Core
    /// </summary>
    public class FinanceDbContext : DbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Таблица категорий расходов
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Таблица статей расходов
        /// </summary>
        public DbSet<ExpenseItem> ExpenseItems { get; set; }

        /// <summary>
        /// Таблица транзакций
        /// </summary>
        public DbSet<Transaction> Transactions { get; set; }

        /// <summary>
        /// Настройка связей между таблицами и дополнительных правил
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExpenseItem>()
                .HasOne(e => e.Category)
                .WithMany(c => c.ExpenseItems)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ExpenseItem)
                .WithMany(e => e.Transactions)
                .HasForeignKey(t => t.ExpenseItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Индекс для быстрого поиска транзакций по дате
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.Date);

            // Индекс для быстрого поиска транзакций по статье
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.ExpenseItemId);
        }
    }
}