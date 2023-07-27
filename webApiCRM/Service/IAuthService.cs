using webApiCRM.Models;

namespace webApiCRM.Service
{
    public interface IAuthService
    {
        public Task<AuthModel> RegisterAsync(RegisterModel model);

        public Task<AuthModel> LoginAsync(LoginModel model);
    }
}
