using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Stash_Automater_Planner
{
    public class SortConfig
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("inventoryToStash")]
        public string inventoryToStash { get; set; }
        
        [JsonProperty("targetTabs")]
        public List<TargetTab> targetTabs { get; set; }

        [JsonProperty("sourceTabs")]
        public List<string> sourceTabs { get; set; }

        [JsonProperty("chaosRecipeTabs")]
        public List<string> chaosRecipeTabs { get; set; }

    }

    public class TargetTab
    {
        [JsonProperty("name")]
        string name { get; set; }

        [JsonProperty("targetStash")]
        public List<string> targetStashes { get; set; }

        [JsonProperty("regexGroups")]
        public List<string> regexGroups { get; set; }
        
        [JsonProperty("order")]
        public int order { get; set; }

        [JsonProperty("chaosRecipeCount")]
        public int chaosRecipeCount { get; set; }

    }
}
