using BSBoilerPlate.Data;
using BSBoilerPlate.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BSBoilerPlate.Authorization
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private BSBoilerPlateContext _BSBoilerPlateContext;

        public CustomAuthenticationStateProvider(BSBoilerPlateContext BSBoilerPlateContext)
        {
            _BSBoilerPlateContext = BSBoilerPlateContext;
            this.CurrentUser = this.GetAnonymous();
        }

        private ClaimsPrincipal CurrentUser { get; set; }

        private async Task<ClaimsPrincipal> GetUser(User user)
        {
            var role = await (from r in _BSBoilerPlateContext.Roles where r.ID == user.RoleID select r).FirstAsync<Role>();

            var identity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes. Sid, user.ID.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.GivenName, user.GivenName),
            new Claim(ClaimTypes.Surname, user.FirstSurname),
            new Claim(ClaimTypes.DateOfBirth, user.DOB.HasValue ? user.DOB.Value.ToString("s") : ""),
            new Claim(ClaimTypes.Role, role.Name)
        }, "Authentication type");
            return new ClaimsPrincipal(identity);
        }

        private ClaimsPrincipal GetAnonymous()
        {
            var identity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Sid, "0"),
            new Claim(ClaimTypes.Name, "Anonymous"),
            new Claim(ClaimTypes.GivenName, "Anonymous"),
            new Claim(ClaimTypes.Surname, ""),
            new Claim(ClaimTypes.DateOfBirth, ""),
            new Claim(ClaimTypes.Role, "Anonymous")
        }, null);
            return new ClaimsPrincipal(identity);
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var task = Task.FromResult(new AuthenticationState(this.CurrentUser));
            return task;
        }

        public async Task<AuthenticationState> ChangeUser(User user)
        {
            this.CurrentUser = await this.GetUser(user);
            var task = this.GetAuthenticationStateAsync();
            this.NotifyAuthenticationStateChanged(task);

            return await task;
        }

        public Task<AuthenticationState> Logout()
        {
            this.CurrentUser = this.GetAnonymous();
            var task = this.GetAuthenticationStateAsync();
            this.NotifyAuthenticationStateChanged(task);
            return task;
        }
    }
}
