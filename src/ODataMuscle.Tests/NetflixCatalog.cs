using System;

namespace ODataMuscle.Tests.Netflix
{
    public partial class NetflixCatalog
    {
        public NetflixCatalog()
            : this(new Uri("http://odata.netflix.com/catalog"))
        {
        }
    }
}