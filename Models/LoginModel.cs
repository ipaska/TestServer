using Newtonsoft.Json;

namespace AuthenticationServer.Models
{

    public class LoginModel
    {
        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        [JsonProperty(PropertyName = "partitionKey")]
        public string? PartitionKey { get; set; }


        public string? user_id { get; set; }

        public string? user_pwd { get; set; }

        public string? pwd_salt { get; set; }

        public string? user_email { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


    }

    public class LoginInfo
    {
        //internal string? id;

        [JsonProperty(PropertyName = "id")]
        public string? Id { get; set; }
        [JsonProperty(PropertyName = "partitionKey")]
        public string? PartitionKey { get; set; }


        public string? user_id { get; set; }

        public string? user_pwd { get; set; }

        public string? pwd_salt { get; set; }

        public string? login_attempts { get; set; }

        public string? last_login_attempt { get; set; }

        public string? user_email { get; set; }

        public string? cust_id { get; set; }
        public string? client_id { get; set; }
        public string? user_lname { get; set; }
        public string? user_fname { get; set; }
        public string? user_mi { get; set; }
        public string? user_phone { get; set; }
        public string? user_active { get; set; }
        public string? user_tree { get; set; }
        public string? pwd_last_updated { get; set; }
        public string? version { get; set; }
        public string? phone_format_id { get; set; }
        public string? login_date { get; set; }
        public string? language_id { get; set; }
        public string? start_page { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }


    }
}
