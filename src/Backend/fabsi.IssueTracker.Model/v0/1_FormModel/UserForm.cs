using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AITIssueTracker.Model.v0._1_FormModel
{
    public class UserForm
    {
        [Required]
        public string Username { get; set; }
    }
}
