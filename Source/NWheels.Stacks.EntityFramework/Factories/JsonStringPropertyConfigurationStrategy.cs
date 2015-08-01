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
    public class JsonStringPropertyConfigurationStrategy : PropertyConfigurationStrategy
    {
        public JsonStringPropertyConfigurationStrategy(ObjectFactoryContext factoryContext, ITypeMetadata metaType, IPropertyMetadata metaProperty)
            : base(factoryContext, metaType, metaProperty)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        #region Overrides of PropertyConfigurationStrategy

        public override void OnWritingEfModelConfiguration(
            MethodWriterBase method, 
            Operand<DbModelBuilder> modelBuilder,
            Operand<ITypeMetadata> typeMetadata,
            Operand<EntityTypeConfiguration<TT.TImpl>> typeConfig)
        {
            var m = method;

            Static.GenericFunc(
                (e, p) => EfModelApi.StringProperty<TT.TImpl>(e, p),
                typeConfig,
                typeMetadata.Func<string, IPropertyMetadata>(x => x.GetPropertyByName, m.Const(MetaProperty.Name)));
        }

        #endregion
    }
}
