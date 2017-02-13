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

            int itemNum = 0;

            List<String> priceList = new List<string>();
            // get price
            itemNum = 0;
            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//span"))
            {
                priceResult += node.InnerText.Trim();
                priceList.Add($"{itemNum} : " + node.InnerText.Trim());
                // create new sub List
                //item.Add(new List<String>());
                //item[itemNum].Add(node.InnerHtml.Trim());
                itemNum++;
            }
            priceList.ForEach(Console.WriteLine);
            //Console.WriteLine(priceResult);


            List<List<String>> item = new List<List<String>>();
            
            List<String> nodeList = new List<string>();
            // item-img will get double information , so we need to trim once.
            bool trimOverlap =  false;
            itemNum = 0;
            int currentNodeIndex = 0;
            int needItemIndex = 4;
            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//*[contains(@class,'item-img')]"))
            {
                if (currentNodeIndex > needItemIndex)
                {
                    if (!trimOverlap)
                    {
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
                            //add item price
                            item[itemNum].Add(priceList[itemNum + 16]);
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
                            //add item price
                            item[itemNum].Add(priceList[itemNum + 16]);
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
                } // end currentNodeIndex > needItemIndex
                else
                {
                    currentNodeIndex++;
                }
            } // end foreach



            //int index = 0;
            //foreach (HtmlNode node in document.DocumentNode.SelectNodes("//h3"))
            //{
            //    result[index++] = node.InnerHtml.Trim();
            //}



            // print 
            for (int i = 0; i < item.Count; i++)
            {
                for (int infoNum = 0; infoNum < item[i].Count; infoNum++)
                {
                    //item.Add(new List<String>());
                    //item[i].Add(priceList[infoNum + 16]);
                    Console.WriteLine($"[{i}] [{infoNum}] : " + item[i][infoNum]);
                }
            }

            Console.ReadLine();


        }// end of main


    }
}



 