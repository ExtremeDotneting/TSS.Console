using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSS.UniverseLogic
{
    public interface ICell:IUniverseObject
    {
        Genome GetGenome();
        MoveDirection GetMoveDisperation();
        float GetEnergyLevel();
        void AddEnergy(float value);
        bool IncAge();
        bool DecEnergy();
        int GetAge();
        ICell CreateChild();
        void CalcMoveDirectionAspiration(IUniverse universe);
        bool CanReproduct();
    }
}
