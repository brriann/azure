using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace ConnectToDBApp
{
   class Program
   {
      static string databaseName = "dbName";
      static string collectionName = "collectionName";
      static DocumentClient dc;

      static string endpoint = "https://cosmosUrl.documents.azure.com:443/";
      static string key = "accessKey";

      static void Main(string[] args)
      {
         dc = new DocumentClient(new Uri(endpoint), key);

         InsertOp("john", "doe");
         InsertOp("gretchen", "miller");

         QueryOp();

         Console.WriteLine("\n\n");
         Console.WriteLine("Press any key to end");
         Console.ReadKey();
      }

      static void InsertOp(string first, string last)
      {
         EmployeeEntity employee = new EmployeeEntity
         {
            FirstName = first,
            LastName = last
         };

         var result = dc.CreateDocumentAsync(
            UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
            employee
         ).GetAwaiter().GetResult();
      }

      static void QueryOp()
      {
         FeedOptions queryOptions = new FeedOptions
         {
            MaxItemCount = -1,
            EnableCrossPartitionQuery = true
         };

         IQueryable<EmployeeEntity> query = dc.CreateDocumentQuery<EmployeeEntity>(
            UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
            .Where(e => e.LastName == "miller");

         Console.WriteLine("staff from miller fam:");
         foreach (EmployeeEntity employee in query)
         {
            Console.Write(employee);
         }
      }
   }

   public class EmployeeEntity
   {
      public string FirstName { get; set; }
      public string LastName { get; set; }

      public override string ToString()
      {
         return JsonConvert.SerializeObject(this);
      }
   }
}
