using System;
using System.Collections.Generic;
using System.Text;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Npgsql;

namespace AITIssueTracker.Model.v0._2_EntityModel
{
    public class Project : IViewModel<ProjectView>
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsDone { get; set; }

        public Project()
        {
            
        }

        public Project(ProjectForm form)
        {
            Title = form.Title;
            Description = form.Description;
            IsDone = form.IsDone;
        }

        public Project(NpgsqlDataReader reader)
        {
            if (reader is null)
                throw new Exception($"Project(NpgsqlDataReader): Reader is null.");

            Id = Guid.Parse(reader["id"].ToString());
            Title = reader["title"].ToString();
            Description = reader["description"].ToString();
            IsDone = bool.Parse(reader["is_done"].ToString());
        }

        public ProjectView AsView()
        {
            return new ProjectView
            {
                Id = Id,
                Title = Title,
                Description = Description,
                IsDone = IsDone,
            };
        }
    }
}
