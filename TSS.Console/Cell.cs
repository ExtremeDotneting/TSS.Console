using System;
using System.Collections.Generic;

namespace TSS.UniverseLogic
{
    [Serializable]
    public class Cell : UniverseObject, ICell
	{
		int age ;
        float energyLevel;
	    Genome genome;
        MoveDirection moveDisperation;

        public Cell() : this(new Genome(), ConstsUniverse.EnergyLevel_CreatingCell, 1)
        {
        }
        public Cell(Genome genome) :this(genome,ConstsUniverse.EnergyLevel_CreatingCell, 1)
        {
        }
        public Cell(Genome genome, float energyLevel) : this(genome, energyLevel,1)
        {
        }
        public Cell(Genome genome, float energyLevel, int descriptor)
        {
            this.genome = genome;
            age = 0;
            this.energyLevel = energyLevel;
            if (descriptor < 100)
            {
                int desc = StableRandom.rd.Next(100, int.MaxValue);
                this.descriptor = desc;
            }
            else
                this.descriptor = descriptor;
        }
        public Genome GetGenome()
        {
            return genome;
        }
        public MoveDirection GetMoveDisperation()
        {
            return moveDisperation;
        }
        public float GetEnergyLevel()
		{
            return energyLevel;
		}
		public void AddEnergy(float value)
		{
            energyLevel += value;
            if (energyLevel > ConstsUniverse.EnergyLevel_MaxForCell)
                energyLevel = ConstsUniverse.EnergyLevel_MaxForCell;

        }
		public bool IncAge()
		{
            return age++ > ConstsUniverse.CellAge_Max;
		}
        public bool DecEnergy()
        {
            energyLevel -= ConstsUniverse.EnergyEntropyPerSecond;
            return energyLevel  < 0;
        }
        public int GetAge()
        {
            return age;
        }
        public ICell CreateChild()
        {
            age = 0;
            energyLevel = (int)(energyLevel / 2);
            if (StableRandom.rd.Next(100) < ConstsUniverse.Mutation_ChancePercent && ConstsUniverse.Mutation_Enable)
            {
                Cell res = new Cell(genome.CloneAndMutate(), 1);
                res.AddEnergy(energyLevel);
                return res;
            }
            else
                return new Cell(genome.Clone(), energyLevel, descriptor);
           
        }
        public void CalcMoveDirectionAspiration(IUniverse universe)
        {
            float up, down, left, right;

            //up = AnalizeDescriptor(x - 1, y - 2) + 2*AnalizeDescriptor(x, y - 2) + AnalizeDescriptor(x + 1, y - 2) +
            //    2 * AnalizeDescriptor(x - 1, y - 1) + 3*AnalizeDescriptor(x, y - 1) + 2*AnalizeDescriptor(x + 1, y - 1);

            //down = AnalizeDescriptor(x - 1, y + 2) + 2*AnalizeDescriptor(x, y + 2) + AnalizeDescriptor(x + 1, y + 2) +
            //    2 * AnalizeDescriptor(x - 1, y + 1) + 3*AnalizeDescriptor(x, y + 1) + 2*AnalizeDescriptor(x + 1, y + 1);

            //left = AnalizeDescriptor(x - 2, y - 1) + 2*AnalizeDescriptor(x - 2, y) + AnalizeDescriptor(x - 2, y + 1) +
            //    2 * AnalizeDescriptor(x - 1, y - 1) + 3*AnalizeDescriptor(x - 1, y) + 2*AnalizeDescriptor(x - 1, y + 1);

            //right = AnalizeDescriptor(x + 2, y - 1) + 2*AnalizeDescriptor(x + 2, y) + AnalizeDescriptor(x + 2, y + 1) +
            //    2 * AnalizeDescriptor(x + 1, y - 1) + 3*AnalizeDescriptor(x + 1, y) + 2*AnalizeDescriptor(x + 1, y + 1);

            up = AnalizeDescriptor(universe,x - 1, y - 2) + AnalizeDescriptor(universe,x + 1, y - 2) + 3 * AnalizeDescriptor(universe,x, y - 1) +
            2 * (AnalizeDescriptor(universe,x, y - 2) + AnalizeDescriptor(universe,x - 1, y - 1) + AnalizeDescriptor(universe,x + 1, y - 1));

            down = AnalizeDescriptor(universe,x - 1, y + 2) + AnalizeDescriptor(universe,x + 1, y + 2) + 3 * AnalizeDescriptor(universe,x, y + 1) +
                2 * (AnalizeDescriptor(universe,x - 1, y + 1) + AnalizeDescriptor(universe,x, y + 2) + AnalizeDescriptor(universe,x + 1, y + 1));

            left = AnalizeDescriptor(universe,x - 2, y - 1) + AnalizeDescriptor(universe,x - 2, y + 1) + 3 * AnalizeDescriptor(universe,x - 1, y) +
                2 * (AnalizeDescriptor(universe,x - 1, y - 1) + AnalizeDescriptor(universe,x - 2, y) + AnalizeDescriptor(universe,x - 1, y + 1));

            right = AnalizeDescriptor(universe,x + 2, y - 1) + 3 * AnalizeDescriptor(universe,x + 1, y) + AnalizeDescriptor(universe,x + 2, y + 1) +
                2 * (AnalizeDescriptor(universe,x + 1, y - 1) + AnalizeDescriptor(universe,x + 2, y) + AnalizeDescriptor(universe,x + 1, y + 1));


            float biggest = up;
            if (down > biggest)
                biggest = down;
            if (left > biggest)
                biggest = left;
            if (right > biggest)
                biggest = right;

            MoveDirection res = MoveDirection.stand;
            if (biggest >= 0)
            {
                List<MoveDirection> md = new List<MoveDirection>(0);
                if (biggest == up)
                    md.Add(MoveDirection.up);
                if (biggest == down)
                    md.Add(MoveDirection.down);
                if (biggest == left)
                    md.Add(MoveDirection.left);
                if (biggest == right)
                    md.Add(MoveDirection.right);

                res = md[StableRandom.rd.Next(md.Count)];

            }
            moveDisperation = res;



        }
        public bool CanReproduct()
        {
            return ((GetEnergyLevel() >= (ConstsUniverse.EnergyLevel_NeededForReproduction - (genome.GetReproduction() * ConstsUniverse.EnergyLevel_NeededForReproduction / 10))) && IsAdult());
        }
        bool IsAdult()
        {
            return GetAge() >= ConstsUniverse.CellAge_AdultCell;
        }
        float AnalizeDescriptor(IUniverse universe, int x, int y)
        {
            int desc = universe.GetObjectDescriptor(x, y);
            if (desc == 0)//empty
            {
                return 0;
            }
            else if (desc == descriptor)//friend
            {
                return genome.GetFriendly();
            }
            else if (desc < 0)//food
            {
                return genome.GetHunger();
            }
            else//enemy
            {
                if (GetAge() >= ConstsUniverse.CellAge_AdultCell)
                    return genome.GetAggression();
                else
                    return ConstsUniverse.CellGenome_Child_Aggression;
            }



        }

    }
}
