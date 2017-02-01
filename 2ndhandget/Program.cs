using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace _2ndhandget
{
    class Program
    {
        public static void Main(string[] args)
        {
            
                        HttpWebRequest request;

                        request = HttpWebRequest.CreateHttp("http://class.ruten.com.tw/user/index00.php?s=jmplus01&d=4797150&o=0&m=1");
                        request.CookieContainer = new CookieContainer();


                       // WebResponse response;
                        var response = request.GetResponse();

                        var stream = response.GetResponseStream();

            var document = new HtmlDocument();
            document.Load(stream, Encoding.UTF8);
            var node = document.DocumentNode.SelectSingleNode("//body");

            if (node != null)
            {
                Console.WriteLine(node.InnerText);
            }
            else
            {
                Console.WriteLine("NOOOOO QQ");
            }
            
            Console.ReadLine();
            
        }


    }
}



