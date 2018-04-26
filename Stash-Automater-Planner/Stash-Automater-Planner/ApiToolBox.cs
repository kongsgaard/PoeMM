using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO.Compression;
using System.IO;

namespace Stash_Automater_Planner
{
    public static class ApiToolBox
    {
        public static JObject GetStashes(string stashConfig)
        {
            byte[] decompFile = null;


            Cookie cookie = new Cookie("POESESSID", "SESSID", "/", ".pathofexile.com");
            Cookie cookie1 = new Cookie("stored_data", "1", "/", ".pathofexile.com");
            
            string _apiEndpoint = "https://www.pathofexile.com/character-window/get-stash-items?league=Hardcore&accountName=sundess888&tabs=1&tabIndex=1";

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

            string JsonTxt = System.Text.Encoding.Default.GetString(decompFile);
            return JObject.Parse(JsonTxt);
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
    }
}
