// <copyright file="CrawlerHelper.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Tomáš Pouzar</author>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webpack.Domain.Analytics.Crawler;
using Webpack.Domain.Analytics.Crawler.Handlers;
using Webpack.Domain.Model.Entities;
using WebpackUI.Models;

namespace WebpackUI.Helpers
{
    /// <summary>
    /// Helper that provides utility methods for Crawler process
    /// </summary>
    public class CrawlerHelper
    {
        /// <summary>
        /// Crawls website and updates website's config with Rawpages
        /// </summary>
        /// <param name="config">Website's config</param>
        /// <returns>
        /// Updated website's config
        /// </returns>
        public WebsiteModel CrawlWebsite(WebsiteModel config)
        {
            config.AnalyzerConfig = new AnalyzerModel();
            config.OrganizerConfig = new OrganizerModel();
            config.ExportConfig = new ExportModel();
            config.ExportConfig.Xml = string.Empty;

            var crawler = ConfigureCrawler(config.CrawlerConfig);

            List<RawPage> list = new List<RawPage>();

            list = crawler.Crawl();
            
            foreach (var rawPage in list)
            {
                string path = rawPage.Path;
                int idx = path.LastIndexOf('/');
                string title = path; //idx == 0 ? path.Substring(0) : ("/" + path.Substring(idx + 1));

                var newRawPage = new RawPageModel
                {
                    Title = title,
                    Url = rawPage.Url.ToString(),
                    SitePath = path
                };
                config.AnalyzerConfig.RawPages.Add(newRawPage);
            }

            return config;
        }

        /// <summary>
        /// Creates object of type DefaultCrawler needed for Crawler process
        /// </summary>
        /// <param name="crawlerConfig">Crawler configuration</param>
        /// <returns>
        /// Crawler
        /// </returns>
        public DefaultCrawler ConfigureCrawler(CrawlerModel crawlerConfig)
        {
            var crawlerCfg = new CrawlerConfiguration
            {
                Uri = new Uri(crawlerConfig.SiteUrl),
                CountLimit = crawlerConfig.CountLimit,
                DepthLimit = crawlerConfig.DepthLimit,
                IgnoredPrefixes = crawlerConfig.IgnoredPrefixes,
                IgnoredPaths = crawlerConfig.IgnoredPaths
            };

            var directory = crawlerConfig.Directory;
            var imageHandler = new ImageHandler(directory, null);
            var scriptHandler = new ScriptHandler(directory, imageHandler);
            var loader = new HtmlAgilityPackLoader(scriptHandler);

            return new DefaultCrawler(crawlerCfg, loader);
        }
    }
}