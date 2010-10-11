using System;
using System.Data.Services.Client;
using System.Linq.Expressions;

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
}