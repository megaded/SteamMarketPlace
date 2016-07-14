using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SteamMarketPlaceDomain;

namespace SteamMarketPlaceWeb.Models
{
    public class UrlAppIViewModel
    {
        public string Url { get; set; }
        public SteamAppId AppID { get; set; }
        public IEnumerable<SelectListItem> ListAppId;
        public UrlAppIViewModel()
        {            
            ListAppId = Enum.GetValues(typeof(SteamAppId)).Cast<SteamAppId>().Select(x =>
            new SelectListItem
            {
                Text = x.ToString(),
                Value = ((int)x).ToString()
            });
        }
    }
}
