using System;
using System.Collections.Generic;
using System.Data.Services;
using NUnit.Framework;
using ODataMuscle.Tests.Netflix;

namespace ODataMuscle.Tests
{
    [TestFixture]
    public class ODataMuscleServerExtensionTests
    {
        [Test]
        public void FluentSetEntitySetAccessRule()
        {
            var fakeConfig = new FakeDataServiceConfiguration();

            fakeConfig.ToFluentConfig<NetflixCatalog>()
                .SetEntitySetAccessRule("*", EntitySetRights.AllRead)
                .SetEntitySetAccessRule(x => x.Genres, EntitySetRights.All)
                .SetEntitySetAccessRule(x => x.People, EntitySetRights.WriteAppend);

            fakeConfig.EntityAccessRules.ContainsKey("*").ShouldBeTrue();
            fakeConfig.EntityAccessRules.ContainsKey("Genres").ShouldBeTrue();
            fakeConfig.EntityAccessRules.ContainsKey("People").ShouldBeTrue();
        }

        [Test]
        public void Should_be_able_to_implicitly_cast_a_DataServiceConfiguration()
        {
            DataServiceConfiguration origin = null;
            FluentDataServiceConfiguration<NetflixCatalog> fluent = origin.ToFluentConfig<NetflixCatalog>();

            origin = fluent;

            origin.ShouldBeNull();
        }


        public class FakeDataServiceConfiguration : IDataServiceConfiguration
        {
            public IDictionary<string, EntitySetRights> EntityAccessRules { get; private set; }
            public IDictionary<string, ServiceOperationRights> ServiceOperationAccessRules { get; private set; }

            public FakeDataServiceConfiguration()
            {
                EntityAccessRules = new Dictionary<string, EntitySetRights>();
                ServiceOperationAccessRules = new Dictionary<string, ServiceOperationRights>();
            }


            public void SetEntitySetAccessRule(string name, EntitySetRights rights)
            {
                EntityAccessRules.Add(name, rights);
            }

            public void SetServiceOperationAccessRule(string name, ServiceOperationRights rights)
            {
                ServiceOperationAccessRules.Add(name, rights);
            }

            #region NotImplemented

            public void RegisterKnownType(Type type)
            {
                throw new NotImplementedException();
            }

            public int MaxBatchCount
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public int MaxChangesetCount
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public int MaxExpandCount
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public int MaxExpandDepth
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public int MaxResultsPerCollection
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public int MaxObjectCountOnInsert
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public bool UseVerboseErrors
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }
            #endregion
        }
    }
}