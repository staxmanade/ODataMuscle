using System;
using System.Data.Services.Client;
using NUnit.Framework;

namespace ODataMuscle.Tests
{

	[TestFixture]
	public class StaticallyTypedEntitiesHelperTests
	{
		public class FooParent
		{
			public FooChild Child { get; set; }
			public DataServiceCollection<FooChild> Children { get; set; }
		}

		public class FooChild
		{
			public FooParent Parent { get; set; }
		}

		public class TestEntities : DataServiceContext
		{
			public TestEntities()
				: base(new Uri("http://localhost/NowhereThatShouldExist"))
			{
				Parents = base.CreateQuery<FooParent>("Parents");
			}

			public DataServiceQuery<FooParent> Parents { get; set; }

		}

		[Test]
		public void Should_expand_single_property()
		{
			var testEntities = new TestEntities();
			var dataServiceQuery = testEntities.Parents.Expand(x => x.Child);
			dataServiceQuery.UriShouldContain("$expand=Child");
		}

		[Test]
		public void Should_expand_single_properties_single_child_property()
		{
			var testEntities = new TestEntities();
			var dataServiceQuery = testEntities.Parents.Expand(x => x.Child.Parent);
			dataServiceQuery.UriShouldContain("$expand=Child/Parent");
		}

		[Test]
		public void Should_expand_nested_property_contained_in_a_collection_()
		{
			var testEntities = new TestEntities();
			var dataServiceQuery = testEntities.Parents.Expand(parent => parent.Children.Expand(child => child.Parent));
			dataServiceQuery.UriShouldContain("$expand=Children/Parent");
		}

	}

	internal static class StaticDataServiceQueryAssertionHelpers
	{
		public static void UriShouldContain(this DataServiceQuery query, string expected)
		{
			query.RequestUri.ToString().ShouldContain(expected);
		}
	}

}
