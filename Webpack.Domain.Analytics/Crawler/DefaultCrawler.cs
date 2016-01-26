// <copyright file="DefaultCrawler.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Webpack.Domain.Model.Entities;
    using Webpack.Domain.Analytics.Extensions;

    /// <summary>
    /// Crawls a website, based on configuration and returns its rawPages and content.
    /// </summary>
    public class DefaultCrawler : ICrawler
    {
        /// <summary>
        /// Backing field for <see cref="BaseUri"/>.
        /// </summary>
        private readonly Uri baseUri;

        /// <summary>
        /// Backing field for <see cref="CountLimit"/>.
        /// </summary>
        private readonly int? countLimit;

        /// <summary>
        /// Backing field for <see cref="DepthLimit"/>.
        /// </summary>
        private readonly int? depthLimit;

        /// <summary>
        /// Backing field for <see cref="IgnoredPaths"/>.
        /// </summary>
        private readonly string[] ignoredPaths;

        /// <summary>
        /// Backing field for <see cref="IgnoredPrefixes"/>.
        /// </summary>
        private readonly string[] ignoredPrefixes;

        /// <summary>
        /// Backing field for <see cref="Loader"/>.
        /// </summary>
        private readonly HtmlAgilityPackLoader loader;

        /// <summary>
        /// Backing field for <see cref="Queue"/>.
        /// </summary>
        private readonly Queue<UriDTO> queue = new Queue<UriDTO>();

        /// <summary>
        /// Crawled rawPages are stored here
        /// </summary>
        private readonly HashSet<RawPage> pages = new HashSet<RawPage>(new RawPageEqualityComparer());

        /// <summary>
        /// URIs of crawled rawPages are stored here
        /// </summary>
        private readonly HashSet<string> uris = new HashSet<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCrawler"/> class.
        /// </summary>
        /// <param name="configuration">The configuration of the crawler, determining what to crawl and when to stop.</param>
        public DefaultCrawler(CrawlerConfiguration configuration, HtmlAgilityPackLoader loader)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            if (loader == null)
            {
                throw new ArgumentNullException("loader");
            }

            this.baseUri = configuration.Uri;
            this.countLimit = configuration.CountLimit;
            this.depthLimit = configuration.DepthLimit;
            this.loader = loader;
            this.ignoredPaths = configuration.IgnoredPaths;
            this.ignoredPrefixes = configuration.IgnoredPrefixes;
        }

        /// <summary>
        /// Gets the URI that gets crawled.
        /// </summary>
        public Uri BaseUri
        {
            get { return baseUri; }
        }

        /// <summary>
        /// Gets them maximum number of rawPages that can be crawled
        /// </summary>
        public int? CountLimit
        {
            get { return countLimit; }
        }

        /// <summary>
        /// Gets the maximum crawl depth
        /// </summary>
        public int? DepthLimit
        {
            get { return depthLimit; }
        }

        /// <summary>
        /// Gets and sets the paths that won't get crawled.
        /// </summary>
        public string[] IgnoredPaths
        {
            get { return ignoredPaths; }
        }
        
        /// <summary>
        /// Gets and sets the prefixes of paths that won't get crawled.
        /// </summary>
        public string[] IgnoredPrefixes
        {
            get { return ignoredPrefixes; }
        }

        /// <summary>
        /// Gets the loader which will be used for downloading single rawPages of a website.
        /// </summary>
        public HtmlAgilityPackLoader Loader
        {
            get { return loader; }
        }

        /// <summary>
        /// Gets the list of resources (other than pages) that were downloaded
        /// </summary>
        public Resource[] Resources
        {
            get { return loader.Resources.ToArray(); }
        }
                        
        /// <summary>
        /// Start crawling a website using BFS
        /// </summary>
        /// <returns>List of rawPages that were crawled.</returns>
        public List<RawPage> Crawl()
        {
            if (CountLimit == 0)
            {
                return pages.ToList();
            }

            queue.Enqueue(new UriDTO { Uri = baseUri, Depth = 0 });
            while (queue.Any())
            {
                UriDTO crawledPage = queue.Dequeue();

                RawPage page = Loader.Load(crawledPage.Uri, baseUri);
                if (page == null)
                {
                    uris.Add(crawledPage.Uri.PathAndQuery);
                    continue;
                }

                bool pathAdded = false;
                foreach (var path in page.Paths)
	            {
		            pathAdded = uris.Add(path) || pathAdded;
	            }
                if (pathAdded && pages.Add(page))
                {
                    EnqueueLinks(crawledPage, page);
                }
                else if (pathAdded)
                {
                    var foundPage = pages.First(p => p.TextData == page.TextData);
                    foundPage.AlternativePaths.AddRange(page.Paths.Except(foundPage.Paths));
                }
            }

            queue.Clear();
            return pages.ToList();
        }

        private void EnqueueLinks(UriDTO crawledPage, RawPage page)
        {
            var urlsToCrawl = FilterLinks(page.Links, crawledPage.Depth);
            foreach (var u in urlsToCrawl)
            {
                queue.Enqueue(new UriDTO
                {
                    Uri = u,
                    Depth = crawledPage.Depth + 1
                });
            }
        }

        private IEnumerable<Uri> FilterLinks(List<Uri> list, int currentDepth)
        {
            if (currentDepth >= DepthLimit)
            {
                return Enumerable.Empty<Uri>();
            }

            var result = list
                .Where(l => IgnoredPrefixes.All(p => !l.PathAndQuery.StartsWith(p)))
                .Where(l => !IgnoredPaths.Contains(l.PathAndQuery));

            if (CountLimit.HasValue)
            {
                result = result.Take(CountLimit.Value - queue.Count - pages.Count);
            }
            return result;
        }
    }
}
