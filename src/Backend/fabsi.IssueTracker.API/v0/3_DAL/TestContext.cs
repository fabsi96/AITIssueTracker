using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.Model.v0._2_EntityModel;
using Microsoft.VisualBasic.CompilerServices;
using Npgsql;
using NpgsqlTypes;

namespace AITIssueTracker.API.v0._3_DAL
{
    public class TestContext : PsqlMaster
    {
        private const string SQL_INSERT_TEST = "insert into \"test\" (name, age, birth_date, status) values (@name, @age, @birth_date, @status);";
        private const string SQL_SELECT_ALL = "select name, age, birth_date, status from \"test\";";
        private const string SQL_DELETE_BY_NAME = "delete from \"test\" where name=@name;";

        public TestContext(PsqlSettings settings) : base(settings)
        {
        }


        public async Task<List<Test>> SelectAllAsync()
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_SELECT_ALL;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                List<Test> allItems = new List<Test>();
                while (await reader.ReadAsync())
                    allItems.Add(new Test(reader));

                return allItems;
            }, null);
        }
        public async Task<int> InsertNewAsync(Test itemToSave)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_INSERT_TEST;
                cmd.Parameters.Add("@name", NpgsqlDbType.Text).Value = itemToSave.Name;
                cmd.Parameters.Add("@age", NpgsqlDbType.Smallint).Value = itemToSave.Age;
                cmd.Parameters.Add("@birth_date", NpgsqlDbType.Timestamp).Value = itemToSave.BirthDate;
                cmd.Parameters.Add("@status", NpgsqlDbType.Smallint).Value = (int) itemToSave.Status;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                return res != -1 ? 1 : -1;
            }, -1);
        }


        public async Task<int> DeleteByNameAsync(string identifier)
        {
            return await ExecuteSqlAsync(async (cmd) =>
            {
                cmd.CommandText = SQL_DELETE_BY_NAME;
                cmd.Parameters.Add("@name", NpgsqlDbType.Text).Value = identifier;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                return res == 1 ? 1 : -1;
            }, -1);
        }

        public async Task DatabaseTryCatch()
        {
            int val = await ExecuteSqlAsync<int>(async (cmd) =>
            {
                // cmd.CommandText = SQL_SELECT_STATEMENT;

                return 0;
            }, -1);
        }
    }
}