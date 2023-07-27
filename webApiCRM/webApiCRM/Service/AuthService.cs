using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using webApiCRM.Halpers;
using webApiCRM.Models;

namespace webApiCRM.Service
{
    public class AuthService : IAuthService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly JWT _jwt;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _userManager = userManager;

        }



        async  Task<AuthModel> IAuthService.RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null || await _userManager.FindByNameAsync(model.Username) is not null)
            {
                return new AuthModel
                {
                    message = "Email or username is already registred!",
                };
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            var result = await _userManager.CreateAsync(user, model.password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error  in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthModel
                {
                    message = errors,
                };
            }


            await _userManager.AddToRoleAsync(user, "RH");

            var jwtSecurityToken = await CreateJWTToken(user);


            return new AuthModel
            {
                Email = user.Email,
                isAuthenticated = true,
                Roles = new List<string> { "RH" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
            };


        }



        async Task<AuthModel> IAuthService.LoginAsync(LoginModel model)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.message = "Email or password is incorecte!!";
                return authModel;
            }

            var jwtSecruteToken = await CreateJWTToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.isAuthenticated = true;
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecruteToken);
            authModel.ExpiresOn = jwtSecruteToken.ValidTo;
            authModel.Roles = rolesList.ToList();
            return authModel;
        }



        private async Task<JwtSecurityToken> CreateJWTToken(ApplicationUser user)
        {

            var userClaims = await _userManager.GetClaimsAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var rolesClaims = new List<Claim>();


            foreach (var role in roles)
            {
                rolesClaims.Add(new Claim("roles", role));
            }

            var Claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid",user.Id),
            }.Union(userClaims)
            .Union(rolesClaims);


            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signinCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);


            var jwtsecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: Claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays)
                );


            return jwtsecurityToken;
        }
    }
}
