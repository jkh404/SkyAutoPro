using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SkyAutoPro
{
    public sealed class AutoPro:IDisposable
    {
        private Dictionary<string, object> keyValues = new Dictionary<string, object>();
        public AutoPro()
        {
            keyValues.Add("AutoPro",this);
        }
        /// <summary>
        /// 添加一个现有对象到IOC容器
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="tag">对象标签</param>
        /// <param name="obj">现有对象</param>
        /// <exception cref="ArgumentNullException">Tag为空值错误</exception>
        /// <exception cref="ArgumentException">Tag重复错误</exception>
        public void Add<T>(string tag,T obj) 
        {
            
            lock (keyValues)
            {
                try
                {

                    keyValues.Add(tag, obj);
                }
                catch (ArgumentNullException ANex)
                {
                    throw new ArgumentException($"参数 Tag 不能为空值", tag);
                }
                catch (ArgumentException Aex)
                {

                    throw new ArgumentException($"Tag:{tag}已存在",tag);
                }
            }
        }
        /// <summary>
        /// 添加一个对象交由IOC容器进行实例化
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="tag">对象标签</param>
        /// <exception cref="ArgumentNullException">Tag为空值错误</exception>
        /// <exception cref="ArgumentException">Tag重复错误</exception>
        public void Add<T>(string tag)
        {
            lock (keyValues)
            {
                try
                {
                    keyValues.Add(tag, CreateInstance<T>());
                }
                catch (ArgumentNullException ANex)
                {
                    throw new ArgumentException($"参数 Tag 不能为空值", tag);
                }
                catch (ArgumentException Aex)
                {
                    throw new ArgumentException($"Tag:{tag}已存在", tag);
                }
            }
        }
        /// <summary>
        /// 添加一个单列对象交由IOC容器进行实例化
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <exception cref="NotFoundOnlyOneTag">没有找到OnlyOneTag错误</exception>
        /// <exception cref="ArgumentException">Tag重复错误</exception>
        public void AddOne<T>()
        {
            lock (keyValues)
            {
                string tag = GetTag<T>();
                if (tag == null)throw new NotFoundOnlyOneTag("没有找到OnlyOneTag");
                try
                {
                    keyValues.Add(tag, CreateInstance<T>());
                }
                catch (ArgumentException Aex)
                {
                    throw new ArgumentException($"Tag:{tag}已存在", tag);
                }
            }
        }
        /// <summary>
        /// 添加一个现有单列对象到IOC容器
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">现有对象</param>
        /// <exception cref="NotFoundOnlyOneTag">没有找到OnlyOneTag错误</exception>
        /// <exception cref="ArgumentException">Tag重复错误</exception>
        public void AddOne<T>(T obj)
        {
            lock (keyValues)
            {
                string tag = GetOnlyTag(obj);
                if (tag == null)throw new NotFoundOnlyOneTag("没有找到OnlyOneTag");
                try
                {
                    keyValues.Add(tag, obj);
                }
                catch (ArgumentException Aex)
                {
                    throw new ArgumentException($"Tag:{tag}已存在", tag);
                }
            }
        }
        /// <summary>
        /// 将一个对象并添加到列表内
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="tag">列表标签</param>
        /// <param name="obj">对象</param>
        /// <exception cref="ArgumentNullException">Tag为空值错误</exception>
        public void AddList<T>(string tag, T obj)
        {
            lock (keyValues)
            {
                
                try
                {
                    bool IsHas = keyValues.ContainsKey(tag);
                    if (!IsHas)
                    {
                        List<T> list = new List<T>();
                        list.Add(obj);
                        keyValues.Add(tag, list);
                    }
                    else
                    {
                        List<T> list = (List<T>)keyValues.GetValueOrDefault(tag);
                        list?.Add(obj);
                    }
                }
                catch (ArgumentNullException ANEx)
                {

                    throw new ArgumentException($"Tag:{tag}不能为空值", tag);
                }
            }
        }
        /// <summary>
        /// 由IOC容器实列化一个对象并添加到列表内
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="tag">列表标签</param>
        /// <exception cref="ArgumentNullException">Tag为空值错误</exception>
        public void AddList<T>(string tag)
        {
            lock (keyValues)
            {
                try
                {
                    bool IsHas = keyValues.ContainsKey(tag);
                    if (!IsHas)
                    {
                        List<T> list = new List<T>();
                        list.Add(CreateInstance<T>());
                        keyValues.Add(tag, list);
                    }
                    else
                    {
                        List<T> list = (List<T>)keyValues.GetValueOrDefault(tag);
                        list.Add(CreateInstance<T>());
                    }
                }
                catch (ArgumentNullException ANEx)
                {

                    throw new ArgumentException($"Tag:{tag}不能为空值", tag);
                }
            }
        }
        /// <summary>
        /// 由IOC容器实列化一个对象并添加到列表内。对象的类需指定 GroupTag 特性
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <exception cref="NotFoundGroupTag">未指定 GroupTag 特性错误</exception>
        public void AddList<T>()
        {
            lock (keyValues)
            {
                string group = GetGroup<T>();
                if (group == null)
                    throw new NotFoundGroupTag($"{typeof(T).FullName}未指定 GroupTag 特性");
                bool IsHas = keyValues.ContainsKey(group);
                if (!IsHas)
                {
                    List<T> list = new List<T>();
                    list.Add(CreateInstance<T>());
                    keyValues.Add(group, list);
                }
                else
                {
                    List<T> list = (List<T>)keyValues.GetValueOrDefault(group);
                    list.Add(CreateInstance<T>());
                }
            }
        }
        /// <summary>
        /// 更新IOC容器内的对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="newObj">新的对象</param>
        /// <returns>修改是否成功</returns>
        public bool Update<T>(T newObj)
        {
            lock (keyValues)
            {
                bool isOk = true;
                string tag = GetTag<T>();
                if (tag == null) return !isOk;
                object findObj = keyValues.GetValueOrDefault(tag);
                if (findObj == null) return !isOk;
                keyValues.Remove(tag);
                keyValues.Add(tag, newObj);
                return isOk;
            }
        }
        /// <summary>
        /// 更新IOC容器内的对象
        /// </summary>
        /// <param name="newObj">新的对象</param>
        /// <returns>修改是否成功</returns>
        public bool Update(object newObj)
        {
            lock (keyValues)
            {
                bool isOk = true;
                string tag = GetOnlyTag(newObj);
                if (tag == null) return !isOk;
                object findObj = keyValues.GetValueOrDefault(tag);
                if (findObj == null) return !isOk;
                keyValues.Remove(tag);
                keyValues.Add(tag, newObj);
                return isOk;
            }
        }
        /// <summary>
        /// 更新IOC容器内的对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="tag">对象标签</param>
        /// <param name="newObj">新的对象</param>
        /// <returns>修改是否成功</returns>
        public bool Update<T>(string tag,T newObj)
        {

            lock (keyValues)
            {
                bool isOk = true;
                if (tag == null) return !isOk;
                object findObj = keyValues.GetValueOrDefault(tag);
                if (findObj == null) return !isOk;
                keyValues.Remove(tag);
                keyValues.Add(tag, newObj);
                return isOk;
            }
        }
        /// <summary>
        /// 更新IOC容器内的对象
        /// </summary>
        /// <param name="tag">对象标签</param>
        /// <param name="newObj">新的对象</param>
        /// <returns>修改是否成功</returns>
        public bool Update(string tag,object newObj)
        {
            lock (keyValues)
            {
                bool isOk = true;
                if (tag == null) return !isOk;
                object findObj = keyValues.GetValueOrDefault(tag);
                if (findObj == null) return !isOk;
                keyValues.Remove(tag);
                keyValues.Add(tag, newObj);
                return isOk;
            }
        }
        /// <summary>
        /// 获取类的GroupTag值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>GroupTag</returns>
        public static string GetGroup<T>()
        {
            GroupTagAttribute groupTag = (GroupTagAttribute)typeof(T)
               .GetCustomAttributes(true)
               .ToList().Find(m => m.GetType() == typeof(GroupTagAttribute));
            return groupTag?.Group;
        }
        /// <summary>
        /// 获取类的GroupTag值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>GroupTag</returns>
        public static string GetTag<T>()
        {
            OnlyOneAttribute OnlyOne = (OnlyOneAttribute)typeof(T)
                .GetCustomAttributes(true)
                .ToList().Find(m => m.GetType() == typeof(OnlyOneAttribute));
            return OnlyOne?.Tag;
        }
        /// <summary>
        /// 获取对象类的OnlyTag值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>OnlyTag</returns>
        public static string GetOnlyTag(object obj)
        {
            OnlyOneAttribute OnlyOne = (OnlyOneAttribute)obj.GetType()
                .GetCustomAttributes(true)
                .ToList().Find(m => m.GetType() == typeof(OnlyOneAttribute));
            return OnlyOne?.Tag;
        }
        /// <summary>
        /// 获取对象的Tag值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>Tag</returns>
        public string GetTag(object obj)
        {
            foreach (var keyValue in keyValues)
            {
                if (keyValue.Value == obj) return keyValue.Key;
            }
            return null;
        }
        /// <summary>
        /// 对实例化对象的属性，字段进行注入
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="instance">实例化对象</param>
        /// <returns>注入后的实例化对象</returns>
        public T FillInstance<T>(T instance)
        {
            if (instance == null) return instance;
            List<PropertyInfo> properties= instance.GetType().GetProperties().ToList();
            List<FieldInfo> fields= instance.GetType().GetFields().ToList();
            foreach (var property in properties)
            {
                InTagAttribute inTag = property.GetCustomAttribute<InTagAttribute>();
                if (inTag==null)break;
                object obj = null;
                if (inTag.GroupTag != null)
                {
                    obj= keyValues.GetValueOrDefault(inTag.GroupTag);
                }
                else
                {
                    obj=keyValues.GetValueOrDefault(inTag.Tag);
                }
                property.SetValue(instance, obj);
            }
            foreach (var field in fields)
            {
                InTagAttribute inTag = field.GetCustomAttribute<InTagAttribute>();
                if (inTag==null)break;
                object obj = null;
                if (inTag.GroupTag != null)
                {
                    obj = keyValues.GetValueOrDefault(inTag.GroupTag);
                }
                else
                {
                    obj = keyValues.GetValueOrDefault(inTag.Tag);
                }
                field.SetValue(instance, obj);
            }
            return instance;
        }
        /// <summary>
        /// 对类进行实例化，并进行构造注入
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <returns>对象</returns>
        public T CreateInstance<T>()
        {
            ConstructorInfo constructor = typeof(T).GetConstructors().Single();
            List<ParameterInfo> parameter = constructor.GetParameters().ToList();
            object[] objs= parameter.Select(u =>
            {
                InTagAttribute inTag = u.GetCustomAttribute<InTagAttribute>();
                if (inTag==null)
                {
                    List<object> list = keyValues.Select(m => m.Value).ToList();
                    var obj= list.Where(m =>m.GetType() == u.ParameterType).FirstOrDefault();
                    if (obj == null) return default(T);
                    else return obj;
                }
                else return Get(inTag.Tag);
            }).ToArray();
            if (objs == null) objs = new object[0];
            return FillInstance((T)constructor.Invoke(objs));
        }
        /// <summary>
        /// 根据标签获取对象
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>对象</returns>
        public object Get(string tag)
        {
            return keyValues.GetValueOrDefault(tag);
        }
        /// <summary>
        /// 根据标签获取对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="tag">对象标签</param>
        /// <returns>对象</returns>
        public T Get<T>(string tag)
        {
            return (T)keyValues.GetValueOrDefault(tag);
        }
        /// <summary>
        /// 获取IOC容器中的单列对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>()
        {
            foreach (var item in keyValues)
            {
                if (item.Value.GetType() == typeof(T)) return (T)item.Value;
            }
            return default(T);
        }
        /// <summary>
        /// 根据标签获取列表对象
        /// </summary>
        /// <typeparam name="T">列表元素类型</typeparam>
        /// <param name="tag">标签</param>
        /// <returns>列表对象</returns>
        public List<T> GetList<T>(string tag)
        {
            try
            {
                if (keyValues.ContainsKey(tag)) return (List<T>)keyValues.GetValueOrDefault(tag);
                else return null;
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentException($"Tag:{tag}不能为空值", tag);
            }
        }
        /// <summary>
        /// 获取列表对象
        /// </summary>
        /// <typeparam name="T">列表元素类型</typeparam>
        /// <returns></returns>
        public List<T> GetList<T>()
        {
            try
            {
                string tag = GetGroup<T>();
                if (keyValues.ContainsKey(tag)) return (List<T>)keyValues.GetValueOrDefault(tag);
                else return null;
            }
            catch (NotFoundGroupTag)
            {
                throw new NotFoundGroupTag($"{typeof(T).FullName}未指定 GroupTag 特性");
            }

        }
        /// <summary>
        /// 根据tag标签访问或修改对象
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns></returns>
        public object this[string tag]
        {
            get
            {
                return Get(tag);
            }
            set
            {
                Update(tag,value);
            }
        }
        
        public void Dispose()
        {
            keyValues.Clear();
        }
    }
}
