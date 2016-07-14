using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SteamMarketPlaceDomain
{
   public class TestProxy
    {
        private List<WebProxy> webProxyList;
        private string urlSteamMarkerPlace="http://steamcommunity.com/market/search/render/?query=&appid=570&count=100";
        public TestProxy()
        {
            webProxyList = new List<WebProxy>();
        }
        public void ProxyTest(string pathTestingProxy,string pathCompleteProxy)
        {
            var proxyList = new List<string>();
            using (StreamReader reader = new StreamReader(pathTestingProxy))
            {
                while (!reader.EndOfStream)
                { proxyList.Add(reader.ReadLine()); }
            }
            foreach (var stringProxy in proxyList)
            {
                var splitString = stringProxy.Split(new[] { ':' });
                var ip = splitString[0];
                var port =Int32.Parse( splitString[1]);
                webProxyList.Add(new WebProxy(ip, port));
            }           
            var tempProxyList = new List<WebProxy>();
            foreach (var proxy in webProxyList)
            {
                HttpWebResponse responce;
                var request = WebRequest.CreateHttp(urlSteamMarkerPlace);
                request.Timeout = 5000;
                request.ReadWriteTimeout = 10000;
                request.Proxy = proxy;
                try
                {
                    using (responce = (HttpWebResponse)request.GetResponse())
                    {
                    }
                }
                catch
                {
                    continue;
                }
                tempProxyList.Add(proxy);
            }
            webProxyList = tempProxyList;
            using (StreamWriter writter = new StreamWriter(pathCompleteProxy))
            {
                foreach (var proxy in webProxyList)
                {
                    writter.WriteLine($"{proxy.Address.Host}:{proxy.Address.Port}");
                }
            }
        }
    }
}
