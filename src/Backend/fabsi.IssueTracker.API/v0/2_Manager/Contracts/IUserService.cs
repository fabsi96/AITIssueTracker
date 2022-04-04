using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._3_ViewModel;

namespace AITIssueTracker.API.v0._2_Manager.Contracts
{
    public interface IUserService
    {
        Task<List<UserView>> GetAllUsersAsync();
        Task<UserView> SaveUserAsync(UserForm newUser);
        Task DeleteUserByUsernameAsync(string username);
        bool UserExists(string newUser);
    }
}