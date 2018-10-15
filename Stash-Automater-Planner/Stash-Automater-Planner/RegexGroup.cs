using System;
using System.Collections.Generic;
using System.Text;

namespace Stash_Automater_Planner
{
    public static class RegexGroup
    {
        public static bool MatchItem(Item item,ItemType regexGroup)
        {
            switch (regexGroup) {
                case ItemType.Boot:
                    if (item.identified == false && item.frameType == 2 && item.ilvl >= 60 &&
                        (item.typeLine.Contains("Boots") || 
                        item.typeLine.Contains("Greaves") || 
                        item.typeLine.Contains("Slippers")))
                        return true;
                    break;
                case ItemType.Helmet:
                    if (item.identified == false && item.frameType == 2 && item.ilvl >= 60 &&
                        (item.typeLine.Contains("Burgonet") ||
                        item.typeLine.Contains("Helmet") ||
                        item.typeLine.Contains("Hat") ||
                        item.typeLine.Contains(" Pelt") ||
                        item.typeLine.Contains("Tricorne") ||
                        item.typeLine.Contains(" Hood") ||
                        item.typeLine.Contains("Leather Cap") ||
                        item.typeLine.Contains(" Circlet") ||
                        item.typeLine.Contains(" Cage") ||
                        item.typeLine.Contains("Bascinet") ||
                        item.typeLine.Contains("Helm") ||
                        item.typeLine.Contains("Sallet") ||
                        item.typeLine.Contains("Crown") ||
                        item.typeLine.Contains("Coif") ||
                        item.typeLine.Contains("Mask")))
                        return true;
                    break;
                case ItemType.Glove:
                    if (item.identified == false && item.frameType == 2 && item.ilvl >= 60 &&
                        (item.typeLine.Contains("Gauntlets") ||
                        item.typeLine.Contains("Gloves") ||
                        item.typeLine.Contains("Mitts")))
                        return true;
                    break;
                case ItemType.OneHand:
                    if (item.identified == false && item.frameType == 2 && item.ilvl >= 60 && item.height == 3 && item.width==1)
                        return true;
                    break;
                case ItemType.Chest:
                    if (item.identified == false && item.frameType == 2 && item.ilvl >= 60 &&
                        (item.typeLine.Contains(" Leather") ||
                        item.typeLine.Contains("Tunic") ||
                        item.typeLine.Contains("Garb") ||
                        item.typeLine.Contains("Robe") ||
                        item.typeLine.Contains("Vestment") ||
                        item.typeLine.Contains("Regalia") ||
                        item.typeLine.Contains("Silks") ||
                        item.typeLine.Contains("Wrap") ||
                        item.typeLine.Contains("Lamellar") ||
                        item.typeLine.Contains("Brigandine") ||
                        item.typeLine.Contains("Dragonscale Doublet") ||
                        item.typeLine.Contains("Full Dragonscale") ||
                        item.typeLine.Contains("Wyrmscale Doublet") ||
                        item.typeLine.Contains("Full Wyrmscale") ||
                        item.typeLine.Contains("Full Scale") ||
                        item.typeLine.Contains("Chainmail") ||
                        item.typeLine.Contains("Hauberk") ||
                        item.typeLine.Contains("Ringmail") ||
                        item.typeLine.Contains("Carnal Armour") ||
                        item.typeLine.Contains(" Raiment") ||
                        item.typeLine.Contains(" Coat") ||
                        item.typeLine.Contains(" Jacket") ||
                        item.typeLine.Contains("Crypt Armour") ||
                        item.typeLine.Contains("Bone Armour") ||
                        item.typeLine.Contains(" Plate") ||
                        item.typeLine.Contains("Vest")))
                        return true;
                    break;
                case ItemType.Belt:
                    if (item.identified == false && item.frameType == 2 && item.ilvl >= 60 &&
                        (item.typeLine.Contains(" Belt") || item.typeLine.Contains("Rustic Sash")))
                        return true;
                    break;
                case ItemType.Ring:
                    if (item.identified == false && item.frameType == 2 && item.ilvl >= 60 && item.width == 1 && item.height == 1 &&
                        (item.typeLine.Contains(" Ring")))
                        return true;
                    break;
                case ItemType.Amulet:
                    if (item.identified == false && item.frameType == 2 && item.ilvl >= 60 &&
                        (item.typeLine.Contains(" Amulet")))
                        return true;
                    break;
                case ItemType.DivCard:
                    if (
                        (item.frameType == 6))
                        return true;
                    break;
                case ItemType.Currency:
                    if (
                        (item.frameType == 5))
                        return true;
                    break;
                case ItemType.Map:
                    if (
                        (item.typeLine.Contains(" Map")))
                        return true;
                    break;
                case ItemType.Essence:
                    if (
                        (item.typeLine.Contains(" Essence")))
                        return true;
                    break;
                case ItemType.Fragment:
                    if (
                        item.typeLine.Contains("Sacrifice") || item.typeLine.Contains("Fragment") || item.typeLine.Contains(" Key") || 
                        item.typeLine.Contains("Splinter of ") || item.typeLine.Contains("Offering to the goddess") || item.typeLine.Contains("Divine vessel"))
                        return true;
                    break;
                case ItemType.None:
                    return true;


            }

            return false;
        }
    }
}
