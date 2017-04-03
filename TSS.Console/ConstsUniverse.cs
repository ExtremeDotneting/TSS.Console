using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Linq;

namespace TSS.UniverseLogic
{
    static public class ConstsUniverse 
    {
        public static  int CellAge_Max = 99999;
        public static  int CellAge_AdultCell = 25;
        public static  float EnergyLevel_CreatingCell = 125;
        public static  float EnergyLevel_NeededForReproduction = 100;
        public static  float EnergyLevel_MaxForCell = 1000;
        public static  float EnergyLevel_DeadCell = 20;
        public static  float EnergyLevel_DefFood = 10;
        public static  float EnergyLevel_MovesFriendly = 1;
        public static  float EnergyLevel_MovesAggression = 5;
        public static  bool Mutation_Enable = true;
        public static  int Mutation_ChangedValuesAtOne = 1;
        public static  int Mutation_ChancePercent = 50;
        public static  int CellsCount_MaxWithOneType = 9000;
        public static  int CellsCount_MaxAtField = 250;
        public static  float EnergyEntropyPerSecond = 1;
        public static  int CellGenome_Child_Aggression = -2;
        public static int Special_FoodCountForTick = 2;
        //public static bool Special_DrawMoveDisperation = true;
        public static bool Special_EnableStatistics = true;
        public static int Special_PrintingPause = 500;

        //Genome
        static int CellGenome_hungerMin = 0;
        static int CellGenome_hungerMax = 0;
        static int CellGenome_aggressionMin = 0;
        static int CellGenome_aggressionMax = 0;
        static int CellGenome_reproductionMin = 0;
        static int CellGenome_reproductionMax = 0;
        static int CellGenome_friendlyMin = 0;
        static int CellGenome_friendlyMax = 0;
        //Genome

        public static  int CellGenome_Hunger
        {
            get { return RandomFromRange(CellGenome_hungerMin, CellGenome_hungerMax); }
        }

        public static  int CellGenome_Aggression
        {
            get { return RandomFromRange(CellGenome_aggressionMin, CellGenome_aggressionMax); }
        }

        public static  int CellGenome_Reproduction
        {
            get { return RandomFromRange(CellGenome_reproductionMin, CellGenome_reproductionMax); }
        }

        public static  int CellGenome_Friendly
        {
            get { return RandomFromRange(CellGenome_friendlyMin, CellGenome_friendlyMax); }
        }

        static int RandomFromRange(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
                return minValue;
            else
                return StableRandom.rd.Next(minValue, maxValue);
        }

        public static string ToXmlString()
        {
            XElement res = new XElement(@"ConstsUniverse");
            Type objType = typeof(ConstsUniverse);
       
            foreach (var item in objType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static))
            {
                XElement xelLoc = new XElement(item.Name);
                xelLoc.Add(
                    item.GetValue(null).ToString()
                    );
                res.Add(xelLoc);
            }
            return res.ToString();
        }

        public static void FromXmlString(string xmlStr)
        {
            string constsBackup = ToXmlString();
            FieldInfo curField = null;
            try
            {
                XElement xel = XElement.Parse(xmlStr);
                Type objType = typeof(ConstsUniverse);
                Dictionary<string, object> dict = new Dictionary<string, object>();
                foreach (var item in xel.Elements())
                {
                    dict.Add(item.Name.ToString(), item.Value);
                }

                foreach (var item in objType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static))
                {
                    curField = item;
                    item.SetValue(null,
                        Convert.ChangeType(dict[item.Name], item.FieldType)
                        );
                }
            }
            catch
            {
                FromXmlString(constsBackup);
                if (curField != null)
                    throw new Exception(
                        string.Format("ConstsUniverse changing aborted. Field \"{0}\" - {1}, doesn`t exist or have invalid type.", curField.Name, curField.FieldType)
                        );
                else
                    throw new Exception(
                        "ConstsUniverse changing aborted. Can`t parse xml."
                        );
            }
        }

    }



}
