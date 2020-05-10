using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TvScheduleUpdateService.BackgroundServices;

using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using TvScheduleUpdateService.YandexScheduleResponseModels;

namespace TvScheduleUpdateService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ChannelWorker>();
                    services.AddHostedService<TvShowWorker>();
                });
    }
}
