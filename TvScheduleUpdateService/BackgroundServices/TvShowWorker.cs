using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TvSchedule.Data.Data;
using TvSchedule.Data.Entities;
using TvScheduleUpdateService.YandexScheduleResponseModels;
using Channel = TvSchedule.Data.Entities.Channel;



namespace TvScheduleUpdateService.BackgroundServices
{
    public class TvShowWorker : BackgroundServiceBase
    {
        private readonly ILogger<TvShowWorker> _logger;

        public TvShowWorker(ILogger<TvShowWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var channelsForUpdate = ChannelDataContext.Instance.GetList(1000,
                        SqlFilter.Lt("LastShow", DateTime.UtcNow.AddMinutes(-1.5))
                            .Or(SqlFilter.IsNull("LastShow")));
                    
                    //ïîëó÷èëà âñå êàíàëû èç áä, äëÿ êîòîðûõ ïî óñëîâèþ íóæíî îáðàáîòàòü èõ øîó è òåïåðü äëÿ êàæäîãî: áëî÷ó åãî è ïîëó÷àþ ïî àïè âñå åãî øîó

                    var apiClient = new TvYandexApiClient();

                    foreach (var channel in channelsForUpdate)
                    {
                        string lastError = null;
                        using (var bulk = TvShowsDataContext.Instance.StartBulkProcessing())
                        { 
                            var lockSessionId =
                                $"{System.AppDomain.CurrentDomain.FriendlyName}_{Thread.CurrentThread.ManagedThreadId}";
                            try
                            {
                                var node = await apiClient.GetSchedule(DateTime.UtcNow,
                                channel.IdFromApi);

                                var isLocked = ChannelDataContext.Instance.TryLock(channel, TimeSpan.FromMinutes(1),
                                    ChannelDataContext.LockType.UpdateShows, lockSessionId);

                                if (isLocked)
                                {
                                    // íà÷èíàþ îáðàáîòêó øîó(ïîëó÷àþ èõ ïî àïè, ôîðìèðóþ êàæäîìó êëþ÷, ìåðæó
                                    var tvShows = node.events;

                                    foreach (var show in tvShows)
                                    {
                                        try
                                        {
                                            var name = show.programTitle == ""
                                                    ? (show.episodeTitle)
                                                    : (show.programTitle);
                                            var startDateShow = show.start;

                                            var showKey = TvShow.GetHash(name, channel.Id, startDateShow);

                                            bulk.Merge(new TvShow(), x =>
                                            {
                                                x.Id = showKey;
                                                x.Name = name;
                                                x.Description = show.episodeTitle;
                                                x.ImageUrl = show.url;
                                                x.StartDateUtc = show.start;
                                                x.ChannelId = channel.Id;
                                                x.UpdatedUtc = DateTime.UtcNow;
                                            });

                                            //ÑÌÛÑË ÒÎÃÄÀ ÁÛËÎ ÄÅËÀÒÜ ÌÅÐÆ ÎÒÄÅËÜÍÎ, ÀÏÄÅÉÒ ÎÒÄÅËÜÍÎ È ÝÄÎÐÀÏÄÅÉÒ ÎÒÄÅËÜÍÎ?
                                        }
                                        catch (WarningException wEx)
                                        {
                                            LogHelper.LogWarning(wEx);
                                        }
                                    }
                                }
                            }

                            catch (Exception ex)
                            {
                                LogHelper.LogError(ex);
                                lastError = ex.ToString();
                            }

                            finally
                            {
                                ChannelDataContext.Instance.Update(channel, ch =>
                                {
                                    ch.LastError = lastError;
                                    ch.LastShow = DateTime.UtcNow;
                                }, ChannelDataContext.LockType.UpdateShows, lockSessionId);
                            }
                        }
                    }

                    await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
                }
                catch (Exception e)
                {
                    LogHelper.LogError(e);
                }
            }
        }
    }
}

