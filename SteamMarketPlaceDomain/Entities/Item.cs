using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;


namespace SteamMarketPlaceDomain
{
    public class Item : IComparable<Item>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Url_Image { get; set; }
        public string GameName { get; set; }
        public int CompareTo(Item other)
        {
            if (Price > other.Price)
                return -1;
            if (Price < other.Price)
                return 1;
            else return 0;
        }
    }
}
