using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Npgsql;

namespace AITIssueTracker.Model.v0._2_EntityModel
{
    [Table("project")]
    public class Project : IViewModel<ProjectView>
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("title")]
        public string Title { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("is_done")]
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
