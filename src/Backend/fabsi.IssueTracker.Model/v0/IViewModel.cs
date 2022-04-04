using System;
using System.Collections.Generic;
using System.Text;

namespace AITIssueTracker.Model.v0
{
    public interface IViewModel<out T>
    {
        T AsView();
    }
}
