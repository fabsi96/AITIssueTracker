using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;

namespace AITIssueTracker.API.v0._1_Controller
{
    [ApiController]
    [ApiVersion("0.0")]
    [Route(Endpoints.BASE_USER)]
    public class UserController : ControllerBase
    {
        private UserManager Manager { get; }

        public UserController(UserManager manager)
        {
            Manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            List<UserView> allUsers = await Manager.GetUsersAsync();

            if (allUsers is null)
            {
                return BadRequest();
            }

            return Ok(allUsers);
        }

        [HttpPost]
        public async Task<IActionResult> PostNewUserAsync(
            [FromBody] UserForm newUser)
        {
            UserView savedUser = await Manager.SaveNewUserAsync(newUser);

            if (savedUser is null)
            {
                return BadRequest();
            }

            return Ok(savedUser);
        }

        [HttpDelete]
        [Route("{username}")]
        public async Task<IActionResult> DeleteUserByName(
            [FromRoute] string username)
        {
            if (!await Manager.DeleteUserByUsername(username))
            {
                return BadRequest();
            }
            return Ok();
        }

        /* === === */
    }

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


    public class UserContext
    {
        private const string SQL_SELECT_ALL = "select * from \"user\";";
        private const string SQL_INSERT_NEW = "insert into \"user\" (username) values (@username);";
        private const string SQL_DELETE_BY_USERNAME = "delete from \"user\" where username=@username;";

        private string ConnectionString
        {
            get
            {
                return $"Server={Settings.DatabaseIp};Port={Settings.DatabasePort};User Id={Settings.Username};Password={Settings.Password};Database={Settings.Database};Pooling=false;";
            }
        }
        private PsqlSettings Settings { get; }

        public UserContext(PsqlSettings settings)
        {
            Settings = settings;
        }

        public async Task<List<User>> SelectAllUsersAsync()
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_SELECT_ALL;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                List<User> users = new List<User>();
                while (await reader.ReadAsync())
                {
                    users.Add(new User(reader));
                }
                
                await cmd.DisposeAsync();
                await conn.CloseAsync();
                await conn.DisposeAsync();

                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SelectAllUsersAsync: Error: {e.Message}");
                return null;
            }
        }

        public async Task<int> InsertNewUserAsync(User userToSave)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_INSERT_NEW;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = userToSave.Username;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return res == 1 ? 1 : -1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"InsertNewUserAsync: Error: {e.Message}");
                return -1;
            }
        }

        public async Task<bool> DeleteUserByUsernameAsync(string username)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_DELETE_BY_USERNAME;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                await cmd.PrepareAsync();
                int deleteRes = await cmd.ExecuteNonQueryAsync();
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return deleteRes != -1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"DeleteUserByUsernameAsync: Error: {e.Message}");
                return false;
            }
        }
    }
}
