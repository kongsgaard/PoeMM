using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.IO.Compression;

namespace Stash_Automater_Planner
{
    public class StashFramework
    {
        public List<StashTab> AllTabs = new List<StashTab>();
        public Inventory inventory;
        
        string StashConfig = "";
        List<string> SortConfigs;

        public StashFramework(string StashConfigFileName, List<string> sortConfigs) {
            StashConfig = StashConfigFileName;
            SortConfigs = sortConfigs;

            InitializeInventory();
            InitializeStashes();
            SetItemTypes();
        }

        public ItemTab StashByName(string stashName) {

            if(stashName == "MainInventory") {
                return inventory;
            }
            StashTab tab = AllTabs.Single(x => x.name == stashName);

            return tab;
        }

        private void InitializeInventory() {
            inventory = JsonConvert.DeserializeObject<Inventory>(GetInventory(StashConfig).ToString());
            inventory.type = TabType.Inventory;

            inventory.items = inventory.items.Where(x => x.inventoryId == "MainInventory").ToList();
        }

        private void InitializeStashes() {


            //Find stashes to fetch
            List<string> StashesToFetch = new List<string>();
            foreach(string config in SortConfigs) {

                string json = File.ReadAllText(Directory.GetCurrentDirectory() + "\\" + config);
                JObject jObject = JObject.Parse(json);
                SortConfig sortConfig = JsonConvert.DeserializeObject<SortConfig>(jObject.ToString());

                foreach(string stash in sortConfig.sourceTabs) {
                    if(stash != "MainInventory") {
                        StashesToFetch.Add(stash);
                    }
                }

                foreach(ConfigItemtype cnfgItemType in sortConfig.configItemTypes) {
                    foreach(ConfigTargetStash configStashes in cnfgItemType.targetStashes) {
                        StashesToFetch.Add(configStashes.stashName);
                    }
                }
            }

            StashesToFetch = StashesToFetch.Distinct().ToList();


            //Fetch stashes
            ApiRequest stashes = JsonConvert.DeserializeObject<ApiRequest>(GetStashes(StashConfig).ToString());

            string currentFetch = "";
            try {
                foreach (var s in StashesToFetch) {
                    currentFetch = s;
                    StashTab tab = stashes.stashTabs.Single(x => x.name == s);

                    ApiRequest stash = JsonConvert.DeserializeObject<ApiRequest>(GetStashes(StashConfig, tab.index).ToString());

                    tab.items = stash.items;

                    AllTabs.Add(tab);
                }
            }
            catch (InvalidOperationException e) {
                throw new Exception("Error when looking for tab " + currentFetch + " .Either 0 or multiple matches");
            }
        }

        private void SetItemTypes() {
            foreach (StashTab tab in AllTabs) {
                foreach (Item item in tab.items) {
                    item.myStashTab = tab;
                    foreach (ItemType type in Enum.GetValues(typeof(ItemType))) {
                        if (RegexGroup.MatchItem(item, type)) {
                            item.itemType = type;
                        }
                    }
                }
            }
        }

        private string GetStashes(string stashConfig, int tabIndex = -1) {
            byte[] decompFile = null;

            IConfiguration Configuration = BuildJsonConfiguration(stashConfig);


            Cookie cookie = new Cookie("POESESSID", $"{Configuration["POESESSID"]}", "/", ".pathofexile.com");
            Cookie cookie1 = new Cookie("stored_data", "1", "/", ".pathofexile.com");

            string para = "";
            if (tabIndex >= 0) {
                para = "&tabIndex=" + tabIndex.ToString();
            }
            else {
                para = "&tabs=1";
            }
            string _apiEndpoint = $"https://www.pathofexile.com/character-window/get-stash-items?league={Configuration["league"]}&accountName={Configuration["account"]}" + para;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_apiEndpoint);
            CookieContainer cont = new CookieContainer();

            webRequest.CookieContainer = cont;
            webRequest.CookieContainer.Add(cookie);
            webRequest.CookieContainer.Add(cookie1);


            webRequest.Headers.Add("Accept-Encoding", "gzip,deflate");

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            using (Stream stream = webResponse.GetResponseStream()) {
                byte[] tmpArr = ReadFully(stream);
                Stream stream1 = new MemoryStream(tmpArr);
                using (GZipStream decompStream = new GZipStream(stream1, CompressionMode.Decompress)) {
                    decompFile = ReadFully(decompStream);
                }
            }

            return System.Text.Encoding.Default.GetString(decompFile);
        }

        private string GetInventory(string stashConfig) {
            byte[] decompFile = null;

            IConfiguration Configuration = BuildJsonConfiguration(stashConfig);

            Cookie cookie = new Cookie("POESESSID", $"{Configuration["POESESSID"]}", "/", ".pathofexile.com");
            Cookie cookie1 = new Cookie("stored_data", "1", "/", ".pathofexile.com");

            string _apiEndpoint = $"https://www.pathofexile.com/character-window/get-items?character={Configuration["character"]}&accountName={Configuration["account"]}";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_apiEndpoint);
            CookieContainer cont = new CookieContainer();
            webRequest.CookieContainer = cont;
            webRequest.CookieContainer.Add(cookie);
            webRequest.CookieContainer.Add(cookie1);


            webRequest.Headers.Add("Accept-Encoding", "gzip,deflate");

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            using (Stream stream = webResponse.GetResponseStream()) {
                byte[] tmpArr = ReadFully(stream);
                Stream stream1 = new MemoryStream(tmpArr);
                using (GZipStream decompStream = new GZipStream(stream1, CompressionMode.Decompress)) {
                    decompFile = ReadFully(decompStream);
                }
            }

            return System.Text.Encoding.Default.GetString(decompFile);
        }

        private static byte[] ReadFully(Stream stream, int bufferSize = 32768) {
            byte[] buffer = new byte[bufferSize];
            using (MemoryStream ms = new MemoryStream()) {
                while (true) {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }

        private static IConfiguration BuildJsonConfiguration(string jsonFile) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(jsonFile);
            return builder.Build();

        }

    }
}
