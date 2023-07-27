using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webApiCRM.Service;
using webApiCRM.Models;
namespace webApiCRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }



        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(model);

            if (!result.isAuthenticated)
            {
                return BadRequest(result.message);
            }


            if (!string.IsNullOrEmpty(result.Token))
            {
                var cookieOptions = new CookieOptions
                {
                   // HttpOnly = true,
                    Expires = result.ExpiresOn.ToLocalTime(),
                };

                Response.Cookies.Append("refreshToken", result.Token, cookieOptions);

            }
          

            return Ok(result);
        }

    }
}
