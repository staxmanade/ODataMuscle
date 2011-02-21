using System;
using System.Collections.ObjectModel;
using System.Data.Services.Client;
using System.Linq;
using NUnit.Framework;

namespace ODataMuscle.Tests
{
    [TestFixture]
    public class ODataMuscleClientExtensionTests
    {
        private TestEntities _testEntities;
        private FooParent _parent;
        private FooChild _child;

        #region Simple Test Classes
        [global::System.Data.Services.Common.DataServiceKeyAttribute("Id")]
        public class FooParent
        {
            public int Id { get; set; }
            public FooChild Child { get; set; }
            public DataServiceCollection<FooChild> Children { get; set; }
            public Collection<FooChild> ChildrenAsCollection { get; set; }
        }

        [global::System.Data.Services.Common.DataServiceKeyAttribute("Id")]
        public class FooChild
        {
            public int Id { get; set; }
            public FooParent Parent { get; set; }
        }

        public class TestEntities : DataServiceContext
        {
            public TestEntities()
                : base(new Uri("http://localhost/NowhereThatShouldExist"))
            {
                Parents = base.CreateQuery<FooParent>("Parents");
            }


            public DataServiceQuery<FooChild> Childs { get; set; }
            public DataServiceQuery<FooParent> Parents { get; set; }
        }
        #endregion

        [SetUp]
        public void Setup()
        {
            _testEntities = new TestEntities();
            _parent = new FooParent();
            _child = new FooChild();
        }

        [Test]
        public void Should_expand_single_property()
        {
            var dataServiceQuery = _testEntities.Parents.Expand(x => x.Child);

            dataServiceQuery.UriShouldContain("$expand=Child");
        }

        [Test]
        public void Should_expand_single_properties_single_child_property()
        {
            var dataServiceQuery = _testEntities.Parents.Expand(x => x.Child.Parent);

            dataServiceQuery.UriShouldContain("$expand=Child/Parent");
        }

        [Test]
        public void Should_expand_single_properties_single_child_property_recursively()
        {
            var dataServiceQuery = _testEntities.Parents.Expand(x => x.Child.Parent.Child.Parent.Child.Parent.Child.Parent);

            dataServiceQuery.UriShouldContain("$expand=Child/Parent/Child/Parent/Child/Parent/Child/Parent");
        }

        [Test]
        public void Should_expand_nested_property_contained_in_a_collection_()
        {
            var dataServiceQuery = _testEntities.Parents.Expand(parent => parent.Children.Expand(child => child.Parent));

            dataServiceQuery.UriShouldContain("$expand=Children/Parent");
        }

        [Test]
        public void Should_expand_nested_property_contained_in_a_collection_X()
        {
            var dataServiceQuery = _testEntities.Parents.Expand(parent => parent.ChildrenAsCollection.Expand(child => child.Parent));

            dataServiceQuery.UriShouldContain("$expand=ChildrenAsCollection/Parent");
        }


        [Test]
        public void Should_be_able_to_AddLink()
        {
            _testEntities.AddObject("Parents", _parent);
            _testEntities.AddObject("Childs", _child);
            _testEntities.AddLink(_parent, x => x.Children, _child);
        }


        [Test]
        public void Should_be_able_to_DeleteLink()
        {
            _testEntities.AddObject("Parents", _parent);
            _testEntities.AddObject("Childs", _child);
            _testEntities.AddLink(_parent, x => x.Children, _child);

            _testEntities.DeleteLink(_parent, x => x.Children, _child);
        }


        [Test]
        public void Should_be_able_to_SetLink()
        {
            _testEntities.AddObject("Parents", _parent);
            _testEntities.AddObject("Childs", _child);
            _testEntities.SetLink(_parent, x => x.Child, _child);
        }


    }
}