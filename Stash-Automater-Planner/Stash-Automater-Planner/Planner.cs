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
        List<StashTab> chaosRecipeTabs = new List<StashTab>();
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

            List<string> chaosTabs = new List<string>();
            foreach (var t in config.chaosRecipeTabs) {
                chaosTabs.Add(t);
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

            //Initialize chaos recipe stashes
            string chaosFetch = "";
            try {
                foreach (var s in chaosTabs) {
                    if (chaosRecipeTabs.Where(x => x.name == s).ToList().Count > 0)
                        continue;
                    chaosFetch = s;
                    StashTab tab = stashes.stashTabs.Single(x => x.name == s);
                    chaosRecipeTabs.Add(tab);
                }
            }
            catch (InvalidOperationException e) {
                throw new Exception("Error when looking for tab " + chaosFetch + " .Either 0 or multiple matches");
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

            //Fetch items targets
            foreach (StashTab st in chaosRecipeTabs) {
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
            #region Clean inventory
            //Clean inventory
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
            #endregion

            #region Clean Droptabs
            //Clean drop tabs
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
                                bool inserted = false;

                                foreach (StashTab chaosTab in chaosRecipeTabs) {
                                    if(ToolBox.MoveItemToChaosRecipeTab(inventory, chaosTab, inventItem, moveOrganizer, config)) {
                                        inserted = true;
                                        break;
                                    }
                                }

                                if (inserted == false && !ToolBox.MoveItem(inventory, targetTab, inventItem, moveOrganizer)) {

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
                        bool inserted_final = false;

                        //Try moving to chaos recipe tab
                        foreach (StashTab chaosTab in chaosRecipeTabs) {
                            if (ToolBox.MoveItemToChaosRecipeTab(inventory, chaosTab, inventItem_final, moveOrganizer, config)) {
                                inserted_final = true;
                                break;
                            }
                        }


                        if (inserted_final==false && !ToolBox.MoveItem(inventory, targetTab, inventItem_final, moveOrganizer)) {

                            if (!ToolBox.MoveItem(inventory, trashTab, inventItem_final, moveOrganizer)) {
                                throw new Exception("No room in trash tab, terminating!");
                            }
                        }
                    }
                }
            }
            #endregion

            #region Setup chaos recipe tabs

            /*
            List<TargetTab> chaosSourceTabsSorted = sortedTargetTabs.Where(x => x.chaosRecipeCount > 0).ToList();

            foreach (TargetTab target in chaosSourceTabsSorted) {
                StashTab targetTab = targetTabs.Single(x => target.targetStashes.Contains(x.name));

                Queue<Item> matchesTarg = new Queue<Item>(targetTab.items.Where(x => ToolBox.MatchTargetTabItem(x, target)));

                foreach(StashTab chaosTab in chaosRecipeTabs) {

                    if(chaosTab.items.Count > 0) {
                        Item it = chaosTab.items.First()
                    }

                }


                //Move item to inventory
                Item currentItem = null;
                while (matchesTarg.TryDequeue(out currentItem)) {
                    if (!ToolBox.MoveItem(targetTab, inventory, currentItem, moveOrganizer)) {

                        //Try moving to chaos recipe tab
                        foreach (StashTab chaosTab in chaosRecipeTabs) {
                            if (ToolBox.MoveItemToChaosRecipeTab(inventory, chaosTab, currentItem, moveOrganizer, config)) {
                                break;
                            }
                        }

                    }
                }

            }
             */
            #endregion

            #region Count item classes
            List<TargetTab> chaosSourceTabTypes = sortedTargetTabs.Where(x => x.chaosRecipeCount > 0).ToList();

            var RegenGroupsForChaosRecipe = chaosSourceTabTypes.SelectMany(x => x.regexGroups).Select(y => y).ToList();

            var StashTabsWithChaosRecipeItems = chaosSourceTabTypes.SelectMany(x => x.targetStashes).Select(y => y).ToList();

            StashTabsWithChaosRecipeItems.AddRange(config.sourceTabs);
            StashTabsWithChaosRecipeItems.AddRange(config.chaosRecipeTabs);

            var AllTabs = targetTabs.Union(sourceTabs).Union(chaosRecipeTabs).ToList();

            Console.WriteLine("Item Counts");
            
            foreach(string regexGrp in RegenGroupsForChaosRecipe) {
                int countForCurrent = 0;

                foreach(string tabName in StashTabsWithChaosRecipeItems) {
                    
                    StashTab currentTab = AllTabs.Single(x => x.name == tabName);
                    
                    foreach (Item it in currentTab.items) {
                        if(RegexGroup.MatchItem(it, regexGrp)) {
                            countForCurrent++;
                        }
                        
                    }

                }

                Console.WriteLine(regexGrp + ": " + countForCurrent.ToString());
            }

            #endregion


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

            Console.WriteLine("Total seconds to move: " + (moveOrganizer.TotalMS / 1000).ToString());
            
        }
    }
    
}
