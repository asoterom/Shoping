using Microsoft.AspNetCore.Identity;
using Shoping.Data.Entities;
using Shoping.Models;

namespace Shoping.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        //metodos para iniciar sesion y cerrar sesion
        //el singnresult es la respuesta si se udo iniciar sesion o fallo algo
        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();


    }
}
