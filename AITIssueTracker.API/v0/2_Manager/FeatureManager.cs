using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;

namespace AITIssueTracker.API.v0._2_Manager
{
    public class FeatureManager
    {
        private FeatureContext DbContext { get; }

        private ProjectContext ProjectDb { get; }

        public FeatureManager(FeatureContext ctx, 
            ProjectContext projectDb)
        {
            DbContext = ctx;
            ProjectDb = projectDb;
        }

        public async Task<FeatureView> SaveNewFeatureAsync(FeatureForm feature, Guid projectId)
        {
            Feature featureToSave = new Feature(feature);
            return await DbContext.InsertNewFeatureAsync(featureToSave, projectId) == 1 ? featureToSave.AsView() : null;
        }

        public async Task<List<FeatureView>> GetFeaturesOfProjectAsync(Guid projectId)
        {
            List<Feature> projectFeatures = await DbContext.SelectFeaturesByProjectTitleAsync(projectId);
            return projectFeatures?.ConvertAll(c => c.AsView());
        }

        public async Task<bool> DeleteFeatureFromProjectAsync(Guid featureId)
        {
            return await DbContext.DeleteFeatureByIdentifier(featureId);
        }

        public async Task<bool> AddUserToFeatureAsync(Guid featureId, string username)
        {
            Project projectOfFeature = await DbContext.SelectProjectOfFeatureAsync(featureId);
            if (projectOfFeature is null)
                return false;

            bool userExists = await ProjectDb.SelectUserIsInProjectAsync(username, projectOfFeature.Id);

            if (!userExists)
                return false;

            return await DbContext.InsertUserToFeatureAsync(featureId, username) == 1;
        }

        public async Task<bool> RemoveUserFromFeatureAsync(Guid featureId, string username)
        {
            return await DbContext.DeleteUserFromFeatureAsync(featureId, username) == 1;
        }

        public async Task<int> UpdateFeatureStatusAsync(Guid featureId, FeatureStatus newStatus, string username)
        {
            Feature targetFeature = await DbContext.SelectFeatureByIdAsync(featureId);
            List<User> featureUsers = await DbContext.SelectUsersOfFeatureAsync(featureId);
            if (targetFeature is null ||
                !featureUsers.Exists(u => u.Username == username))
                return -1;

            return await DbContext.UpdateFeatureStatusAsync(featureId, newStatus);
        }
    }
}