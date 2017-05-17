using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types

namespace _2ndhandget
{
    class Program
    {
        public static void Main(string[] args)
        {
            //ruten url ID
            string[] categoryID = new string[] { "4802204", "4797150", "4797150" };

            List<List<String>> item = new List<List<String>>();


            bool trimOverlap;

            // index of product list 
            int itemNum = 0;
            int priceNum = 0;
            int currentNodeIndex;
            int needItemIndex = 4;
            List<String> nodeList = new List<string>();
            for (int index = 0; index < categoryID.Length; index++)
            {
               
                HttpWebRequest request;
                 string url;
                if (index == 2)
                {
                    url = "http://" + $"class.ruten.com.tw/user/index00.php?s=jmplus01&d={categoryID[index]}&o=0&m=1" + "&p=2";
                }
                else
                {
                    url = "http://" + $"class.ruten.com.tw/user/index00.php?s=jmplus01&d={categoryID[index]}&o=0&m=1";
                }
               
                request = WebRequest.CreateHttp(url);
                request.CookieContainer = new CookieContainer();


                // WebResponse response;
                var response = request.GetResponse();

                var stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(response.GetResponseStream());

                // Console.WriteLine(sr.ReadToEnd().Trim());

                string sourceCode = sr.ReadToEnd().Trim();

                Console.WriteLine(sourceCode);
                var document = new HtmlDocument();

                document.LoadHtml(sourceCode);



                List<String> priceList = new List<string>();
                // get price
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//span"))
                {
                    string priceInfo = node.InnerText.Trim();
                    // trim everything but price 
                    priceInfo = Regex.Replace(priceInfo, "[^0-9]", "");
                    priceList.Add(priceInfo);
                    Console.WriteLine(index + " : " + priceInfo);
                }


                //only need data > 4
                currentNodeIndex = 0;
                // item-img will get double information , so we need to trim once.
                trimOverlap = false;



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

                            // add item link
                            item[itemNum].Add(nodeList[1].Replace("http://goods.ruten.com.tw/item/show?", ""));

                            // handle image link
                            string imageString = nodeList[3];
                            // change original image link s.jpg to m.jpg
                            imageString = string.Concat(imageString.Remove(imageString.Length - 6, 2));
                            // imageString.Remove(imageString.Length - 1, 1) + ",";
                            Console.WriteLine(imageString);
                            item[itemNum].Add(imageString);


                            // find title and trim 【門市展示機 , 特價品 ,福利品"title"】
                            MatchCollection matches = Regex.Matches(nodeList[5], "【(?<item>[^\"]+)】", RegexOptions.IgnoreCase);

                            string matchString = "";

                            foreach (Match match in matches)
                            {
                                matchString = match.Groups["item"].Value.Replace("門市展示機", "")
                                    .Replace("福利品", "")
                                    .Replace("特價品", "")
                                    /*.Trim().Replace(" ", "")*/;
                            }


                            // add item Title
                            item[itemNum].Add(matchString);
                            Console.WriteLine(matchString);
                            //add item price

                            item[itemNum].Add(priceList[priceNum+ 18]);

                            // handle voriginal price
                            List<String> originalPriceList = new List<string>();
                            originalPriceList = nodeList[5].Split(' ').ToList();
                            // add original price
                            item[itemNum].Add(originalPriceList[originalPriceList.Count - 1].Replace("原價", ""));
                            // clear originalPriceList to get new value next time 
                            originalPriceList.Clear();

                            itemNum++;
                            priceNum++;
                            // clear nodeList to get new node 
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
                // reset priceNum
                priceNum = 0; 
            } // end categoryID for loop 

            // print 
            Console.WriteLine();
            Console.WriteLine(item.Count);

            for (int i = 0; i < item.Count; i++)
            {
                Console.WriteLine($"{i} : {item[i][2]} :" +
                     $"特價 {item[i][3]}" +
                     $"原價 {item[i][4]}" +
                     "\n");
            }


            //for (int i = 0; i < item.Count; i++)
            //{
            //    for (int infoNum = 0; infoNum < item[i].Count; infoNum++)
            //    {
            //        Console.Write(item[i][infoNum]);
            //    }
            //}


            Program program = new Program();

            var table =  program.GetTableInfo();
            //program.InitialTable();
            program.ClearTable(table);
            program.InserTable(item , table);


            Console.ReadLine();



        }// end of main

        private CloudTable  GetTableInfo()
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("Item");

            // Create the table if it doesn't exist.
            //table.CreateIfNotExists();
            if (table.CreateIfNotExists())
            {
                Console.WriteLine("table created.");
            }
            else
            {
                Console.WriteLine("table exist");
            }
            return table;
        }

        private void InitialTable()
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("SpecialPrice");

            // Create the table if it doesn't exist.
            //table.CreateIfNotExists();
            if (table.CreateIfNotExists())
            {
                Console.WriteLine("table created.");
            }
            else
            {
                Console.WriteLine("table exist");
            }

        }
        private void ClearTable( CloudTable table )
        {
         
            // Construct the query operation for all customer entities where PartitionKey = "Smith".
            TableQuery<ItemEntity> query = new TableQuery<ItemEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, "Item"));


            // Loop through the results, displaying information about the entity.
            foreach (ItemEntity entity in table.ExecuteQuery(query))
            {
                //Console.WriteLine("{0}, {1}", entity.PartitionKey, entity.RowKey);
                var entityToDelete = new DynamicTableEntity(entity.PartitionKey, entity.RowKey);
                entityToDelete.ETag = "*";

                table.Execute(TableOperation.Delete(entityToDelete));
            }

            Console.WriteLine("table cleared");
        }
        private void InserTable(List<List<string>> list, CloudTable table)
        {

            // Create a new customer entity.

            for (int NodeIndex = 0; NodeIndex < list.Count; NodeIndex++)
            {
                ItemEntity product = new ItemEntity(NodeIndex.ToString(), "Item");

                product.ItemID = list[NodeIndex][0];
                product.ImageLink = list[NodeIndex][1];
                product.Price = list[NodeIndex][3];
                product.OriginalPrice = list[NodeIndex][4];
                product.ProductName = list[NodeIndex][2];
                // Create the TableOperation object that inserts the customer entity.   
                TableOperation insertOperation = TableOperation.Insert(product);

                // Execute the insert operation.
                table.Execute(insertOperation);
                
            }
            Console.WriteLine("table inserted");
        }
    }

    public class ItemEntity : TableEntity
    {
        public ItemEntity(string title, string rowKey)
        {
            this.PartitionKey = title;
            this.RowKey = rowKey;
        }

        public ItemEntity() { }

        // now title is partitionkey
        // public string title { get; set; }

        public string ItemID { get; set; }

        public string ImageLink { get; set; }

        public string Price { get; set; }

        public string OriginalPrice { get; set; }

        public string ProductName { get; set; }

    }

}