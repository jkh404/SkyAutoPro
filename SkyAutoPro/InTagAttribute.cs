using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyAutoPro
{
    [AttributeUsage(AttributeTargets.Parameter|
  AttributeTargets.Field |
  AttributeTargets.Property,
  AllowMultiple = false)]
    public class InTagAttribute: Attribute
    {
        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set;}
        /// <summary>
        /// 组标签
        /// </summary>
        public string OldTag { get; set;}
        /// <summary>
        /// 是否启用更新。为true时Ioc容器里的对象更改时，此次绑定的属性成员将会响应更新。
        /// </summary>
        public bool Update { get; set; } = false;

        public InTagAttribute(string tag,string oldTag=null)
        {
            Tag = tag;
            OldTag = oldTag;
        }
    }
}
