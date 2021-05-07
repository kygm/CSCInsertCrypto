using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using System.Dynamic;
using Amazon.Lambda.APIGatewayEvents;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CSCInsertCrypto
{
    public class Coin
    {
        public string id { get; set; }
        public string rank { get; set; }
        public string symbol { get; set; }
        public string supply { get; set; }
        public double priceUsd { get; set; }

        public Coin(string i, string r, string sy, string su, double pri)
        {
            id = i;
            rank = r;
            symbol = sy;
            supply = su;
            priceUsd = pri;
        }
    }

    public class Function
    {

        private static AmazonDynamoDBClient cli = new AmazonDynamoDBClient();
        private static string tableName = "FinalAssignment";
        public async Task<List<dynamic>> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {

            HttpClient client = new HttpClient();
            dynamic obj = new ExpandoObject();
            //top cryptos api endpoint  
            HttpResponseMessage response = client.GetAsync("https://api.coincap.io/v2/assets").Result;
            response.EnsureSuccessStatusCode();
            string result = response.Content.ReadAsStringAsync().Result;

            
            var expConverter = new Newtonsoft.Json.Converters.ExpandoObjectConverter();
            obj = JsonConvert.DeserializeObject<ExpandoObject>(result, expConverter);


            Table coins = Table.LoadTable(cli, tableName);

            
            var json = JsonConvert.SerializeObject(obj.data);


            List<Coin> lCoins = JsonConvert.DeserializeObject<List<Coin>>(json);


            PutItemOperationConfig config = new PutItemOperationConfig();

            foreach (Coin c in lCoins)
            {
                Console.WriteLine(c.id);
                Document res =
                    await coins.PutItemAsync(Document.FromJson(JsonConvert.SerializeObject(c)), config);
            }


            return obj.data;
        }
    }
}
