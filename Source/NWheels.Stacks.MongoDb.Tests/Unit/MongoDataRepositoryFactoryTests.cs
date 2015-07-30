﻿using System.Linq;
using Autofac;
using Hapil;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;
using NWheels.DataObjects;
using NWheels.DataObjects.Core;
using NWheels.DataObjects.Core.Conventions;
using NWheels.Entities;
using NWheels.Entities.Core;
using NWheels.Stacks.MongoDb.Factories;
using NWheels.Testing;
using NWheels.Testing.Entities.Stacks;
using IR1 = NWheels.Testing.Entities.Stacks.Interfaces.Repository1;

namespace NWheels.Stacks.MongoDb.Tests.Unit
{
    [TestFixture]
    public class MongoDataRepositoryFactoryTests : UnitTestBase
    {
        private Hapil.DynamicModule _dyamicModule;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            _dyamicModule = new DynamicModule(
                "EmittedByMongoDataRepositoryFactoryTests",
                allowSave: true,
                saveDirectory: TestContext.CurrentContext.TestDirectory);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            _dyamicModule.SaveAssembly();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanCreateDataRepositoryObject()
        {
            //-- arrange
            
            var repoFactory = CreateDataRepositoryFactory();

            //-- act

            var repository = repoFactory.NewUnitOfWork<IR1.IOnlineStoreRepository>(autoCommit: true);

            //-- assert

            Assert.That(repository, Is.Not.Null);

            Assert.That(
                repository.GetEntityRepositories().Select(repo => repo != null ? repo.ContractType : null),
                Is.EquivalentTo(new[] {
                    typeof(IR1.ICategory), typeof(IR1.IAttribute), typeof(IR1.IProduct), typeof(IR1.IOrder), typeof(IR1.IOrderLine), null, null, null
                }));
            
            Assert.That(
                repository.GetEntityContractsInRepository(),
                Is.EquivalentTo(new[] {
                    typeof(IR1.ICategory), typeof(IR1.IAttribute), typeof(IR1.IProduct), typeof(IR1.IOrder), typeof(IR1.IOrderLine), 
                    typeof(IR1.IAttributeValue), typeof(IR1.IAttributeValueChoice), typeof(IR1.IPostalAddress)
                }));
            
            Assert.That(
                repository.GetEntityTypesInRepository().Select(type => type.GetInterfaces().FirstOrDefault(EntityContractAttribute.IsEntityContract)),
                Is.EquivalentTo(new[] {
                    typeof(IR1.ICategory), typeof(IR1.IAttribute), typeof(IR1.IProduct), typeof(IR1.IOrder), typeof(IR1.IOrderLine), null, null, null
                }));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private MongoDataRepositoryFactory CreateDataRepositoryFactory()
        {
            var configAuto = ResolveAuto<IFrameworkDatabaseConfig>();
            configAuto.Instance.ConnectionString = string.Format("server=localhost;database=TEST");

            var metadataCache = TestFramework.CreateMetadataCacheWithDefaultConventions(new IMetadataConvention[] {
                new DefaultIdMetadataConvention(typeof(ObjectId))
            });

            var updater = new ContainerBuilder();
            updater.RegisterInstance(metadataCache).As<ITypeMetadataCache, TypeMetadataCache>();
            updater.Update(Framework.Components.ComponentRegistry);

            var entityFactory = new MongoEntityObjectFactory(Framework.Components, _dyamicModule, metadataCache);
            var repoFactory = new MongoDataRepositoryFactory(Framework.Components, _dyamicModule, entityFactory, metadataCache, configAuto.Instance);
            
            return repoFactory;
        }
    }
}
