using Newtonsoft.Json;
using SkyAutoPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyAutoProTest
{
    [OnlyOne("超级管理员")]
    [GroupTag("用户表")]
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
}
