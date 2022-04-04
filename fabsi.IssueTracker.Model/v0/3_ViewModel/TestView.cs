using System;
using System.Collections.Generic;
using System.Text;
using AITIssueTracker.Model.v0._2_EntityModel;

namespace AITIssueTracker.Model.v0._3_ViewModel
{
    public class TestView
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public DateTime BirthDate { get; set; }

        public MyType Status { get; set; }
    }
}
