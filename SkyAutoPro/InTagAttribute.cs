using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkyAutoPro
{
    /// <summary>
    /// 无参数将默认置入Tag值
    /// </summary>
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
        /// 别名标签
        /// </summary>
        public string OldTag { get; set;}
        /// <summary>
        /// 是否启用更新。为true时Ioc容器里的对象更改时，此次绑定的属性成员将会响应更新。
        /// </summary>
        public bool Update { get; set; } = false;

        public InTagAttribute(string tag=null)
        {
            Tag = tag;
        }
        /// <summary>
        /// 更新成员属性
        /// </summary>
        /// <typeparam name="T">成员类型</typeparam>
        /// <param name="instance">成员</param>
        /// <param name="property">属性</param> 
        /// <param name="newData">新值</param>
        public void UpdateData<T>(T instance, PropertyInfo property,object newData)
        {
            property.GetSetMethod(true).Invoke(instance, new object[] { newData });
        }
        /// <summary>
        /// 更新成员字段
        /// </summary>
        /// <typeparam name="T">成员类型</typeparam>
        /// <param name="instance">成员</param>
        /// <param name="field">字段</param>
        /// <param name="newData"></param>
        internal void UpdateData<T>(T instance, FieldInfo field, object newData)
        {
            field.SetValue(instance, newData);
        }
    }
}
