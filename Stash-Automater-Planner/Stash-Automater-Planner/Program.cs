using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Stash_Automater_Planner
{
    class Program
    {
        static void Main(string[] args)
        {
            string ob = ApiToolBox.GetStashes("StashConfig.json").ToString();

            ApiRequest req = JsonConvert.DeserializeObject<ApiRequest>(ob);
            int k = 0;
        }
    }
}
