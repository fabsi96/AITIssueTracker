using System;
using Npgsql;

namespace AITIssueTracker.API.v0._1_Controller
{
    public class GithubProject
    {
        public int GithubProjectId { get; set; }

        public Guid ProjectId { get; set; }

        public DateTime SyncDate { get; set; }

        public GithubProject(NpgsqlDataReader reader)
        {
            if (reader is null || reader.IsClosed)
                throw new Exception("GithubProject(NpgsqlDataReader): Error. Reader is closed.");

            GithubProjectId = int.Parse(reader["github_repo_id"].ToString() ?? "");
            ProjectId = Guid.Parse(reader["project_id"].ToString() ?? "");
            SyncDate = DateTime.Parse(reader["sync_date"].ToString() ?? "");
        }
    }
}