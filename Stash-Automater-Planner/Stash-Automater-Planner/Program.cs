using System;
using Newtonsoft.Json.Linq;

namespace Stash_Automater_Planner
{
    class Program
    {
        static void Main(string[] args)
        {
            JObject ob = ApiToolBox.GetStashes("StashConfig.json");
            int k = 0;
        }
    }
}
