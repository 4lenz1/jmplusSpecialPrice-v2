using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;

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
            StreamReader sr = new StreamReader(response.GetResponseStream());

           // Console.WriteLine(sr.ReadToEnd().Trim());

            string sourceCode = sr.ReadToEnd().Trim();

           // Console.WriteLine(sourceCode);
            var document = new HtmlDocument();

            document.LoadHtml(sourceCode);

            
            var titleXPath = "//h3";
            var priceXPath = "";

            var titleNode = document.DocumentNode.SelectNodes(titleXPath);
            var titleResult = "";
            var priceResult = "";
            var imgResult = "";

            //foreach (HtmlNode node in document.DocumentNode.SelectNodes("//a"))
            //{
            //    result += node.InnerHtml;
            //}

            //var result[] = "";
            string[] result = new string[100];
            //rt-store-goods-disp-mix

 
            //List<List<String>> item = new List<List<String>>(); //Creates new nested List
            //item.Add(new List<String>()); //Adds new sub List
            List<List<String>> item = new List<List<String>>();
            
            //item[0].Add("2349"); //Add values to the sub List at index 0
            //item[0].Add("The Prime of Your Life");
            //item[0].Add("Daft Punk");
            //item[0].Add("Human After All");
            //item[0].Add("3");
            //item[0].Add("2");

            //item[0] = str.Split('"').ToList();

            // listStrLineElements.ForEach(Console.WriteLine);

            List<String> nodeList = new List<string>();
            // item-img will get double information , so we need to trim once.
            bool trimOverlap =  false;
            int itemNum = 0;
            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//*[contains(@class,'item-img')]"))
            {
                if(! trimOverlap) { 
                    // get html and import in list to split 
                    nodeList = node.InnerHtml.Trim().Split('"').ToList();

                    // create new sub List
                    item.Add(new List<String>());

                    if (nodeList.Count == 11)
                    {
                        // assign item  1 , 3 ,5 to 2D List
                        //add item link
                        item[itemNum].Add(nodeList[1]);
                        //add item image
                        item[itemNum].Add(nodeList[3]);
                        //add item title
                        item[itemNum].Add(nodeList[5]);
                    }
                    else
                    {
                        // assign item  1 , 3 ,5 to 2D List
                        //add item link
                        item[itemNum].Add(nodeList[3]);
                        //add item image
                        item[itemNum].Add(nodeList[5]);
                        //add item title
                        item[itemNum].Add(nodeList[7]);
                    }
                    itemNum++;
                    //imgResult += node.InnerHtml.Trim(' ');
                    nodeList.Clear();
                    trimOverlap = true;

                    } // end if of trim double information
                else
                {
                    trimOverlap = false;
                }
            } // end foreach


            for( int i = 4; i< item.Count; i++)
            {
                for (int infoNum = 0; infoNum < 3; infoNum++)
                {
                    Console.WriteLine($"[{i}] [{infoNum}] : " + item[i][infoNum]);
                }
            }


            //Console.WriteLine(infoList[3]);
            //for (int i=0; i < infoList.Count; i ++)
            //    Console.WriteLine(i + " : " +infoList[i]);

         // item[0].ForEach(Console.WriteLine );

           // Console.WriteLine(imgResult);

            int index = 0;
            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//h3"))
            {
                result[index++] = node.InnerHtml.Trim();
            }

            //*[@id="mod-store-main-1"]/div[3]/div[3]/div[2]/div[1]/div[1]/p[1]/span


            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//span"))
            {
                priceResult += node.InnerText.Trim();
            }

        //    Console.WriteLine(priceResult);


            // var allLinksWithDivAndClass = document.DocumentNode.SelectNodes("//*[@class=\"float\"]");
            //*[contains(@class,'rt-store-goods-disp-mix')]
            //img[@class='item-img']
            //foreach (HtmlNode node in document.DocumentNode.SelectNodes("//*[@class,'item-img']"))
            //{
            //    imgResult += node.InnerText.Trim();
            //}

            //Console.WriteLine("img Result" + imgResult);


            if (result != null)
            {

               // result.ToList().ForEach(Console.WriteLine);
                //Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine("Nothing Found");
            }


         



            //var root = document.DocumentNode;
            //var p = root.Descendants()
            //    .Where(n => n.GetAttributeValue("class", "").Equals("item-img-wrap"))
            //    .Single()
            //    .Descendants("img")
            //    .Single();
            //var content = p.InnerText;



            //Console.WriteLine(content);
            //        var findclasses = document.DocumentNode
            //.Descendants("div")
            //.Where(d =>
            //    d.Attributes.Contains("class")
            //        &&
            //    d.Attributes["class"].Value.Contains("item-img-wrap")
            //);
            //        Console.WriteLine(findclasses);

          
            //var trimImg = "";
            //var trimDoc = new HtmlDocument();
            //trimDoc.LoadHtml(imgResult);
           
            //foreach (HtmlNode node in trimDoc.DocumentNode.SelectNodes("//a"))
            //{

            //     trimImg += node.InnerHtml;
            //}

            //Console.WriteLine("trim : " + trimImg);




            //string str = "<img src=\"http://e.rimg.com.tw/s1/3/a0/93/21702113118355_503_s.jpg\" alt=\"「JM-PLUS 加煒電子」【門市展示機 ATH-WS99 】耳罩式耳機 原價10000\" title=\"「JM-PLUS 加煒電子」【門市展示機 ATH-WS99】耳罩式耳機 原價10000\" border=\"0\">";
            //List<String> listStrLineElements;
            //listStrLineElements = str.Split('"').ToList();

            //listStrLineElements.ForEach(Console.WriteLine);

            Console.ReadLine();


        }// end of main


    }
}



 