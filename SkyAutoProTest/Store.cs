using Newtonsoft.Json;
using SkyAutoPro;

namespace SkyAutoProTest
{
    [GroupTag("商场内的商店")]
    internal class Store
    {
        [InTag]
        public int Name { get; set; }

        [InTag("商店所在城市",OldTag = "商场所在城市", Update =true)] 
        public string City { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,Formatting.Indented);
        }
    }
}