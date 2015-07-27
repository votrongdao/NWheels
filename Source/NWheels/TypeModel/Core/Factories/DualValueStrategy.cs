﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Hapil;
using Hapil.Operands;
using Hapil.Writers;
using NWheels.DataObjects;
using NWheels.DataObjects.Core;
using NWheels.DataObjects.Core.Factories;
using NWheels.DataObjects.Core.StorageTypes;
using NWheels.Extensions;
using TT = Hapil.TypeTemplate;

namespace NWheels.TypeModel.Core.Factories
{
    public abstract class DualValueStrategy : PropertyImplementationStrategy
    {
        private readonly Type _storageType;
        private Field<TT.TProperty> _contractField;
        private Field<TT.TValue> _storageField;
        private Field<DualValueStates> _stateField;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected DualValueStrategy(
            ObjectFactoryContext factoryContext, 
            ITypeMetadataCache metadataCache, 
            ITypeMetadata metaType, 
            IPropertyMetadata metaProperty,
            Type storageType)
            : base(factoryContext, metadataCache, metaType, metaProperty)
        {
            _storageType = storageType;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        #region Overrides of PropertyImplementationStrategy

        protected override void OnBeforeImplementation(ClassWriterBase writer)
        {
            using ( TT.CreateScope<TT.TValue>(_storageType) )
            {
                _contractField = writer.Field<TT.TProperty>("pcf_" + MetaProperty.Name);
                _storageField = writer.Field<TT.TValue>("psf_" + MetaProperty.Name);
                _stateField = writer.Field<DualValueStates>("pff_" + MetaProperty.Name);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override void OnImplementContractProperty(ClassWriterBase writer)
        {
            using ( TT.CreateScope<TT.TValue>(_storageType) )
            {
                writer.ImplementInterfaceExplicitly<TT.TInterface>().Property(MetaProperty.ContractPropertyInfo).Implement(
                    getter: p => p.Get(m => {
                        m.If(_stateField == DualValueStates.Storage).Then(() => {
                            OnWritingStorageToContractConversion(m, _contractField, _storageField);
                            _stateField.Assign(_stateField | DualValueStates.Contract);
                        });
                        m.Return(_contractField.CastTo<TT.TProperty>());
                    }),
                    setter: p => p.Set((m, value) => {
                        _contractField.Assign(value.CastTo<TT.TProperty>());
                        _stateField.Assign(DualValueStates.Contract);
                    })
                );
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override void OnImplementStorageProperty(ClassWriterBase writer)
        {
            using ( TT.CreateScope<TT.TValue>(_storageType) )
            {
                writer.ImplementBase<object>().NewVirtualWritableProperty<TT.TValue>(MetaProperty.Name).Implement(
                    getter: p => p.Get(m => {
                        m.If(_stateField == DualValueStates.Contract).Then(() => {
                            OnWritingContractToStorageConversion(m, _contractField, _storageField);
                            _stateField.Assign(_stateField | DualValueStates.Storage);
                        });
                        m.Return(_storageField);
                    }),
                    setter: p => p.Set((m, value) => {
                        _storageField.Assign(value);
                        _stateField.Assign(DualValueStates.Storage);
                    })
                );
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected abstract void OnWritingContractToStorageConversion(
            MethodWriterBase method, 
            IOperand<TypeTemplate.TProperty> contractValue, 
            MutableOperand<TypeTemplate.TValue> storageValue);

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected abstract void OnWritingStorageToContractConversion(
            MethodWriterBase method,
            MutableOperand<TypeTemplate.TProperty> contractValue, 
            IOperand<TypeTemplate.TValue> storageValue);

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        
        public Type StorageType
        {
            get { return _storageType; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        
        public Field<TypeTemplate.TProperty> ContractField
        {
            get { return _contractField; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        
        public Field<TypeTemplate.TValue> StorageField
        {
            get { return _storageField; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        
        public Field<DualValueStates> StateField
        {
            get { return _stateField; }
        }
    }
}
