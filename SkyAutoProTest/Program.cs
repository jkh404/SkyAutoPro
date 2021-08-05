using SkyAutoPro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace SkyAutoProTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AutoPro autoPro = new AutoPro();
            autoPro.Add("商场所在城市", "上海");
            autoPro.AddGroup<Store>("商店1");
            autoPro.AddGroup<Store>("商店2");
            autoPro.AddGroup<Store>("商店3");
            autoPro.AddGroup<Store>("商店4");
            autoPro.AddOne<Mall>();
            Console.WriteLine(autoPro.Get<Mall>());
            //商场搬到了北京
            Console.WriteLine("商场搬到了北京");
            autoPro.Update("商场所在城市", "北京");
            Console.WriteLine(autoPro.Get<Mall>());
        }   
    }       
}
