using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Npgsql;

namespace AITIssueTracker.Model.v0._2_EntityModel
{
    public class ProjectIssue : IViewModel<IssueView>
    {
        public Guid Id { get; set; }

        public Guid ProjectId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public IssueType Type { get; set; }

        public int EffortEstimation { get; set; }

        public FeatureStatus Status { get; set; }

        public ProjectIssue()
        {

        }

        public ProjectIssue(ProjectIssueForm form)
        {
            ProjectId = form.ProjectId;
            Title = form.Title;
            Description = form.Description;
            Type = form.Type;
            EffortEstimation = form.EffortEstimation;
            Status = form.Status;
        }

        public ProjectIssue(NpgsqlDataReader reader)
        {
            if (reader is null || reader.IsClosed)
                throw new Exception($"FeatureIssue(NpgsqlDataReader): Error: Cannot read datareader");


            Id = Guid.Parse(reader["id"].ToString());
            ProjectId = Guid.Parse(reader["project_id"].ToString());
            Title = reader["title"].ToString();
            Description = reader["description"].ToString();
            Type = (IssueType)Enum.Parse(typeof(IssueType), reader["issue_type"].ToString());
            EffortEstimation = int.Parse(reader["effort_estimation"].ToString());
            Status = (FeatureStatus)Enum.Parse(typeof(FeatureStatus), reader["status"].ToString());
        }

        public IssueView AsView()
        {
            return new IssueView
            {
                Id = Id,
                Title = Title,
                Description = Description,
                EffortEstimation = EffortEstimation,
                Status = Status,
                Type = Type
            };
        }
    }
}
