using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Stash_Indexer_netcore
{
    public class ConsoleMonitor
    {

        private Queue<ApiRequestDoneArgs> _latestRequests = new Queue<ApiRequestDoneArgs>();
        private static Mutex _updateMut = new Mutex();

        private int[] _changeIdStart = {0,0,0,0,0,};
        private int[] _changeIdNow = {0,0,0,0,0};
        
        private Stopwatch _watch = new Stopwatch();
        private long _totalFilesBytes { get; set; }

        private int _writeDelayMs { get; }
        

        public ConsoleMonitor(StashApiRequester requester, int writeDelayMS, bool estimateToTarget=false, string changeIdTarget="")
        {
            requester.RequestDone += RequestRecieved;
            _writeDelayMs = writeDelayMS;
            _watch.Start();
        }

        public void StartConsoleMonitor()
        {
            while (true) {
                Console.SetCursorPosition(0,0);
                Console.WriteLine(GenerateOutput());
                System.Threading.Thread.Sleep(_writeDelayMs);
            }
        }

        private string GenerateOutput()
        {
            string latest = "";

            _updateMut.WaitOne();
            foreach(ApiRequestDoneArgs r in _latestRequests) {
                latest = latest + r._changeId + " in " + r._requestTimeMs / 1000 + "s " + r._requestTimeMs % 1000 + "ms\n";
            }

            string shards = "\n \nStatistics: \n   Shard1|   Shard2|   Shard3|   Shard4|   Shard5\n";

            string shardUpdts = "";

            for(int i = 0; i < 5; i++) {
                int shardUpdt = _changeIdNow[i] - _changeIdStart[i];
                string shardUpdt_str = shardUpdt.ToString();

                shardUpdts = shardUpdts + spaces(9 - shardUpdt_str.Length) + shardUpdt_str + "|";
            }

            string stats = "\n\nDownload size: " + (_totalFilesBytes/(float)1000000).ToString("F") + "mb in " + _watch.Elapsed.Minutes + ":" + (_watch.Elapsed.Seconds < 9 ? ("0" + _watch.Elapsed.Seconds.ToString()) : _watch.Elapsed.Seconds.ToString());

            _updateMut.ReleaseMutex();


            return latest + shards + shardUpdts + stats;
        }



        private void RequestRecieved(object sender, ApiRequestDoneArgs eventArgs)
        {
            _updateMut.WaitOne();

            if (_latestRequests.Count == 0) {
                _changeIdStart = ChangeIdToArr(eventArgs._changeId);
                _changeIdNow = ChangeIdToArr(eventArgs._changeId);
                _totalFilesBytes = eventArgs._fileBytes;
            }
            else {
                _changeIdNow = ChangeIdToArr(eventArgs._changeId);
                _totalFilesBytes = _totalFilesBytes + eventArgs._fileBytes;
            }



            if (_latestRequests.Count < 5) {
                _latestRequests.Enqueue(eventArgs);
                Console.Clear();
            }
            else {
                _latestRequests.Dequeue();
                _latestRequests.Enqueue(eventArgs);
            }

            _updateMut.ReleaseMutex();
        }

        private int[] ChangeIdToArr(string changeId)
        {
            int[] idArr = new int[5];

            string[] splitChangeId = changeId.Split('-');

            for(int i=0; i < 5; i++) {
                idArr[i] = int.Parse(splitChangeId[i]);
            }

            return idArr;
        }

        private string spaces(int num)
        {
            string rtString = "";
            for (int i = 0; i < num; i++) {
                rtString = rtString + " ";
            }
            return rtString;
        }

    }
}
