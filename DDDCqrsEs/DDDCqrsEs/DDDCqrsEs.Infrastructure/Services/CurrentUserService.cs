using Microsoft.AspNetCore.Http;
using DDDCqrsEs.Common;
using DDDCqrsEs.Common.Constants;
using DDDCqrsEs.Common.Identity;
using System;
using System.Security.Claims;

namespace DDDCqrsEs.Infrastructure.Services
{
    [MapServiceDependency(nameof(CurrentUserService))]
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }
        public CurrentUser GetCurrentUser()
        {
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                return null;
            var result = new CurrentUser();
            result.Email = _httpContextAccessor.HttpContext.User.Identity.Name;
            result.UserId = new Guid(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                return null;
            foreach (var role in (Role[])Enum.GetValues(typeof(Role)))
            {
                if(_httpContextAccessor.HttpContext.User.IsInRole(role.ToString()))
                {
                    result.Role = role;
                }
            }
            return result;
        }
    }
}
