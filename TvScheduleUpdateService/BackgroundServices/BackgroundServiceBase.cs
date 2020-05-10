using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TvSchedule.Data.Data;
using TvSchedule.Data.Entities;
using TvScheduleUpdateService.YandexScheduleResponseModels;


namespace TvScheduleUpdateService.BackgroundServices
{
    public class BackgroundServiceBase : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }

    }
}
