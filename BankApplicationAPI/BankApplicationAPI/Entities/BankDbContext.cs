using Microsoft.EntityFrameworkCore;

namespace BankApplicationAPI.Entities
{
    public class BankDbContext : DbContext
    {
        private string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=BankDatabase;Trusted_Connection=True";
        public DbSet<User>Users { get; set; }
        public DbSet<Account>Accounts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Transfer> Transfers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Accounts)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);

            // Configure Account
            modelBuilder.Entity<Account>()
                .HasKey(a => a.AccountNumber);

            modelBuilder.Entity<Account>()
                .Property(a => a.AccountNumber)
                .ValueGeneratedNever();

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Payments)
                .WithOne(t => t.Account)
                .HasForeignKey(t => t.AccountNumber);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.TransfersOut)
                .WithOne(t => t.SourceAccount)
                .HasForeignKey(t => t.SourceAccountNumber)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.TransfersIn)
                .WithOne(t => t.DestinationAccount)
                .HasForeignKey(t => t.DestinationAccountNumber)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Payment
            modelBuilder.Entity<Payment>()
                .HasKey(t => t.PaymentId);

            modelBuilder.Entity<Payment>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Payments)
                .HasForeignKey(t => t.AccountNumber);

            // Configure Transfer
            modelBuilder.Entity<Transfer>()
                .HasKey(t => t.TransferId);

            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.SourceAccount)
                .WithMany(a => a.TransfersOut)
                .HasForeignKey(t => t.SourceAccountNumber)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.DestinationAccount)
                .WithMany(a => a.TransfersIn)
                .HasForeignKey(t => t.DestinationAccountNumber)
                .OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
