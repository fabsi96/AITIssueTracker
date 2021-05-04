using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.Model.v0._2_EntityModel;
using Npgsql;
using NpgsqlTypes;

namespace AITIssueTracker.API.v0._3_DAL
{
    public class UserContext : PsqlMaster
    {
        private const string SQL_SELECT_ALL = "select * from \"user\";";
        private const string SQL_INSERT_NEW = "insert into \"user\" (username) values (@username);";
        private const string SQL_DELETE_BY_USERNAME = "delete from \"user\" where username=@username;";

        public UserContext(PsqlSettings settings) : base(settings)
        {
        }

        public async Task<List<User>> SelectAllUsersAsync()
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_SELECT_ALL;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                List<User> users = new List<User>();
                while (await reader.ReadAsync())
                    users.Add(new User(reader));

                return users;
            }, null);
        }

        public async Task<int> InsertNewUserAsync(User userToSave)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_INSERT_NEW;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = userToSave.Username;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                return res == 1 ? 1 : -1;

            }, -1);
        }

        public async Task<bool> DeleteUserByUsernameAsync(string username)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_DELETE_BY_USERNAME;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                await cmd.PrepareAsync();
                int deleteRes = await cmd.ExecuteNonQueryAsync();
                return deleteRes != -1;
            }, false);
        }
    }
}