using System;
using System.Collections.Generic;

namespace TSS.UniverseLogic
{
    [Serializable]
    public class Genome
	{
        int hunger;
        int aggression;
        int Reproduction;
        int friendly;

        public Genome(int hunger, int aggression, int Reproduction, int friendly)
        {

            this.hunger = hunger;
            this.aggression = aggression;
            this.Reproduction = Reproduction;
            this.friendly = friendly;
        }
        public Genome() : this(ConstsUniverse.CellGenome_Hunger, 
            ConstsUniverse.CellGenome_Aggression,
            ConstsUniverse.CellGenome_Reproduction,
            ConstsUniverse.CellGenome_Friendly)
        {
        }
        public int GetHunger()
        {
            return hunger;
        }
        public int GetAggression()
        {
            return aggression;
        }
        public int GetReproduction()
        {
            return Reproduction;
        }
        public int GetFriendly()
        {
            return friendly;
        }
        public Genome Clone()
        {
            return new Genome(hunger, aggression, Reproduction, friendly);
        }
        public Genome CloneAndMutate()
        {
            int modificator;
            int hunger = this.hunger, aggression = this.aggression, Reproduction = this.Reproduction, friendly = this.friendly;

            for (int i = 0; i < ConstsUniverse.Mutation_ChangedValuesAtOne; i++)
            {
                if (StableRandom.rd.Next(2) == 0)
                    modificator = -1;
                else
                    modificator = 1;
                switch (StableRandom.rd.Next(1, 5))
                {
                    case 1:
                        hunger += modificator;
                        break;

                    case 2:
                        aggression += modificator;
                        break;

                    case 3:
                        Reproduction += modificator;
                        break;

                    case 4:
                        friendly +=modificator;
                        break;
                }
                
            }
            return new Genome(hunger, aggression, Reproduction, friendly);
           

        }



    }
}
