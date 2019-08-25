using HotChocholateExpandable.GraphQL.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

namespace HotChocholateExpandable.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFields<T>(this IQueryable<T> source, IEnumerable<FieldWrapper> fieldWrappers)
        {
            return source.CreateSelection(fieldWrappers);
        }

        public static IQueryable<T> CreateSelection<T>(this IQueryable<T> source, IEnumerable<FieldWrapper> fieldWrappers)
        {
            var expression = CreateSelection(typeof(T), fieldWrappers) as Expression<Func<T, T>>;

            return source.Select(expression);
        }

        static Expression CreateSelection(Type type, IEnumerable<FieldWrapper> fieldWrappers)
        {
            var propertyExpr = Parameter(
                type,
                type.Name);

            var memberAssignments = CreateMemberAssignments(propertyExpr, fieldWrappers);

            var lambdaExpr = Lambda(
                MemberInit(
                    New(type.GetConstructors().First()),
                    memberAssignments.ToArray()
                    ),
                propertyExpr);

            return lambdaExpr;
        }

        static IEnumerable<MemberAssignment> CreateMemberAssignments(ParameterExpression propertyExpression, IEnumerable<FieldWrapper> fieldWrappers)
        {
            foreach (var wrapper in fieldWrappers)
            {
                var wrapperPropertyInfo = propertyExpression.Type.GetProperty(wrapper.Name);

                if (wrapperPropertyInfo == null)
                {
                    var nested = CreateMemberAssignments(propertyExpression, wrapper.Nested);

                    foreach (var item in nested)
                    {
                        yield return item;
                    }
                }
                else
                {
                    var wrapperPropertyType = wrapperPropertyInfo.PropertyType;

                    if (!wrapperPropertyType.IsGenericType && (!wrapperPropertyType.IsClass || wrapperPropertyType.Equals(typeof(string))))
                    {
                        //for flat property
                        yield return CreateBind(wrapperPropertyInfo, propertyExpression);
                    }
                    else if (wrapperPropertyType.IsGenericType)
                    {
                        //for collections
                        yield return CreateSelectBind(wrapperPropertyInfo, propertyExpression, wrapper.Nested);
                    }
                    else
                    {
                        //for non collection navigation properties
                        yield return CreateClassBind(wrapperPropertyInfo, propertyExpression, wrapper.Nested);
                    }
                }
            }
        }

        static MemberAssignment CreateBind(PropertyInfo property, ParameterExpression paramExpr)
        {
            return Bind(property,
                MakeMemberAccess(paramExpr, property));
        }

        static MemberAssignment CreateSelectBind(PropertyInfo property, ParameterExpression paramExpr, IEnumerable<FieldWrapper> fieldWrappers)
        {
            var memberExpression = MakeMemberAccess(paramExpr, property);

            return CreateSelectBindInternal(property, paramExpr, memberExpression, fieldWrappers);
        }

        static MemberAssignment CreateClassBind(PropertyInfo property, ParameterExpression paramExpr, IEnumerable<FieldWrapper> fieldWrappers)
        {
            var propertyType = property.PropertyType;
            var classBinds = new List<MemberAssignment>();

            foreach (var wrapper in fieldWrappers)
            {
                var wrapperPropertyInfo = propertyType.GetProperty(wrapper.Name);

                if (wrapperPropertyInfo == null)
                    continue;

                var wrapperPropertyType = wrapperPropertyInfo.PropertyType;

                MemberAssignment propertyBind = null;

                if (!wrapperPropertyType.IsGenericType && (!wrapperPropertyType.IsClass || wrapperPropertyType.Equals(typeof(string))))
                {
                    //for flat property
                    propertyBind = CreateClassNestedBind(property, wrapperPropertyInfo, paramExpr);
                }
                else if (wrapperPropertyType.IsGenericType)
                {
                    //for collections
                    propertyBind = CreateClassNestedSelectBind(property, wrapperPropertyInfo, paramExpr, wrapper.Nested);
                }
                else
                {
                    //for non collection navigation properties
                    //TODO
                }

                if (propertyBind != null)
                {
                    classBinds.Add(propertyBind);
                }
            }

            return Bind(
                property,
                MemberInit(
                    New(
                        propertyType.GetConstructors().First()),
                    classBinds.ToArray()));
        }

        static MemberAssignment CreateClassNestedSelectBind(PropertyInfo rootPropertyInfo, PropertyInfo property, ParameterExpression paramExpr, IEnumerable<FieldWrapper> fieldWrappers)
        {
            var memberExpression = MakeMemberAccess(
                        MakeMemberAccess(
                            paramExpr,
                            rootPropertyInfo),
                        property);

            return CreateSelectBindInternal(property, paramExpr, memberExpression, fieldWrappers);
        }

        static MemberAssignment CreateSelectBindInternal(PropertyInfo property, ParameterExpression paramExpr, MemberExpression memberExpression, IEnumerable<FieldWrapper> fieldWrappers)
        {
            var actualCollectionType = property.PropertyType.GetGenericArguments().First();

            return Bind(
                property,
                Call(
                    typeof(Enumerable),
                    "ToList",
                    new Type[] {
                        actualCollectionType
                    },
                    Call(
                        typeof(Enumerable),
                        "Select",
                        new Type[] {
                            actualCollectionType,
                            actualCollectionType
                        },
                        memberExpression,
                        //new Lambda
                        CreateSelection(actualCollectionType, fieldWrappers)
                        ))
                );
        }

        static MemberAssignment CreateClassNestedBind(PropertyInfo rootPropertyInfo, PropertyInfo property, ParameterExpression paramExpr)
        {
            return Bind(
                property,
                MakeMemberAccess(
                    MakeMemberAccess(
                        paramExpr,
                        rootPropertyInfo),
                    property));
        }
    }
}
