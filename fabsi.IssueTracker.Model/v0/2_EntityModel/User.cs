using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AITIssueTracker.Model.v0._1_FormModel;
using AITIssueTracker.Model.v0._3_ViewModel;
using Npgsql;

namespace AITIssueTracker.Model.v0._2_EntityModel
{
    public class User
    {   
        [Key]
        public string Username { get; set; }
    }
}
