using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Stash_Automater_Planner
{
    public class StashTab : ItemTab
    {

        [JsonProperty("hidden")]
        bool hidden { get; set; }

        [JsonProperty("selected")]
        bool selected { get; set; }
        
    }

    public class Inventory : ItemTab
    {
        [JsonProperty("character")]
        public Character character { get; set; }
    }

    public class Character
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("league")]
        public string league { get; set; }

        [JsonProperty("classId")]
        public int classId { get; set; }

        [JsonProperty("ascendancyClass")]
        public int ascendencyClass { get; set; }

        [JsonProperty("class")]
        public string _class { get; set; }

        [JsonProperty("level")]
        public int level { get; set; }

        [JsonProperty("experience")]
        public long experience { get; set; }
    }

    public class ItemTab
    {
        [JsonProperty("i")]
        public int index { get; set; }

        [JsonProperty("n")]
        public string name { get; set; }

        [JsonProperty("type")]
        public TabType type { get; set; }

        private List<Item> _items;
        public List<Item> items
        {
            get { return _items; }

            set {
                _items = value;
                CreateGrid(value);
            }
        }

        public bool AddItem(Item item)
        {
            if(!(type == TabType.NormalStash || type == TabType.PremiumStash || type == TabType.QuadStash || type == TabType.Inventory || type == TabType.Vendor)) {
                return true;
            }

            int StashHeight = itemGrid.GetLength(0);
            int StashWidth = itemGrid.GetLength(1);
            //Iterate itemgrid to find insertion spot
            for (int x = 0; x < itemGrid.GetLength(1); x++) {
                for (int y = 0; y < itemGrid.GetLength(0); y++) {

                    if (itemGrid[y, x] == false) {
                        bool fits = true;

                        for (int widthCounter = 0; widthCounter < item.width; widthCounter++) {
                            for (int heightCounter = 0; heightCounter < item.height; heightCounter++) {

                                //See if tab index is taken
                                if(y + heightCounter >= StashHeight || x + widthCounter >= StashWidth) {
                                    fits = false;
                                    break;
                                }
                                if (itemGrid[y + heightCounter, x + widthCounter] == true) {
                                    fits = false;
                                }

                            }
                        }

                        if (fits) {
                            //Insert item
                            item.x = x;
                            item.y = y;

                            if(type == TabType.Inventory) {
                                item.inventoryId = "MainInventory";
                            }
                            else if (type == TabType.Vendor) {
                                item.inventoryId = "Vendor";
                            }
                            else {
                                item.inventoryId = "Stash" + (1 + index).ToString();
                            }
                            _items.Add(item);

                            //Mark index in itemGrid as taken
                            for (int widthCounter = 0; widthCounter < item.width; widthCounter++) {
                                for (int heightCounter = 0; heightCounter < item.height; heightCounter++) {
                                    itemGrid[item.y + heightCounter, item.x + widthCounter] = true;
                                }
                            }

                            return true;
                        }

                    }

                }
            }
            return false;

        }

        public bool RemoveItem(Item item)
        {
            int countBefore = _items.Count;
            _items.RemoveAll(x => x.id == item.id);

            if (type == TabType.Inventory) {
                if (countBefore - 1 != _items.Count || item.inventoryId != "MainInventory") {
                    throw new Exception("Item " + item.id + " not found in stash " + name);
                }
                else {

                    //Mark index in itemGrid as free
                    for (int widthCounter = 0; widthCounter < item.width; widthCounter++) {
                        for (int heightCounter = 0; heightCounter < item.height; heightCounter++) {
                            itemGrid[item.y + heightCounter, item.x + widthCounter] = false;
                        }
                    }
                    return true;
                }
            }
            else if (type == TabType.NormalStash || type == TabType.PremiumStash || type == TabType.QuadStash) {
                if (countBefore - 1 != _items.Count || item.inventoryId != "Stash" + (1 + index).ToString()) {
                    throw new Exception("Item " + item.id + " not found in stash " + name);
                }
                else {

                    //Mark index in itemGrid as free
                    for (int widthCounter = 0; widthCounter < item.width; widthCounter++) {
                        for (int heightCounter = 0; heightCounter < item.height; heightCounter++) {
                            itemGrid[item.y + heightCounter, item.x + widthCounter] = false;
                        }
                    }
                    return true;
                }
            }
            else if (type == TabType.Vendor) {
                throw new NotImplementedException("Cant remove item from vendor window");
            }
            else return false;


        }

        //Itemgrid to indicate which index in the tab are taken
        public bool[,] itemGrid = new bool[1, 1];

        private void CreateGrid(List<Item> setItems)
        {

            if (type == TabType.NormalStash || type == TabType.PremiumStash) {
                itemGrid = new bool[12, 12];
            }
            else if (type == TabType.QuadStash) {
                itemGrid = new bool[24, 24];
            }
            else if (type == TabType.Inventory || type == TabType.Vendor) {
                itemGrid = new bool[5, 12];
            }
            else
                return;

            foreach (Item i in setItems) {
                for (int widthCounter = 0; widthCounter < i.width; widthCounter++) {
                    for (int heightCounter = 0; heightCounter < i.height; heightCounter++) {
                        itemGrid[i.y + heightCounter, i.x + widthCounter] = true;
                    }
                }
            }
        }
    }

    public enum TabType { NormalStash, PremiumStash, QuadStash, DivinationCardStash, CurrencyStash, EssenceStash, MapStash, Inventory, FragmentStash, Vendor }
}
