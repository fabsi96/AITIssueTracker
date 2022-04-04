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
    [Table("feature")]
    public class Feature : IViewModel<FeatureView>
    {
        // TODO: Save own generated guid to return the correct id or ?
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("title")]
        public string Title { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("project_id")]
        [ForeignKey("project")]
        public Guid ProjectId { get; set; }

        [Column("dealine")]
        public DateTime Deadline { get; set; }

        [Column("startdate")]
        public DateTime StartDate { get; set; }

        [Column("status")]
        public FeatureStatus Status { get; set; }

        public Feature()
        {
            
        }

        public Feature(FeatureForm form)
        {
            Title = form.Title;
            Description = form.Description;
            Deadline = form.Deadline;
            StartDate = form.StartDate;
            Status = form.Status;

        }

        public Feature(NpgsqlDataReader reader)
        {
            if (reader is null || reader.IsClosed)
                throw new Exception($"Feature(NpgsqlDataReader): Error: Reader is not available.");

            Id = Guid.Parse(reader["id"].ToString());
            Title = reader["title"].ToString();
            Description = reader["description"].ToString();
            // ProjectId = Guid.Parse(reader["project_id"].ToString());
            Deadline = DateTime.Parse(reader["deadline"].ToString());
            StartDate = DateTime.Parse(reader["startdate"].ToString());
            string status = reader["status"].ToString();
            Status = (FeatureStatus) Enum.Parse(typeof(FeatureStatus), status);
        }

        public FeatureView AsView()
        {
            return new FeatureView
            {
                Id = Id,
                Title = Title,
                Description = Description,
                Deadline = Deadline,
                StartDate = StartDate,
                Status = Status,
            };
        }
    }
}
