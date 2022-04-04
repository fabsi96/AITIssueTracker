using System;
using System.Collections.Generic;
using System.Text;

namespace AITIssueTracker.Model.v0
{
    public static class Endpoints
    {
        public const string BASE_TEST = "api/test";

        public const string BASE_USER = "api/user";
        public static class User
        {
            public const string SWAGGER_TAG = "Manages Users. Basic operations like GET, POST and DELETE";
            public const string USER_BY_USERNAME = "{username}";
        }

        public const string BASE_PROJECT = "api/project";

        public const string BASE_FEATURE = "api/feature";

        public const string BASE_ISSUE = "api/issue";

        public const string BASE_VIEW = "api/view";
    }
}
