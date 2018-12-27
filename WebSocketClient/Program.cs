using System;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;
using Auctus.DomainObjects.Exchange;
using Newtonsoft.Json;
using Auctus.DataAccess.Core;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WebSocketClient
{
    class Program
    {
        static string AuctusApiAuthToken;
        static string AuctusApiUrl;
        static void Main(string[] args)
        {
            Configure();
            using (var ws = new WebSocket("wss://stream.binance.com:9443/ws/!ticker@arr"))
            {
                ws.OnMessage += (sender, e) => ReceiveData(e.Data);
                ws.Connect();
                while (true)
                {
                    Thread.Sleep(15000);
                }
            }
        }

        private static void ReceiveData(string JSONdata)
        {
            var obj = JsonConvert.DeserializeObject<BinanceWebSocketTicker[]>(JSONdata);
            new AuctusApi(AuctusApiAuthToken, AuctusApiUrl).PostExecuteOrders(obj);
        }

        private static void Configure()
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                            .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{environment}.json", optional: true);
            
            var configuration = builder.Build();

            AuctusApiAuthToken = configuration.GetSection("AuctusApiAuthToken").Get<string>();
            AuctusApiUrl = configuration.GetSection("AuctusApiUrl").Get<string>();
        }
    }
}
