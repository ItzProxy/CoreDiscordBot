using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using System.Text.RegularExpressions;
using Google.Apis.Urlshortener.v1;
using Google.Apis.Urlshortener.v1.Data;
using NLog;
using Google.Apis.Customsearch.v1;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using Core_Discord.CoreServices.Interfaces;
using System.Xml;
using Core_Discord.CoreExtensions;

namespace Core_Discord.CoreServices
{
    /// <summary>
    /// Provides implementation for IGoogleApiService
    /// </summary>
    public class GoogleApiService : IGoogleApiService
    {
        private readonly string cse_id = "006325137154071361934:-fvaienfoag";
        private readonly Logger _log;
        private YouTubeService _youtube;
        private UrlshortenerService _urlss;
        private CustomsearchService cs;
        private readonly ICoreCredentials _cred;
        private readonly Dictionary<string, string> langDictionary = new Dictionary<string, string>()
        {
            { "english","en" }
        };

        public GoogleApiService(ICoreCredentials cred)
        {
            _log = LogManager.GetCurrentClassLogger();
            _cred = cred;
            var cli = new BaseClientService.Initializer
            {
                ApplicationName = "Core Discord",
                ApiKey = _cred.GoogleApiKey
            };
        }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Languages => langDictionary.Keys;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public async Task<ImageResult> GetImageAsync(string query, int start = 1)
        {
            await Task.Yield();
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            var req = cs.Cse.List(query);
            req.Cx = cse_id;
            req.Num = 1;
            req.Fields = "items(image(contextLink,thumbnailLink),link)";
            req.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;
            req.Start = start;

            var search = await req.ExecuteAsync().ConfigureAwait(false);

            return new ImageResult(search.Items[0].Image, search.Items[0].Link);
        }

        public async Task<IEnumerable<string>> GetPlaylistIdsByKeywordsAsync(string keywords, int count = 1)
        {
            await Task.Yield();
            if (string.IsNullOrWhiteSpace(keywords))
                throw new ArgumentNullException(nameof(keywords));

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var query = _youtube.Search.List("snippet");
            query.MaxResults = count;
            query.Type = "playlist";
            query.Q = keywords;

            return (await query.ExecuteAsync()).Items.Select(i => i.Id.PlaylistId);
        }
        /// <summary>
        /// Similar to getting video duration
        /// </summary>
        /// <param name="playlistId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetPlaylistTracksAsync(string playlistId, int count = 50)
        {
            await Task.Yield();
            await Task.Yield();
            if (string.IsNullOrWhiteSpace(playlistId))
                throw new ArgumentNullException(nameof(playlistId));

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var list = playlistId.ToList();
            string nextPageToken = null;

            List<string> returnVideos = new List<string>();

            int remaining = list.Count;
            const int MAX_NUM_QUERY = 25;
            do 
            {
                int num = remaining > MAX_NUM_QUERY ? remaining : MAX_NUM_QUERY;
                remaining -= num;
                //get the videos from list
                var ls = _youtube.PlaylistItems.List("contentDetails");
                ls.MaxResults = num; //set the max number of results to send to api
                ls.PageToken = nextPageToken;
                ls.Id = string.Join(",", list.Take(num));
                list = list.Skip(num).ToList(); //gets remaining videos in list
                var content = (await ls.ExecuteAsync().ConfigureAwait(false));
                foreach (var m in content.Items)
                {
                    returnVideos.Add(m.ContentDetails.VideoId);//adds video id and time duration to dictionary
                }
                nextPageToken = content.NextPageToken;
            }while (remaining > 0 && !string.IsNullOrWhiteSpace(nextPageToken)) ;
            return returnVideos;
        }
        /// <summary>
        /// Gets video(s) related to previous video
        /// </summary>
        /// <param name="url">Youtube URL</param>
        /// <param name="count">number of videos to be returned</param>
        /// <returns>Next related video url</returns>
        public async Task<IEnumerable<string>> GetRelatedVideosAsync(string url, int count = 1)
        {
            await Task.Yield();
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }
            if(count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var q = _youtube.Search.List("snippet");
            q.MaxResults = count;
            q.RelatedToVideoId = url;
            q.Type = "video";
            return (await q.ExecuteAsync()).Items.Select(i => "http://www.youtube.com/watch?v=" + i.Id.VideoId);
        }
        /// <summary>
        /// Gets the duration of a set of videos
        /// </summary>
        /// <param name="videoIds">video id</param>
        /// <returns>id,time</returns>
        public async Task<IReadOnlyDictionary<string, TimeSpan>> GetVideoDurationsAsync(IEnumerable<string> videoIds)
        {
            await Task.Yield();
            var list = videoIds.ToList();

            Dictionary<string, TimeSpan> returnVideoDuration = new Dictionary<string, TimeSpan>();

            if (videoIds.Count() <= 0)
            {
                return returnVideoDuration;
            }
            int remaining = list.Count;
            const int MAX_NUM_QUERY = 25;
            while (remaining > 0)
            {
                int num = remaining > MAX_NUM_QUERY ? remaining : MAX_NUM_QUERY;
                remaining -= num;
                //get the videos from list
                var ls = _youtube.Videos.List("contentDetails");
                ls.Id = string.Join(",", list.Take(num));
                list = list.Skip(num).ToList(); //gets remaining videos in list
                var content = (await ls.ExecuteAsync().ConfigureAwait(false)).Items;
                foreach(var m in content)
                {
                    returnVideoDuration.Add(m.Id, XmlConvert.ToTimeSpan(m.ContentDetails.Duration));//adds video id and time duration to dictionary
                }
            }
            return returnVideoDuration;
        }

        public async Task<IEnumerable<(string Name, string Id, string Url)>> GetVideoInfosByKeywordAsync(string keywords, int count = 1)
        {
            await Task.Yield();
            if (string.IsNullOrWhiteSpace(keywords))
                throw new ArgumentNullException(nameof(keywords));

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var query = _youtube.Search.List("snippet");
            query.MaxResults = count;
            query.Q = keywords;
            query.Type = "video";
            return (await query.ExecuteAsync()).Items.Select(i => (i.Snippet.Title.TrimTo(50), i.Id.VideoId, "http://www.youtube.com/watch?v=" + i.Id.VideoId));
        }
        /// <summary>
        /// Based on
        /// https://developers.google.com/youtube/v3/code_samples/java#search_by_keyword
        /// 
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="count"></param>
        /// <returns>List of ids matching keywords</returns>
        public async Task<IEnumerable<string>> GetVideoLinksByKeywordAsync(string keywords, int count = 1)
        {
            await Task.Yield();
            if (string.IsNullOrWhiteSpace(keywords))
            {
                throw new ArgumentNullException(nameof(keywords));
            }
            if(count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var query = _youtube.Search.List("id, snippet");
            query.MaxResults = count;
            query.Type = "playlist";
            query.Q = keywords;

            return (await query.ExecuteAsync()).Items.Select(m => m.Id.PlaylistId);
        }

        public async Task<string> ShortenUrl(string url)
        {
            await Task.Yield();
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            if (string.IsNullOrWhiteSpace(_cred.GoogleApiKey))
                return url;

            try
            {
                var response = await _urlss.Url.Insert(new Url { LongUrl = url }).ExecuteAsync();
                return response.Id;
            }
            catch (Exception ex)
            {
                _log.Warn(ex);
                return url;
            }
        }

        public Task<string> Translate(string sourceText, string sourceLanguage, string targetLanguage)
        {
            throw new NotImplementedException();
        }
    }
}
