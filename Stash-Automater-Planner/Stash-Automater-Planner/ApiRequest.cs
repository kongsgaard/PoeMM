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
        List<StashTab> stashTabs { get; set; }

    }
}
