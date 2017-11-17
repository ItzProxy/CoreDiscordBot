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

namespace Core_Discord.CoreServices
{
    public class GoogleApiService : IGoogleApiService
    {

        private YouTubeService _youtube;
        private UrlshortenerService _urlss;
        private CustomsearchService cs;
        private readonly ICoreCredentials _cred;

        public GoogleApiService(ICoreCredentials cred)
        {
            _cred = cred;
            var cli = new BaseClientService.Initializer
            {
                ApplicationName = "Core Discord",
                ApiKey = _cred.GoogleApiKey
            };
        }

        public IEnumerable<string> Languages => throw new NotImplementedException();

        public Task<ImageResult> GetImageAsync(string query, int start = 1)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetPlaylistIdsByKeywordsAsync(string keywords, int count = 1)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetPlaylistTracksAsync(string playlistId, int count = 50)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Gets videos related to previous video
        /// </summary>
        /// <param name="url">Youtube URL</param>
        /// <param name="count">number of videos to be returned</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetRelatedVideosAsync(string url, int count = 1)
        {
            
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

        public Task<IEnumerable<(string Name, string Id, string Url)>> GetVideoInfosByKeywordAsync(string keywords, int count = 1)
        {
            throw new NotImplementedException();
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

            var query = _youtube.Search.List("id, snippit");
            query.MaxResults = count;
            query.Type = "playlist";
            query.Q = keywords;

            return (await query.ExecuteAsync()).Items.Select(m => m.Id.PlaylistId);
        }

        public async Task<string> ShortenUrl(string url)
        {
            throw new NotImplementedException();
        }

        public Task<string> Translate(string sourceText, string sourceLanguage, string targetLanguage)
        {
            throw new NotImplementedException();
        }
    }
}
