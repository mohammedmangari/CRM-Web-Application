using Microsoft.Identity.Client;
using System.Text;
using System.Text.Json;

namespace webApiCRM.Service
{
	public class ConnectionService
	{

		private readonly IConfiguration _configuration;

        public ConnectionService(IConfiguration configuration)
        {
			_configuration = configuration;
        }

        public async Task<string> GetTokenId(HttpContext httpContext)
		{
			string? cachedAccessToken = httpContext.Items["AccessToken"] as string;
			DateTimeOffset? tokenExpiration = httpContext.Items["TokenExpiration"] as DateTimeOffset?;

			if (!string.IsNullOrEmpty(cachedAccessToken) && tokenExpiration.HasValue && tokenExpiration.Value > DateTimeOffset.UtcNow)
			{
				return cachedAccessToken;
			}

			string clientId = _configuration["ClientId"]?? "c7f45b81-8695-483e-bf8e-9c687618e6b6";
			string appKey = _configuration["AppKey"]?? "LST8Q~7wH6OK87Q_Ey2DFUOq_lUTZpEt~EdcmbMm"; 
			string tenantId = _configuration["TenantId"]?? "92c51029-3c20-41b5-a395-248bea1a13b4";
			string[] scope = new string[] { "https://orgc35b7e1e.crm4.dynamics.com/.default" };

			var app =  ConfidentialClientApplicationBuilder.Create(clientId)
				.WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
				.WithClientSecret(appKey)
				.Build();

			AuthenticationResult token = await app.AcquireTokenForClient(scope).ExecuteAsync();

			// Store the token and its expiration in the HttpContext for future use within the same request
			httpContext.Items["AccessToken"] = token.AccessToken;
			httpContext.Items["TokenExpiration"] = token.ExpiresOn;

			return token.AccessToken;
		}


		public async Task<HttpResponseMessage> CrmRequest(HttpMethod httpMethod, string accessToken, string requestUri, Account body = null)
		{
			var client = new HttpClient();
			var msg = new HttpRequestMessage(httpMethod, requestUri);
			msg.Headers.Add("OData-MaxVersion", "4.0");
			msg.Headers.Add("OData-Version", "4.0");
			msg.Headers.Add("Prefer", "odata.include-annotations=\"*\"");

			// Passing AccessToken in Authentication header  
			msg.Headers.Add("Authorization", $"Bearer {accessToken}");

			if (body != null)
			{
				var jsonBody = JsonSerializer.Serialize(body);
				msg.Content = new StringContent(jsonBody, UnicodeEncoding.UTF8, "application/json");
			}

			return await client.SendAsync(msg);
		}

	}
}
	