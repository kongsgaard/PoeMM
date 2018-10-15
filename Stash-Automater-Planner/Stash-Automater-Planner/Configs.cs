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

}
