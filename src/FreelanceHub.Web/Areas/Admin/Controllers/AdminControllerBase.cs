using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public abstract class AdminControllerBase : Controller
{
    protected void SetPageTitle(string title) => ViewBag.AdminPageTitle = title;
}
