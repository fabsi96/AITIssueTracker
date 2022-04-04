using System;
using System.Collections.Generic;
using System.Text;
using AITIssueTracker.Model.v0._2_EntityModel;

namespace AITIssueTracker.Model.v0._1_FormModel
{
    public class TestForm
    {
            public string Name { get; set; }

            public int Age { get; set; }

            public DateTime BirthDate { get; set; }

            public MyType Status { get; set; }
    }
}
