﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hapil;
using NUnit.Framework;

namespace NWheels.Testing
{
    [TestFixture]
    public class DynamicTypeIntegrationTestBase : IntegrationTestBase
    {
        private Hapil.DynamicModule _dyamicModule;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [TestFixtureSetUp]
        public void BaseFixtureSetUp()
        {
            _dyamicModule = new DynamicModule(
                "EmittedBy" + this.GetType().Name,
                allowSave: true,
                saveDirectory: TestContext.CurrentContext.TestDirectory);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [TestFixtureTearDown]
        public void BaseFixtureTearDown()
        {
            _dyamicModule.SaveAssembly();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public DynamicModule DyamicModule
        {
            get { return _dyamicModule; }
        }
    }
}
