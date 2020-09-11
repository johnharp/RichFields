using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RichFields_Demo.Models;

namespace RichFields_Demo.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new RichFieldsDemoViewModel();
            return View(model);
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public ActionResult Save()
        {
            var model = new RichFieldsDemoViewModel();
            if (TryUpdateModel(model, "", new string[] {
                "ExamplePercent", "ExampleMoney", "ExampleInt",
                "ExampleDate", "ExampleMonthYear", "ExampleText" }))
            {

            }
            return View(model);
        }
    }
}