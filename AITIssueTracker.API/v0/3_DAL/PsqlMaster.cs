using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace AITIssueTracker.API.v0._3_DAL
{
    public class PsqlMaster
    {
        protected PsqlSettings Settings { get; }
        protected string ConnectionString
        {
            get
            {
                return $"Server={Settings.DatabaseIp};Port={Settings.DatabasePort};User Id={Settings.Username};Password={Settings.Password};Database={Settings.Database};Pooling=false;";
            }
        }

        public PsqlMaster(PsqlSettings dbSettings)
        {
            Settings = dbSettings;
        }

        public async Task<T> ExecuteSqlAsync<T>(Func<NpgsqlCommand, Task<T>> func, T errorValue)
        {
            NpgsqlConnection conn = default;
            NpgsqlCommand cmd = default;
            T result = default;
            try
            {
                conn = new NpgsqlConnection(ConnectionString);
                await conn.OpenAsync();
                cmd = conn.CreateCommand();
                result = await func(cmd);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ExecuteSqlAsync: Error: {e.Message}");
                result = errorValue;
            }
            finally
            {
                cmd?.Dispose();
                conn?.Close();
                conn?.Dispose();
            }

            return result;
        }
    }
}
