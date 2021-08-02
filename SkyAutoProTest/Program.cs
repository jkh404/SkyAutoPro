using SkyAutoPro;
using System;
using System.Diagnostics;

namespace SkyAutoProTest
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    AutoPro autoPro = new AutoPro();
        //    int count = 100000;
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();
        //    autoPro.Add("年龄", 18);
        //    autoPro.Add("用户名", "sky");
        //    autoPro.Add("密码", "skyqwe");
        //    for (int i = 0; i < count; i++)
        //    {
        //        autoPro.Add<User>($"用户{i}");
        //        Console.WriteLine($"用户{i}:{autoPro.Get($"用户{i}")}");
        //    }
        //    sw.Stop();
        //    Console.WriteLine($"AutoPro 添加{count}个用户耗时{sw.ElapsedMilliseconds}");
        //    sw.Restart();
        //    for (int i = 0; i < count; i++)
        //    {
        //        var user=new User("sky", "skyqwe", 18);
        //    }
        //    sw.Stop();
        //    Console.WriteLine($"new 添加{count}个用户耗时{sw.ElapsedMilliseconds}");
        //}
        static void Main(string[] args)
        {
            AutoPro autoPro = new AutoPro();
            autoPro.Add("年龄", 18);
            autoPro.Add("用户名", "sky");
            autoPro.Add("密码", "skyqwe");
            //autoPro.AddOne<User>();
            //autoPro.AddOne(new User("sky", "skyqwe", 18));//重复添加将报错
            //autoPro.AddOne<User>();//重复添加将报错
            autoPro.AddList<User>();
            autoPro.AddList<User>();
            autoPro.AddList<User>();
            //autoPro.AddList("用户表",new User("qwe", "qwe123", 18));
            //autoPro.AddList<User>("用户表");
            var users= autoPro.GetList<User>("用户表");
            //var users2= autoPro.GetList<User>();
            //Console.WriteLine(users.Count);
            autoPro.Add<Tool>("工具");
            Console.WriteLine(autoPro.Get<Tool>());
            //Console.WriteLine($"{AutoPro.GetTag<User>()}:{autoPro.Get<User>()}");
        }   
    }       
}
