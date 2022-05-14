using System.Threading.Tasks;

namespace Dialysis.Mobile.Core.Services
{
    public interface IAuthService
    {
        Task<bool> Authencticate(string username, string password);
        Task<bool> Logout();
        Task<bool> RefreshToken();
        Task<bool> IsAuthenticated();
    }
}