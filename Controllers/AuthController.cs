using AuthenticationServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace AuthenticationServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static readonly string EndpointUri = "https://ipaska.documents.azure.com:443/";

        private static readonly string PrimaryKey = "vT6qmEERWY15n5FpTzHog2sCOG6Vzn0N82dxWKTQh91bLHshbbsB6Q1agUJFWYsmIUDq16LoWOThnQQwN75l8w==";

        private string databaseId = "UserInfo";


        static string Hash(string input)
        {
            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginInfo>> LoginAsync([FromBody] LoginModel user)
        {
            if (user is null)
            {
                return BadRequest("Invalid client request");
            }
            LoginInfo loginInfo;
            CosmosClient cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "AuthenticationServer" });
            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Container container = await database.CreateContainerIfNotExistsAsync(id: "users", partitionKeyPath: "/partitionKey", throughput: 400);

            var sqlQueryText = "SELECT * FROM c WHERE c.user_email = '" + user.user_id + "'";



            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<LoginInfo> queryResultSetIterator = container.GetItemQueryIterator<LoginInfo>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<LoginInfo> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (LoginInfo userInfo in currentResultSet)
                {
                    user.user_pwd = Hash(user.user_pwd + userInfo.pwd_salt);
                    if (user.user_id == userInfo.user_email && user.user_pwd == userInfo.user_pwd.ToLower())
                    {
                        return userInfo;
                        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                        var claims = new List<Claim>
                {

                    new Claim(ClaimTypes.Name, userInfo.user_id),
                    new Claim(ClaimTypes.Role, "Manager")
                };

                        var tokeOptions = new JwtSecurityToken(
                            issuer: "https://localhost:5001",
                            audience: "https://localhost:5001",
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(5),
                            signingCredentials: signinCredentials
                        );

                        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                        Ok(new AuthenticatedResponse { Token = tokenString });
                    }

                }
            }

            return BadRequest("invalid login or password");
        }

        [HttpPost("loked-user")]
        public async Task<IActionResult> ReplaceItemAsync([FromBody] LoginModel user)
        {

            CosmosClient cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "AuthenticationServer" });
            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Container container = await database.CreateContainerIfNotExistsAsync(id: "users", partitionKeyPath: "/partitionKey", throughput: 400);

            var sqlQueryText = "SELECT * FROM c WHERE c.user_email = '" + user.user_id + "'";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<LoginInfo> queryResultSetIterator = container.GetItemQueryIterator<LoginInfo>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<LoginInfo> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (LoginInfo userInfo in currentResultSet)
                {

                    string part = userInfo.user_id;

                    ItemResponse<LoginInfo> userResponse = await container.ReadItemAsync<LoginInfo>(userInfo.Id, new PartitionKey(userInfo.user_id));
                    var itemBody = userResponse.Resource;

                    itemBody.user_id = part + " LOKED";

                    await container.ReplaceItemAsync<LoginInfo>(itemBody, itemBody.Id, new PartitionKey(itemBody.PartitionKey));

                    return Ok("{}");
                    
                }
            }

            return BadRequest(" ");
        }

        [HttpPost("new-pass")]
        public async Task<IActionResult> ReplaceFamilyItemAsync([FromBody] LoginModel user)
        {
            
            string passEncoding = encoding(user.user_pwd);

            CosmosClient cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, new CosmosClientOptions() { ApplicationName = "AuthenticationServer" });
            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Container container = await database.CreateContainerIfNotExistsAsync(id: "users", partitionKeyPath: "/partitionKey", throughput: 400);

            var sqlQueryText = "SELECT * FROM c WHERE c.user_email = '" + user.user_id + "'";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<LoginInfo> queryResultSetIterator = container.GetItemQueryIterator<LoginInfo>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<LoginInfo> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (LoginInfo userInfo in currentResultSet)
                {

                    string newPass = Hash(passEncoding + userInfo.pwd_salt);

                    //string part = userInfo.user_id;

                    LoginInfo test = userInfo;

                    ItemResponse<LoginInfo> userResponse = await container.ReadItemAsync<LoginInfo>(userInfo.Id, new PartitionKey(userInfo.user_id));
                    var itemBody = userResponse.Resource;

                    itemBody.user_pwd = newPass;
                    itemBody.last_login_attempt = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

                    await container.ReplaceItemAsync<LoginInfo>(itemBody, itemBody.Id, new PartitionKey(itemBody.PartitionKey));

                    return Ok("{}");


                }
            }

            return BadRequest(" ");
        }

        public string encoding(string toEncode)
        {
            
            return Encoding.GetEncoding(28591).GetString(Convert.FromBase64String(toEncode));
        }

      
    }
}
