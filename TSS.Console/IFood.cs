﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSS.UniverseLogic
{
    public interface IFood:IUniverseObject
    {
        float GetEnergyLevel();
    }
}
