﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using NWheels.Conventions.Core;
using NWheels.DataObjects.Core;
using NWheels.Testing;
using NWheels.Testing.Entities.Stacks;
using HR1 = NWheels.Stacks.EntityFramework.ComponentTests.HardCodedImplementations.Repository1;

namespace NWheels.Stacks.EntityFramework.ComponentTests
{
    [TestFixture, Category("Integration")]
    public class HardCodedImplementationTests : DatabaseTestBase
    {
        //private Autofac.ContainerBuilder _componentsBuilder = null;
        //private Autofac.IContainer _components = null;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [SetUp]
        public void SetUp()
        {
            //_componentsBuilder = new ContainerBuilder();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanBuildModelAndInitializeObjectContext()
        {
            EnsureDbCompiledModel();

            using ( var connection = CreateDbConnection() )
            {
                var objectContext = base.CompiledModel.CreateObjectContext<ObjectContext>(connection);
                var productObjectSet = objectContext.CreateObjectSet<HR1.EntityObject_Product>();
                var orderObjectSet = objectContext.CreateObjectSet<HR1.EntityObject_Order>();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanObtainDatabaseScript()
        {
            EnsureDbCompiledModel();

            using ( var connection = CreateDbConnection() )
            {
                var objectContext = base.CompiledModel.CreateObjectContext<ObjectContext>(connection);
                var script = objectContext.CreateDatabaseScript();

                Assert.That(string.IsNullOrEmpty(script), Is.False);
                Console.WriteLine(script);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanCreateDatabaseObjects()
        {
            //-- Arrange

            EnsureDbCompiledModel();
            DropAndCreateTestDatabase();

            //-- Act
            
            CreateTestDatabaseObjects();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanInitializeHardCodedDataRepository()
        {
            //-- Arrange

            DropAndCreateTestDatabase();

            //-- Act

            InitializeHardCodedDataRepository().Dispose();
            CreateTestDatabaseObjects();

            //-- Assert

            var productsTable = SelectFromTable("Products");
            var ordersTable = SelectFromTable("Orders");
            var orderLinesTable = SelectFromTable("OrderLines");

            Assert.That(GetCommaSeparatedColumnList(productsTable), Is.EqualTo("Id:Int32,CatalogNo:String,Name:String,Price:Decimal"));
            Assert.That(GetCommaSeparatedColumnList(ordersTable), Is.EqualTo("Id:Int32,OrderNo:String,PlacedAt:DateTime,Status:Int32"));
            Assert.That(GetCommaSeparatedColumnList(orderLinesTable), Is.EqualTo("Id:Int32,Quantity:Int32,Attributes:String,OrderId:Int32,ProductId:Int32"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test, Ignore("WIP")]
        public void CanFineTuneEntityConfiguration()
        {
            //-- Arrange

            DropAndCreateTestDatabase();

            //-- Act

            //HR1.RegisterEntityFineTunings(_componentsBuilder);
            InitializeHardCodedDataRepository().Dispose();
            CreateTestDatabaseObjects();

            //-- Assert

            var productsTable = SelectFromTable("MY_PRODUCTS");
            var ordersTable = SelectFromTable("MY_ORDERS");
            var orderLinesTable = SelectFromTable("MY_ORDER_LINES");

            Assert.That(
                GetCommaSeparatedColumnList(productsTable), 
                Is.EqualTo("Id:Int32,Name:String,MY_SPECIAL_PRICE_COLUMN:Decimal"));
            
            Assert.That(
                GetCommaSeparatedColumnList(ordersTable),
                Is.EqualTo("MY_SPECIAL_ORDER_ID_COLUMN:Int32,PlacedAt:DateTime,Status:Int32"));
            
            Assert.That(
                GetCommaSeparatedColumnList(orderLinesTable),
                Is.EqualTo("Id:Int32,Quantity:Int32,OrderId:Int32,MY_SPECIAL_PRODUCT_ID_COLUMN:Int32"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanPerformBasicCrudOperationsOnHardCodedRepository()
        {
            //-- Arrange

            DropAndCreateTestDatabase();
            InitializeHardCodedDataRepository().Dispose();
            CreateTestDatabaseObjects();

            //-- Act & Assert

            CrudOperations.Repository1.ExecuteBasic(repoFactory: InitializeHardCodedDataRepository);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanPerformAdvancedRetrievalsOnHardCodedRepository()
        {
            //-- Arrange

            DropAndCreateTestDatabase();
            InitializeHardCodedDataRepository().Dispose();
            CreateTestDatabaseObjects();

            //-- Act & Assert

            CrudOperations.Repository1.ExecuteAdvancedRetrievals(repoFactory: InitializeHardCodedDataRepository);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test, Ignore("Requires maintenance")]
        public void CanInitializeHardCodedRepositoryWithCustomNames()
        {
            //-- Arrange

            DropAndCreateTestDatabase();

            //-- Act
            
            InitializeHardCodedDataRepositoryWithCustomNames().Dispose();
            CreateTestDatabaseObjects();

            //-- Assert

            var productsTable = SelectFromTable("MY_PRODUCTS");
            var ordersTable = SelectFromTable("MY_ORDERS");
            var orderLinesTable = SelectFromTable("MY_ORDER_LINES");

            Assert.That(
                GetCommaSeparatedColumnList(productsTable),
                Is.EqualTo("Id:Int32,Name:String,MY_SPECIAL_PRICE_COLUMN:Decimal"));

            Assert.That(
                GetCommaSeparatedColumnList(ordersTable),
                Is.EqualTo("MY_SPECIAL_ORDER_ID_COLUMN:Int32,PlacedAt:DateTime,Status:Int32"));

            Assert.That(
                GetCommaSeparatedColumnList(orderLinesTable),
                Is.EqualTo("Id:Int32,Quantity:Int32,OrderId:Int32,MY_SPECIAL_PRODUCT_ID_COLUMN:Int32"));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test, Ignore("Requires maintenance")]
        public void CanPerformBasicCrudOnHardCodedRepositoryWithCustomNames()
        {
            //-- Arrange

            DropAndCreateTestDatabase();
            InitializeHardCodedDataRepositoryWithCustomNames().Dispose();
            CreateTestDatabaseObjects();

            //-- Act & Assert

            CrudOperations.Repository1.ExecuteBasic(repoFactory: InitializeHardCodedDataRepositoryWithCustomNames);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test, Ignore("Requires maintenance")]
        public void CanPerformAdvancedRetrievalsOnHardCodedRepositoryWithCustomNames()
        {
            //-- Arrange

            DropAndCreateTestDatabase();
            InitializeHardCodedDataRepositoryWithCustomNames().Dispose();
            CreateTestDatabaseObjects();

            //-- Act & Assert

            CrudOperations.Repository1.ExecuteAdvancedRetrievals(repoFactory: InitializeHardCodedDataRepositoryWithCustomNames);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private Interfaces.Repository1.IOnlineStoreRepository InitializeHardCodedDataRepository()
        {
            //_components = _componentsBuilder.Build();
            
            var connection = CreateDbConnection();
            connection.Open();
            var repo = new HR1.DataRepositoryObject_DataRepository(Framework.Components, connection, autoCommit: false);
            base.CompiledModel = repo.CompiledModel;
            return repo;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private Interfaces.Repository1.IOnlineStoreRepository InitializeHardCodedDataRepositoryWithCustomNames()
        {
            //_components = _componentsBuilder.Build();

            var metadataCache = TestFramework.CreateMetadataCacheWithDefaultConventions();

            var productMetadata = ((TypeMetadataBuilder)metadataCache.GetTypeMetadata(typeof(Interfaces.Repository1.IProduct)));
            var orderMetadata = ((TypeMetadataBuilder)metadataCache.GetTypeMetadata(typeof(Interfaces.Repository1.IOrder)));
            var orderLineMetadata = ((TypeMetadataBuilder)metadataCache.GetTypeMetadata(typeof(Interfaces.Repository1.IOrderLine)));

            productMetadata.UpdateImplementation(typeof(EntityObjectFactory), typeof(HR1.EntityObject_Product));
            orderMetadata.UpdateImplementation(typeof(EntityObjectFactory), typeof(HR1.EntityObject_Order));
            orderLineMetadata.UpdateImplementation(typeof(EntityObjectFactory), typeof(HR1.EntityObject_OrderLine));

            metadataCache.EnsureRelationalMapping(productMetadata);
            metadataCache.EnsureRelationalMapping(orderMetadata);
            metadataCache.EnsureRelationalMapping(orderLineMetadata);

            orderMetadata.RelationalMapping.PrimaryTableName = "MY_ORDERS";
            orderMetadata.Properties.Single(p => p.Name == "Id").RelationalMapping.ColumnName = "MY_SPECIAL_ORDER_ID_COLUMN";

            productMetadata.RelationalMapping.PrimaryTableName = "MY_PRODUCTS";
            productMetadata.Properties.Single(p => p.Name == "Price").RelationalMapping.ColumnName = "MY_SPECIAL_PRICE_COLUMN";
            productMetadata.Properties.Single(p => p.Name == "Price").RelationalMapping.ColumnType = "MONEY";

            orderLineMetadata.RelationalMapping.PrimaryTableName = "MY_ORDER_LINES";
            orderLineMetadata.Properties.Single(p => p.Name == "Product").RelationalMapping.ColumnName = "MY_SPECIAL_PRODUCT_ID_COLUMN";

            var connection = CreateDbConnection();
            connection.Open();
            var repo = new HR1.DataRepositoryObject_CustomNames(Framework.Components, metadataCache, connection, autoCommit: false);
            base.CompiledModel = repo.CompiledModel;
            return repo;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private void EnsureDbCompiledModel()
        {
            if ( base.CompiledModel != null )
            {
                return;
            }

            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<HR1.EntityObject_Category>().HasEntitySetName("Category");
            modelBuilder.Entity<HR1.EntityObject_Product>().HasEntitySetName("Product");
            modelBuilder.Entity<HR1.EntityObject_Order>().HasEntitySetName("Order");
            modelBuilder.Entity<HR1.EntityObject_OrderLine>().HasEntitySetName("OrderLine");

            var dbFactory = DbProviderFactories.GetFactory(base.ConnectionStringProviderName);

            using ( var connection = dbFactory.CreateConnection() )
            {
                connection.ConnectionString = base.ConnectionString;
                var model = modelBuilder.Build(connection);
                base.CompiledModel = model.Compile();
            }
        }
    }
}
