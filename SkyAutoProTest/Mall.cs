using Newtonsoft.Json;
using SkyAutoPro;
using System.Collections.Generic;

namespace SkyAutoProTest
{
    /// <summary>
    /// 商场
    /// </summary>
    [OnlyOneTag("企鹅大商场")]
    internal class Mall
    {
        [InTag]
        public string Name { get;private set; }
        [InTag("商场内的商店")]
        public List<Store> stores { get; set; }
        [InTag("商场所在城市",Update =true)]
        public string City { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,Formatting.Indented);
        }
    }
}