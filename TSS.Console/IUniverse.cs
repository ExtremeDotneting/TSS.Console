using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSS.UniverseLogic
{
    public interface IUniverse
    {
	
        int GetWidth();
        int GetHeight();
        void DoUniverseTick();
        long GetTicksCount();
        Tuple<ICell, int> GetMostFit();
        int GetCellsCount();
        int[,] GetAllDescriptors();
        int GetObjectDescriptor(int x, int y);
    }
}
