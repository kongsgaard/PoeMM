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
        List<Property> properties { get; set; }

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
        int frameType { get; set; }

        [JsonProperty("h")]
        int height { get; set; }

        [JsonProperty("icon")]
        string iconUrl { get; set; }

        [JsonProperty("id")]
        string id { get; set; }

        [JsonProperty("identified")]
        bool identified { get; set; }

        [JsonProperty("ilvl")]
        int ilvl { get; set; }

        [JsonProperty("implicitMods")]
        List<string> implicitMods { get; set; }

        [JsonProperty("inventoryId")]
        string inventoryId { get; set; }

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


    [JsonProperty("w")]
        int width { get; set; }




        

        
        [JsonProperty("typeLine")]
        string typeLine { get; set; }



    }

    public class Property
    {
        [JsonProperty("name")]
        string name { get; set; }

        [JsonProperty("values")]
        Tuple<string, int> width { get; set; }

        [JsonProperty("displayMode")]
        int displayMode { get; set; }

        [JsonProperty("type")]
        int propertyType { get; set; }

        [JsonProperty("progress")]
        int xpProgress { get; set; }
        
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


    }

    public class Socket
    {
        [JsonProperty("group")]
        int groupId { get; set; }


        [JsonProperty("attr")]
        private string _attribute {
            get { return _attribute; }
            set {
                switch (_attribute) {
                    case "S":
                        _attribute = value;
                        type = SocketType.Str;
                        break;
                    case "I":
                        _attribute = value;
                        type = SocketType.Int;
                        break;
                    case "D":
                        _attribute = value;
                        type = SocketType.Dex;
                        break;
                    case "G":
                        _attribute = value;
                        type = SocketType.White;
                        break;
                    case "false":
                        _attribute = value;
                        type = SocketType.Abyss;
                        break;
                    default:
                        new Exception("Socket type not seen!");
                        break;
                }
            }
        }
        public SocketType type { get; set; }
    }

    public enum SocketType { Str, Int, Dex, White, Abyss, Default}
}
