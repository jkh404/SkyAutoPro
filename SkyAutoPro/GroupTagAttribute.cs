using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyAutoPro
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GroupTagAttribute: Attribute
    {
        public string Group { get; set; }

        public GroupTagAttribute( string group)
        {
            Group = group;
        }
    }
}
