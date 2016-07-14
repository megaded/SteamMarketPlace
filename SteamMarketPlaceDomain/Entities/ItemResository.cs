using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamMarketPlaceDomain
{
    public class ItemRepository : IRepository<Item>
    {
        private ItemContext context;

        public ItemRepository()
        {
            context = new ItemContext();
        }

        public void Create(Item item)
        {
            context.Items.Add(item);
        }     

        public Item Get(string name)
        {
            var item = context.Items.Where(i => i.Name == name).FirstOrDefault();
            return item;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public IEnumerable<Item> GetAll()
        {
            return context.Items.ToList();
        }

        public void Dispose()
        {
            context.Dispose();
        }
      
    }
}
