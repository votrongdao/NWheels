﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hapil.Testing.NUnit;
using NUnit.Framework;
using NWheels.Conventions.Core;
using NWheels.DataObjects.Core.Conventions;
using NWheels.Entities.Core;
using NWheels.Testing;
using NWheels.Testing.Entities.Stacks;
using NWheels.Stacks.EntityFramework.Factories;

namespace NWheels.Stacks.EntityFramework.ComponentTests
{
    [TestFixture, Ignore("Requires maintenance")]
    public class EfEntityObjectFactoryTests : DynamicTypeUnitTestBase
    {
        [Test]
        public void CanCreateFlatEntityObject()
        {
            //-- Arrange

            var factory = CreateEntityObjectFactory();

            //-- Act

            var obj = factory.NewEntity<Interfaces.Repository1.IProduct>();

            obj.CatalogNo = "CN111";
            obj.Name = "ABC";
            obj.Price = 123.45m;

            //-- Assert

            Assert.That(obj.CatalogNo, Is.EqualTo("CN111"));
            Assert.That(obj.Name, Is.EqualTo("ABC"));
            Assert.That(obj.Price, Is.EqualTo(123.45m));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanCreateEntityObjectsWithNavigations()
        {
            //-- Arrange

            var factory = CreateEntityObjectFactory();

            //-- Act

            var product = factory.NewEntity<Interfaces.Repository1.IProduct>();
            var order = factory.NewEntity<Interfaces.Repository1.IOrder>();
            var orderLine = factory.NewEntity<Interfaces.Repository1.IOrderLine>();

            orderLine.Order = order;
            orderLine.Product = product;
            order.OrderLines.Add(orderLine);

            //-- Assert

            Assert.That(orderLine.Order, Is.SameAs(order));
            Assert.That(orderLine.Product, Is.SameAs(product));
            Assert.That(order.OrderLines.Count, Is.EqualTo(1));
            Assert.That(order.OrderLines.First(), Is.SameAs(orderLine));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void EntityObjectFactoryReusesConcreteTypes()
        {
            //-- Arrange

            var factory = CreateEntityObjectFactory();

            //-- Act

            var product1 = factory.NewEntity<Interfaces.Repository1.IProduct>();
            var product2 = factory.NewEntity<Interfaces.Repository1.IProduct>();

            //-- Assert

            Assert.That(product1.GetType(), Is.SameAs(product2.GetType()));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void EntityObjectFactoryCreatesTransientObjects()
        {
            //-- Arrange

            var factory = CreateEntityObjectFactory();

            //-- Act

            var orderLine1 = factory.NewEntity<Interfaces.Repository1.IOrderLine>();
            var orderLine2 = factory.NewEntity<Interfaces.Repository1.IOrderLine>();

            //-- Assert

            Assert.That(orderLine1, Is.Not.SameAs(orderLine2));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NavigationPropertiesAreOfEntityImplementationTypes()
        {
            //-- Arrange

            var factory = CreateEntityObjectFactory();

            var productType = factory.NewEntity<Interfaces.Repository1.IProduct>().GetType();
            var orderType = factory.NewEntity<Interfaces.Repository1.IOrder>().GetType();
            var orderLineType = factory.NewEntity<Interfaces.Repository1.IOrderLine>().GetType();

            //-- Act

            var orderLinesPropertyOfOrder = orderType.GetProperty("OrderLines");
            var orderPropertyOfOrderLine = orderLineType.GetProperty("Order");
            var productPropertyOfOrderLine = orderLineType.GetProperty("Product");

            //-- Assert

            var isConcreteType = new Func<Type, bool>(t => t.IsClass && !t.IsInterface && !t.IsAbstract);

            Assert.IsTrue(isConcreteType(productType));
            Assert.IsTrue(isConcreteType(orderType));
            Assert.IsTrue(isConcreteType(orderLineType));

            Assert.That(orderLinesPropertyOfOrder.PropertyType, Is.EqualTo(typeof(ICollection<>).MakeGenericType(orderLineType)));
            Assert.That(orderPropertyOfOrderLine.PropertyType, Is.EqualTo(orderType));
            Assert.That(productPropertyOfOrderLine.PropertyType, Is.EqualTo(productType));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NavigationCollectionsAreInitializedInConstructor()
        {
            //-- Arrange

            var factory = CreateEntityObjectFactory();

            var orderImpl = factory.NewEntity<Interfaces.Repository1.IOrder>();
            Interfaces.Repository1.IOrder orderAsContract = (Interfaces.Repository1.IOrder)orderImpl;

            var orderLinesPropertyOfOrder = orderImpl.GetType().GetProperty("OrderLines");

            //-- Act

            var orderLinesOfOrderImpl = (System.Collections.IEnumerable)orderLinesPropertyOfOrder.GetValue(orderImpl);

            //-- Assert

            Assert.That(orderLinesOfOrderImpl, Is.Not.Null);
            Assert.That(orderAsContract.OrderLines, Is.Not.Null);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void ScalarPropertiesOfContractAndImplementationAreSynchronized()
        {
            //-- Arrange

            var factory = CreateEntityObjectFactory();

            var productImpl = factory.NewEntity<Interfaces.Repository1.IProduct>();
            Interfaces.Repository1.IProduct productAsContract = (Interfaces.Repository1.IProduct)productImpl;

            var namePropertyOfProduct = productImpl.GetType().GetProperty("Name");
            var pricePropertyOfProduct = productImpl.GetType().GetProperty("Price");

            //-- Act

            productAsContract.Name = "ABC";
            pricePropertyOfProduct.SetValue(productImpl, 123.45m);

            //-- Assert

            Assert.That(productAsContract.Name, Is.EqualTo("ABC"));
            Assert.That(productAsContract.Price, Is.EqualTo(123.45m));

            Assert.That(namePropertyOfProduct.GetValue(productImpl), Is.EqualTo("ABC"));
            Assert.That(pricePropertyOfProduct.GetValue(productImpl), Is.EqualTo(123.45m));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void SingleNavigationPropertiesOfContractAndImplementationAreSynchronized()
        {
            //-- Arrange

            var factory = CreateEntityObjectFactory();

            var product = factory.NewEntity<Interfaces.Repository1.IProduct>();
            var order = factory.NewEntity<Interfaces.Repository1.IOrder>();

            var orderLineImpl = factory.NewEntity<Interfaces.Repository1.IOrderLine>();
            Interfaces.Repository1.IOrderLine orderLineAsContract = (Interfaces.Repository1.IOrderLine)orderLineImpl;

            var productPropertyOfOrderLine = orderLineImpl.GetType().GetProperty("Product");
            var orderPropertyOfOrderLine = orderLineImpl.GetType().GetProperty("Order");

            //-- Act

            orderLineAsContract.Product = product;
            orderPropertyOfOrderLine.SetValue(orderLineImpl, order);

            //-- Assert

            Assert.That(orderLineAsContract.Product, Is.SameAs(product));
            Assert.That(orderLineAsContract.Order, Is.SameAs(order));

            Assert.That(productPropertyOfOrderLine.GetValue(orderLineImpl), Is.SameAs(product));
            Assert.That(orderPropertyOfOrderLine.GetValue(orderLineImpl), Is.SameAs(order));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void NavigationCollectionPropertiesOfContractAndImplementationAreSynchronized()
        {
            //-- Arrange

            var factory = CreateEntityObjectFactory();

            var orderLine1 = factory.NewEntity<Interfaces.Repository1.IOrderLine>();
            var orderLine2 = factory.NewEntity<Interfaces.Repository1.IOrderLine>();

            var orderImpl = factory.NewEntity<Interfaces.Repository1.IOrder>();
            Interfaces.Repository1.IOrder orderAsContract = (Interfaces.Repository1.IOrder)orderImpl;

            var orderLinesPropertyOfOrder = orderImpl.GetType().GetProperty("OrderLines");
            var addToOrderLinesMethodOfOrder = orderLinesPropertyOfOrder.PropertyType.GetMethod("Add");

            //-- Act

            orderAsContract.OrderLines.Add(orderLine1);

            var orderLinesOfOrderImpl = (System.Collections.IEnumerable)orderLinesPropertyOfOrder.GetValue(orderImpl);
            addToOrderLinesMethodOfOrder.Invoke(orderLinesOfOrderImpl, new object[] { orderLine2 });

            //-- Assert

            var orderLinesFromImpl = orderLinesOfOrderImpl.Cast<object>().ToArray();
            var orderLinesFromContract = orderAsContract.OrderLines.Cast<object>().ToArray();

            Assert.That(orderLinesFromImpl.Length, Is.EqualTo(2));
            Assert.That(orderLinesFromContract.Length, Is.EqualTo(2));

            Assert.That(orderLinesFromImpl[0], Is.Not.Null);
            Assert.That(orderLinesFromImpl[1], Is.Not.Null);
            Assert.That(orderLinesFromImpl[0], Is.Not.SameAs(orderLinesFromImpl[1]));

            Assert.That(orderLinesFromImpl[0], Is.SameAs(orderLinesFromContract[0]));
            Assert.That(orderLinesFromImpl[1], Is.SameAs(orderLinesFromContract[1]));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void EntityObjectNavigationPropertiesAreVirtual()
        {
            //-- Arrange

            var factory = CreateEntityObjectFactory();

            var productType = factory.NewEntity<Interfaces.Repository1.IProduct>().GetType();
            var orderType = factory.NewEntity<Interfaces.Repository1.IOrder>().GetType();
            var orderLineType = factory.NewEntity<Interfaces.Repository1.IOrderLine>().GetType();


            //-- Act

            var orderLinesPropertyOfOrder = orderType.GetProperty("OrderLines");
            var orderPropertyOfOrderLine = orderLineType.GetProperty("Order");
            var productPropertyOfOrderLine = orderLineType.GetProperty("Product");

            //-- Assert

            var isPropertyVirtual = new Func<PropertyInfo, bool>(p => p.GetGetMethod().IsVirtual && !p.GetGetMethod().IsFinal);

            Assert.That(isPropertyVirtual(orderLinesPropertyOfOrder), Is.True);
            Assert.That(isPropertyVirtual(orderPropertyOfOrderLine), Is.True);
            Assert.That(isPropertyVirtual(productPropertyOfOrderLine), Is.True);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void EntityImplementsIEntityObject()
        {
            //-- Arrange

            var factory = CreateEntityObjectFactory();
            var product = factory.NewEntity<Interfaces.Repository1.IProduct>();

            //-- Act

            var entityObject = (product as IEntityObject);

            //-- Assert

            Assert.That(entityObject, Is.Not.Null);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanGetEntityContractType()
        {
            //-- Arrange

            var factory = CreateEntityObjectFactory();
            var product = factory.NewEntity<Interfaces.Repository1.IProduct>();

            //-- Act

            var contractType = ((IEntityObject)product).ContractType;

            //-- Assert

            Assert.That(contractType, Is.EqualTo(typeof(Interfaces.Repository1.IProduct)));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanGetEntityId()
        {
            //-- Arrange

            var factory = CreateEntityObjectFactory();
            var product = factory.NewEntity<Interfaces.Repository1.IProduct>();

            ((IEntityObject)product).SetId(123);

            //-- Act

            var entityId = ((IEntityObject)product).GetId();

            //-- Assert

            Assert.That(entityId, Is.Not.Null);
            Assert.That(entityId.ContractType, Is.EqualTo(typeof(Interfaces.Repository1.IProduct)));
            Assert.That(entityId.Value, Is.EqualTo(123));
            Assert.That(entityId.ValueAs<int>(), Is.EqualTo(123));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanSetEntityId()
        {
            object x = 123;
            int y = (int)x;


            //-- Arrange

            var factory = CreateEntityObjectFactory();
            var product = factory.NewEntity<Interfaces.Repository1.IProduct>();

            //-- Act

            ((IEntityObject)product).SetId(123);

            //-- Assert

            Assert.That(((IEntityObject)product).GetId().Value, Is.EqualTo(123));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private EntityObjectFactory CreateEntityObjectFactory()
        {
            var metadataCache = TestFramework.CreateMetadataCacheWithDefaultConventions(customMetadataConventions: new IMetadataConvention[] {
                new DefaultIdMetadataConvention(typeof(int)), 
            });
            
            return new EfEntityObjectFactory(Framework.Components, base.DyamicModule, metadataCache);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        //[Test]
        //public void CanCreateApplicationDataRepository()
        //{
        //    //-- Arrange

        //    var entityFactory = new EntityObjectFactory(base.Module);
        //    var repoFactory = new DataRepositoryObjectFactory(base.Module, entityFactory);
        //    var unitOfWork = new UnitOfWork(null, false);

        //    //-- Act

        //    var repo = repoFactory.CreateInstanceOf<Interfaces.Repository1.IDataRepository>().UsingConstructor(unitOfWork);

        //    //-- Assert

        //    Assert.That(repo.Products, Is.Not.Null);
        //    Assert.That(repo.Orders, Is.Not.Null);
        //}

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        //[Test]
        //public void CanCreateRepositoryContextObject()
        //{
        //    //-- Arrange

        //    var factory = new EntityRepositoryFactory(base.Module);

        //    //-- Act

        //    var product = factory.NewEntity<Interfaces.Repository1.IProduct>();
        //    var order = factory.NewEntity<Interfaces.Repository1.IOrder>();
        //    var orderLine = factory.NewEntity<Interfaces.Repository1.IOrderLine>();

        //    orderLine.Order = order;
        //    orderLine.Product = product;
        //    order.OrderLines.Add(orderLine);

        //    //-- Assert

        //    Assert.That(orderLine.Order, Is.SameAs(order));
        //    Assert.That(orderLine.Product, Is.SameAs(product));
        //    Assert.That(order.OrderLines.Count, Is.EqualTo(1));
        //    Assert.That(order.OrderLines.First(), Is.SameAs(orderLine));
        //}
    }
}
