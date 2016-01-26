// <copyright file="CrawlerConfiguration.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.Crawler
{
    using System;

    /// <summary>
    /// Configuration against which the crawler shall run
    /// </summary>
    public class CrawlerConfiguration
    {
        /// <summary>
        /// Backing field for Uri
        /// </summary>
        private readonly Uri uri;

        /// <summary>
        /// Backing field for DepthLimit
        /// </summary>
        private readonly int depthLimit;

        /// <summary>
        /// Backing field for CountLimit
        /// </summary>
        private readonly int countLimit;

        /// <summary>
        /// Create an instance
        /// </summary>
        /// <param name="uri">URL to crawl</param>
        /// <param name="depthLimit">Maximum crawl depth</param>
        /// <param name="countLimit">Maximum number of pages that can be crawled</param>
        public CrawlerConfiguration(Uri uri, int depthLimit, int countLimit)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            if (depthLimit < 0)
            {
                throw new ArgumentOutOfRangeException("depthLimit");
            }

            if (countLimit < 0)
            {
                throw new ArgumentOutOfRangeException("countLimit");
            }
        }
        
        /// <summary>
        /// Gets the URL to crawl
        /// </summary>
        public Uri Uri 
        { 
            get { return uri; } 
        }

        /// <summary>
        /// Gets the maximum crawl depth
        /// </summary>
        public int DepthLimit 
        { 
            get { return depthLimit; } 
        }

        /// <summary>
        /// Gets them maximum number of pages that can be crawled
        /// </summary>
        public int CountLimit 
        { 
            get { return countLimit; } 
        }
    }
}
