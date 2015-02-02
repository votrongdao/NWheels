﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWheels.Hosting
{
    public interface INodeHost
    {
        INodeConfiguration Node { get; }
        NodeState State { get; }
    }
}
