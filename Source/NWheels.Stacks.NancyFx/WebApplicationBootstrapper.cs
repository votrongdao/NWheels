﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Nancy;
using Nancy.Bootstrappers.Autofac;
using Nancy.Conventions;

namespace NWheels.Stacks.NancyFx
{
    public class WebApplicationBootstrapper : AutofacNancyBootstrapper, IRootPathProvider
    {
        private readonly WebApplicationModule _module;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public WebApplicationBootstrapper(WebApplicationModule module)
        {
            _module = module;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override IEnumerable<INancyModule> GetAllModules(ILifetimeScope container)
        {
            return new[] { _module };
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override INancyModule GetModule(ILifetimeScope container, Type moduleType)
        {
            return _module;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/assets", "assets"));
            base.ConfigureConventions(nancyConventions);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        protected override IRootPathProvider RootPathProvider
        {
            get { return this; }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public string GetRootPath()
        {
            return _module.ContentRootPath;
        }
    }
}
