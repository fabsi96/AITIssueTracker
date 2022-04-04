using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AITIssueTracker.Model.v0;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._2_EntityModel.GitHub;
using AITIssueTracker.Model.v0._3_ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AITIssueTracker.Sync
{
    class Program
    {
        private const string HTTP_GITHUB_GET_REPOSITORIES = "/users/{0}/repos";
        private const string HTTP_GITHUB_GET_REPOSITORY_INFO = "repos/{0}/{1}";
        private const string HTTP_GITHUB_GET_REPOSITORY_MILESTONES = "/repos/{0}/{1}/milestones";
        private const string HTTP_GITHUB_GET_REPOSITORY_ISSUES = "/repos/{0}/{1}/issues";

        static async Task Main(string[] args)
        {
            const string PROJECT_1 = "Issuetracker";
            
            const string PROJECT_2 = "TCP Kommunikation";
            
            List<Project> projects = new List<Project>
            {
                new Project
                {
                    Id = Guid.NewGuid(),
                    Title = PROJECT_1,
                    Description = "Verwaltung von Projekten. Synchronisierung mit einem GitHub Projekt.",
                    IsDone = false,
                },
                
                new Project
                {
                    Id = Guid.NewGuid(),
                    Title = PROJECT_2,
                    Description = "Versenden von Nachrichten (Beispiel: HTTP) als rohe TCP Pakete. Anwendung in mobilen Apps.",
                    IsDone = false,
                },
            };

            const string PROJECT_1_FEATURE_1 = "Datenbank";
            const string PROJECT_1_FEATURE_2 = "Integrationstests";
            
            const string PROJECT_2_FEATURE_1 = "Schnittstelle Senden/Empfangen";
            
            List<Feature> features = new List<Feature>
            {
                new Feature
                {
                    Id = Guid.NewGuid(),
                    Title = PROJECT_1_FEATURE_1,
                    Description = "",
                    Status = FeatureStatus.ToDo
                },
                new Feature
                {
                    Id = Guid.NewGuid(),
                    Title = PROJECT_1_FEATURE_2,
                    Description = "",
                    Status = FeatureStatus.ToDo
                },
                new Feature
                {
                    Id = Guid.NewGuid(),
                    Title = PROJECT_2_FEATURE_1,
                    Description = "",
                    Status = FeatureStatus.ToDo
                },
            };

            List<ProjectFeature> projectFeatures = new List<ProjectFeature>
            {
                new ProjectFeature
                {
                    FeatureId = features.Single(f => f.Title.Equals(PROJECT_1_FEATURE_1)).Id,
                    ProjectId = projects.Single(p => p.Title.Equals(PROJECT_1)).Id,
                },

                new ProjectFeature
                {
                    FeatureId = features.Single(f => f.Title.Equals(PROJECT_1_FEATURE_2)).Id,
                    ProjectId = projects.Single(p => p.Title.Equals(PROJECT_1)).Id,
                },

                new ProjectFeature
                {
                    FeatureId = features.Single(f => f.Title.Equals(PROJECT_2_FEATURE_1)).Id,
                    ProjectId = projects.Single(p => p.Title.Equals(PROJECT_2)).Id,
                },
            };
            
            const string PROJECT_1_ISSUE_1 = "Datenbank Tabellen";
            const string PROJECT_1_ISSUE_2 = "Integrationstest der Projekt-Endpunkte.";
            const string PROJECT_1_ISSUE_3 = "Integrationstest der Feature-Endpunkte.";
            
            const string PROJECT_2_ISSUE_1 = "Konsistentes Zustandssystem";
            const string PROJECT_2_ISSUE_2 = "Senden von Standardisierten Objekten";
            
            List<Issue> issues = new List<Issue>
            {
                new Issue
                {
                    Id = Guid.NewGuid(),
                    Title = PROJECT_1_ISSUE_1,
                    Description = "Relationen mit Schlüsseln, Referenzen und Typen.",
                    EffortEstimation = 0,
                    Status = FeatureStatus.ToDo,
                    Type = IssueType.Task,
                },
                new Issue
                {
                    Id = Guid.NewGuid(),
                    Title = PROJECT_1_ISSUE_2,
                    Description = "",
                    EffortEstimation = 0,
                    Status = FeatureStatus.ToDo,
                    Type = IssueType.Task,
                },
                new Issue
                {
                    Id = Guid.NewGuid(),
                    Title = PROJECT_1_ISSUE_3,
                    Description = "",
                    EffortEstimation = 0,
                    Status = FeatureStatus.ToDo,
                    Type = IssueType.Task,
                },
                
                new Issue
                {
                    Id = Guid.NewGuid(),
                    Title = PROJECT_2_ISSUE_1,
                    Description = "Zustände Verbunden, Getrennt verwalten.",
                    EffortEstimation = 0,
                    Status = FeatureStatus.ToDo,
                    Type = IssueType.Task,
                },
                new Issue
                {
                    Id = Guid.NewGuid(),
                    Title = PROJECT_2_ISSUE_2,
                    Description = "Vordefinierte Objekte im JSON Format.",
                    EffortEstimation = 0,
                    Status = FeatureStatus.ToDo,
                    Type = IssueType.Task,
                },
            };
    
            List<ProjectIssue> projectIssues = new List<ProjectIssue>
            {
                new ProjectIssue
                {
                    IssueId = issues.Single(i => i.Title.Equals(PROJECT_2_ISSUE_2)).Id,
                    ProjectId = projects.Single(p => p.Title.Equals(PROJECT_2)).Id,
                },
                new ProjectIssue
                {
                    IssueId = issues.Single(i => i.Title.Equals(PROJECT_2_ISSUE_1)).Id,
                    ProjectId = projects.Single(p => p.Title.Equals(PROJECT_2)).Id,
                },
            };

            List<FeatureIssue> featureIssues = new List<FeatureIssue>
            {
                new FeatureIssue
                {
                    IssueId = issues.Single(i => i.Title.Equals(PROJECT_1_ISSUE_1)).Id,
                    FeatureId = features.Single(f => f.Title.Equals(PROJECT_2_FEATURE_1)).Id,
                },
                new FeatureIssue
                {
                    IssueId = issues.Single(i => i.Title.Equals(PROJECT_1_ISSUE_2)).Id,
                    FeatureId = features.Single(f => f.Title.Equals(PROJECT_1_FEATURE_2)).Id,
                },
                new FeatureIssue
                {
                    IssueId = issues.Single(i => i.Title.Equals(PROJECT_1_ISSUE_3)).Id,
                    FeatureId = features.Single(f => f.Title.Equals(PROJECT_1_FEATURE_2)).Id,
                },

                new FeatureIssue
                {
                    IssueId = issues.Single(i => i.Title.Equals(PROJECT_2_ISSUE_2)).Id,
                    FeatureId = features.Single(f => f.Title.Equals(PROJECT_2_FEATURE_1)).Id,
                },
            };

            Console.ReadKey();
        }
    }
}
