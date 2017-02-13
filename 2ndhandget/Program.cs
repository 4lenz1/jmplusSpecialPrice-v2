using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

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
            
            int itemNum = 0;

            List<String> priceList = new List<string>();
            // get price
            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//span"))
            {
                string priceInfo = node.InnerText.Trim();
                // 只保留數字
                priceInfo = Regex.Replace(priceInfo,"[^0-9]","");
                priceList.Add(priceInfo);
                //priceList.Add(node.InnerText.Trim(' ')
                //    .Replace("直","")
                //    .Replace("接", "")
                //    .Replace("購", "")
                //    .Replace("買", "")
                //    .Replace("價", "")
                //    .Replace("：", "")
                //    .Replace("元", "")
                //    .Replace(" ", "")
                //    .Replace("　", "")
                //    );
            }
            
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
                            for (int index = 1; index <= 5; index += 2)
                                // assign item  1(link) , 3(image) ,5(title) to 2D List
                                item[itemNum].Add(nodeList[index]);

                            //item[itemNum].Add(nodeList[5]);
                            //add item price
                            item[itemNum].Add(priceList[itemNum + 16]);
                        }
                        else
                        {
                            for (int index = 3; index <= 7; index += 2)
                                // assign item  3(link) , 5(image) ,7(title) to 2D List
                                item[itemNum].Add(nodeList[index]);
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