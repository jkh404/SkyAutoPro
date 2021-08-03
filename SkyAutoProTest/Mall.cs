﻿using Newtonsoft.Json;
using SkyAutoPro;
using System.Collections.Generic;

namespace SkyAutoProTest
{
    /// <summary>
    /// 商场
    /// </summary>
    [OnlyOne("企鹅大商场")]
    internal class Mall
    {
        [InOnlyOneTag]
        public string name { get; set; }
        [InTag("商场内的商店")]
        public List<Store> stores { get; set; }
        [InTag("商场所在城市")]
        public string City { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,Formatting.Indented);
        }
    }
}