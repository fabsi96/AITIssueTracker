using System;
using System.Collections.Generic;
using System.Text;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Npgsql;

namespace AITIssueTracker.Model.v0._2_EntityModel
{
    public class Issue : IViewModel<IssueView>
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; }

        public string Description { get; set; }

        public IssueType Type { get; set; }

        public int EffortEstimation { get; set; }

        public FeatureStatus Status { get; set; }

        public Issue()
        {

        }

        public Issue(IssueForm form)
        {
            Title = form.Title;
            Description = form.Description;
            Type = form.Type;
            EffortEstimation = form.EffortEstimation;
            Status = form.Status;
        }

        public Issue(NpgsqlDataReader reader)
        {
            if (reader is null || reader.IsClosed)
                throw new Exception($"Issue(NpgsqlDataReader): Error: Cannot read datareader");


            Id = Guid.Parse(reader["id"].ToString());
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
