using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.Model.v0._2_EntityModel;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Npgsql;
using NpgsqlTypes;

namespace AITIssueTracker.API.v0._3_DAL
{
    public class ProjectContext : PsqlMaster
    {
        private const string SQL_INSERT_NEW = "insert into \"project\" (title, description, is_done) values (@title, @description, @is_done);";
        private const string SQL_SELECT_ALL = "select id, title, description, is_done from \"project\";";
        private const string SQL_DELETE_BY_TITLE = "delete from \"project\" where id=@id;";

        private const string SQL_INSERT_USER_TO_PROJECT = "insert into \"project_user\" (project_id, username) values (@project_id, @username);";
        private const string SQL_DELETE_USER_FROM_PROJECT = "delete from \"project_user\" where project_id=@project_id and username=@username;";
        private const string SQL_USER_EXISTS_IN_PROJECT = "select * from \"project_user\" where username=@username and project_id=@project_id;";

        public ProjectContext(PsqlSettings settings) : base(settings)
        {
        }

        public async Task<int> InsertNewAsync(Project newProject)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_INSERT_NEW;
                cmd.Parameters.Add("@title", NpgsqlDbType.Text).Value = newProject.Title;
                cmd.Parameters.Add("@description", NpgsqlDbType.Text).Value = newProject.Description;
                cmd.Parameters.Add("@is_done", NpgsqlDbType.Boolean).Value = newProject.IsDone;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                return res != 1 ? -1 : 1;

            }, -1);
        }

        public async Task<List<Project>> SelectAllAsync()
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_SELECT_ALL;

                await cmd.PrepareAsync();
                List<Project> projects = new List<Project>();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    projects.Add(new Project(reader));
                }
                return projects;

            }, null);
        }

        public async Task<bool> DeleteByTitleAsync(Guid id)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_DELETE_BY_TITLE;
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = id;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                return res != -1;

            }, false);
        }

        public async Task<int> InsertUserToProjectAsync(string username, Guid projectId)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_INSERT_USER_TO_PROJECT;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                return res != -1 ? 1 : -1;

            }, -1);
        }

        public async Task<int> DeleteUserFromProjectAsync(string username, Guid projectId)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_DELETE_USER_FROM_PROJECT;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                return res != -1 ? 1 : -1;
            }, -1);
        }

        public async Task<bool> SelectUserIsInProjectAsync(string username, Guid projectId)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_USER_EXISTS_IN_PROJECT;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                bool userExists = reader.HasRows;
                return userExists;

            }, false);
        }
    }
}