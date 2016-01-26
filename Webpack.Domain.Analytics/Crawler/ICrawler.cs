// <copyright file="ICrawler.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.Crawler
{
    using System;
    using System.Collections.Generic;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Interface for Crawlers that they must implement
    /// </summary>
    public interface ICrawler
    {
        /// <summary>
        /// Gets the URL from which the crawling started
        /// </summary>
        Uri BaseUri { get; }

        /// <summary>
        /// Crawl the websites
        /// </summary>
        /// <returns>A list of rawPages that were found</returns>
        List<RawPage> Crawl();

        /// <summary>
        /// Gets the list of resources (other than pages) that were downloaded
        /// </summary>
        Resource[] Resources { get; }
    }
}
