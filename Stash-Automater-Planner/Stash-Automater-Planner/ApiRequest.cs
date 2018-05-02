using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Stash_Automater_Planner
{
    public class ApiRequest
    {
        [JsonProperty("numTabs")]
        public int numTabs { get; set; }

        [JsonProperty("tabs")]
        public List<StashTab> stashTabs { get; set; }

        [JsonProperty("items")]
        public List<Item> items { get; set; }
    }
}
