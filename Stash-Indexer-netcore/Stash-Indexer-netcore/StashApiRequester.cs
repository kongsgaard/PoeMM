using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO.Compression;
using System.Diagnostics;
using log4net;
using log4net.Repository;
using System.Reflection;

namespace Stash_Indexer_netcore
{
    public class StashApiRequester
    {
        /* Current json format
         * {
         *  "next_change_id":""
         * }
         * 
         * Abyss start:
         * next_change_id=111929789-117354395-110061941-127008893-118581490
         * 
         */
        
        

        //private ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        private static readonly log4net.ILog errorLog = log4net.LogManager.GetLogger(LogManager.GetRepository(Assembly.GetEntryAssembly()).Name, "ErrorLogger");
        private static readonly log4net.ILog transactionLog = log4net.LogManager.GetLogger(LogManager.GetRepository(Assembly.GetEntryAssembly()).Name, "TransctionLogger");

        private static string _checkpointFileName = "checkpoint.json";
        private static string _apiEndpoint = "http://www.pathofexile.com/api/public-stash-tabs?id=";


        private string _directoryPath { get; set; }
        private bool _useCached { get; set; }
        
        private string _nextChangeId { get; set; }

        /// <summary>
        /// Class to manage http requests to the PoE Stash Api
        /// </summary>
        /// <param name="directoryPath">Path the main directory for storage</param>
        /// <param name="useCached">If false will redownload http requests even if they are already saved on storage</param>
        public StashApiRequester(string directoryPath, bool useCached=true)
        {
            _directoryPath = directoryPath;
            _useCached = useCached;
        }

        public void InitializeFromCheckpoint()
        {
            if(!File.Exists(_directoryPath + _checkpointFileName)) {

                _nextChangeId = "111929789-117354395-110061941-127008893-118581490";

                JObject jObject =
                    new JObject(
                        new JProperty("next_change_id", "111929789-117354395-110061941-127008893-118581490"));

                
                using (StreamWriter writer = File.CreateText(_directoryPath + _checkpointFileName)) {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, jObject);
                }
            }
            else {
                using (StreamReader file = File.OpenText(_directoryPath + _checkpointFileName))
                using (JsonTextReader reader = new JsonTextReader(file)) {
                    JObject jObject = (JObject)JToken.ReadFrom(reader);
                    _nextChangeId = (string)jObject["next_change_id"];
                }
            }
        }

        private void UpdateCheckpoint(string nextChangeId)
        {
            JObject jObject = null;
            using (StreamReader file = File.OpenText(_directoryPath + _checkpointFileName))
            using (JsonTextReader reader = new JsonTextReader(file)) {
                jObject = (JObject)JToken.ReadFrom(reader);
                jObject["next_change_id"] = nextChangeId;
            }

            using (StreamWriter file = File.CreateText(_directoryPath + _checkpointFileName)) {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, jObject);
            }
        }

        public void StartRequesting()
        {
            for (int i = 0; i < 100; i++) {
                JObject requestObj = ApiRequest(_nextChangeId);
                _nextChangeId = (string)requestObj["next_change_id"];
                UpdateCheckpoint(_nextChangeId);
            }
        }

        private JObject ApiRequest(string changeId, bool redownload=false)
        {
            byte[] decompFile = null;
            string cachedFilePath = CachedFilePath(_directoryPath, changeId);
            if (!File.Exists(cachedFilePath)){
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_apiEndpoint + changeId);
                webRequest.Headers.Add("Accept-Encoding", "gzip,deflate");

                transactionLog.Info("Requesting: " + changeId);
                Console.WriteLine("Requesting: " + changeId);

                Stopwatch watch = new Stopwatch();
                watch.Start();
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                TimeSpan ts = watch.Elapsed;
                string elapsedTime = String.Format("{0:00}s {1:00}ms", ts.Seconds,ts.Milliseconds);
                transactionLog.Info("Done in: " + elapsedTime);
                Console.WriteLine("Done in: " + elapsedTime);

                using (Stream stream = webResponse.GetResponseStream())
                using (FileStream file = File.Create(cachedFilePath)) {

                    byte[] tmpArr = ReadFully(stream);

                    file.Write(tmpArr, 0, tmpArr.Length);

                    Stream stream1 = new MemoryStream(tmpArr);
                    using (GZipStream decompStream = new GZipStream(stream1, CompressionMode.Decompress)) {
                        decompFile = ReadFully(decompStream);
                    }
                }
            }
            else {

                transactionLog.Info(changeId + " already exists");

                using (FileStream file = new FileStream(cachedFilePath,FileMode.Open, FileAccess.Read))
                using (GZipStream decompStream = new GZipStream(file, CompressionMode.Decompress)) {
                    try {
                        decompFile = ReadFully(decompStream);
                    }
                    catch (Exception e) {
                        if (redownload) {
                            errorLog.Info("Redownload of " + changeId + " failed. Terminating program.");
                            Environment.Exit(0);
                        }

                        file.Close();
                        errorLog.Info("Decompression of "+ changeId + " failed, redownloading");
                        File.Delete(cachedFilePath);
                        
                        return ApiRequest(changeId, redownload=true);
                    }
                    
                }
            }

            string jsonTxt = System.Text.Encoding.Default.GetString(decompFile);
            
            return JObject.Parse(jsonTxt);
        }

        private static string CachedFilePath(string dir, string changeId) { return dir + "files/" + changeId + ".gz"; }
        private static byte[] ReadFully(Stream stream, int bufferSize=32768) {
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
