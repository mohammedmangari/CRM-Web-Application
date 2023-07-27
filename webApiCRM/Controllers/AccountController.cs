using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;
using webApiCRM.Service;

namespace webApiCRM.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	
	public class AccountController : ControllerBase
	{


		private readonly ConnectionService _connectionservice;

		public AccountController(ConnectionService connectionService)
		{
			_connectionservice = connectionService;
		}


		/*[HttpGet]
		public async Task<IActionResult> GetSavedQueries()
		{
			string accessToken = await _connectionservice.GetTokenId(HttpContext);
			string url = "https://orgc35b7e1e.api.crm4.dynamics.com/api/data/v9.0/savedqueries?$select=name,savedqueryid&$filter=name eq 'Active Accounts'";

			var result = await _connectionservice.CrmRequest(HttpMethod.Get, accessToken, url);

			if (result.IsSuccessStatusCode)
			{
				var contentStream = await result.Content.ReadAsStreamAsync();
				var streamReader = new StreamReader(contentStream);
				var jsonReader = new JsonTextReader(streamReader);
				var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
				var savedQueries = jsonSerializer.Deserialize<SavedQueriesResponse>(jsonReader);
				return Ok(savedQueries);
			}

			return StatusCode((int)result.StatusCode);
		}*/


		// retrive all accounts  from CRM
		[HttpGet]
		
		public async Task<IActionResult> GetAllAccount()
		{
			string accessToken = await _connectionservice.GetTokenId(HttpContext);

			if (string.IsNullOrEmpty(accessToken))
			{
				return NoContent();
			}
			else
			{

				string endpoint = "https://orgc35b7e1e.api.crm4.dynamics.com/api/data/v9.0/accounts";
				var result = await _connectionservice.CrmRequest(HttpMethod.Get, accessToken, endpoint);

				if (result.IsSuccessStatusCode)
				{
					var content = await result.Content.ReadAsStringAsync();
					var accountsList = JsonConvert.DeserializeObject<AccountsList>(content);

					var accounts = accountsList.Value.Select(json => new Account
					{
                        accountid = json.accountid,
						name = json.name,
						creditonhold = json.creditonhold,
						// address1_latitude = json.address1_latitude,
						description = json.description,
						revenue = json.revenue,
                        websiteurl = json.websiteurl,
                        address1_composite = json.address1_composite,

                    }).ToList();


					return Ok(accounts);
				}

				return StatusCode((int)result.StatusCode);
			}

		}




		// retrive  a specifique account  from CRM using id  and the target as parame  
		[HttpGet]
		public async Task<IActionResult> GetAccount(string accountId)
		{
			string accessToken = await _connectionservice.GetTokenId(HttpContext);

			if (string.IsNullOrEmpty(accessToken))
			{
				return NoContent();
			}
			else
			{
				// Specify the account ID you want to retrieve
				//Guid accountId = new Guid("83883308-7ad5-ea11-a813-000d3a33f3b4");
				                    
				string endpoint = $"https://orgc35b7e1e.api.crm4.dynamics.com/api/data/v9.0/accounts({accountId})";
				var result = await _connectionservice.CrmRequest(HttpMethod.Get, accessToken, endpoint);

				if (result.IsSuccessStatusCode)
				{
					var content = await result.Content.ReadAsStringAsync();
					var accountJson = JsonConvert.DeserializeObject<Account>(content);

					var account = new Account
					{
                        accountid= accountJson.accountid,
                        name = accountJson.name,
						creditonhold = accountJson.creditonhold,
						//address1_latitude = accountJson.address1_latitude,
						description = accountJson.description,
						revenue = accountJson.revenue ,
                        address1_composite= accountJson.address1_composite,
                        websiteurl = accountJson.websiteurl,
                    };

					return Ok(account);
				}

				return StatusCode((int)result.StatusCode);
			}
	    }

		 
		// Create an accounte on CRM

		[HttpPost]
	
		public async Task<IActionResult> CreateAccount(Account account)
		{
			var accessToken = await _connectionservice.GetTokenId(HttpContext);
			var url = "https://orgc35b7e1e.api.crm4.dynamics.com/api/data/v9.0/accounts";
			
			if (account==null)
			{
				return BadRequest("The account is not accepted");
			}
			else
			{
				var result = await _connectionservice.CrmRequest(HttpMethod.Post,accessToken,url,account);

				if (result.IsSuccessStatusCode)
				{
					var content = await result.Content.ReadAsStreamAsync();

					return Ok(content);
				}
				else
				{
					return StatusCode((int)(result.StatusCode));
				}

			}

		}

		// Basic Delete an account on CRM

		[HttpDelete]
	
		public async Task<IActionResult> DeleteAccount(string id)
		{

			var accessToken = await _connectionservice.GetTokenId(HttpContext);
			var url = $"https://orgc35b7e1e.api.crm4.dynamics.com/api/data/v9.0/accounts({id})";

			var isFoundAccound = await _connectionservice.CrmRequest(HttpMethod.Get,accessToken,url);


			if (isFoundAccound.IsSuccessStatusCode)
			{

				var deleteAccount = await _connectionservice.CrmRequest(HttpMethod.Delete, accessToken, url);

				if (deleteAccount.IsSuccessStatusCode)
				{
					return Ok(deleteAccount.StatusCode);
				}

				return StatusCode((int)deleteAccount.StatusCode);
			}
			else
			{
				return StatusCode((int)isFoundAccound.StatusCode);
			}
		}


		//  Basic Update an account on CRM
		[HttpPut]
		public async Task<IActionResult> UpdateAccount(string id, Account account)
		{
			var accessToken = await _connectionservice.GetTokenId(HttpContext);

			var url = $"https://orgc35b7e1e.api.crm4.dynamics.com/api/data/v9.0/accounts({id})";


			var isAccountFound = await _connectionservice.CrmRequest(HttpMethod.Get,accessToken,url);

			if (isAccountFound.IsSuccessStatusCode)
			{

				var   updateAccount = await _connectionservice.CrmRequest(HttpMethod.Patch,accessToken,url,account);

				if (updateAccount.IsSuccessStatusCode)
				{
					return Ok(updateAccount.StatusCode);
				}


				return StatusCode((int)updateAccount.StatusCode);
			}

			return StatusCode((int)isAccountFound.StatusCode);

		}

	}



	




}




