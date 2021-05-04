using System;
using System.Collections.Generic;
using System.Text;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Npgsql;

namespace AITIssueTracker.Model.v0._2_EntityModel
{
    public class Feature : IViewModel<FeatureView>
    {
        // TODO: Save own generated guid to return the correct id or ?
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; }

        public string Description { get; set; }

        // public Guid ProjectId { get; set; }

        public DateTime Deadline { get; set; }

        public DateTime StartDate { get; set; }

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
