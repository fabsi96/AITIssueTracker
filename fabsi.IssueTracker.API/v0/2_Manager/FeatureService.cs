using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Extensions;
using AITIssueTracker.API.v0._2_Manager.Contracts;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Microsoft.EntityFrameworkCore;

namespace AITIssueTracker.API.v0._2_Manager
{
    public class FeatureService : IFeatureService
    {
        public DataDb Database { get; }

        public FeatureService(DataDb database)
        {
            Database = database;
        }
        public async Task<List<FeatureView>> GetFeaturesOfProjectAsync(Guid projectId)
        {
            try
            {
                return (await Database.Features.ToListAsync()).ConvertAll(conv => conv.AsView());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<FeatureView> SaveFeatureToProjectAsync(Guid projectId, FeatureForm formModel)
        {
            try
            {
                Feature featureToSave = new Feature(formModel);
                var entity = await Database.Features.AddAsync(featureToSave);

                int rows = await Database.SaveChangesAsync();
                return rows > 0 ? entity.Entity.AsView() : null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<bool> DeleteFeatureAsync(Guid featureId)
        {
            try
            {
                Feature feature = await Database.Features.FindAsync(featureId);
                if (feature is null)
                    return false;

                Database.Features.Remove(feature);
                int rows = await Database.SaveChangesAsync();
                
                return rows > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public Task<bool> AddUserToFeatureAsync(Guid featureId, string username)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveUserFromFeatureAsync(Guid featureId, string username)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ProjectExistsAsync(Guid projectId)
        {
            try
            {
                Project targetProject = await Database.Projects.FindAsync(projectId);
                return targetProject is not null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}