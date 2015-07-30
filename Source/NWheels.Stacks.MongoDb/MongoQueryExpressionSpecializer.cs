﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NWheels.DataObjects;
using NWheels.Extensions;
using NWheels.Stacks.MongoDb.Factories;

namespace NWheels.Stacks.MongoDb
{
    public class MongoQueryExpressionSpecializer
    {
        private readonly ITypeMetadata _thisTypeMetadata;
        private readonly ITypeMetadataCache _metadataCache;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public MongoQueryExpressionSpecializer(ITypeMetadata thisTypeMetadata, ITypeMetadataCache metadataCache)
        {
            _thisTypeMetadata = thisTypeMetadata;
            _metadataCache = metadataCache;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------
        
        public Expression Specialize(Expression general)
        {
            if ( general == null )
            {
                return null;
            }

            var visitor = new SpecializingVisitor(this, _thisTypeMetadata, _metadataCache);
            var specialized = visitor.Visit(general);
            return specialized;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private class SpecializingVisitor : ExpressionVisitor
        {
            private readonly MongoQueryExpressionSpecializer _ownerSpecializer;
            private readonly ITypeMetadata _thisTypeMetadata;
            private readonly ITypeMetadataCache _metadataCache;

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            public SpecializingVisitor(MongoQueryExpressionSpecializer ownerSpecializer, ITypeMetadata thisTypeMetadata, ITypeMetadataCache metadataCache)
            {
                _ownerSpecializer = ownerSpecializer;
                _thisTypeMetadata = thisTypeMetadata;
                _metadataCache = metadataCache;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if ( node.Method.IsGenericMethod && node.Method.GetGenericArguments().Any(ShouldReplaceType) )
                {
                    var replacedTypeArguments = node.Method.GetGenericArguments().Select(t => t.Replace(ShouldReplaceType, GetReplacingType)).ToArray();
                    
                    var specialized = Expression.Call(
                        _ownerSpecializer.Specialize(node.Object),
                        node.Method.GetGenericMethodDefinition().MakeGenericMethod(replacedTypeArguments),
                        node.Arguments.Select(arg => _ownerSpecializer.Specialize(arg)));

                    return specialized;
                }
                else if ( 
                    node.Method.DeclaringType != null && 
                    node.Method.DeclaringType.IsGenericType &&
                    node.Method.DeclaringType.GetGenericArguments().Any(ShouldReplaceType) )
                {
                    var replacedTypeArguments = node.Method.DeclaringType.GetGenericArguments().Select(t => t.Replace(ShouldReplaceType, GetReplacingType)).ToArray();
                    var replacedDeclaringType = node.Method.DeclaringType.GetGenericTypeDefinition().MakeGenericType(replacedTypeArguments);
                    var replacedParameterTypes = node.Method.GetParameters().Select(p => p.ParameterType.Replace(ShouldReplaceType, GetReplacingType)).ToArray();
                    var replacedMethod = replacedDeclaringType.GetMethod(node.Method.Name, replacedParameterTypes);
                    var specialized = Expression.Call(
                        _ownerSpecializer.Specialize(node.Object),
                        replacedMethod,
                        node.Arguments.Select(arg => _ownerSpecializer.Specialize(arg)));

                    return specialized;
                }
                else if ( node.Arguments.Any(arg => ShouldReplaceType(arg.Type)) )
                {
                    var specialized = Expression.Call(
                        _ownerSpecializer.Specialize(node.Object),
                        node.Method,
                        node.Arguments.Select(arg => _ownerSpecializer.Specialize(arg)));

                    return specialized;
                }

                return node;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                var replacedDelegateType = typeof(T).Replace(findWhat: ShouldReplaceType, replaceWith: GetReplacingType);

                var specialized = Expression.Lambda(
                    replacedDelegateType,
                    _ownerSpecializer.Specialize(node.Body),
                    node.Parameters.Select(p => _ownerSpecializer.Specialize(p)).Cast<ParameterExpression>());

                return specialized;
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override Expression VisitParameter(ParameterExpression node)
            {
                ITypeMetadata typeMetadata;

                if ( node.Type == _thisTypeMetadata.ContractType )
                {
                    var replaced = Expression.Parameter(_thisTypeMetadata.GetImplementationBy<MongoEntityObjectFactory>(), node.Name);
                    return replaced;
                }
                else if ( _metadataCache.TryGetTypeMetadata(node.Type, out typeMetadata) )
                {
                    var replaced = Expression.Parameter(typeMetadata.GetImplementationBy<MongoEntityObjectFactory>(), node.Name);
                    return replaced;
                }
                else
                {
                    return base.VisitParameter(node);
                }
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override Expression VisitMember(MemberExpression node)
            {
                var declaringType = node.Member.DeclaringType;

                if ( declaringType != null && declaringType.IsInterface && node.Member is PropertyInfo )
                {
                    ITypeMetadata typeMetadata;

                    if ( declaringType == _thisTypeMetadata.ContractType )
                    {
                        typeMetadata = _thisTypeMetadata;
                    }
                    else
                    {
                        _metadataCache.TryGetTypeMetadata(declaringType, out typeMetadata);
                    }

                    if ( typeMetadata != null )
                    {
                        var implementationPropertyInfo = typeMetadata.GetPropertyByDeclaration((PropertyInfo)node.Member).GetImplementationBy<MongoEntityObjectFactory>();
                        var replaced = Expression.MakeMemberAccess(_ownerSpecializer.Specialize(node.Expression), implementationPropertyInfo);
                        return replaced;
                    }
                }

                return base.VisitMember(node);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            protected override Expression VisitUnary(UnaryExpression node)
            {
                return base.VisitUnary(node);
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private bool ShouldReplaceType(Type type)
            {
                return (type == _thisTypeMetadata.ContractType || type.IsInterface && _metadataCache.ContainsTypeMetadata(type));
            }

            //-------------------------------------------------------------------------------------------------------------------------------------------------

            private Type GetReplacingType(Type type)
            {
                return _metadataCache.GetTypeMetadata(type).GetImplementationBy<MongoEntityObjectFactory>();
            }
        }
    }
}
