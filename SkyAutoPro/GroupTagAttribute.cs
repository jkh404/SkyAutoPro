using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyAutoPro
{
    /// <summary>
    /// 分组标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GroupTagAttribute: Attribute
    {
        /// <summary>
        /// 分组Tag
        /// </summary>
        public string Group { get; set; }

        public GroupTagAttribute( string groupTag)
        {
            Group = groupTag;
        }
    }
}
