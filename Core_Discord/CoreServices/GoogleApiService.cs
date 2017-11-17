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

namespace Core_Discord.CoreServices
{
    public class GoogleApiService : IGoogleApiService
    {

        private YouTubeService _youtube;
        private UrlshortenerService _urlss;
        private CustomsearchService cs;

        private 

        public GoogleApiService(ICoreCredentials cred)
        {

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

        public Task<IEnumerable<string>> GetPlaylistTracksAsync(string playlistId, int count = 50)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetRelatedVideosAsync(string url, int count = 1)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyDictionary<string, TimeSpan>> GetVideoDurationsAsync(IEnumerable<string> videoIds)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<(string Name, string Id, string Url)>> GetVideoInfosByKeywordAsync(string keywords, int count = 1)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetVideoLinksByKeywordAsync(string keywords, int count = 1)
        {
            throw new NotImplementedException();
        }

        public Task<string> ShortenUrl(string url)
        {
            throw new NotImplementedException();
        }

        public Task<string> Translate(string sourceText, string sourceLanguage, string targetLanguage)
        {
            throw new NotImplementedException();
        }
    }
}
