using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO.Compression;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Linq;

namespace Stash_Automater_Planner
{
    public static class ToolBox
    {
        public static string GetStashes(string stashConfig, int tabIndex=-1)
        {
            byte[] decompFile = null;

            IConfiguration Configuration = BuildJsonConfiguration(stashConfig);


            Cookie cookie = new Cookie("POESESSID", $"{Configuration["POESESSID"]}", "/", ".pathofexile.com");
            Cookie cookie1 = new Cookie("stored_data", "1", "/", ".pathofexile.com");

            string para = "";
            if(tabIndex >= 0) {
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

        public static string GetInventory(string stashConfig)
        {
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

        public static bool MoveItem(ItemTab source, ItemTab target, Item item, MoveOrganizer organizer)
        {
            Item itemCopy = new Item();
            itemCopy.inventoryId = item.inventoryId;
            itemCopy.x = item.x;
            itemCopy.y = item.y;

            bool removed = source.RemoveItem(item);
            bool added = target.AddItem(item);

            if(added == false) {
                //Failed add, insert item in List<Item> for source again
                source.items.Add(item);
            }
            else {
                organizer.MoveItem(source, target, itemCopy);
            }

            return removed && added;
        }

        public static bool MoveItemToChaosRecipeTab(ItemTab source, ItemTab target, Item item, MoveOrganizer organizer, SortConfig config) {
            Item itemCopy = new Item();
            itemCopy.inventoryId = item.inventoryId;
            itemCopy.x = item.x;
            itemCopy.y = item.y;
            
            if (!ControlChaosRecipeReqs(target, item, config)) {
                return false;
            }

            bool removed = source.RemoveItem(item);
            bool added = target.AddItem(item);
            
            if (added == false) {
                //Failed add, insert item in List<Item> for source again
                source.items.Add(item);
            }
            else {
                organizer.MoveItem(source, target, itemCopy);
            }

            return removed && added;
        }

        public static bool ControlChaosRecipeReqs(ItemTab target, Item item, SortConfig config) {

            var ChaosTabsSource = config.targetTabs.Where(x => x.chaosRecipeCount > 0);
            var ChaosTabsSourceSorted = ChaosTabsSource.OrderBy(x => x.order).ToList();

            foreach(TargetTab tab in ChaosTabsSourceSorted) {
                int matchesCount = 0;

                foreach(Item i in target.items) {
                    if (MatchTargetTabItem(i, tab)) {
                        matchesCount++;
                    }
                }

                bool thisType = false;
                if (MatchTargetTabItem(item, tab)) {
                    thisType = true;
                }

                if(thisType == false) {
                    if (matchesCount != tab.chaosRecipeCount) {
                        return false;
                    }
                }
                else {
                    if(matchesCount >= tab.chaosRecipeCount) {
                        return false;
                    }
                    else {
                        return true;
                    }
                }
            }

            return false;
        }



        public static bool MatchTargetTabItem(Item item, TargetTab targetTab)
        {
            foreach(string rG in targetTab.regexGroups) {
                if(RegexGroup.MatchItem(item, rG)) {
                    return true;
                }
            }
            return false;
        }
        
        private static byte[] ReadFully(Stream stream, int bufferSize = 32768)
        {
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

        public static IConfiguration BuildJsonConfiguration(string jsonFile)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(jsonFile);
            return builder.Build();

        }
    }
}
