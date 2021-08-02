using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SkyAutoPro
{
    public class NotFoundOnlyOneTag : Exception
    {
        public NotFoundOnlyOneTag(string message) : base(message)
        {
        }

        protected NotFoundOnlyOneTag(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
