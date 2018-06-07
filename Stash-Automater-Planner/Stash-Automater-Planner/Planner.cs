using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

namespace Stash_Automater_Planner
{
    public class Planner
    {
        string ConfigFile = "";

        List<StashTab> sourceTabs = new List<StashTab>();
        List<StashTab> targetTabs = new List<StashTab>();
        StashTab trashTab = new StashTab();

        Inventory inventory;

        List<Action> actions = new List<Action>();

        SortConfig config;

        public MoveOrganizer moveOrganizer = new MoveOrganizer();

        public Planner(string configFile, string sortConfigName)
        {
            ConfigFile = configFile;

            string json = File.ReadAllText(Directory.GetCurrentDirectory() + "\\" + configFile);
            JObject jObject = JObject.Parse(json);
            List<SortConfig> configs = JsonConvert.DeserializeObject < List<SortConfig>>(jObject["sortConfigs"].ToString());
            config = configs.Where(x => x.name == sortConfigName).Single();

            InitializeInventory();
            InitializeStashData();
            CreatePlan();
            
        }

        private void InitializeInventory()
        {
            inventory = JsonConvert.DeserializeObject<Inventory>(ToolBox.GetInventory(ConfigFile).ToString());
            inventory.type = TabType.Inventory;

            inventory.items = inventory.items.Where(x => x.inventoryId == "MainInventory").ToList();
        }

        private void InitializeStashData() {

            ApiRequest stashes = JsonConvert.DeserializeObject<ApiRequest>(ToolBox.GetStashes(ConfigFile).ToString());

            List<string> fetchTabs_source = new List<string>(config.sourceTabs);

            List<string> fetchTabs_target = new List<string>();
            foreach (var t in config.targetTabs) {
                foreach(var tt in t.targetStashes) {
                    fetchTabs_target.Add(tt);
                }
            }

            //Initialize stashes sources
            string currentFetch = "";
            try {
                foreach (var s in fetchTabs_source) {
                    if (sourceTabs.Where(x => x.name == s).ToList().Count>0)
                        continue;
                    currentFetch = s;
                    StashTab tab = stashes.stashTabs.Single(x => x.name == s);
                    sourceTabs.Add(tab);
                }
            }
            catch(InvalidOperationException e) {
                throw new Exception("Error when looking for tab " + currentFetch + " .Either 0 or multiple matches");
            }
            
            //Initialize stashes targets
            try {
                foreach (var s in fetchTabs_target) {
                    if (targetTabs.Where(x => x.name == s).ToList().Count > 0)
                        continue;
                    currentFetch = s;
                    StashTab tab = stashes.stashTabs.Single(x => x.name == s);
                    targetTabs.Add(tab);
                }
            }
            catch (InvalidOperationException e) {
                throw new Exception("Error when looking for tab " + currentFetch + " .Either 0 or multiple matches");
            }

            //Initialize trash tab
            try {
                
                currentFetch = config.inventoryToStash;
                StashTab tab = stashes.stashTabs.Single(x => x.name == config.inventoryToStash);
                trashTab = tab;
                

                
            }
            catch (InvalidOperationException e) {
                throw new Exception("Error when looking for tab " + currentFetch + " .Either 0 or multiple matches");
            }


            //Fetch items sources
            foreach (StashTab st in sourceTabs) {
                ApiRequest stash = JsonConvert.DeserializeObject<ApiRequest>(ToolBox.GetStashes(ConfigFile, st.index).ToString());

                st.items = stash.items;
            }

            //Fetch items targets
            foreach (StashTab st in targetTabs) {
                ApiRequest stash = JsonConvert.DeserializeObject<ApiRequest>(ToolBox.GetStashes(ConfigFile, st.index).ToString());

                st.items = stash.items;
            }

            //Fetch trashtab items
            ApiRequest Tstash = JsonConvert.DeserializeObject<ApiRequest>(ToolBox.GetStashes(ConfigFile, trashTab.index).ToString());

            trashTab.items = Tstash.items;

            int k = 0;
            //StashTab oneH = Tabs.Single(x => x.name == "1H");
            //Item carved = oneH.items.Single(x => x.typeLine == "Carved Wand");
            //StashTab He = Tabs.Single(x => x.name == "Ch");
            //
            //bool sucess = ToolBox.MoveItem(oneH, He, carved);

        }

        private void CreatePlan()
        {
            Queue<Item> currentInventory_initial = new Queue<Item>(inventory.items.Where(x => true));

            Item inventItem_initial = null;
            while (currentInventory_initial.TryDequeue(out inventItem_initial)) {
                if (!ToolBox.MoveItem(inventory, trashTab, inventItem_initial, moveOrganizer)) {

                    //Move to trashtab if full
                    if (!ToolBox.MoveItem(inventory, trashTab, inventItem_initial, moveOrganizer)) {
                        throw new Exception("No room in trash tab, terminating!");
                    }
                }
            }


            List<TargetTab> sortedTargetTabs = config.targetTabs.OrderBy(x => x.order).ToList();

            foreach(StashTab source in sourceTabs) {

                foreach(TargetTab target in sortedTargetTabs) {
                    StashTab targetTab = targetTabs.Single(x => target.targetStashes.Contains(x.name));

                    Queue<Item> matchesTarg = new Queue<Item>(source.items.Where(x => ToolBox.MatchTargetTabItem(x, target)));

                    //Move item to inventory
                    Item currentItem = null;
                    while (matchesTarg.TryDequeue(out currentItem)) {
                        if (!ToolBox.MoveItem(source, inventory, currentItem, moveOrganizer)) {

                            //When inventory full, move items from inventory to targetTab
                            Queue<Item> currentInventory = new Queue<Item>(inventory.items.Where(x => true));

                            Item inventItem = null;
                            while (currentInventory.TryDequeue(out inventItem)) {
                                if (!ToolBox.MoveItem(inventory, targetTab, inventItem, moveOrganizer)) {

                                    //Move to trashtab if full
                                    if (!ToolBox.MoveItem(inventory, trashTab, inventItem, moveOrganizer)) {
                                        throw new Exception("No room in trash tab, terminating!");
                                    }
                                }
                            }

                            if (!ToolBox.MoveItem(source, inventory, currentItem, moveOrganizer)) {
                                throw new Exception("No room in inventory after it was emptied");
                            }
                        }
                    }

                    //Finally move all items from inventory to targetTab
                    Queue<Item> currentInventory_final = new Queue<Item>(inventory.items.Where(x => true));
                    Item inventItem_final = null;
                    while (currentInventory_final.TryDequeue(out inventItem_final)) {
                        if (!ToolBox.MoveItem(inventory, targetTab, inventItem_final, moveOrganizer)) {

                            if (!ToolBox.MoveItem(inventory, trashTab, inventItem_final, moveOrganizer)) {
                                throw new Exception("No room in trash tab, terminating!");
                            }
                        }
                    }
                }
            }

            moveOrganizer.finalize();

            int limit = 0;
            StreamWriter writer = File.CreateText("C:\\PoE\\Automater\\moveScript.ahk");
            foreach(string line in moveOrganizer.moves) {
                writer.WriteLine(line);

                //if(limit > 100) {
                //    break;
                //}

                limit++;
            }
            writer.Close();

            Console.WriteLine("Enter to start sorting");

            Console.ReadLine();
            
        }
    }
    
}
