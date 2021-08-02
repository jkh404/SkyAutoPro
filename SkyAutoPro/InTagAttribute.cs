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
        public string Tag { get; set;}
        public string GroupTag { get; set;}

        public InTagAttribute(string tag,string groupTag=null)
        {
            Tag = tag;
            GroupTag = groupTag;
        }
    }
}
