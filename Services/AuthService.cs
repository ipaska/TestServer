using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationServer.Services
{
    public class AuthService
    {
        private static readonly string EndpointUri = "https://ipaska.documents.azure.com:443/";

        private static readonly string PrimaryKey = "vT6qmEERWY15n5FpTzHog2sCOG6Vzn0N82dxWKTQh91bLHshbbsB6Q1agUJFWYsmIUDq16LoWOThnQQwN75l8w==";

        private string databaseId = "UserInfo";
    }
}
