# SkyAutoPro
简单的依赖注入框架
采用反射+特性的方式简单实现的依赖注入
样例
> 定义一个用户实体类
'''
    class User
    {
        [InTag("用户名")]
        public string Name { get; set; }
        [InTag("密码")]
        public string PassWord { get; set; }
        [InTag("年龄")]
        public int Age { get; set; }

        public User(string name, string passWord, int age)
        {
            Name = name;
            PassWord = passWord;
            Age = age;
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    
'''
> Program.cs
'''
        static void Main(string[] args)
        {
            AutoPro autoPro = new AutoPro();
            autoPro.Add("年龄", 18);
            autoPro.Add("用户名", "sky");
            autoPro.Add("密码", "skyqwe");
            autoPro.Add<User>("用户1");
            autoPro.Add<User>("用户2");
            Console.WriteLine($"用户1:{ autoPro.Get<User>("用户1")}");
            Console.WriteLine($"用户2:{ autoPro.Get<User>("用户2")}");
        }        
'''
更多的例子请查看 SkyAutoProTest 下的代码
