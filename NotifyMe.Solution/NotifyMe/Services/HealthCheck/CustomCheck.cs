using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NotifyMe.Services
{
    class CustomCheck : IHealthCheck
    {
        public string Name => "Custom Check";
        private readonly IVisitorService _visitorService;

        public CustomCheck(IServiceProvider provider)
        {
            _visitorService = (IVisitorService)provider.GetService(typeof(IVisitorService));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                DateTime lastDate = await _visitorService.GetLastConnection();
                TimeSpan difference = DateTime.Now - lastDate;
                int days = (int)difference.Days;

                if (days > 1)
                {
                    var data = new Dictionary<string, object>();
                    data.Add("LastUpdateDate", lastDate.ToString());
                    data.Add("DifferenceAsDays", days.ToString());

                    return HealthCheckResult.Unhealthy("No connection is done",null, data);
                }

                return HealthCheckResult.Healthy("Ok");
            }
            catch (System.Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message);
            }

        }
    }
}