using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SteamMarketPlaceDomain;
using SteamMarketPlaceWeb.Models;

namespace SteamMarketPlaceWeb.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        private ItemRepository repository;
        public MainController()
        {
            repository = new ItemRepository();
        }
        [HttpGet]
        public ActionResult Index()
        {
            UrlAppIViewModel model = new UrlAppIViewModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(UrlAppIViewModel model)
        {
            CalculatePriceInventory calculate = new CalculatePriceInventory(repository);
            var url = model.Url;
            var appId = (int)model.AppID;
            var result = calculate.GetInventoryItems(url, appId);
            if (result == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("Result", result);
            }           
        }
    }
}