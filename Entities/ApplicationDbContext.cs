using Microsoft.EntityFrameworkCore;

namespace WatchMate_API.Entities
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<UserRoles> UserRole { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Package> Package { get; set; }
        public DbSet<CustomerPackage> CustomerPackage { get; set; }
        public DbSet<CustomerInfo> CustomerInfo { get; set; }
        public DbSet<AdVideo> AdVideo { get; set; }
        public DbSet<Transctions> Transctions { get; set; }
        public DbSet<TransactionType> TransactionType { get; set; }
        public DbSet<PaymentMethod> PaymentMethod { get; set; }
        public DbSet<PaymentAccount> PaymentAccount { get; set; }
        public DbSet<AccountBalance> AccountBalance { get; set; }
        public DbSet<Withdraw> Withdraw { get; set; }

    }
}
