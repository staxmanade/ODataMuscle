using System.Data.Services.Client;

namespace ODataMuscle.Tests
{
    internal static class StaticDataServiceQueryAssertionHelpers
    {
        public static void UriShouldContain(this DataServiceQuery query, string expected)
        {
            query.RequestUri.ToString().ShouldContain(expected);
        }
    }

}
