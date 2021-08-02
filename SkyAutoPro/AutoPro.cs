using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SkyAutoPro
{
    public  class AutoPro:IDisposable
    {
        private Dictionary<string, object> keyValues = new Dictionary<string, object>();
        public  void Add<T>(string tag,T obj) 
        {
            lock (keyValues)
            {
                try
                {
                    keyValues.Add(tag, obj);
                }
                catch (ArgumentException Aex)
                {

                    throw new ArgumentException($"Tag:{tag}已存在",tag);
                }
            }
        }
        public  void Add<T>(string tag)
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
        public  void AddOne<T>()
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
        public  void AddOne<T>(T obj)
        {
            lock (keyValues)
            {
                string tag = GetTag(obj);
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
                        list.Add(obj);
                    }
                }
                catch (ArgumentNullException ANEx)
                {

                    throw new ArgumentException($"Tag:{tag}不能为空值", tag);
                }
            }
        }
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
        public void AddList<T>()
        {
            lock (keyValues)
            {
                string group = GetGroup<T>();
                try
                {
                    
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
                catch (ArgumentNullException ANEx)
                {

                    throw new ArgumentException($"Group:{group}不能为空值", group);
                }
            }
        }
        public static string GetGroup<T>()
        {
            GroupTagAttribute groupTag = (GroupTagAttribute)typeof(T)
               .GetCustomAttributes(true)
               .ToList().Find(m => m.GetType() == typeof(GroupTagAttribute));
            return groupTag?.Group;
        }
        public static string GetTag<T>()
        {
            OnlyOneAttribute OnlyOne = (OnlyOneAttribute)typeof(T)
                .GetCustomAttributes(true)
                .ToList().Find(m => m.GetType() == typeof(OnlyOneAttribute));
            return OnlyOne?.Tag;
        }
        public static string GetTag(object obj)
        {
            OnlyOneAttribute OnlyOne = (OnlyOneAttribute)obj.GetType()
                .GetCustomAttributes(true)
                .ToList().Find(m => m.GetType() == typeof(OnlyOneAttribute));
            return OnlyOne?.Tag;
        }
        public  T FillInstance<T>(T instance)
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
        public  T CreateInstance<T>()
        {

            ConstructorInfo constructor = typeof(T).GetConstructors().Single();
            List<ParameterInfo> parameter = constructor.GetParameters().ToList();
            object[] objs= parameter.Select(u =>
            {
                InTagAttribute inTag = u.GetCustomAttribute<InTagAttribute>();
                if (inTag == null) return null;
                else return Get(inTag.Tag);
            }).ToArray();
            if (objs == null) objs = new object[0];
            return FillInstance((T)constructor.Invoke(objs));
        }
        public  object Get(string tag)
        {
            lock (keyValues)
            {
                return keyValues.GetValueOrDefault(tag);
            }
        }
        public  object Get<T>()
        {
            lock (keyValues)
            {
                foreach (var item in keyValues)
                {
                    if (item.Value.GetType() == typeof(T)) return item.Value;
                }
                return null;
            }

        }
        public List<T> GetList<T>(string tag)
        {
            if (keyValues.ContainsKey(tag))return (List<T>)keyValues.GetValueOrDefault(tag);
            else return null;
        }
        public List<T> GetList<T>()
        {
            string tag = GetGroup<T>();
            if (keyValues.ContainsKey(tag))return (List<T>)keyValues.GetValueOrDefault(tag);
            else return null;
        }

        public void Dispose()
        {
            keyValues.Clear();
        }
    }
}
