using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamMarketPlaceDomain
{
   public interface IRepository<Item> :IDisposable
    {
        Item Get(string name);
        IEnumerable<Item> GetAll();
        void Create(Item item);
        void Save();
    }
}
