using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemi.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout
        public ActionResult Index()
        {
            //çıkış yaptığımızda bütün sessionları boşaltıyoruz
            Session.Abandon();
            return RedirectToAction("Index","Login");
        }
    }
}