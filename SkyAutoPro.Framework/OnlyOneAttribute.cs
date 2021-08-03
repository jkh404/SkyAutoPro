using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyAutoPro.Framework
{
    [AttributeUsage(AttributeTargets.Class ,AllowMultiple = false)]
    public class OnlyOneAttribute: Attribute
    {
        public string Tag { get; set; }

        public OnlyOneAttribute(string tag)
        {
            Tag = tag;
        }
    }
}
