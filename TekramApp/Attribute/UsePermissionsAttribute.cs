using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace TekramApp.Attribute
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class UsePermissionsAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _permissions;

        public UsePermissionsAttribute(params string[] permissions)
        {
            _permissions = permissions;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userPermissions = user.Claims
                                      .Where(c => c.Type == "permission")
                                      .Select(c => c.Value)
                                      .ToList();

            // Debug: log permissions
            Console.WriteLine("User Permissions: " + string.Join(",", userPermissions));

            if (!_permissions.All(p => userPermissions.Contains(p)))
            {
                context.Result = new ForbidResult(); // 403
            }
        }
    }
}