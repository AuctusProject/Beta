using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auctus.Util;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    public class JobBaseController : BaseController
    {
        private readonly IServiceScopeFactory ServiceScopeFactory;

        protected JobBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory) : 
            base(loggerFactory, cache, serviceProvider)
        {
            ServiceScopeFactory = serviceScopeFactory;
        }

        protected virtual IActionResult UpdateAssetsValues(string api)
        {
            if (!ValidApi(api))
                return BadRequest();

            RunJobAsync(() => HandleUpdateAssetsValues(api));
            return Ok();
        }

        protected virtual IActionResult CreateAssets(string api)
        {
            if (!ValidApi(api))
                return BadRequest();

            RunJobAsync(() => HandleCreateAssets(api));
            return Ok();
        }

        protected virtual IActionResult UpdateAssetsIcons(string api)
        {
            if (!ValidApi(api))
                return BadRequest();

            RunJobAsync(() => HandleUpdateAssetsIcons(api));
            return Ok();
        }

        private bool ValidApi(string api)
        {
            return !string.IsNullOrWhiteSpace(api) && (api.ToLower() == "coinmarketcap" || api.ToLower() == "coingecko");
        }

        private void HandleUpdateAssetsValues(string api)
        {
            if (api == "coingecko")
                AssetValueBusiness.UpdateCoingeckoAssetsValues();
            else
                AssetValueBusiness.UpdateCoinmarketcapAssetsValues();
        }

        private void HandleCreateAssets(string api)
        {
            if (api == "coingecko")
                AssetBusiness.CreateCoingeckoNotIncludedAssets();
            else
                AssetBusiness.CreateCoinmarketcapNotIncludedAssets();
        }

        private void HandleUpdateAssetsIcons(string api)
        {
            if (api == "coingecko")
                AssetBusiness.UpdateCoingeckoAssetsIcons();
            else
                AssetBusiness.UpdateCoinmarketcapAssetsIcons();
        }

        private void RunJobAsync(Action action)
        {
            Task.Factory.StartNew(() =>
            {
                using (var scope = ServiceScopeFactory.CreateScope())
                {
                    ServiceProvider = scope.ServiceProvider;
                    TelemetryClient telemetry = new TelemetryClient();
                    try
                    {
                        telemetry.TrackEvent(action.Method.Name);
                        RunJobSync(action);
                    }
                    catch (Exception e)
                    {
                        telemetry.TrackException(e);
                        Logger.LogCritical(e, $"Exception on {action.Method.Name} job");
                    }
                    finally
                    {
                        telemetry.Flush();
                    }
                }
            });
        }

        private void RunJobSync(Action action)
        {
            Logger.LogInformation($"Job {action.Method.Name} started.");
            action();
            Logger.LogInformation($"Job {action.Method.Name} ended.");
        }
    }
}