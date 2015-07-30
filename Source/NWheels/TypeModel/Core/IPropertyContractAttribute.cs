﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NWheels.DataObjects.Core;

namespace NWheels.DataObjects.Core
{
    public interface IPropertyContractAttribute
    {
        void ApplyTo(PropertyMetadataBuilder property, TypeMetadataCache cache);
    }
}
