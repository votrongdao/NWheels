﻿#if false

using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NWheels.Extensions;
using NWheels.Hosting;
using NWheels.Modules.Security;
using NWheels.Samples.BloggingPlatform.Apps;
using NWheels.Samples.BloggingPlatform.Domain;

namespace NWheels.Samples.BloggingPlatform
{
    public class ModuleLoader : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.NWheelsFeatures().ObjectContracts().Concretize<IUserAccountEntity>().With<IBlogUserAccountEntity>();
            builder.NWheelsFeatures().Entities().RegisterDataRepository<IBlogDataRepository>().WithInitializeStorageOnStartup();
            builder.NWheelsFeatures().UI().RegisterApplication<BlogApp>().WithWebEndpoint();
        }
    }
}


#endif