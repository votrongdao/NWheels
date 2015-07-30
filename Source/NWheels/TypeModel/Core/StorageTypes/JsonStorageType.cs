﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Hapil;
using Hapil.Operands;
using Hapil.Writers;

namespace NWheels.DataObjects.Core.StorageTypes
{
    public class JsonStorageType<TContractValue> : IStorageDataType<TContractValue, string>, IStorageContractConversionWriter
    {
        public string ContractToStorage(TContractValue contractValue)
        {
            return JsonConvert.SerializeObject(contractValue);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TContractValue StorageToContract(string storageValue)
        {
            return JsonConvert.DeserializeObject<TContractValue>(storageValue);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public Type StorageDataType
        {
            get { return typeof(string); }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        void IStorageContractConversionWriter.WriteContractToStorageConversion(
            MethodWriterBase method, 
            IOperand<TypeTemplate.TContract> contractValue, 
            MutableOperand<TypeTemplate.TValue> storageValue)
        {
            storageValue.Assign(Static.Func(JsonConvert.SerializeObject, contractValue.CastTo<object>()).CastTo<TypeTemplate.TValue>());
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        void IStorageContractConversionWriter.WriteStorageToContractConversion(
            MethodWriterBase method, 
            MutableOperand<TypeTemplate.TContract> contractValue, 
            IOperand<TypeTemplate.TValue> storageValue)
        {
            contractValue.Assign(Static.Func(JsonConvert.DeserializeObject<TypeTemplate.TContract>, storageValue.CastTo<string>()).CastTo<TypeTemplate.TContract>());
        }
    }
}
