using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.Model.v0._2_EntityModel;
using Npgsql;
using NpgsqlTypes;

namespace AITIssueTracker.API.v0._3_DAL
{
    public class TestContext
    {
        private const string SQL_INSERT_TEST = "insert into \"test\" (name, age, birth_date, status) values (@name, @age, @birth_date, @status);";
        private const string SQL_SELECT_ALL = "select name, age, birth_date, status from \"test\";";
        private const string SQL_DELETE_BY_NAME = "delete from \"test\" where name=@name;";

        private PsqlSettings Settings { get; }
        private string ConnectionString
        {
            get
            {
                return $"Server={Settings.DatabaseIp};Port={Settings.DatabasePort};User Id={Settings.Username};Password={Settings.Password};Database={Settings.Database};Pooling=false;";
            }
        }
        public TestContext(PsqlSettings settings)
        {
            Settings = settings;
        }

        public async Task<int> InsertNewAsync(Test itemToSave)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_INSERT_TEST;
                cmd.Parameters.Add("@name", NpgsqlDbType.Text).Value = itemToSave.Name;
                cmd.Parameters.Add("@age", NpgsqlDbType.Smallint).Value = itemToSave.Age;
                cmd.Parameters.Add("@birth_date", NpgsqlDbType.Timestamp).Value = itemToSave.BirthDate;
                cmd.Parameters.Add("@status", NpgsqlDbType.Smallint).Value = (int) itemToSave.Status;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return res != 1 ? -1 : 1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"InsertNewAsync: Error: {e.Message}");
                return -1;
            }
        }

        public async Task<List<Test>> SelectAllAsync()
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_SELECT_ALL;

                await cmd.PrepareAsync();
                NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                List<Test> allItems = new List<Test>();
                while (await reader.ReadAsync())
                {
                    allItems.Add(new Test(reader));
                }

                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return allItems;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SeelctAllAsync: Error: {e.Message}");
                return null;
            }
        }

        public async Task<int> DeleteByNameAsync(string identifier)
        {
            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = SQL_DELETE_BY_NAME;
                cmd.Parameters.Add("@name", NpgsqlDbType.Text).Value = identifier;

                await cmd.PrepareAsync();
                int res = await cmd.ExecuteNonQueryAsync();
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                return res == 1 ? 1 : -1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SeelctAllAsync: Error: {e.Message}");
                return -1;
            }
        }

        public async Task DatabaseTryCatch()
        {

            try
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                await using NpgsqlCommand cmd = conn.CreateCommand();
                // TODO
                cmd.CommandText = "";
                // cmd.Parameters.Add("@", NpgsqlDbType.).Value = ;

                await cmd.PrepareAsync();
                // TODO
                await cmd.DisposeAsync();

                await conn.CloseAsync();
                await conn.DisposeAsync();

                // TODO
            }
            catch (Exception e)
            {
                Console.WriteLine($"SeelctAllAsync: Error: {e.Message}");
                // TODO
            }
        }
    }
}