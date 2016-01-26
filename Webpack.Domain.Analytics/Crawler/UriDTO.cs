// <copyright file="UriDTO.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.Crawler
{
    using System;

    /// <summary>
    /// A crawledPage for carrying URIs and their depth
    /// </summary>
    public struct UriDTO
    {
        /// <summary>
        /// Gets or sets the URI
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Gets or sets the depth of this URI
        /// </summary>
        public int Depth { get; set; }
    }
}
