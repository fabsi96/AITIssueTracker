using System;
using System.Collections.Generic;
using System.Text;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Npgsql;

namespace AITIssueTracker.Model.v0._2_EntityModel
{
    public class User : IViewModel<UserView>
    {
        public string Username { get; set; }

        public User()
        {
            
        }

        public User(UserForm form)
        {
            Username = form.Username;
        }

        public User(NpgsqlDataReader reader)
        {
            if (reader is null)
                throw new Exception($"User(NpgsqlDataReader): Reader is null.");

            Username = reader["username"].ToString();
        }

        public UserView AsView()
        {
            return new UserView
            {
                Username = Username,
            };
        }
    }
}
