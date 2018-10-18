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

                        ItemTab targetStash = stashFramework.StashByName(configTargetStash.stashName);

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
    }

}
