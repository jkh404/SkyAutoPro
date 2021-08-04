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
        private Dictionary<string, KeyValuePair<List<string>, object>> 
            keyValueGroup = new Dictionary<string, KeyValuePair<List<string>,object>> ();
        private Dictionary<string, object>keyValues = new Dictionary<string, object>();
        private Dictionary<object,PropertyInfo> RegUpdate = new Dictionary<object,PropertyInfo>();
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
        public AutoPro Add<T>(string tag,T obj) 
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
                return this;
            }

        }
        /// <summary>
        /// 添加一个对象交由IOC容器进行实例化
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="tag">对象标签</param>
        /// <param name="action">回调动作</param>
        /// <exception cref="ArgumentNullException">Tag为空值错误</exception>
        /// <exception cref="Exception"></exception>
        public AutoPro Add<T>(string tag,Action<T> action = default(Action<T>))
        {
            lock (keyValues)
            {
                try
                {
                    T Instance = CreateInstance<T>(tag);
                    action?.Invoke(Instance);
                    keyValues.Add(tag, Instance);
                }
                catch (ArgumentNullException ANex)
                {
                    throw new ArgumentNullException($"参数 Tag 不能为空值", tag);
                }
                catch (Exception Aex)
                {
                    throw Aex;
                }
                return this;
            }
        }
        /// <summary>
        /// 添加一个单列对象交由IOC容器进行实例化
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <exception cref="NotFoundOnlyOneTag">没有找到OnlyOneTag错误</exception>
        /// <exception cref="ArgumentException">Tag重复错误</exception>
        public AutoPro AddOne<T>(Action<T> action=default(Action<T>))
        {
            lock (keyValues)
            {
                string tag = GetOnlyOneTag<T>();
                T Instance = CreateInstance<T>(tag);
                action?.Invoke(Instance);
                CheckAndAddOne<T>(tag, Instance);
                return this;
            }
        }
        /// <summary>
        /// 添加一个现有单列对象到IOC容器
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">现有对象</param>
        /// <exception cref="NotFoundOnlyOneTag">没有找到OnlyOneTag错误</exception>
        /// <exception cref="ArgumentException">Tag重复错误</exception>
        public AutoPro AddOne<T>(T obj)
        {
            lock (keyValues)
            {
                string tag = GetOnlyOneTag(obj);
                CheckAndAddOne<T>(tag,obj);
                return this;
            }
        }
        private void CheckAndAddOne<T>(string tag,T obj)
        {
            if (tag == null) throw new NotFoundOnlyOneTag("没有找到OnlyOneTag");
            try
            {
                keyValues.Add(tag, obj);
            }
            catch (ArgumentException Aex)
            {
                throw new ArgumentException($"Tag:{tag}已存在", tag);
            }
        }
        /// <summary>
        /// 将一个对象并添加到列表内
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="objTag">对象标签</param>
        /// <param name="obj">对象</param>
        /// <exception cref="ArgumentNullException">Tag为空值错误</exception>
        public void AddGroup<T>(string objTag, T obj)
        {
            lock (keyValues)
            {
                
                try
                {
                    string GroupTag = GetGroupTag<T>();
                    bool IsHas = keyValueGroup.ContainsKey(GroupTag);
                    if (!IsHas)
                    {
                        CreateListAndAdd<T>(objTag,obj);
                    }
                    else
                    {
                        GetListAndAdd<T>(objTag,obj);
                    }

                }
                catch (ArgumentNullException ANEx)
                {

                    throw new ArgumentException($"Tag:{objTag}不能为空值", objTag);
                }
            }
        }
        /// <summary>
        /// 由IOC容器实列化一个对象并添加到列表内
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="objTag">列表标签</param>
        /// <exception cref="ArgumentNullException">Tag为空值错误</exception>
        public void AddGroup<T>(string objTag)
        {
            lock (keyValues)
            {
                string groupTag = GetGroupTag<T>();
                try
                {
                    
                    bool IsHas = keyValueGroup.ContainsKey(groupTag);
                    if (groupTag == null)
                        throw new NotFoundGroupTag($"{typeof(T).FullName}未指定 GroupTag 特性");
                    T obj = CreateInstance<T>(objTag);
                    if (!IsHas)
                    {
                        CreateListAndAdd<T>(objTag, obj);
                    }
                    else
                    {
                        GetListAndAdd<T>(objTag, obj);
                    }
                }
                catch (ArgumentNullException ANEx)
                {

                    throw new ArgumentException($"Tag:{nameof(groupTag)}不能为空值", groupTag);
                }
            }
        }
        private void CreateListAndAdd<T>(string objTag,T obj)
        {

            List<T> list = new List<T>();
            List<string> tags = new List<string>();
            list.Add(obj);
            tags.Add(objTag);
            var Group = new KeyValuePair<List<string>, object>(tags, list);
            keyValueGroup.Add(GetGroupTag<T>(), Group);

        }
        private void GetListAndAdd<T>(string objTag,T obj)
        {
            string groupTag= GetGroupTag<T>();
            var Group= keyValueGroup.GetValueOrDefault(groupTag);
            List<T> list = (List<T>)Group.Value;
            if (list!=null)
            {
                if (Group.Key.Contains(objTag)) throw new ArgumentException($"Tag:{objTag}已存在", objTag);
                Group.Key.Add(objTag);
                list.Add(obj);
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
                string tag = GetOnlyOneTag<T>();
                return CheckAndUpdate<object>(tag, newObj);
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
                
                string tag = GetOnlyOneTag(newObj);
                return CheckAndUpdate<object>(tag,newObj);
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
                return CheckAndUpdate<T>(tag, newObj);
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
                return CheckAndUpdate <object>(tag, newObj);
            }
        }
        private bool CheckAndUpdate<T>(string tag, T newObj)
        {
            bool isOk = true;
            if (tag == null) return !isOk;
            object findObj = keyValues.GetValueOrDefault(tag);
            if (findObj == null) return !isOk;
            keyValues[tag] = newObj;
            RegUpdate.Where(u => {
                InTagAttribute inTag = u.Value.GetCustomAttribute<InTagAttribute>();
                if (inTag?.OldTag != null) return inTag.OldTag == tag;
                else return inTag?.Tag == tag;
            }).ToList().ForEach((u)=> {
                InTagAttribute inTag = u.Value.GetCustomAttribute<InTagAttribute>();
                inTag.UpdateData(u.Key,u.Value, newObj);
                //u.Value.GetSetMethod()?.Invoke(u.Key,new object[] {newObj});
            });
            return isOk;
        }
        /// <summary>
        /// 获取类的GroupTag值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>GroupTag</returns>
        public static string GetGroupTag<T>()
        {
            return GetGroupTagAttribute<T>()?.Group;
        }
        private static GroupTagAttribute GetGroupTagAttribute<T>()
        {
            GroupTagAttribute groupTag = (GroupTagAttribute)typeof(T)
               .GetCustomAttributes(true)
               .ToList().Find(m => m.GetType() == typeof(GroupTagAttribute));
            return groupTag;
        }
        /// <summary>
        /// 获取类的OnlyOneTag值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>GroupTag</returns>
        public static string GetOnlyOneTag<T>()
        {
            return GetOnlyOneAttribute(typeof(T))?.Tag;
        }

        /// <summary>
        /// 获取对象类的OnlyTag值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>OnlyTag</returns>
        public static string GetOnlyOneTag<T>(T obj)
        {
            return GetOnlyOneAttribute(obj.GetType())?.Tag;
        }
        private static OnlyOneAttribute GetOnlyOneAttribute(Type type)
        {
            OnlyOneAttribute OnlyOne = (OnlyOneAttribute)(type
                .GetCustomAttributes(true)
                .ToList().Find(m => m.GetType() == typeof(OnlyOneAttribute)));
            return OnlyOne;
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
        public T FillInstance<T>(T instance, string objTag = null)
        {
            if (instance == null) return instance;
            List<PropertyInfo> properties= instance.GetType().GetProperties().ToList();
            List<FieldInfo> fields= instance.GetType().GetFields().ToList();
            foreach (var property in properties)
            {
                object obj = null;
                InTagAttribute inTag = property.GetCustomAttribute<InTagAttribute>();
                if (inTag==null) continue;//过滤
                if (inTag.Update==true) RegUpdate.Add(instance,property);
                string realTag = (inTag.OldTag != null ? inTag.OldTag : inTag.Tag);
                if (realTag == null)
                {
                    obj = objTag;
                }
                else if(keyValues.ContainsKey(realTag))
                {
                    obj = keyValues.GetValueOrDefault(realTag);
                }
                else if(keyValueGroup.ContainsKey(realTag))
                {
                    obj = keyValueGroup.GetValueOrDefault(realTag).Value;
                }
                //property.GetSetMethod()?.Invoke(instance, new object[] { obj });
                inTag.UpdateData(instance,property,obj);
            }
            foreach (var field in fields)
            {
                object obj = null;
                InTagAttribute inTag = field.GetCustomAttribute<InTagAttribute>();
                if (inTag == null) continue;//过滤
                if (inTag.OldTag != null) obj = keyValues.GetValueOrDefault(inTag.OldTag);
                else if (inTag.Tag != null) obj = keyValues.GetValueOrDefault(inTag.Tag);
                else obj = objTag;
                field.SetValue(instance, obj);
            }
            return instance;
        }
        /// <summary>
        /// 对类进行实例化，并进行构造注入
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <returns>对象</returns>
        public T CreateInstance<T>(string objTag=null)
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
            return FillInstance((T)constructor.Invoke(objs), objTag);
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
        /// 获取列表
        /// </summary>
        /// <typeparam name="T">列表元素类型</typeparam>
        /// <returns>列表对象</returns>
        public List<T> GetList<T>()
        {
            string groupTag = GetGroupTag<T>();
            try
            {
                if (keyValueGroup.ContainsKey(groupTag)) return (List<T>)keyValueGroup.GetValueOrDefault(groupTag).Value;
                else return null;
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentException($"Tag:{nameof(groupTag)}不能为空值", groupTag);
            }
        }
        /// <summary>
        /// 获取列表内对象
        /// </summary>
        /// <typeparam name="T">列表元素类型</typeparam>
        /// <returns></returns>
        public T GetListObj<T>(string objTag)
        {
            return GetListObj<T>(GetGroupTag<T>(),objTag);
        }
        /// <summary>
        /// 获取列表内对象
        /// </summary>
        /// <typeparam name="T">列表元素类型</typeparam>
        /// <param name="groupTag">列表Tag</param>
        /// <param name="objTag">对象Tag</param>
        /// <returns></returns>
        public T GetListObj<T>(string groupTag,string objTag)
        {
            try
            {
                //string groupTag = GetGroupTag<T>();
                if (keyValueGroup.ContainsKey(groupTag))
                {
                    var data = keyValueGroup.GetValueOrDefault(groupTag);
                    int index = data.Key.FindIndex(m => m == objTag);
                    if (index > -1) return ((List<T>)data.Value)[index];
                    return default(T);
                }
                else
                {
                    return default(T);
                }
            }
            catch (NotFoundGroupTag)
            {
                throw new NotFoundGroupTag($"{typeof(T).FullName}未指定 GroupTag 特性");
            }
        }
        /// <summary>
        /// 根据tag标签获取或修改对象
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>对象</returns>
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
        /// <summary>
        /// 根据type获取对象集合
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <returns>对象集合</returns>
        public List<object> this[Type type]
        {
            get
            {
                return keyValues.Select(u => u.Value).Where(u => u.GetType() == type).ToList();
            }
        }
        /// <summary>
        /// 获取容器中的所有对象
        /// </summary>
        /// <returns>所有对象</returns>
        public List<object> GetAll()
        {
            return keyValues.Select(u => u.Value).ToList();
        }
        public void Dispose()
        {
            keyValues.Clear();
        }
    }
}
