// <copyright file="AnalyzerHelper.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Tomáš Pouzar</author>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using Webpack.Domain.Analytics;
using Webpack.Domain.Model.Entities;
using WebpackUI.Analyzer;
using WebpackUI.Models;

namespace WebpackUI.Helpers
{
    /// <summary>
    /// Helper that provides utility methods for Analyzer process
    /// </summary>
    public class AnalyzerHelper
    {
        /// <summary>
        /// Analyzes website and creates XML document
        /// </summary>
        /// <param name="config">Website's config</param>
        /// <returns>
        /// Updated website's config
        /// </returns>
        public WebsiteModel AnalyzeWebsite(WebsiteModel config)
        {
            CrawlerHelper crawlHelper = new CrawlerHelper();
            OrganizerHelper orgHelper = new OrganizerHelper();

            config.OrganizerConfig = new OrganizerModel();
            config.ExportConfig = new ExportModel();
            config.ExportConfig.Xml = string.Empty;

            var crawler = crawlHelper.ConfigureCrawler(config.CrawlerConfig);
            var list = crawler.Crawl();

            var analyzer = new WebsiteAnalyzer(config.AnalyzerConfig);
            var analyticsRunner = new AnalyticsRunner(crawler, analyzer);

            var site = analyticsRunner.Run(list);

            site.Name = config.Name;

            using (var sw = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(Site));
                serializer.Serialize(sw, site);

                config.ExportConfig.Xml = sw.ToString();
            }
                
            config.OrganizerConfig.Pages = orgHelper.AddChildren(site.Root, site);

            return config;
        }
    }
}