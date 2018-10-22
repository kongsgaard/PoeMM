using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

namespace Stash_Automater_Planner {
    public class Planner {
        string ConfigFile = "";

        SortConfig config;
        ChaosConfig chaosConfig;

        public MoveOrganizer moveOrganizer;
        public StashFramework stashFramework;

        public Planner(MoveOrganizer organizer, StashFramework framework) {
            moveOrganizer = organizer;
            stashFramework = framework;
        }
        
        public void CreatePlan(string sortConfigFile) {

            string json = File.ReadAllText(Directory.GetCurrentDirectory() + "\\" + sortConfigFile);
            JObject jObject = JObject.Parse(json);
            config = JsonConvert.DeserializeObject<SortConfig>(jObject.ToString());
            
            List<ConfigItemtype> SortedItemTypes = config.configItemTypes.OrderBy(x => x.order).ToList();

            List<ItemTab> sourceTabs = new List<ItemTab>();

            foreach (string tabName in config.sourceTabs) {
                sourceTabs.Add(stashFramework.StashByName(tabName));
            }

            foreach (ConfigItemtype configItemType in SortedItemTypes) {
                var ItemsToMove = new Queue<Item>(sourceTabs.SelectMany(x => x.items).Where(y => y.itemType == configItemType.itemtype));
                
                Item currentItem = null;
                while (ItemsToMove.TryDequeue(out currentItem)) {
                    if (ToolBox.MoveItem(currentItem.myStashTab, stashFramework.inventory, currentItem, moveOrganizer)) {

                    }
                    else {
                        EmptyInventory();
                        if (ToolBox.MoveItem(currentItem.myStashTab, stashFramework.inventory, currentItem, moveOrganizer)) {

                        }
                        else {
                            EmptyInventory();
                            throw new Exception("Couldn't fit item after inventory was emptied");
                        }
                    }
                }
            }

            //Finally empty inventory one last time
            EmptyInventory();
            
        }

        private void EmptyInventory() {

            List<ConfigItemtype> SortedItemTypes = config.configItemTypes.OrderBy(x => x.order).ToList();

            foreach (ConfigItemtype configItemType in SortedItemTypes) {

                Queue<Item> ItemsToRemove = new Queue<Item>(stashFramework.inventory.items);
                Item inventItem = null;
                while(ItemsToRemove.TryDequeue(out inventItem)) { 
                    
                    bool itemMoved = false;
                    foreach (ConfigTargetStash configTargetStash in configItemType.targetStashes.OrderBy(x => x.order)) {
                        if(itemMoved == true) {
                            break;
                        }

                        ItemTab targetStash = stashFramework.StashByName(configTargetStash.stashName);

                        //If current item stash has same order, indicate that the item should not move.
                        if(configItemType.targetStashes.Where(x => x.order == configTargetStash.order).Where(x => x.stashName == targetStash.name).Count() > 0) {
                            itemMoved = true;
                            break;
                        }

                        //Check limit and reqs for other items in same stash
                        bool AllItemTypesOK = true;
                        if (configTargetStash.limit != -1) {
                            var Stashes = SortedItemTypes.Where(x => x.targetStashes.Exists(y => y.stashName == configTargetStash.stashName));

                            var ItemTypesWithSameStashName = from x in SortedItemTypes
                                                             where x.targetStashes.Exists(y => y.stashName == configTargetStash.stashName && y.limit > -1)
                                                             select new Tuple<int, ConfigTargetStash, ItemType>(x.order, x.targetStashes.Single(y => y.stashName == configTargetStash.stashName), x.itemtype);

                            var OrderedItemTypesWithSame = ItemTypesWithSameStashName.OrderBy(x => x.Item1);

                            foreach (var tuple in OrderedItemTypesWithSame) {

                                int countItemType = targetStash.items.Where(x => x.itemType == tuple.Item3).Count();

                                if (tuple.Item3 == configItemType.itemtype) {
                                    if (countItemType >= tuple.Item2.limit) {
                                        AllItemTypesOK = false;
                                        break;
                                    }
                                    else {
                                        break;
                                    }
                                }

                                if (countItemType != tuple.Item2.limit) {
                                    AllItemTypesOK = false;
                                    break;
                                }
                            }
                        }
                        else {
                            if (AllItemTypesOK || configTargetStash.limit == -1) {
                                if (ToolBox.MoveItem(stashFramework.inventory, targetStash, inventItem, moveOrganizer)) {
                                    itemMoved = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (itemMoved == false) {
                        Console.WriteLine("Could not move item " + inventItem.typeLine + ". Terminating after this point");
                        return;
                    }
                }
            }
        }

        public void SellChaosRecipe(string chaosConfigPath) {

            string json = File.ReadAllText(Directory.GetCurrentDirectory() + "\\" + chaosConfigPath);
            JObject jObject = JObject.Parse(json);
            chaosConfig = JsonConvert.DeserializeObject<ChaosConfig>(jObject.ToString());

            List<ItemTab> sourceTabs = new List<ItemTab>();

            foreach (string tabName in chaosConfig.sourceTabs) {
                sourceTabs.Add(stashFramework.StashByName(tabName));
            }

            Dictionary<ItemType, List<Item>> ItemItemTypeDict = new Dictionary<ItemType, List<Item>>();

            foreach(ItemBucket bucket in chaosConfig.buckets) {
                var items = sourceTabs.SelectMany(x => x.items).Where(y => y.itemType == bucket.itemtype).ToList();
                ItemItemTypeDict.Add(bucket.itemtype, items);
            }

            

            bool CouldComplete = true;
            while (CouldComplete) {
                bool GotChaosComponent = false;
                List<Item> QueuedItems = new List<Item>();

                foreach (ItemBucket bucket in chaosConfig.buckets) {

                    for (int i = 0; i < bucket.count; i++) {
                        var items = ItemItemTypeDict[bucket.itemtype];

                        var itemsRegal = items.Where(x => x.ilvl >= 75).ToList();
                        var itemsChaos = items.Where(x => x.ilvl < 75).ToList();


                        Item currentItem = null;

                        if (GotChaosComponent) {
                            if (itemsRegal.Count > 0) {
                                currentItem = itemsRegal[0];
                                itemsRegal.RemoveAt(0);
                            }
                            else if (itemsChaos.Count > 0) {
                                currentItem = itemsChaos[0];
                                itemsChaos.RemoveAt(0);
                            }
                            else {
                                if (itemsChaos.Count > 0) {
                                    currentItem = itemsChaos[0];
                                    itemsChaos.RemoveAt(0);
                                }
                                else if (itemsRegal.Count > 0) {
                                    currentItem = itemsRegal[0];
                                    itemsRegal.RemoveAt(0);
                                }
                            }
                        }
                        if(currentItem != null) {
                            QueuedItems.Add(currentItem);
                        }
                        else {
                            CouldComplete = false;
                            break;
                        }
                    }
                }

                if(chaosConfig.completeRegal == false && GotChaosComponent == false) {
                    CouldComplete = false;
                    break;
                }

                //Move all items in queue
                foreach(Item i in QueuedItems) {
                    if(!ToolBox.MoveItem(i.myStashTab,stashFramework.inventory, i, moveOrganizer)) {
                        CouldComplete = false;
                        break;
                    }
                }

                //Sell to vendor
                ItemTab vendor = new ItemTab();
                vendor.type = TabType.Inventory;
                vendor.items = new List<Item>();
                
                moveOrganizer.SellToVendor(QueuedItems, stashFramework.inventory, vendor);


                //Move items from inventory to stash
                ItemTab targetTab = stashFramework.StashByName(chaosConfig.resultToTab);
                foreach(Item i in stashFramework.inventory.items) {
                    if(i.itemType != ItemType.Currency) {
                        throw new Exception("Unexpected non-currency item in inventory after sale");
                    }
                    else {
                        if(!ToolBox.MoveItem(stashFramework.inventory,targetTab,i, moveOrganizer)) {
                            throw new Exception("Could not move item to stash tab after vendor trade");
                        }
                    }
                }
            }
        }
    }

}
