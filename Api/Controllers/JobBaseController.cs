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

        protected virtual IActionResult UpdateAssetsValues()
        {
            RunJobAsync(() => AssetValueBusiness.UpdateAllAssetsValues());
            return Ok();
        }

        protected virtual IActionResult CreateAssets()
        {
            RunJobSync(AssetBusiness.CreateCoinMarketCapNotIncludedAssets);
            return Ok();
        }

        internal IActionResult UpdateAllAssetsIcons()
        {
            RunJobSync(AssetBusiness.UpdateAllAssetsIcons);
            return Ok();
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
            Logger.LogTrace($"Job {action.Method.Name} ended.");
        }
    }
}