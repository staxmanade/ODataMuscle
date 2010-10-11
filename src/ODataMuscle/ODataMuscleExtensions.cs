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
            string propertyName = ExpandPropertyName(propertyExpressions);
            return entities.Expand(propertyName);
        }

        public static string Expand<T, TProperty>(this DataServiceCollection<T> collection, Expression<Func<T, TProperty>> propertyExpressions)
        {
            string propertyName = ExpandPropertyName(propertyExpressions);
            return propertyName;
        }

        private static string ExpandPropertyName(Expression exp)
        {
            string str = string.Empty;
            switch (exp.NodeType)
            {
                case ExpressionType.MemberAccess:
                    MemberExpression mx = (exp as MemberExpression);
                    str = ExpandPropertyName(mx.Expression);
                    str += (str.Length > 0 ? "/" : "") + mx.Member.Name;
                    break;
                case ExpressionType.Lambda:
                    str = ExpandPropertyName((exp as LambdaExpression).Body);
                    break;
                case ExpressionType.Call:
                    MethodCallExpression mcx = (exp as MethodCallExpression);
                    //this might have to iterate them right to left and not left to right for propery argument order
                    foreach (Expression arg in mcx.Arguments)
                    {
                        str += (str.Length > 0 ? "/" : "") + ExpandPropertyName(arg);
                    }
                    break;
                case ExpressionType.Quote:
                    UnaryExpression ux = (exp as UnaryExpression);
                    str = ExpandPropertyName(ux.Operand);
                    break;
            }
            return str;
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