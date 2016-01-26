// <copyright file="CrawlerConfiguration.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.Crawler
{
    using System;

    /// <summary>
    /// Configuration against which the crawler shall run.
    /// </summary>
    public class CrawlerConfiguration
    {
        /// <summary>
        /// Backing field for <see cref="IgnoredPaths"/>.
        /// </summary>
        private string[] ignoredPaths = new string[0];

        /// <summary>
        /// Backing field for <see cref="IgnoredPrefixes"/>.
        /// </summary>
        private string[] ignoredPrefixes = new string[0];

        /// <summary>
        /// Gets them maximum number of rawPages that can be crawled.
        /// </summary>
        public int? CountLimit { set; get; }

        /// <summary>
        /// Gets the maximum crawl depth.
        /// </summary>
        public int? DepthLimit { get; set;}

        /// <summary>
        /// Gets the URL to crawl.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Gets and sets the paths that won't get crawled.
        /// </summary>
        public string[] IgnoredPaths 
        {
            get { return ignoredPaths; }
            set { ignoredPaths = value ?? new string[0]; }
        }

        /// <summary>
        /// Gets and sets the prefixes of paths that won't get crawled.
        /// </summary>
        public string[] IgnoredPrefixes
        {
            get { return ignoredPrefixes; }
            set { ignoredPrefixes = value ?? new string[0]; }
        }
    }
}