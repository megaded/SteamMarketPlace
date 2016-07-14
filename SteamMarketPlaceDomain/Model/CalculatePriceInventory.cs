using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace SteamMarketPlaceDomain
{
    public class CalculatePriceInventory
    {

        private IRepository<Item> repository;

        public CalculatePriceInventory(IRepository<Item> repository)
        {
            this.repository = repository;
        } 
      
        public IEnumerable<Item> GetInventoryItems(string url, int appId)
        {
            IEnumerable<Item> result = new List<Item>();
            string responce_string = "";
            string pathInventory = $"/inventory/json/{appId}/2";
            Uri UrlSteamInventory = new Uri(url + pathInventory);
            HttpWebRequest request = WebRequest.CreateHttp(UrlSteamInventory);
            HttpWebResponse responce = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(responce.GetResponseStream()))
            {
                responce_string = reader.ReadToEnd();
            }
            var jsonobj = (JObject)JsonConvert.DeserializeObject(responce_string);
            var success = jsonobj["success"].ToString();
            if (!bool.Parse(success))
            {
                return null;
            }
            else {
                //Получение коллекции  Id предмета rgInventory все предметы по ID.
                var ItemsIdCount = jsonobj["rgInventory"].Children().Select(x => x.First()["classid"].ToString());
                //Получение коллекции  Id предмета и название. rgDescriptions содержит описание предметов с ID. Описания не дублируются.
                var TempItemsName = jsonobj["rgDescriptions"].Children().Select(x => new
                {
                    id = x.First()["classid"].ToString(),
                    name = x.First()["market_hash_name"].ToString()
                });
                var ItemsIdName = TempItemsName.GroupBy(x => x.id, (key, x) => x.FirstOrDefault());
                // Сопоставление Id и Name.
                var ItemsNames = from itemsIdCount in ItemsIdCount
                                 from itemIdName in ItemsIdName
                                 where itemIdName.id == itemsIdCount
                                 select itemIdName.name;
                var allItems = repository.GetAll();
                result = from itemName in ItemsNames
                         from itemRep in allItems
                         where itemName == itemRep.Name
                         select itemRep;
                return result;
            }
        }
    }
}
