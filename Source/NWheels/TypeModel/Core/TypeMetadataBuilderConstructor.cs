﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hapil;
using Hapil.Members;

namespace NWheels.DataObjects.Core
{
    internal class TypeMetadataBuilderConstructor
    {
        private readonly MetadataConventionSet _conventions;
        private Type _primaryContract;
        private Type[] _mixinContracts;
        private HashSet<PropertyInfo> _allProperties;
        private TypeMetadataBuilder _thisType;
        private TypeMetadataCache _cache;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TypeMetadataBuilderConstructor(MetadataConventionSet conventions)
        {
            _conventions = conventions;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public bool ConstructMetadata(
            Type primaryContract,
            Type[] mixinContracts,
            TypeMetadataBuilder builder,
            TypeMetadataCache cache,
            out Type[] addedMixinContracts)
        {
            _mixinContracts = mixinContracts;
            _primaryContract = primaryContract;
            _thisType = builder;
            _cache = cache;

            _allProperties = UnionPropertiesInAllContracts(primaryContract, mixinContracts);

            builder.Name = primaryContract.Name.TrimPrefix("I");
            builder.ContractType = primaryContract;
            builder.MixinContractTypes.AddRange(mixinContracts);

            var initialMixinContractCount = _mixinContracts.Length;

            ApplyObjectContractAttribute();
            RegisterTypeInheritance();
            ConstructProperties();

            if ( builder.MixinContractTypes.Count == initialMixinContractCount )
            {
                builder.EndBuild();
                addedMixinContracts = Type.EmptyTypes;
                return true;
            }
            else
            {
                addedMixinContracts = builder.MixinContractTypes.Skip(initialMixinContractCount).ToArray();
                return false;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void ApplyObjectContractAttribute()
        {
            var contractAttribute = (_primaryContract.GetCustomAttribute<DataObjectContractAttribute>() as IObjectContractAttribute);

            if ( contractAttribute != null )
            {
                contractAttribute.ApplyTo(_thisType, _cache);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void RegisterTypeInheritance()
        {
            if ( _thisType.BaseType != null )
            {
                _thisType.BaseType.RegisterDerivedType(_thisType);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void ConstructProperties()
        {
            CreateProperties();
            ApplyConventions();
            ValidateProperties();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void CreateProperties()
        {
            foreach ( var propertyInfo in _allProperties )
            {
                _thisType.Properties.Add(new PropertyMetadataBuilder(_thisType, propertyInfo));
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void ApplyConventions()
        {
            _conventions.ApplyTypeConventions(_thisType);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void ValidateProperties()
        {
            //TODO
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static HashSet<PropertyInfo> UnionPropertiesInAllContracts(Type primaryContract, Type[] mixinContracts)
        {
            var result = new HashSet<PropertyInfo>();

            result.UnionWith(TypeMemberCache.Of(primaryContract).ImplementableProperties);

            foreach ( var contract in mixinContracts )
            {
                result.UnionWith(TypeMemberCache.Of(contract).ImplementableProperties);
            }

            return result;
        }
    }
}
