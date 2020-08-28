using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TvSchedule.Data.Data;
using TvSchedule.Data.Entities;
using TvScheduleUpdateService.YandexScheduleResponseModels;
using Channel = TvSchedule.Data.Entities.Channel;
using Exception = System.Exception;


namespace TvScheduleUpdateService.BackgroundServices
{
    public class ChannelWorker : BackgroundServiceBase
    {
        private readonly ILogger<ChannelWorker> _logger;

        public ChannelWorker(ILogger<ChannelWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var apiClient = new TvYandexApiClient();
                    var schedule = await apiClient.GetSchedule(DateTime.UtcNow);

                   

                    var idApiList = new List<int>();
                    foreach (var item in schedule.schedule.schedules)
                    {
                        idApiList.Add(item.channel.id);
                    }

                    var dbItem = ChannelDataContext.Instance
                        .GetList(100, 
                            SqlFilter.In("IdFromApi", idApiList))
                        .ToDictionary(x => x.IdFromApi, x => x.Id);

                    foreach (var channel in schedule.schedule.schedules)
                    {
                        try
                        {
                            if (dbItem.ContainsKey(channel.channel.id))
                            {
                                ChannelDataContext.Instance.Update(new Channel(), x =>
                                {
                                    int val;
                                    dbItem.TryGetValue(channel.channel.id, out val);
                                    x.Id = val;
                                    x.Name = channel.channel.familyTitle;
                                    x.Description = channel.channel.title;
                                    x.UpdatedUtc = DateTime.UtcNow;
                                    x.IdFromApi = channel.channel.id;
                                });  
                            }

                            else
                            {
                                ChannelDataContext.Instance.Create(new Channel(), x =>
                                {
                                    x.Name = channel.channel.familyTitle;
                                    x.Description = channel.channel.title;

                                    x.UpdatedUtc = DateTime.UtcNow;
                                    x.IdFromApi = channel.channel.id;
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.LogError(ex);
                        }
                    }


                    await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken).ConfigureAwait(false);
                }

                catch (OperationCanceledException ex)
                {
                    LogHelper.LogInfo(ex);
                }

                catch (Exception ex)
                {
                    LogHelper.LogError(ex);
                }
            }
        }
    }
}
