using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.Model.v0._2_EntityModel;
using Npgsql;
using NpgsqlTypes;

namespace AITIssueTracker.API.v0._3_DAL
{
    public class FeatureContext : PsqlMaster
    {
        // === Basic ===
        private const string SQL_SELECT_FEATURE_BY_ID = "select * from \"feature\" where id=@id;";
        private const string SQL_SELECT_BY_PROJECT_TITLE = "select * from \"feature\" where project_id=@project_id;";

        private const string SQL_INSERT_NEW = "insert into \"feature\" (project_id, title, description, deadline, startdate, status) " +
                                              " values (@project_id, @title, @description, @deadline, @startdate, @status);";

        private const string SQL_DELETE_BY_TITLE = "delete from \"feature\" where id=@id;";

        // === Extended ===
        private const string SQL_INSERT_USER_TO_FEATURE = "insert into \"feature_user\" (feature_id, username) values (@feature_id, @username);";
        private const string SQL_DELETE_USER_FROM_FEATURE = "delete from \"feature_user\" where feature_id=@feature_id and username=@username";
        private const string SQL_SELECT_PROJECT_BY_FEATURE = "select * from \"project\" as p join \"feature\" as f on p.id = f.project_id and f.id = @feature_id;";


        public FeatureContext(PsqlSettings settings) : base (settings)
        {
        }

        public async Task<int> InsertNewFeatureAsync(Feature featureToSave, Guid projectId)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_INSERT_NEW;
                cmd.Parameters.Add("@title", NpgsqlDbType.Text).Value = featureToSave.Title;
                cmd.Parameters.Add("@description", NpgsqlDbType.Text).Value = featureToSave.Description;
                cmd.Parameters.Add("@deadline", NpgsqlDbType.Timestamp).Value = featureToSave.Deadline;
                cmd.Parameters.Add("@startdate", NpgsqlDbType.Timestamp).Value = featureToSave.StartDate;
                cmd.Parameters.Add("@status", NpgsqlDbType.Smallint).Value = (int)featureToSave.Status;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                return res == 1 ? 1 : -1;

            }, -1);
        }

        public async Task<List<Feature>> SelectFeaturesByProjectTitleAsync(Guid projectId)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_SELECT_BY_PROJECT_TITLE;
                cmd.Parameters.Add("@project_id", NpgsqlDbType.Uuid).Value = projectId;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                List<Feature> projectFeatures = new List<Feature>();
                while (await reader.ReadAsync())
                {
                    projectFeatures.Add(new Feature(reader));
                }
                return projectFeatures;

            }, null);
        }

        public async Task<bool> DeleteFeatureByIdentifier(Guid featureId)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_DELETE_BY_TITLE;
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = featureId;

                await cmd.PrepareAsync();
                int deleteResult = await cmd.ExecuteNonQueryAsync();
                return deleteResult != -1;
            }, false);
        }

        public async Task<int> InsertUserToFeatureAsync(Guid featureId, string username)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_INSERT_USER_TO_FEATURE;
                cmd.Parameters.Add("@feature_id", NpgsqlDbType.Uuid).Value = featureId;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                return res != -1 ? 1 : -1;

            }, -1);
        }

        public async Task<int> DeleteUserFromFeatureAsync(Guid featureId, string username)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_DELETE_USER_FROM_FEATURE;
                cmd.Parameters.Add("@feature_id", NpgsqlDbType.Uuid).Value = featureId;
                cmd.Parameters.Add("@username", NpgsqlDbType.Text).Value = username;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();

                return res != -1 ? 1 : -1;

            }, -1);
        }

        public async Task<Project> SelectProjectOfFeatureAsync(Guid featureId)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_SELECT_PROJECT_BY_FEATURE;
                cmd.Parameters.Add("@feature_id", NpgsqlDbType.Uuid).Value = featureId;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                Project project = default;
                if (!await reader.ReadAsync())
                {
                    project = null;
                }
                else
                {
                    project = new Project(reader);
                }
                return project;
            }, null);
        }

        public async Task<Feature> SelectFeatureByIdAsync(Guid featureId)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_SELECT_FEATURE_BY_ID;
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = featureId;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

                Feature feature = default;
                if (!await reader.ReadAsync())
                {
                    feature = null;
                }
                else
                {
                    feature = new Feature(reader);
                }

                return feature;
            }, null);
        }

        public async Task<int> UpdateFeatureStatusAsync(Guid featureId, FeatureStatus newStatus)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                string SQL_UPDATE_FEATURE_STATUS = "update \"feature\" set status=@status where id=@id";
                cmd.CommandText = SQL_UPDATE_FEATURE_STATUS;
                cmd.Parameters.Add("@status", NpgsqlDbType.Varchar).Value = newStatus.ToString();
                cmd.Parameters.Add("@id", NpgsqlDbType.Uuid).Value = featureId;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                return res != -1 ? 1 : -1;

            }, -1);
        }
    }
}