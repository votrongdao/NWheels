﻿using System;
using System.Collections.Generic;
using NWheels.DataObjects;
using NWheels.Entities;
using IR3A = NWheels.Testing.Entities.Stacks.Interfaces.Repository3A;

namespace NWheels.Testing.Entities.Stacks
{
    public static class Interfaces
    {
        public static class Repository1
        {
            public enum OrderStatus
            {
                New = 1,
                PaymentReceived = 2,
                ProductsShipped = 3
            }

            public interface IOnlineStoreRepository : IApplicationDataRepository
            {
                IOrderLine NewOrderLine(IOrder order, IProduct product, int quantity);
                IAttributeValue NewAttributeValue(string value, int displayOrder);
                IAttributeValueChoice NewAttributeValueChoice(IAttribute attribute, string value);
                
                IEntityRepository<ICategory> Categories { get; }
                IEntityRepository<IProduct> Products { get; }
                IEntityRepository<IOrder> Orders { get; }
                IEntityRepository<IOrderLine> OrdersLines { get; }
                IEntityRepository<IAttribute> Attributes { get; }
            }

            [EntityContract]
            public interface ICategory : IEntityPartUniqueDisplayName
            {
                //int Id { get; set; }
            }

            [EntityContract]
            public interface IProduct
            {
                //[PropertyContract.Storage.RelationalMapping(Column = "Id")]
                //int Id { get; set; }

                string CatalogNo { get; set; }

                string Name { get; set; }
                
                decimal Price { get; set; }

                [PropertyContract.Relation.Aggregation, PropertyContract.Relation.ManyToMany]
                ICollection<ICategory> Categories { get; }

                [PropertyContract.Relation.Composition, PropertyContract.Relation.ManyToMany]
                ICollection<IAttribute> Attributes { get; }
            }

            [EntityContract]
            public interface IAttribute : IEntityPartUniqueDisplayName
            {
                //int Id { get; set; }

                string TitleForUser { get; set; }

                [PropertyContract.Storage.Json]
                IList<IAttributeValue> Values { get; }
            }

            [EntityPartContract]
            public interface IAttributeValue
            {
                [PropertyContract.Semantic.OrderBy]
                int DisplayOrder { get; set; }

                [PropertyContract.Required(AllowEmpty = false)]
                string Value { get; set; }
            }

            [EntityPartContract]
            public interface IAttributeValueChoice
            {
                [PropertyContract.Relation.Aggregation]
                IAttribute Attribute { get; set; }

                [PropertyContract.Required(AllowEmpty = false)]
                string Value { get; set; }
            }

            [EntityContract]
            public interface IOrder
            {
                //int Id { get; set; }

                string OrderNo { get; set; }

                DateTime PlacedAt { get; set; }

                [PropertyContract.Relation.Composition]
                ICollection<IOrderLine> OrderLines { get; }
                
                [PropertyContract.DefaultValue(OrderStatus.New)]
                OrderStatus Status { get; set; }
            }

            [EntityContract]
            public interface IOrderLine
            {
                //int Id { get; set; }
                
                [PropertyContract.Relation.CompositionParent]
                IOrder Order { get; set; }

                [PropertyContract.Relation.Aggregation]
                IProduct Product { get; set; }

                [PropertyContract.Validation.MinValue(0)]
                int Quantity { get; set; }

                [PropertyContract.Relation.Composition]
                ICollection<IAttributeValueChoice> Attributes { get; }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static class Repository2
        {
            public interface IBlogDataRepository : IApplicationDataRepository
            {
                IEntityRepository<IR3A.IUserAccountEntity> AllUsers { get; }
                IEntityRepository<IAuthorEntity> Authors { get; }
                IEntityRepository<IArticleEntity> Articles { get; }
                IEntityRepository<IPostEntity> Posts { get; }
                IEntityRepository<ITagEntity> Tags { get; }
            }

            public enum UserRole
            {
                Reader = 0,
                Author = 1,
                Admin = 2
            }

            public enum ReplyStatus
            {
                Submitted,
                Accepted,
                Rejected
            }

            [EntityContract]
            public interface IAuthorEntity : Repository3A.IUserAccountEntity
            {
                ICollection<ITopLevelContentEntity> AuthoredContents { get; }
            }

            [EntityContract]
            public interface IAbstractContentEntity : IEntityPartId<int>, Repository3A.IEntityPartAudit
            {
                string Markdown { get; set; }
                ICollection<ITagEntity> Tags { get; }
            }

            [EntityContract]
            public interface ITopLevelContentEntity : IAbstractContentEntity
            {
                ICollection<IReplyEntity> Replies { get; }
            }

            [EntityContract]
            public interface IArticleEntity : ITopLevelContentEntity
            {
                string Title { get; set; }
            }

            [EntityContract]
            public interface IPostEntity : ITopLevelContentEntity
            {
            }

            [EntityContract]
            public interface IReplyEntity : IAbstractContentEntity
            {
                ReplyStatus Status { get; set; }
            }

            [EntityContract]
            public interface ITagEntity : IEntityPartId<int>
            {
                string Name { get; set; }
                string Description { get; set; }
                ICollection<IAbstractContentEntity> RelatedContents { get; }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static class Repository3A
        {
            [EntityContract]
            [MustHaveMixin(typeof(IEntityPartId<>))]
            public interface IUserAccountEntity
            {
                [PropertyContract.Required, PropertyContract.Unique, PropertyContract.Validation.Length(4, 50)]
                string LoginName { get; set; }
                
                [PropertyContract.Validation.MaxLength(50)]
                string NickName { get; set; }

                [PropertyContract.Validation.MaxLength(50)]
                string FullName { get; set; }
                
                [PropertyContract.Required, PropertyContract.Semantic.EmailAddress]
                string EmailAddress { get; set; }
                
                bool IsEmailVerified { get; set; }
                
                [PropertyContract.Required]
                ICollection<IPasswordEntity> Passwords { get; }
                
                ICollection<IUserRoleEntity> Roles { get; }
                
                DateTime LastLoginAt { get; set; }
                
                [PropertyContract.Validation.MinValue(0)]
                int FailedLoginCount { get; set; }
                
                bool IsLockedOut { get; set; }
            }

            [EntityContract]
            [MustHaveMixin(typeof(IEntityPartId<>))]
            [MustHaveMixin(typeof(IEntityPartUserRole<>))]
            public interface IUserRoleEntity
            {
                [PropertyContract.Required, PropertyContract.Unique, PropertyContract.Validation.MaxLength(50)]
                string Name { get; set; }
            }

            [EntityPartContract]
            public interface IEntityPartUserRole<TRole>
            {
                [PropertyContract.Required]
                TRole Role { get; set; }
            }

            [EntityContract]
            [MustHaveMixin(typeof(IEntityPartId<>))]
            public interface IPasswordEntity
            {
                [PropertyContract.Required]
                IUserAccountEntity User { get; set; }

                [PropertyContract.WriteOnly, PropertyContract.Security.Sensitive]
                string ClearText { get; set; }

                [PropertyContract.Required, PropertyContract.SearchOnly, PropertyContract.Security.Sensitive]
                byte[] Hash { get; set; }

                DateTime Expiration { get; set; }

                bool MustChange { get; set; }
            }
            
            [EntityPartContract]
            public interface IEntityPartAudit
            {
                [PropertyContract.Validation.Past("00:00:00")]
                DateTime CreatedAt { get; set; }

                [PropertyContract.Required]
                IUserAccountEntity CreatedBy { get; set; }

                [PropertyContract.Validation.Past("00:00:00")]
                DateTime ModifiedAt { get; set; }

                [PropertyContract.Required]
                IUserAccountEntity ModifiedBy { get; set; }
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public static class Repository3B
        {
            public enum MyAppUserRole
            {
                MyGuest = 0,
                MyPower = 1,
                MyAdmin = 2
            }

            public interface IMyAppUserAccountEntity : Repository3A.IUserAccountEntity, IEntityPartId<int>
            {
                new ICollection<IMyAppUserRoleEntity> Roles { get; }
            }

            public interface IMyAppUserRoleEntity : Repository3A.IUserRoleEntity, IEntityPartId<int>, Repository3A.IEntityPartUserRole<MyAppUserRole>
            {
            }

            public interface IMyAppDataRepository : IApplicationDataRepository
            {
                IEntityRepository<IMyAppUserAccountEntity> Users { get; }
            }
        }
    }
}
