using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace VkBot
{
    public partial class Form1 : Form
    {
        string userid;
        Vk http = new Vk();
        string img;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string post = "email=" + textBox1.Text + "&pass=" + textBox2.Text + "&q=1&act=login&q=1&al_frame=1&from_host=vk.com&from_protocol=http&ip_h=4e78766a2890ac1115&quick_expire=1";// генерируем POST запрос
            string html = http.gHtml("https://vk.com/", "");
            html = http.gHtml("https://login.vk.com/?act=login", post); // Отправляем запрос на авторизацию
            Regex rex4 = new Regex("parent\\.onLoginDone\\(\'(.*?)\'\\)", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace); //Создаём правило для парсинга
            Match matc4 = rex4.Match(html);// Примерняем его для нашего ответа от VK,в котором должен хранится ID нашей страницы
            userid = matc4.Groups[1].ToString().Replace("/id", "");
            html = http.gHtml("https://vk.com/id" + userid, "");
            int stat = http.CheckAuth(html); // Проверяем всё ли успешно
            if (stat == 1) // если всё успешно
                panel1.BackColor = Color.Green;
        }

        public void Addcap(string sid)
        {
            this.img = "http://vk.com/captcha.php?sid=" + sid;
            Image image = http.GetImg(img, pictureBox1.Image);
            pictureBox1.Image = image;

        }
    }
}
