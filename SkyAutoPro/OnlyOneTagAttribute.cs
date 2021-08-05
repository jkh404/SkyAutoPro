using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyAutoPro
{
    /// <summary>
    /// 用于标记单例类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class ,AllowMultiple = false)]
    public class OnlyOneTagAttribute: Attribute
    {
        /// <summary>
        /// 单例标签
        /// </summary>
        public string OnlyOne { get; set; }

        public OnlyOneTagAttribute(string tag)
        {
            OnlyOne = tag;
        }
    }
}
