using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Stash_Automater_Planner
{


    public class StashConfig {
        [JsonProperty("account")]
        public string name { get; set; }

        [JsonProperty("character")]
        public string inventoryToStash { get; set; }

        [JsonProperty("POESESSID")]
        public string targetTabs { get; set; }

        [JsonProperty("league")]
        public string sourceTabs { get; set; }

        [JsonProperty("StashX")]
        public int stashX { get; set; }

        [JsonProperty("StashY")]
        public int stashY { get; set; }

        [JsonProperty("VendorX")]
        public int vendorX { get; set; }

        [JsonProperty("VendorY")]
        public int vendorY { get; set; }

        [JsonProperty("VendorSellDialogX")]
        public int VendorSellDialogX { get; set; }

        [JsonProperty("VendorSellDialogY")]
        public int VendorSellDialogY { get; set; }

        [JsonProperty("VendorSellButtonX")]
        public int VendorSellButtonX { get; set; }

        [JsonProperty("VendorSellButtonY")]
        public int VendorSellButtonY { get; set; }
        
    }

    public class SortConfig {
        [JsonProperty("Name")]
        public string name { get; set; }

        [JsonProperty("SourceTabs")]
        public List<string> sourceTabs { get; set; }

        [JsonProperty("ItemTypes")]
        public List<ConfigItemtype> configItemTypes { get; set; }
    }

    public class ConfigItemtype
    {
        [JsonProperty("ItemType")]
        public ItemType itemtype { get; set; }

        [JsonProperty("Order")]
        public int order { get; set; }

        [JsonProperty("TargetStashes")]
        public List<ConfigTargetStash> targetStashes { get; set; }
    }

    public class ConfigTargetStash {
        [JsonProperty("StashName")]
        public string stashName { get; set; }

        [JsonProperty("Order")]
        public int order { get; set; }

        [JsonProperty("Limit")]
        public int limit { get; set; }
    }

    public class ChaosConfig {
        [JsonProperty("SourceTabs")]
        public List<string> sourceTabs { get; set; }

        [JsonProperty("ResultToTab")]
        public string resultToTab { get; set; }

        [JsonProperty("CompleteRegal")]
        public bool completeRegal { get; set; }


        [JsonProperty("ItemBuckets")]
        public List<ItemBucket> buckets { get; set; }
        
    }

    public class ItemBucket {
        [JsonProperty("ItemType")]
        public ItemType itemtype { get; set; }

        [JsonProperty("Count")]
        public int count { get; set; }
    }

}
