using Newtonsoft.Json;
using SkyAutoPro;

namespace SkyAutoProTest
{
    [GroupTag("商场内的商店")]
    internal class Store
    {
        [InTag("商店所在城市",OldTag = "商场所在城市", Update =true)]
        public string City { get; set; }

    }
}