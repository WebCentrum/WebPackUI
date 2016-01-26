// <copyright file="ILoader.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.Crawler
{
    using System;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Loads a page from a specified URI
    /// </summary>
    public interface ILoader
    {
        /// <summary>
        /// Finds a page on the web specified by its URI, extracts the links on this page and creates a raw page from it.
        /// </summary>
        /// <param name="uri">The URI of the page to download.</param>
        /// <param name="baseUri">The URI of the base page of the search.</param>
        /// <returns>A <seealso cref="RawPage"/> created from the downloaded page</returns>
        RawPage Load(Uri uri, Uri baseUri);
    }
}
