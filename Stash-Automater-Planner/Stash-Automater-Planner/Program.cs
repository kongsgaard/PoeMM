using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stash_Automater_Planner
{
    class Program
    {
        static void Main(string[] args)
        {
            //string ob = ToolBox.GetStashes("StashConfig.json").ToString();
            //
            //ApiRequest req = JsonConvert.DeserializeObject<ApiRequest>(ob);

            List<string> sortConfigs = new List<string>();
            sortConfigs.Add("Configs\\CleanInventory.json");
            sortConfigs.Add("Configs\\CleanDropTabs.json");

            StashFramework stashFramework = new StashFramework("Configs\\StashConfig.json", sortConfigs);

            MoveOrganizer organizer = new MoveOrganizer();

            Planner p = new Planner(organizer, stashFramework);

            p.CreatePlan("Configs\\CleanInventory.json");
            p.CreatePlan("Configs\\CleanDropTabs.json");

            //Count items and write
            foreach (ItemType type in Enum.GetValues(typeof(ItemType))) {

                int count = stashFramework.AllTabs.SelectMany(x => x.items.Where(y => y.itemType == type)).Count();

                Console.WriteLine(type.ToString() + ": " + count.ToString());
            }

            organizer.finalize();

            int limit = 0;
            StreamWriter writer = File.CreateText("C:\\PoE\\Automater\\moveScript.ahk");
            foreach (string line in organizer.moves) {
                writer.WriteLine(line);

                //if(limit > 100) {
                //    break;
                //}

                limit++;
            }
            writer.Close();

            Console.WriteLine("Total seconds to move: " + (organizer.TotalMS / 1000).ToString());



            Console.ReadLine();
        }
    }
}
