using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AITIssueTracker.API.v0._3_DAL
{
    public class PsqlSettings
    {
        public const string KEY = "PostgresSettings";

        public string DatabaseIp { get; set; }

        public short DatabasePort { get; set; }
        
        public string Username { get; set; }

        public string Password { get; set; }

        public string Database { get; set; }

        public string ConnectionString
        {
            get
            {
                return $"Server={DatabaseIp};Port={DatabasePort};User Id={Username};Password={Password};Database={Database};Pooling=false;";
            }
        }
    }
}
