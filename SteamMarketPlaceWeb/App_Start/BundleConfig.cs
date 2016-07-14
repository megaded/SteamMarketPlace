using System.Web;
using System.Web.Optimization;

namespace SteamMarketPlaceWeb
{
   public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                     "~/Scripts/bootstrap.js",
                     "~/Scripts/respond.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css"));
        }
    }
}
