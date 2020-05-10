using System;
using System.Collections.Generic;
using System.Text;

namespace TvScheduleUpdateService.YandexScheduleResponseModels
{
    public class YandexScheduleResponse
    {
        public Schedule schedule { get; set; }
    }


    public class Schedule
    {
        public TvChannelNode[] schedules;
    }

    public class Episode
    {
        public int id { get; set; }
        public string title { get; set; }
    }

    public class Type
    {
        public int id { get; set; }
        public string name { get; set; }
        public string alias { get; set; }
        public bool isFilm { get; set; }
        public bool isSerial { get; set; }
        public bool isForChildren { get; set; }
    }

    public class TvProgram
    {
        public List<object> trailers { get; set; }
        public List<object> onlines { get; set; }
        public int id { get; set; }
        public Type type { get; set; }
        public string title { get; set; }
        public string transliteratedTitle { get; set; }
        public string mainImageBaseUrl { get; set; }
        public bool favourite { get; set; }
        public List<object> tags { get; set; }
        public bool displayIfNoEvents { get; set; }
        public List<int> duplicateIds { get; set; }
        public string url { get; set; }
        public List<object> images { get; set; }
        public int? coId { get; set; }
        public List<string> coIds { get; set; }
    }

    public class Event
    {
        public int id { get; set; }
        public int channelId { get; set; }
        public int channelFamilyId { get; set; }
        public bool live { get; set; }
        public Episode episode { get; set; }
        public TvProgram program { get; set; }
        public DateTime start { get; set; }
        public DateTime finish { get; set; }
        public int liveId { get; set; }
        public int yacFamilyId { get; set; }
        public string title { get; set; }
        public string programTitle { get; set; }
        public string episodeTitle { get; set; }
        public string seasonTitle { get; set; }
        public string url { get; set; }
        public bool hasDescription { get; set; }
        public bool hasReminder { get; set; }
        public bool hasReminderButton { get; set; }
        public string startTime { get; set; }
        public bool hasStarted { get; set; }
        public bool isNow { get; set; }
        public bool hasFinished { get; set; }
        public int progress { get; set; }
        public string humanDate { get; set; }
    }

    public class Original
    {
        public string src { get; set; }
        public bool original { get; set; }
    }

    public class __invalid_type__38
    {
        public string src { get; set; }
    }

    public class __invalid_type__64
    {
        public string src { get; set; }
    }

    public class __invalid_type__80
    {
        public string src { get; set; }
    }

    public class __invalid_type__114
    {
        public string src { get; set; }
    }

    public class __invalid_type__160
    {
        public string src { get; set; }
    }

    public class Sizes
    {
        public __invalid_type__38 __invalid_name__38 { get; set; }
        public __invalid_type__64 __invalid_name__64 { get; set; }
        public __invalid_type__80 __invalid_name__80 { get; set; }
        public __invalid_type__114 __invalid_name__114 { get; set; }
        public __invalid_type__160 __invalid_name__160 { get; set; }
    }

    public class OriginalSize
    {
        public string src { get; set; }
        public bool original { get; set; }
    }

    public class MaxSize
    {
        public string src { get; set; }
    }

    public class Logo
    {
        public Original original { get; set; }
        public Sizes sizes { get; set; }
        public OriginalSize originalSize { get; set; }
        public MaxSize maxSize { get; set; }
    }

    public class Channel
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastCheck { get; set; }
        public DateTime UpdatedUtc { get; set; }

        /// 
        public string siteUrl { get; set; }

        public string title { get; set; }
        public string familyTitle { get; set; }
        public string transliteratedFamilyTitle { get; set; }
        public Logo logo { get; set; }
        public List<string> synonyms { get; set; }
        public int familyId { get; set; }
        public int id { get; set; }
        public List<object> genres { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public bool isFavorite { get; set; }
        public bool hasBroadcasting { get; set; }
        public string broadcastingUrl { get; set; }
        public bool hasBroadcastingPlayer { get; set; }
        public string broadcastingPlayerUrl { get; set; }
    }

    public class TvChannelNode
    {
        public DateTime finish { get; set; }
        public Event[] events { get; set; }
        public Channel channel { get; set; }
        public bool hasFinished { get; set; }
    }
}
