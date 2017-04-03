using System;

namespace TSS.UniverseLogic
{
    [Serializable]
    public class Food : UniverseObject, IFood
    {
        float energyLevel;

        public Food(FoodType foodType)
        {
            if (foodType == FoodType.defaultFood)
            {
                descriptor = -1;
                energyLevel = ConstsUniverse.EnergyLevel_DefFood;
            }
            else
            {
                descriptor = -2;
                energyLevel = ConstsUniverse.EnergyLevel_DeadCell;
            }
        }
        public float GetEnergyLevel()
		{
            return energyLevel;
		}
	}
}
