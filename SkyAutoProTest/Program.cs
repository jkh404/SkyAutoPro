using SkyAutoPro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

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
            autoPro.Add("商场所在城市", "上海");
            //autoPro.Add<Store>("玩具商店");
           // Console.WriteLine(autoPro["玩具商店"]);

            //autoPro.Add("商场所在城市", "上海");
            autoPro.AddGroup<Store>("商店1");
            autoPro.AddGroup<Store>("商店2");
            autoPro.AddGroup<Store>("商店3");
            autoPro.AddGroup<Store>("商店4");
            autoPro.AddOne<Mall>();
            Console.WriteLine(autoPro.Get<Mall>());
            //商场搬到北京
            autoPro.Update("商场所在城市", "北京");
            Console.WriteLine(autoPro.Get<Mall>());
        }   
    }       
}
