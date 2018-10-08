using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Stash_Automater_Planner
{
    class Program
    {
        static void Main(string[] args)
        {
            //string ob = ToolBox.GetStashes("StashConfig.json").ToString();
            //
            //ApiRequest req = JsonConvert.DeserializeObject<ApiRequest>(ob);
            try {
                Planner p = new Planner("StashConfig.json", "cleanDropTabs");

                Console.ReadKey();
            }
            catch (Exception e) {
                Console.WriteLine(e);

                Console.ReadLine();
            }

        }
    }
}
