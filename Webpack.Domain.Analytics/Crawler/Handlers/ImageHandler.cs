// <copyright file="ImageHandler.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.Crawler.Handlers
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Model;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Image Handler
    /// </summary>
    public class ImageHandler : ResourceHandler
    {
        private readonly string[] extensions;

        /// <summary>
        /// Image Handler
        /// </summary>
        /// <param name="dirName">dir Name</param>
        /// <param name="next">next</param>
        /// <param name="extensions">extensions</param>
        /// <returns></returns>
        public ImageHandler (string dirName, ResourceHandler next, params string[] extensions)
            : base(dirName, next)
	    {
            this.extensions = extensions == null || !extensions.Any() 
                ? new[] { ".gif", ".png", ".jpg", ".jpeg" }
                : extensions;
	    }

        /// <summary>
        /// Gets the resorce type
        /// </summary>
        protected override ResourceType ResourceType
        {
            get { return ResourceType.Image; }
        }

        /// <summary>
        /// Gets an array of supported extensions
        /// </summary>
        protected override string[] Extensions
        {
            get
            {
                return extensions;
            }
        }
    }
}
