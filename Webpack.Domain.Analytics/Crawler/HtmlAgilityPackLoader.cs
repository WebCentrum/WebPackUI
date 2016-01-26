// <copyright file="HtmlAgilityPackLoader.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HtmlAgilityPack;
    using Webpack.Domain.Model.Entities;
    using Webpack.Domain.Model;
    using System.Web;

    /// <summary>
    /// Html Agility Pack Loader
    /// </summary>
    public class HtmlAgilityPackLoader : ILoader
    {
        private List<Resource> resources = new List<Resource>();
        private ISet<string> resourcesUrls = new HashSet<string>();

        private readonly ResourceHandler resourceHandler;
        private int counter = 1;

        /// <summary>
        /// Html Agility Pack Loader
        /// </summary>
        /// <param name="resourceHandler">resource Handler</param>
        /// <returns></returns>
        public HtmlAgilityPackLoader(ResourceHandler resourceHandler = null)
        {
            this.resourceHandler = resourceHandler;
        }

        public List<Resource> Resources 
        { 
            get { return resources; } 
        }

        /// <summary>
        /// Finds a page on the web specified by its URI and creates a raw page from it.
        /// </summary>
        /// <param name="uri">The URI of the page to download.</param>
        /// <param name="baseUri">The uri to which all are relative to.</param>
        /// <returns>A <seealso cref="RawPage"/> created from the downloaded page</returns>
        public virtual RawPage Load(Uri uri, Uri baseUri)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(uri.ToString());
            if (web.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            if (IsResource(uri, doc))
            {
                ProcessResource(uri, doc);
                return null;
            }

            var links = doc.DocumentNode.Descendants()
                .Where(a => a.Attributes.Contains("href") || a.Attributes.Contains("src"))
                .Select(a => a.Attributes.Contains("href") ? a.Attributes["href"].Value : a.Attributes["src"].Value)
                .Select(HttpUtility.HtmlDecode)
                .FilterValidUrls();
            string responcePath = web.ResponseUri.PathAndQuery;
            var normalizedLinks = NormalizeLinks(links, baseUri).ToList();

            return new RawPage(uri, HttpUtility.HtmlDecode(doc.DocumentNode.WriteTo()), GetReference(), normalizedLinks, responcePath);
        }

        private string GetReference()
        {
            return string.Format("@Raw(Model.Page{0})", counter++);
        }

        /// <summary>
        /// Process Resource
        /// </summary>
        /// <param name="uri">uri</param>
        /// <param name="doc">doc</param>
        /// <returns></returns>
        private void ProcessResource(Uri uri, HtmlDocument doc)
        {
            if (resourceHandler != null && !resourcesUrls.Contains(uri.ToString()))
	        {
                var r = resourceHandler.Handle(uri, doc);
                if (r != null)
	            {
                    resourcesUrls.Add(uri.ToString());
                    resources.Add(r);
	            }   
	        }
        }

        /// <summary>
        /// Is Resource
        /// </summary>
        /// <param name="uri">uri</param>
        /// <param name="doc">doc</param>
        /// <returns></returns>
        private bool IsResource(Uri uri, HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("html").Count() == 0;
        }

        /// <summary>
        /// Normalizes links, converts them to absolute.
        /// </summary>
        /// <param name="links">A list of links.</param>
        /// <param name="baseUri">The base URI for them.</param>
        /// <returns>A list of normalized links that are absolute.</returns>
        private IEnumerable<Uri> NormalizeLinks(IEnumerable<string> links, Uri baseUri)
        {
            foreach (var link in links)
            {
                Uri result;
                if (Uri.TryCreate(link, UriKind.Relative, out result) &&
                    Uri.TryCreate(baseUri, result, out result))
                {
                    yield return result;
                }
            }
        }
    }

    /// <summary>
    /// Url Helper
    /// </summary>
    static class UrlHelper
    {
        /// <summary>
        /// Filter Valid Urls
        /// </summary>
        /// <param name="source">source</param>
        /// <returns></returns>
        public static IEnumerable<string> FilterValidUrls(this IEnumerable<string> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Uri test;
            return source
                .Where(url => !url.StartsWith("#"))
                .Where(url => !Uri.TryCreate(url, UriKind.Absolute, out test));
        }
    }
}
