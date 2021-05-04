using System;
using System.Collections.Generic;
using System.Text;
using AITIssueTracker.Model.v0._1_FormModel;
using Swashbuckle.AspNetCore.Filters;

namespace AITIssueTracker.Model.v0._4_Examples
{
    public class UserExample : IExamplesProvider<UserForm>
    {
        public UserForm GetExamples()
        {
            return new UserForm
            {
                Username = "Dein Name",
            };
        }
    }
}
