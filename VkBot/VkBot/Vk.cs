using System;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;

namespace VkBot
{
    class Vk
    {
        string ssid;
        public string llc;

        public string gHtml(string url, string postData) //Возвращает содержимое поданной страницы
        {
            string HTML = "";

            Regex rex1 = new Regex("remixsid=(.*?);", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            if (url == "0") return "0"; //Проверяем не получили ли мы ошибку
            HttpWebRequest mRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            //mRequest.Proxy = new WebProxy("127.0.0.1", 8888); //Если нужно то юзаем прокси ^_^
            if (!string.IsNullOrEmpty(postData)) mRequest.Method = "POST"; // устанавливаем метод общения с сайтом в POST
            mRequest.Referer = "https://vk.com";
            mRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";// Мою userAgent который я вытащил блогодоря httpanalyzer
            mRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg,image/pjpeg, application/x-shockwave-flash,application/vnd.ms-excel,application/vnd.ms-powerpoint,application/msword";
            mRequest.Headers.Add("Accept-Language", "ru");
            mRequest.ContentType = "application/x-www-form-urlencoded";
            mRequest.KeepAlive = false;

            // передаем Сookie, полученные в предыдущем запросе
            if (!string.IsNullOrEmpty(this.ssid))
            {
                llc = "remixchk=5;remixsid=" + ssid;
            }
            if (!string.IsNullOrEmpty(llc))
            {
                mRequest.Headers.Add(System.Net.HttpRequestHeader.Cookie, llc);
            }
            // ставим False, чтобы при получении кода 302, не делать 
            // автоматического перенаправления
            mRequest.AllowAutoRedirect = false;

            // передаем параметры
            string sQuerystring = postData;
            byte[] ByteArr = System.Text.Encoding.GetEncoding(1251).GetBytes(sQuerystring); //Вконтакте использует кирилическую кодировку
            try
            {
                if (!string.IsNullOrEmpty(postData))
                {
                    mRequest.ContentLength = ByteArr.Length;
                    mRequest.GetRequestStream().Write(ByteArr, 0, ByteArr.Length);
                };

                // делаем запрос
                HttpWebResponse mResponse = (HttpWebResponse)mRequest.GetResponse();
                StreamReader mReader;

                //Сохраняем Cookie 
                llc = string.IsNullOrEmpty(mResponse.Headers["Set-Cookie"]) ? "" : mResponse.Headers["Set-Cookie"];
                Match matc1 = rex1.Match(llc);

                //Если есть имя сессии, то подменяем Cookie 
                if (matc1.Groups.Count == 2) { this.ssid = matc1.Groups[1].ToString(); llc = "remixchk=5;ssid=" + this.ssid; }
                if (mResponse.Headers["Content-Type"].IndexOf("windows-1251") > 0)
                {
                    mReader = new StreamReader(mResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("windows-1251"));
                }
                else
                {
                    mReader = new StreamReader(mResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                }
                HTML = mReader.ReadToEnd();
                if (HTML == "") //Проверяем на редирект
                {
                    HTML = this.gHtml(mResponse.Headers["Location"].ToString(), "");

                }
            }
            catch (Exception err)
            {
                //Ошибка в чтении страницы
                return "0";
            }
            return HTML;
        }

        public int CheckAuth(string html)
        {
            int status;
            if (html.IndexOf("login?act=blocked") > 0) { status = 2; return status; } // Проверяем не заблокирован ли наш аккаунт
            if (html.IndexOf("onLoginFailed") > 0) { status = 3; return status; } // проверяем была ли попытка успешной
            Regex rex1 = new Regex("href=\"\\/edit\"", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            Match matc1 = rex1.Match(html);
            if (matc1.Groups[0].Length == 0) { status = 4; return status; } // Проверяем есть ли у нас возможность редактировать страницу
            status = 1; // если всё успешно то возвращаем 2
            return status;
        }

        public string CheckCaptch(string html)
        {

            Regex rex1 = new Regex("captcha_sid\\\":\\\"(\\d*)\\\"", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            Match matc1 = rex1.Match(html);
            if (matc1.Groups[1].Length == 0) return "0";
            return matc1.Groups[1].ToString(); // возвращаем ID капчи
        }

        public Image GetImg(string url, Image image) //Возвращает изображение
        {
            string postData = "";
            string HTML = "";
            HttpWebRequest mRequest =
            (HttpWebRequest)HttpWebRequest.Create(
            url);
            //mRequest.Proxy = new WebProxy("127.0.0.1", 8888);
            if (!String.IsNullOrEmpty(postData)) mRequest.Method = "POST";
            mRequest.Referer = "http://vk.com";
            mRequest.UserAgent = "Mozila/4.0 (compatible; MSIE 6.0;Windows NT 5.1; SV1; MyIE2;";
            mRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg,image/pjpeg, application/x-shockwave-flash,application/vnd.ms-excel,application/vnd.ms-powerpoint,application/msword";
            mRequest.Headers.Add("Accept-Language", "ru");
            mRequest.ContentType = "application/x-www-form-urlencoded";
            mRequest.KeepAlive = false;

            // передаем Cookie, полученные в предыдущем запросе 
            if (!String.IsNullOrEmpty(ssid))
            {
                llc = "remixchk=5;remixsid=" + ssid;
            }
            if (!String.IsNullOrEmpty(llc))
            {
                mRequest.Headers.Add(HttpRequestHeader.Cookie, llc);
            }
            // ставим False, чтобы при получении кода 302, не делать 
            // автоматического перенаправления
            mRequest.AllowAutoRedirect = true;

            // передаем параметры
            string sQueryString = postData;
            byte[] ByteArr = System.Text.Encoding.GetEncoding(1251).GetBytes(sQueryString);

            if (!String.IsNullOrEmpty(postData))
            {
                mRequest.ContentLength = ByteArr.Length;
                mRequest.GetRequestStream().Write(ByteArr, 0, ByteArr.Length);
            };

            // делаем запрос

            try
            {
                HttpWebResponse mResponse = (HttpWebResponse)mRequest.GetResponse();
                StreamReader mReader;


                if (mResponse.Headers["Content-Type"].IndexOf("windows-1251") > 0)
                {
                    mReader = new StreamReader(mResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("windows-1251"));
                }
                else
                {
                    mReader = new StreamReader(mResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                }
                image = Image.FromStream(mResponse.GetResponseStream());
                HTML = mReader.ReadToEnd();
                mResponse.Close();
                if (HTML == "") HTML = mResponse.Headers["Location"].ToString();
            }
            catch (Exception err)
            {
                //Ошибка в чтении страницы
                return image;
            }
            return image;
        }
    }
}
