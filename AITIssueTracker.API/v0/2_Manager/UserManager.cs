using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;

namespace AITIssueTracker.API.v0._2_Manager
{
    public class UserManager
    {
        private UserContext DbContext { get; }

        public UserManager(UserContext ctx)
        {
            DbContext = ctx;
        }

        public async Task<List<UserView>> GetUsersAsync(string filter="")
        {
            List<User> users = await DbContext.SelectAllUsersAsync();

            return users?.ConvertAll(c => c.AsView());
        }

        public async Task<UserView> SaveNewUserAsync(UserForm user)
        {
            User userToSave = new User(user);
            return await DbContext.InsertNewUserAsync(userToSave) == 1 ? userToSave.AsView() : null;
        }

        public async Task<bool> DeleteUserByUsername(string username)
        {
            return await DbContext.DeleteUserByUsernameAsync(username);
        }
    }
}