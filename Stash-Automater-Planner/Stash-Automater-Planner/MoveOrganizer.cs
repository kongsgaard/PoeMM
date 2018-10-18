using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Stash_Automater_Planner
{
    public class MoveOrganizer
    {
        public int currentTabIndex = 0;
        public double currentMouseX = 0;
        public double currentMouseY = 0;

        public double StashX = 0;
        public double StashY = 0;
        public double VendorX = 0;
        public double VendorY = 0;

        public double VendorSellDialogX = 0;
        public double VendorSellDialogY = 0;
        public double VendorSellButtonX = 0;
        public double VendorSellButtonY = 0;

        public int TotalMS = 0;
    
        public List<string> moves = new List<string>();
        
        Random ran = new Random();

        public MoveOrganizer(string stashConfig)
        {
            string json = File.ReadAllText(Directory.GetCurrentDirectory() + "\\" + stashConfig);
            JObject jObject = JObject.Parse(json);
            var config = JsonConvert.DeserializeObject<StashConfig>(jObject.ToString());

            StashX = config.stashX; StashY = config.stashY;
            VendorX = config.vendorX; VendorY = config.vendorY;
        }

        public void finalize()
        {
            moves.Add("ExitApp");
            moves.Add("");
            moves.Add("");
            moves.Add("F11::");
            moves.Add("send, {ctrl up}{right up}{left up}");
            moves.Add("ExitApp");
            moves.Add("Return");

        }

        public void SellToVendor(List<Item> ItemsToSell, ItemTab inventory, ItemTab vendor) {

            //Move away from stash to vendor. Click sell
            moves.Add("send, {ESC}");
            addSleep(20, 100);
            moves.Add("send, {ESC}");
            addSleep(20, 30);
            moves.Add("MouseMove, " + VendorX.ToString() + ", " + VendorY.ToString() + ", 2");
            addSleep(20, 30);
            moves.Add("Send, {click}");




            foreach (Item i in ItemsToSell) {
                if(!ToolBox.MoveItem(inventory, vendor, i, this))
                {
                    throw new Exception("Could not give item to vendor.");
                }
            }

            //Add chaos to inventory


            //Close vendor shop

            //Move back to stash

            throw new NotImplementedException();
        }

        public void MoveItem(ItemTab source, ItemTab target, Item item)
        {
            if(!(target.type == TabType.Vendor)) {
                //Calculate difference in tabs, i.e how much we need to move
                int tabDiff = (source.type == TabType.Inventory ? target.index : source.index) - currentTabIndex;

                if (tabDiff > 0) {
                    for (int i = 0; i < tabDiff; i++) {
                        moves.Add("Send, {right down}{right up}");
                        addSleep(174, 243);

                    }
                }
                else if (tabDiff < 0) {
                    for (int i = 0; i < -tabDiff; i++) {
                        moves.Add("Send, {left down}{left up}");
                        addSleep(163, 259);
                    }
                }

                currentTabIndex = source.type == TabType.Inventory ? target.index : source.index;

            }


            TotalMS += 20;
            moves.Add("MouseMove, " + Math.Round(calcMouseX(source.type, item.x)).ToString() + ", " + Math.Round(calcMouseY(source.type, item.y)).ToString() + ", " + ran.Next(2,10).ToString());
            addSleep(54, 76);
            moves.Add("Send, {ctrl down}{click}{ctrl up}");
            addSleep(12, 35);
        }

        private void addSleep(int minMs, int maxMs) {

            int sleep = ran.Next(minMs, maxMs);
            TotalMS += sleep;

            moves.Add("Sleep, " + sleep);
        }

        private double calcMouseX(TabType tabType, double xOffset)
        {
            int RandomOffSet = ran.Next(-7, 7);
             
            if(tabType == TabType.PremiumStash || tabType == TabType.NormalStash) {
                return NormalX + xOffset * NormalOffsetX + RandomOffSet;
            }
            else if(tabType == TabType.QuadStash) {
                return QuadX + xOffset * QuadOffsetX + RandomOffSet;
            }
            else if (tabType == TabType.Inventory) {
                return InventoryX + xOffset * InventoryOffsetX + RandomOffSet;
            }
            else {
                throw new Exception("Can't remove items from tabtype " + tabType.ToString());
            }
        }

        private double calcMouseY(TabType tabType, double yOffset)
        {
            int RandomOffSet = ran.Next(-7, 7);

            if (tabType == TabType.PremiumStash || tabType == TabType.NormalStash) {
                return NormalY + yOffset * NormalOffsetY + RandomOffSet;
            }
            else if (tabType == TabType.QuadStash) {
                return QuadY + yOffset * QuadOffsetY + RandomOffSet;
            }
            else if (tabType == TabType.Inventory) {
                return InventoryY + yOffset * InventoryOffsetY + RandomOffSet;
            }
            else {
                throw new Exception("Can't remove items from tabtype " + tabType.ToString());
            }
        }

        private static double QuadX = 30;
        private static double QuadY = 175;
        private static double QuadOffsetX = 26.3;
        private static double QuadOffsetY = 26.3;

        private static double NormalX = 45;
        private static double NormalY = 185;
        private static double NormalOffsetX = 52.5;
        private static double NormalOffsetY = 52.5;

        private static double InventoryX = 1300;
        private static double InventoryY = 615;
        private static double InventoryOffsetX = 52.5;
        private static double InventoryOffsetY = 52.5;

        private double GaussianRandom(double minVal, double mean, double stdDev)
        {

            double u1 = 1.0 - ran.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - ran.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            if (randNormal < minVal)
                return minVal;
            else
                return randNormal;
        }




    }

}
