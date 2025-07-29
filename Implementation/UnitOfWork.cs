using Microsoft.EntityFrameworkCore.Storage;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;


        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public UnitOfWork(ApplicationDbContext dbContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _connectionString = _configuration.GetConnectionString("DbConnection");
            User = new UserRepository(_dbContext, _httpContextAccessor);
            Package = new PackageRepository(_dbContext, _httpContextAccessor);
            UserPackages = new CustomerPackageRepository(_dbContext, _httpContextAccessor);
            UserRole = new UserRoleRepository(_dbContext);
            Login = new LoginRepository(_dbContext, _configuration);
            CustomerInfo = new CustomerlInfoRepository(_dbContext, _httpContextAccessor);
            Video = new VideoRepository(_dbContext, _httpContextAccessor);
            Transaction = new TransctionRepository(_dbContext, _httpContextAccessor);
            Account = new AccountRepository(_dbContext);
            Withdraw = new WithdrawRepository(_dbContext, _httpContextAccessor);

        }
        public IUserRepository User { get; private set; }
        public IUserRoleRepository UserRole { get; private set; }
        public ILoginRepository Login { get; private set; }
        public IPackageRepository Package { get; private set; }
        public ICustomerPackageRepository UserPackages { get; private set; }
        public ICustomerlInfoRepository CustomerInfo { get; private set; }
        public IVideoRepository Video { get; private set; }
        public ITransctionRepository Transaction { get; private set; }
        public IAccountRepository Account { get; private set; }

        public IWithdrawRepository Withdraw { get; private set; }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public Task<int> Save()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}
