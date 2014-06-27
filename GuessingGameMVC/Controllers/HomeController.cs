using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GuessingGameMVC.Controllers
{
    public partial class HomeController : Controller
    {
        /// <summary>
        /// Guessing Game loader
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult GuessingGame()
        {
            return View();
        }
    }
}
