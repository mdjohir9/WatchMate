using Microsoft.EntityFrameworkCore;
using WatchMate_API.DTO;
using WatchMate_API.Entities;
using WatchMate_API.Repository;

namespace WatchMate_API.Implementation
{
    public class PackageRepository : GenericRepository<Package>, IPackageRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
   
        public PackageRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) : base(dbContext)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Package>> GetActivePackagesAsync()
        {
            return await _dbContext.Package
                 .Where(p => p.Status == 1 && p.IsFree !=1)
                .ToListAsync();
        }
        public async Task<Package> GetFreePackagesAsync()
        {
            return await _dbContext.Package
                .Where(p => p.IsFree == 1 && p.Deleted != true && p.Status == 1)
                .FirstOrDefaultAsync();
        }

    }
}
