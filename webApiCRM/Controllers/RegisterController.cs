using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webApiCRM.Models;
using webApiCRM.Service;

namespace webApiCRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
       
        private readonly IAuthService _authService;

        public RegisterController(IAuthService authService)
        {
            _authService = authService;
        }



        [HttpPost]
        public async  Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(model);

            if (!result.isAuthenticated)
            {
                return BadRequest(result.message);
            }


            return Ok(result);
        }


        
      




    }
}
