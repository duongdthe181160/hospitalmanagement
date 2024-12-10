using BELibrary.Core.Utils;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HospitalManagement.Areas.Admin.Authorization
{
    public class PermissionAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 1. Admin
        /// 2. Employee
        /// 3. Customer
        /// </summary>
        public int Role { set; get; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = CookiesManage.GetUser();

            if (user != null)
            {
                return this.Role == user.Role || user.Role == RoleKey.Admin;
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var TempData = filterContext.Controller.TempData;
            TempData["Messages"] = "Bạn không có quyền truy cập mục này";
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "action", "E401" },
                    { "controller", "Login" },
                    { "Area", "admin" }
                });
        }
    }
}