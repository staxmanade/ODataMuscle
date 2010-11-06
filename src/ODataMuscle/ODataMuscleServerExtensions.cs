using System;
using System.Data.Services;
using System.Linq.Expressions;

namespace ODataMuscle
{
    public static class ODataMuscleServerExtensions
    {
        public static FluentDataServiceConfiguration<TEntityContext> ToFluentConfig<TEntityContext>(this IDataServiceConfiguration configuration)
        {
            return new FluentDataServiceConfiguration<TEntityContext>(configuration);
        }
    }

    public class FluentDataServiceConfiguration<TEntityContext>
    {
        private readonly IDataServiceConfiguration _config;

        public FluentDataServiceConfiguration(IDataServiceConfiguration config)
        {
            _config = config;
        }

        public IDataServiceConfiguration Config { get { return _config; } }

        public FluentDataServiceConfiguration<TEntityContext> SetEntitySetAccessRule(string name, EntitySetRights rights)
        {
            _config.SetEntitySetAccessRule(name, rights);
            return this;
        }

        public FluentDataServiceConfiguration<TEntityContext> SetEntitySetAccessRule<TProperty>(Expression<Func<TEntityContext, TProperty>> propertyExpression, EntitySetRights rights)
        {
            var propName = GetExpressionMemberName(propertyExpression);
            _config.SetEntitySetAccessRule(propName, rights);
            return this;
        }

        //public FluentDataServiceConfiguration<TEntityContext> SetServiceOperationAccessRule(Expression<Action<TEntityContext>> propertyExpression, ServiceOperationRights rights)
        //{
        //    var propName = GetExpressionMemberName(propertyExpression);
        //    _config.SetServiceOperationAccessRule(propName, rights);
        //    return this;
        //}

        private static string GetExpressionMemberName<TProperty>(Expression<Func<TEntityContext, TProperty>> propertyExpression)
        {
            return ((MemberExpression)(propertyExpression.Body)).Member.Name;
        }

        public static implicit operator DataServiceConfiguration(FluentDataServiceConfiguration<TEntityContext> fluentConfig)
        {
            return (DataServiceConfiguration)fluentConfig._config;
        }
    }
}