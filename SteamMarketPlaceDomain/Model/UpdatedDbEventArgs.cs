using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamMarketPlaceDomain
{
  public  class UpdatedDbEventArgs:EventArgs
    {
        public int DowloadItems { get; set; }
        public string NameGame;
        public bool Result;
        private UpdatedDbEventArgs()
        {

        }
        public UpdatedDbEventArgs(int dowloadItems,string nameGame,bool result)
        {
            DowloadItems = dowloadItems;
            NameGame = nameGame;
            Result = result;
        }  
    }
}
