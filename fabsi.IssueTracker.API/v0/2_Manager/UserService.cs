using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AITIssueTracker.API.v0._1_Controller;
using AITIssueTracker.API.v0._2_Manager.Contracts;
using AITIssueTracker.API.v0._3_DAL;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._2_EntityModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AITIssueTracker.API.v0._2_Manager
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private DataDb Data { get; }
        
        public UserService(DataDb db, IMapper mapper)
        {
            _mapper = mapper;
            Data = db;
        }
        
        public async Task<List<UserView>> GetAllUsersAsync()
        {
            var users = await Data.Users.ToListAsync();
            return _mapper.Map<List<UserView>>(users);
        }

        public async Task<UserView> SaveUserAsync(UserForm newUser)
        {
            var savedUser = await Data.Users.AddAsync(_mapper.Map<User>(newUser));
            int result = await Data.SaveChangesAsync();
            Console.WriteLine($"SaveUserAsync: Result: {result}");
            return _mapper.Map<UserView>(savedUser.Entity);
        }

        public async Task DeleteUserByUsernameAsync(string username)
        {
            var targetUser = await Data.Users.FindAsync(username);
            if (targetUser is null)
                throw new Exception($"DeleteUserByUsernameAsync: User not found.");
            
            var entity = Data.Users.Remove(targetUser);
            int result = await Data.SaveChangesAsync();
            Console.WriteLine($"DeleteUserByUsernameAsync: Result: {result}");
            if (result != 1)
                throw new Exception($"DeleteUserByUsernameAsync: Nothing deleted.");
        }

        public bool UserExists(string username)
        {
            return Data.Users.Find(username) is not null;
        }
    }
}