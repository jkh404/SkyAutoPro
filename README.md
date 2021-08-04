# SkyAutoPro
简单的依赖注入框架
采用反射+特性的方式简单实现的依赖注入
样例
> 定义一个商店实体类和一个商场单列类

```c#
    [GroupTag("商场内的商店")]
    internal class Store
    {
        [InTag]
        public string Name { get;private set; }

        [InTag("商店所在城市",OldTag = "商场所在城市", Update =true)] 
        public string City { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,Formatting.Indented);
        }
    }
    [OnlyOne("企鹅大商场")]
    internal class Mall
    {
        [InTag]
        public string Name { get;private set; }
        [InTag("商场内的商店")]
        public List<Store> stores { get; set; }
        [InTag("商场所在城市",Update =true)]
        public string City { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,Formatting.Indented);
        }
    }
```
```c#
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
```
输出结果:
```json
{
  "Name": "企鹅大商场",
  "stores": [
    {
      "Name": "商店1",
      "City": "上海"
    },
    {
      "Name": "商店2",
      "City": "上海"
    },
    {
      "Name": "商店3",
      "City": "上海"
    },
    {
      "Name": "商店4",
      "City": "上海"
    }
  ],
  "City": "上海"
}
商场搬到了北京
{
  "Name": "企鹅大商场",
  "stores": [
    {
      "Name": "商店1",
      "City": "北京"
    },
    {
      "Name": "商店2",
      "City": "北京"
    },
    {
      "Name": "商店3",
      "City": "北京"
    },
    {
      "Name": "商店4",
      "City": "北京"
    }
  ],
  "City": "北京"
}

```

更多的例子请查看 SkyAutoProTest 下的代码

