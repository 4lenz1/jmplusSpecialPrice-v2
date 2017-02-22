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
                // trim everything but price 
                priceInfo = Regex.Replace(priceInfo, "[^0-9]", "");
                priceList.Add(priceInfo);
            }

            List<List<String>> item = new List<List<String>>();

            List<String> nodeList = new List<string>();
            // item-img will get double information , so we need to trim once.
            bool trimOverlap = false;
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

                        // add item link
                        item[itemNum].Add(nodeList[1].Replace("http://goods.ruten.com.tw/item/show?", ""));

                        // handle image link
                        string imageString = nodeList[3];
                        // change original image link s.jpg to m.jpg
                        imageString = string.Concat(imageString.Remove(imageString.Length - 6, 2));
                        // imageString.Remove(imageString.Length - 1, 1) + ",";
                        Console.WriteLine(imageString);
                        item[itemNum].Add(imageString);

                        // find title and trim 【門市展示機 "title"】
                        MatchCollection matches = Regex.Matches(nodeList[5], "【(?<item>[^\"]+)】", RegexOptions.IgnoreCase);
                        string matchString = "";
                        foreach (Match match in matches)
                        {
                            matchString = match.Groups["item"].Value.Replace("門市展示機", "").Trim().Replace(" ","");
                        }
                        // add item Title
                        item[itemNum].Add(matchString);
                        //add item price
                        item[itemNum].Add(priceList[itemNum + 16]);

                        // handle voriginal price
                        List<String> originalPriceList = new List<string>();
                        originalPriceList = nodeList[5].Split(' ').ToList();
                        // add original price
                        item[itemNum].Add(originalPriceList[originalPriceList.Count - 1].Replace("原價", ""));
                        // clear originalPriceList to get new value next time 
                        originalPriceList.Clear();

                        itemNum++;
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

            // print 
            Console.WriteLine();
            for (int i = 0; i < item.Count; i++)
            {
                Console.WriteLine($"{item[i][2],30}" +
                   $"{item[i][3],10}" +
                   $"{item[i][4], 10}" +
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


            program.InitialTable();
            program.ClearTable();
            program.InserTable(item);


            Console.ReadLine();



        }// end of main

        private void InitialTable()
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

        }
        private void ClearTable()
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("Item");


            // Construct the query operation for all customer entities where PartitionKey = "Smith".
            TableQuery <ItemEntity> query = new TableQuery<ItemEntity>()
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
        private void InserTable(List<List<string>> list)
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("Item");

            // Create a new customer entity.

            for (int NodeIndex = 0; NodeIndex < list.Count; NodeIndex++)
            {
                ItemEntity product = new ItemEntity(list[NodeIndex][2], "Item");

                product.itemID = list[NodeIndex][0];
                product.imageLink = list[NodeIndex][1];
                product.price = list[NodeIndex][3];
                product.originalPrice = list[NodeIndex][4];

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

        public string itemID { get; set; }

        public string imageLink { get; set; }

        public string price { get; set; }

        public string originalPrice { get; set; }

    }

}