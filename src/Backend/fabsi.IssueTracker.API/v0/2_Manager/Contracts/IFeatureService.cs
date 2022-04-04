using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._3_ViewModel;

namespace AITIssueTracker.API.v0._2_Manager.Contracts
{
    public interface IFeatureService
    {
        Task<List<FeatureView>> GetFeaturesOfProjectAsync(Guid projectId);

        Task<FeatureView> SaveFeatureToProjectAsync(Guid projectId, FeatureForm formModel);

        Task<bool> DeleteFeatureAsync(Guid featureId);

        Task<bool> AddUserToFeatureAsync(Guid featureId, string username);

        Task<bool> RemoveUserFromFeatureAsync(Guid featureId, string username);
        
        Task<bool> ProjectExistsAsync(Guid projectId);
    }
}