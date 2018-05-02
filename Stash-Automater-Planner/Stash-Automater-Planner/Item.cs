using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Stash_Automater_Planner
{
    public class Item
    {
        [JsonProperty("abyssJewel")]
        bool abyssJewel { get; set; }

        [JsonProperty("additionalProperties")]
        List<Property> additionalProperties { get; set; }

        [JsonProperty("category")]
        Category category { get; set; }

        [JsonProperty("corrupted")]
        bool corrupted { get; set; }

        [JsonProperty("cosmeticMods")]
        List<string> cosmeticMods { get; set; }

        [JsonProperty("craftedMods")]
        List<string> craftedMods { get; set; }

        [JsonProperty("descrText")]
        string description { get; set; }

        [JsonProperty("dubplicated")]
        bool dubplicated { get; set; }

        [JsonProperty("elder")]
        bool elder { get; set; }

        [JsonProperty("enchantMods")]
        List<string> enchantMods { get; set; }

        [JsonProperty("explicitMods")]
        List<string> explicitMods { get; set; }

        [JsonProperty("flavourText")]
        List<string> flavourText { get; set; }

        [JsonProperty("frameType")]
        public int frameType { get; set; }

        [JsonProperty("h")]
        public int height { get; set; }

        [JsonProperty("icon")]
        string iconUrl { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("identified")]
        public bool identified { get; set; }

        [JsonProperty("ilvl")]
        public int ilvl { get; set; }

        [JsonProperty("implicitMods")]
        List<string> implicitMods { get; set; }

        [JsonProperty("inventoryId")]
        public string inventoryId { get; set; }

        [JsonProperty("isRelic")]
        bool isRelic { get; set; }

        [JsonProperty("league")]
        string league { get; set; }

        [JsonProperty("lockedToCharacter")]
        bool lockedToCharacter { get; set; }

        [JsonProperty("maxStackSize")]
        int maxStackSize { get; set; }

        [JsonProperty("name")]
        string name { get; set; }

        [JsonProperty("nextLevelRequierments")]
        List<Property> nextLevelReqs { get; set; }

        [JsonProperty("note")]
        string note { get; set; }

        [JsonProperty("properties")]
        List<Property> properties { get; set; }

        [JsonProperty("prophecyDiffText")]
        string prophecyDiffText { get; set; }

        [JsonProperty("prophecyText")]
        string prophecyText { get; set; }

        [JsonProperty("requirements")]
        List<Property> requirements { get; set; }

        [JsonProperty("secDescrText")]
        string secDescrText { get; set; }

        [JsonProperty("shaper")]
        bool shaper { get; set; }

        [JsonProperty("socketedItems")]
        List<Item> socketedItems { get; set; }

        [JsonProperty("sockets")]
        List<Socket> sockets { get; set; }

        [JsonProperty("stackSize")]
        int stackSize { get; set; }

        [JsonProperty("support")]
        bool support { get; set; }

        [JsonProperty("talismanTier")]
        int talismanTier { get; set; }

        [JsonProperty("typeLine")]
        public string typeLine { get; set; }

        [JsonProperty("utilityMods")]
        List<string> utilityMods { get; set; }

        [JsonProperty("verified")]
        bool verified { get; set; }

        [JsonProperty("w")]
        public int width { get; set; }

        [JsonProperty("x")]
        public int x { get; set; }

        [JsonProperty("y")]
        public int y { get; set; }

        public int targetStashIndex { get; set; }
    }

    public class Property
    {
        [JsonProperty("name")]
        string name { get; set; }

        [JsonProperty("values")]
        List<List<string>> values { get; set; }

        [JsonProperty("displayMode")]
        int displayMode { get; set; }

        [JsonProperty("type")]
        int propertyType { get; set; }

        [JsonProperty("progress")]
        double xpProgress { get; set; }
        
    }
    
    public class Category
    {
        [JsonProperty("maps")]
        List<string> maps { get; set; }

        [JsonProperty("currency")]
        List<string> currency { get; set; }

        [JsonProperty("jewels")]
        List<string> jewels { get; set; }

        [JsonProperty("gems")]
        List<string> gems { get; set; }

        [JsonProperty("cards")]
        List<string> cards { get; set; }

        [JsonProperty("flasks")]
        List<string> flasks { get; set; }

        [JsonProperty("weapons")]
        List<string> weapons { get; set; }

        [JsonProperty("accessories")]
        List<string> accessories { get; set; }
    }

    public class Socket
    {
        [JsonProperty("group")]
        int groupId { get; set; }


        [JsonProperty("attr")]
        private string _attribute { get; set; }

        public SocketType type { get; set; }
    }

    public enum SocketType { Str, Int, Dex, White, Abyss, Default}
}
