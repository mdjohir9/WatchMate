using WatchMate_API.DTO;
using WatchMate_API.Entities;

namespace WatchMate_API.Repository
{
    public interface ILoginRepository : IGenericRepository<Users>
    {
        IEnumerable<Users> GetLoginInfo(string userName, string userPassword);
        //string GetUserDepartment(string EmpId);
        //string GetUserDesignation(string EmpId);
        //IEnumerable<string> GetUserPermission(string userId);
        UserProfileDTO GetUserProfileInfo(int Id);
        CustomerInfo GetCustomerInfoByUserId(int UserId);
        //CompanyStatusDTO GetUserCompany(int userId);
        UserRoles GetUserRoleByDataAccessLevel(int dataAccessLevel);

        string GenerateJwtToken(Users user);
    }
}
