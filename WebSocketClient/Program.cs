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
        static bool ShouldRestart = false;
        static void Main(string[] args)
        {
            Configure();
            while (true)
            {
                CreateWSConnection();
            }
        }

        private static void CreateWSConnection()
        {
            ShouldRestart = false;
            using (var ws = new WebSocket("wss://stream.binance.com:9443/ws/!ticker@arr"))
            {
                ws.OnMessage += (sender, e) => {
                    if (e != null)
                        ReceiveData(e.Data);
                };
                ws.OnClose += (sender, e) => OnConnectionClosedOrError();
                ws.OnError += (sender, e) => OnConnectionClosedOrError();
                ws.Connect();
                while (!ShouldRestart)
                {
                    
                    Thread.Sleep(15000);
                }
            }
        }

        private static void OnConnectionClosedOrError()
        {
            ShouldRestart = true;
        }

        private static void ReceiveData(string JSONdata)
        {
            var obj = JsonConvert.DeserializeObject<BinanceWebSocketTicker[]>(JSONdata);
            new AuctusApi(AuctusApiAuthToken, AuctusApiUrl).PostExecuteOrders(obj);
        }

        private static void Configure()
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Console.WriteLine("Environment:");
            Console.WriteLine(environment);
            Console.WriteLine("Path:");
            Console.WriteLine(Path.Combine(AppContext.BaseDirectory));
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{environment}.json", optional: true);
            
            var configuration = builder.Build();

            AuctusApiAuthToken = configuration.GetSection("WebJob:AuctusApiAuthToken").Get<string>();
            AuctusApiUrl = configuration.GetSection("WebJob:AuctusApiUrl").Get<string>();

            Console.WriteLine("Auth:");
            Console.WriteLine(AuctusApiAuthToken);
            Console.WriteLine("AuctusApiUrl:");
            Console.WriteLine(AuctusApiUrl);
        }
    }
}
