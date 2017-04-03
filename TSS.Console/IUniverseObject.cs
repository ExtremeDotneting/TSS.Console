using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSS.UniverseLogic
{
    public interface IUniverseObject
    {
        void SetCords(int x, int y);
        int GetX();
        int GetY();
        int GetDescriptor();
        void Dispose();
        bool isDisposed();
    }
}
