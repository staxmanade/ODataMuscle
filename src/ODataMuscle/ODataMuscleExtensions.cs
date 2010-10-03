using System;
using System.Data.Services.Client;
using System.Linq.Expressions;
using System.Text;

namespace ODataMuscle
{
    public static class ODataMuscleExtensions
    {
        public static DataServiceQuery<T> Expand<T, TProperty>(this DataServiceQuery<T> entities, Expression<Func<T, TProperty>> propertyExpressions)
        {
            string propertyName = propertyExpressions.GetMemberName();
            return entities.Expand(propertyName);
        }

        public static string Expand<T, TProperty>(this DataServiceCollection<T> collection, Expression<Func<T, TProperty>> propertyExpressions)
        {
            string propertyName = propertyExpressions.GetMemberName();
            return propertyName;
        }

        private static string GetMemberName<T, TProperty>(this Expression<Func<T, TProperty>> propertyExpression)
        {
            var lambdaExpression = propertyExpression.Body as MethodCallExpression;
            if (lambdaExpression != null)
            {
                dynamic innerExpression = lambdaExpression.Arguments[1];
                var innerProperty = innerExpression.Operand;
                var ipBody = innerProperty.Body;

                var x = ipBody.Member.Name;
                dynamic firstArg = lambdaExpression.Arguments[0];
                return firstArg.Member.Name + "/" + x.ToString();
            }
            var node = propertyExpression.Body as MemberExpression;
            var stringBuilder = new StringBuilder();
            var buildName = BuildName(node, stringBuilder);
            return buildName.ToString();
        }

        private static StringBuilder BuildName(MemberExpression exp, StringBuilder stringBuilder)
        {
            if (exp != null)
            {
                BuildName(exp.Expression as MemberExpression, stringBuilder);
                stringBuilder.Append(exp.Member.Name);
                stringBuilder.Append("/");
            }

            return stringBuilder;
        }
    }

    //// copied from http://blogs.msdn.com/b/stuartleeks/archive/2008/09/15/dataservicequery-t-expand.aspx
    //public static class DataServiceQueryExtensions
    //{
    //    public static DataServiceQuery<TSource> Expand<TSource, TPropType>(this DataServiceQuery<TSource> source, Expression<Func<TSource, TPropType>> propertySelector)
    //    {
    //        string expandString = BuildString(propertySelector);
    //        return source.Expand(expandString);
    //    }
    //    private static string BuildString(Expression propertySelector)
    //    {
    //        switch (propertySelector.NodeType)
    //        {
    //            case ExpressionType.Lambda:
    //                LambdaExpression lambdaExpression = (LambdaExpression)propertySelector;
    //                return BuildString(lambdaExpression.Body);

    //            case ExpressionType.Quote:
    //                UnaryExpression unaryExpression = (UnaryExpression)propertySelector;
    //                return BuildString(unaryExpression.Operand);

    //            case ExpressionType.MemberAccess:
    //                MemberInfo propertyInfo = ((MemberExpression)propertySelector).Member;
    //                return propertyInfo.Name;

    //            case ExpressionType.Call:
    //                MethodCallExpression methodCallExpression = (MethodCallExpression)propertySelector;
    //                if (IsSubExpand(methodCallExpression.Method)) // check that it's a SubExpand call
    //                {
    //                    // argument 0 is the expression to which the SubExpand is applied (this could be member access or another SubExpand)
    //                    // argument 1 is the expression to apply to get the expanded property
    //                    // Pass both to BuildString to get the full expression
    //                    return BuildString(methodCallExpression.Arguments[0]) + "/" +
    //                           BuildString(methodCallExpression.Arguments[1]);
    //                }
    //                // else drop out and throw
    //                break;
    //        }
    //        throw new InvalidOperationException("Expression must be a member expression or an SubExpand call: " + propertySelector.ToString());

    //    }

    //    private static readonly MethodInfo[] SubExpandMethods;
    //    static DataServiceQueryExtensions()
    //    {
    //        Type type = typeof(DataServiceQueryExtensions);
    //        SubExpandMethods = type.GetMethods().Where(mi => mi.Name == "SubExpand").ToArray();
    //    }
    //    private static bool IsSubExpand(MethodInfo methodInfo)
    //    {
    //        if (methodInfo.IsGenericMethod)
    //        {
    //            if (!methodInfo.IsGenericMethodDefinition)
    //            {
    //                methodInfo = methodInfo.GetGenericMethodDefinition();
    //            }
    //        }
    //        return SubExpandMethods.Contains(methodInfo);
    //    }

    //    public static TPropType SubExpand<TSource, TPropType>(this Collection<TSource> source, Expression<Func<TSource, TPropType>> propertySelector)
    //        where TSource : class
    //        where TPropType : class
    //    {
    //        throw new InvalidOperationException("This method is only intended for use with DataServiceQueryExtensions.Expand to generate expressions trees"); // no actually using this - just want the expression!
    //    }
    //    public static TPropType SubExpand<TSource, TPropType>(this TSource source, Expression<Func<TSource, TPropType>> propertySelector)
    //        where TSource : class
    //        where TPropType : class
    //    {
    //        throw new InvalidOperationException("This method is only intended for use with DataServiceQueryExtensions.Expand to generate expressions trees"); // no actually using this - just want the expression!
    //    }
    //}

}