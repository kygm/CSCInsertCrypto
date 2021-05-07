using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using CSCInsertCrypto;

namespace CSCInsertCrypto.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void CheckPrice()
        {

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var fx = new APIGatewayProxyRequest();
            var upperCase = function.FunctionHandler(fx, context);
            Coin coin = new Coin("etherium", "2", "553333020.1122", "ETH", 3500);
            Assert.Equal(3500, coin.priceUsd);
        }
    }
}
