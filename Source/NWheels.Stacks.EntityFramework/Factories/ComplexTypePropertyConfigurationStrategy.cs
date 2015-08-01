﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil;
using Hapil.Operands;
using Hapil.Writers;
using NWheels.DataObjects;
using TT = Hapil.TypeTemplate;

namespace NWheels.Stacks.EntityFramework.Factories
{
    public class ComplexTypePropertyConfigurationStrategy : PropertyConfigurationStrategy
    {
        public ComplexTypePropertyConfigurationStrategy(ObjectFactoryContext factoryContext, ITypeMetadata metaType, IPropertyMetadata metaProperty)
            : base(factoryContext, metaType, metaProperty)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        #region Overrides of PropertyConfigurationStrategy

        public override void OnWritingEfModelConfiguration(
            MethodWriterBase method, 
            Operand<DbModelBuilder> modelBuilder, 
            Operand<ITypeMetadata> typeMetadata, 
            Operand<EntityTypeConfiguration<TypeTemplate.TImpl>> typeConfig)
        {
            var m = method;
            Type implementationType = FindImpementationType(MetaProperty.ClrType);

            // TODO: configure complex type
        }

        #endregion
    }
}
