using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminPanel.Areas.Admin.Controllers
{
    /// <summary>
    /// Base controller with authentication for another controllers
    /// </summary>
    [Authorize(Roles = "Admin")]
    public abstract class AdminController : Controller
    {
    }
}