﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Autofac;
using NWheels.DataObjects;
using NWheels.Entities;

namespace NWheels.Stacks.EntityFramework.Impl
{
    public class EfEntityRepository<TEntityContract, TEntityImpl> : IEntityRepository<TEntityContract>, IEntityRepository
        where TEntityContract : class
        where TEntityImpl : class, TEntityContract
    {
        private readonly EfDataRepositoryBase _ownerRepo;
        private readonly ObjectSet<TEntityImpl> _objectSet;
        private InterceptingQueryProvider _queryProvider;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public EfEntityRepository(EfDataRepositoryBase ownerRepo)
        {
            _ownerRepo = ownerRepo;
            _objectSet = ownerRepo.CreateObjectSet<TEntityImpl>();
            _queryProvider = null;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        IEnumerator<TEntityContract> IEnumerable<TEntityContract>.GetEnumerator()
        {
            _ownerRepo.ValidateOperationalState();
            return _objectSet.AsEnumerable().GetEnumerator();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            _ownerRepo.ValidateOperationalState();
            return _objectSet.AsEnumerable().GetEnumerator();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        Type IQueryable.ElementType
        {
            get
            {
                _ownerRepo.ValidateOperationalState();
                return _objectSet.AsQueryable().ElementType;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        System.Linq.Expressions.Expression IQueryable.Expression
        {
            get
            {
                _ownerRepo.ValidateOperationalState();
                return _objectSet.AsQueryable().Expression;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        IQueryProvider IQueryable.Provider
        {
            get
            {
                _ownerRepo.ValidateOperationalState();

                if ( _queryProvider == null )
                {
                    _queryProvider = new InterceptingQueryProvider(this);
                }
                
                return _queryProvider;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        #region Implementation of IEntityRepository

        object IEntityRepository.New()
        {
            return this.New();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        object IEntityRepository.New(Type concreteContract)
        {
            return this.New(concreteContract);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        void IEntityRepository.Insert(object entity)
        {
            this.Insert((TEntityContract)entity);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        void IEntityRepository.Update(object entity)
        {
            this.Update((TEntityContract)entity);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        void IEntityRepository.Delete(object entity)
        {
            this.Delete((TEntityContract)entity);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        Type IEntityRepository.ContractType
        {
            get
            {
                return typeof(TEntityContract);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        Type IEntityRepository.ImplementationType
        {
            get
            {
                return typeof(TEntityImpl);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        ITypeMetadata IEntityRepository.Metadata
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TEntityContract New()
        {
            return _ownerRepo.EntityFactory.NewEntity<TEntityContract>();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TConcreteEntity New<TConcreteEntity>() where TConcreteEntity : class, TEntityContract
        {
            return _ownerRepo.EntityFactory.NewEntity<TConcreteEntity>();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TEntityContract New(Type concreteContract)
        {
            return (TEntityContract)_ownerRepo.EntityFactory.NewEntity(concreteContract);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public IQueryable<TEntityContract> Include(Expression<Func<TEntityContract, object>>[] properties)
        {
            _ownerRepo.ValidateOperationalState();
            return QueryWithIncludedProperties(_objectSet, properties);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public void Insert(TEntityContract entity)
        {
            _ownerRepo.ValidateOperationalState();
            _objectSet.AddObject((TEntityImpl)entity);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public void Update(TEntityContract entity)
        {
            _ownerRepo.ValidateOperationalState();
            
            _objectSet.Attach((TEntityImpl)entity);
            _ownerRepo.ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.Entity.EntityState.Modified);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public void Delete(TEntityContract entity)
        {
            _ownerRepo.ValidateOperationalState();
            _objectSet.DeleteObject((TEntityImpl)entity);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public ObjectSet<TEntityImpl> ObjectSet
        {
            get
            {
                return _objectSet;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private static IQueryable<TEntityContract> QueryWithIncludedProperties(
            IQueryable<TEntityContract> query,
            IEnumerable<Expression<Func<TEntityContract, object>>> propertiesToInclude)
        {
            return propertiesToInclude.Aggregate(query, (current, property) => current.Include(property));
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class InterceptingQueryProvider : IQueryProvider
        {
            private readonly EfEntityRepository<TEntityContract, TEntityImpl> _ownerRepo;
            private readonly IQueryProvider _actualQueryProvider;

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public InterceptingQueryProvider(EfEntityRepository<TEntityContract, TEntityImpl> ownerRepo)
            {
                _ownerRepo = ownerRepo;
                _actualQueryProvider = ownerRepo.ObjectSet.AsQueryable().Provider;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            #region IQueryProvider Members

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                var specializedExpression = QueryExpressionSpecializer.Specialize(expression);
                var query = _actualQueryProvider.CreateQuery<TElement>(specializedExpression);
                return query;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public IQueryable CreateQuery(Expression expression)
            {
                var specializedExpression = QueryExpressionSpecializer.Specialize(expression);
                var query = _actualQueryProvider.CreateQuery(specializedExpression);
                return query;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public TResult Execute<TResult>(Expression expression)
            {
                return _actualQueryProvider.Execute<TResult>(expression);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public object Execute(Expression expression)
            {
                return _actualQueryProvider.Execute(expression);
            }

            #endregion
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public TEntityContract CheckOutOne<TState>(Expression<Func<TEntityContract, bool>> where, Expression<Func<TEntityContract, TState>> stateProperty, TState newStateValue)
        {
            throw new NotImplementedException();
        }
    }
}
