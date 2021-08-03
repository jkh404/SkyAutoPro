using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyAutoPro.Framework
{
    /// <summary>
    /// 未指定 GroupTag 特性错误
    /// </summary>
    public class NotFoundGroupTag : Exception
    {
        public NotFoundGroupTag(string message) : base(message)
        {

        }
    }
}
