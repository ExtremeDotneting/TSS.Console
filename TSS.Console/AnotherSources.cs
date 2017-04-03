using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSS.UniverseLogic
{
    static public class StableRandom
    {
        public static Random rd = new Random();
    }
    public enum MoveDirection
    {
        stand,
        up,
        down,
        left,
        right
    }
    public enum FoodType
    {
        deadCell,
        defaultFood
    }
}
