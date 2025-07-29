using WatchMate_API.Entities;

namespace WatchMate_API.Repository
{
    public interface IVideoRepository : IGenericRepository<AdVideo>
    {
        Task<string> SaveVideoAsync(IFormFile videoFile, HttpRequest request);
        Task<IEnumerable<object>> GetCustomerAdVideos(int customerId);



    }
}
