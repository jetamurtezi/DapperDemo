using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DapperDemo.Filters
{
    public class AuthorizeAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                context.Result = new NotFoundResult(); // ose mund të përdorësh RedirectToAction
            }

            base.OnActionExecuting(context);
        }
    }
}
