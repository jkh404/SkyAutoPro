using Newtonsoft.Json;
using SkyAutoPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyAutoProTest
{
    /// <summary>
    /// 工具
    /// </summary>
    class Tool
    {
        [InTag("工具所属的用户集", GroupTag = "用户表")]
        public List<User> users;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
