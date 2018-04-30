using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Stash_Automater_Planner
{
    public class StashTab
    {
        [JsonProperty("n")]
        string name { get; set; }

        [JsonProperty("i")]
        int index { get; set; }

        [JsonProperty("type")]
        TabType type { get; set; }

        [JsonProperty("hidden")]
        bool hidden { get; set; }

        [JsonProperty("selected")]
        bool selected { get; set; }
    }

    public enum TabType { NormalStash, PremiumStash, QuadStash, DivinationCardStash, CurrencyStash, EssenceStash, MapStash }
}
