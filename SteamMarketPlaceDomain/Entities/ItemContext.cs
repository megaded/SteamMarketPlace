﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace SteamMarketPlaceDomain
{
  public  class ItemContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
    }
}
