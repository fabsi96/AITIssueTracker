using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._2_Manager.Contracts;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Microsoft.EntityFrameworkCore;

namespace AITIssueTracker.API.v0._2_Manager
{
    public class ProjectService : IProjectService
    {
        
        private DataDb Db { get; }

        public ProjectService(DataDb db)
        {
            Db = db;
        }
        
        public async Task<List<ProjectView>> GetProjectsAsync(string filter = "")
        {
            try
            {
                List<Project> projects = await Db.Projects.ToListAsync();
                return projects?.ConvertAll(c => c.AsView());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<ProjectView> SaveProjectAsync(ProjectForm projectForm)
        {
            try
            {
                var createdProject = await Db.Projects.AddAsync(new Project(projectForm));
                int rows = await Db.SaveChangesAsync();
                return rows > 0 ? createdProject.Entity.AsView() : null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<bool> DeleteProjectAsync(Guid projectId)
        {
            try
            {
                Project projectToDelete = await Db.Projects.FindAsync(projectId);
                if (projectToDelete is null)
                    return false;

                Db.Projects.Remove(projectToDelete);
                int rows = await Db.SaveChangesAsync();
                return rows > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public Task<bool> AddUserToProjectAsync(string username, Guid projectId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveUserFromProjectAsync(string username, Guid projectId)
        {
            throw new NotImplementedException();
        }
    }
}