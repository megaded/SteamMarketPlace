using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using CsQuery;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Timers;
using System.Threading;
using System.Globalization;

namespace SteamMarketPlaceDomain
{
    public class ParseSteamMarketPlace
    {
        public ParseSteamMarketPlace(string pathProxyList,IRepository<Item> repository)
        {
            proxyClassList = new Queue<WebProxy>();
            uriPool = new Queue<Uri>();
            this.repository = repository;
            SetProxyList(pathProxyList);
        }

        private int totalItems = 0;
        private object locker = new object();
        private Queue<WebProxy> proxyClassList;
        private Queue<Uri> uriPool;
        private IRepository<Item> repository;
        private int itemsDowload = 0;
        private System.Timers.Timer timer;
        private SteamAppId updateGame;
        private int tryCount = 0;
        private bool stopFlag = false; 
        public event EventHandler<UpdatedDbEventArgs> UpdatedDb;
        public void UpdateDb(SteamAppId gameID)
        {
            updateGame = gameID;
            CreateUrlPool((int)gameID);
            timer = new System.Timers.Timer();
            timer.Interval = 6000;
            timer.AutoReset = true;
            for (int i = 0; i < 10; i++)
            {
                timer.Elapsed += Download;             
            }
            timer.Start();
        }

        private void Download(object sender, ElapsedEventArgs e)
        {
            if (tryCount >= 10)
            {
                if (stopFlag)
                    return;
                timer.Stop();
                stopFlag = true;
                if (uriPool.Count != 0)
                {
                    UpdatedDb(this, new UpdatedDbEventArgs(itemsDowload, updateGame.ToString(), false));
                    return;
                }
                else
                {
                    UpdatedDb(this, new UpdatedDbEventArgs(itemsDowload, updateGame.ToString(), true));
                    return;
                }                
            }
            if (uriPool.Count != 0 && proxyClassList.Count != 0)
            {
                DowloadPricesItems(uriPool.Dequeue(), proxyClassList.Dequeue());
            }
            else
            {
                tryCount++;
            }
        }

        private void DowloadPricesItems(Uri url, WebProxy _proxy)
        {
            string json = "";
            var jobject = new JObject();
            if (_proxy == null || url == null)
                return;
            WebProxy proxy = _proxy;
            HttpWebResponse responce;
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Proxy = proxy;
            request.Timeout = 5000;
            request.ReadWriteTimeout = 15000;
            try
            {
                responce = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(responce.GetResponseStream()))
                {
                    json = reader.ReadToEnd();
                    responce.Close();
                }
                jobject = (JObject)JsonConvert.DeserializeObject(json);
            }
            catch
            {
                uriPool.Enqueue(url);
                return;
            }
            proxyClassList.Enqueue(_proxy);
            var dom = CQ.Create(jobject["results_html"].ToString());
            var ListItemName = dom[".market_listing_item_name"].Select(x => x.Cq().Text()).ToList();
            var ListPricesItems = dom[".sale_price"].Select(x => x.Cq().Text()).ToList();
            var ListUrlImageItems = dom["img"].Select(x => x.GetAttribute("src")).ToList();
            var NamePrice = ListItemName.Zip(ListPricesItems, (name, price) =>
            new
            {
                Name = name,
                Price = price
            });
            var Items = NamePrice.Zip(ListUrlImageItems, (Nameprice, Url) =>
            new
            {
                Name = Nameprice.Name,
                Price = Nameprice.Price,
                Url = Url
            });
            lock (locker)
            {
                foreach (var item in Items)
                {
                    if (repository.Get(item.Name)==null)
                    {
                        repository.Create(new Item
                        {
                            Name = item.Name,
                            Price = ParseStringToDecimal(item.Price),
                            Url_Image = item.Url,
                            GameName = updateGame.ToString()
                        });
                    }
                    else
                    {
                        var itemUpdPrice = repository.Get(item.Name);
                        itemUpdPrice.Price = ParseStringToDecimal(item.Price);
                    }
                    itemsDowload++;
                }
                repository.Save();               
            }
        }

        private void CreateUrlPool(int appId)
        {
            var targetPage = $"http://steamcommunity.com/market/search/render/?query=&appid={appId}&count=100";
            var request = WebRequest.CreateHttp(targetPage);
            var responce = request.GetResponse();
            string json = "";
            using (StreamReader reader = new StreamReader(responce.GetResponseStream()))
            {
                json = reader.ReadToEnd();
                responce.Close();
            }
            var jobject = (JObject)JsonConvert.DeserializeObject(json);
            totalItems = (int)jobject.SelectToken("total_count");
            for (int i = 0; i <= totalItems; i += 100)
            {
                var uri = $"{targetPage}&start={i}";
                uriPool.Enqueue(new Uri(uri));
            }
        }

        private void SetProxyList(string pathProxyList)
        {
            var proxyList = new List<string>();
            using (StreamReader reader = new StreamReader(pathProxyList))
            {
                while (!reader.EndOfStream)
                { proxyList.Add(reader.ReadLine()); }
            }
            foreach (var stringProxy in proxyList)
            {
                var splitString = stringProxy.Split(new[] { ':' });
                var ip = splitString[0];
                var port = int.Parse(splitString[1]);
                proxyClassList.Enqueue(new WebProxy(ip, port));
            }
        }

        private decimal ParseStringToDecimal(string priceString)
        {
            if (priceString.IndexOf('$') >= 0)
                priceString = priceString.Remove(priceString.IndexOf('$'), 1);
            if (priceString.IndexOf("USD") >= 0)
                priceString = priceString.Remove(priceString.IndexOf("USD"), 3);
            var price = decimal.Parse(priceString, new CultureInfo("en-US"));
            return price;
        }
    }
}