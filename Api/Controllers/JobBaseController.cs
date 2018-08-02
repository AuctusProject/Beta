using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auctus.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    public class JobBaseController : BaseController
    {
        protected JobBaseController(ILoggerFactory loggerFactory, Cache cache, IServiceProvider serviceProvider) : base(loggerFactory, cache, serviceProvider) { }

        protected virtual IActionResult UpdateAssetsValues()
        {
            RunJobAsync(AssetServices.UpdateAllAssetsValues);
            return Ok();
        }

        protected virtual IActionResult CreateAssets()
        {
            RunJobSync(AssetServices.CreateAssets);
            return Ok();
        }

        internal IActionResult UpdateAllAssetsIcons()
        {
            RunJobSync(AssetServices.UpdateAllAssetsIcons);
            return Ok();
        }
        private void RunJobAsync(Action action)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    RunJobSync(action);
                }
                catch (Exception e)
                {
                    Logger.LogCritical(e, $"Exception on {action.Method.Name} job");
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