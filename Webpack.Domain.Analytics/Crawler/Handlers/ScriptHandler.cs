// <copyright file="ScriptHandler.cs" company="ÚVT MU">
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
    using System.Web;
    using Webpack.Domain.Model;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Script Handler
    /// </summary>
    public class ScriptHandler : ResourceHandler
    {
        private readonly string[] extensions;

        /// <summary>
        /// Script Handler
        /// </summary>
        /// <param name="dirName">dir Name</param>
        /// <param name="next">next</param>
        /// <param name="extensions">extensions</param>
        /// <returns></returns>
        public ScriptHandler (string dirName, ResourceHandler next, params string[] extensions)
            : base(dirName, next)
	    {
            this.extensions = 
                extensions == null || !extensions.Any()
                    ? new[] { ".js", ".css", ".axd" }
                    : extensions; ;
	    }

        /// <summary>
        /// Gets the resource type.
        /// </summary>
        protected override ResourceType ResourceType
        {
            get { return ResourceType.Javascript | ResourceType.Stylesheet; }
        }

        /// <summary>
        /// Gets the suported extension.
        /// </summary>
        protected override string[] Extensions
        {
            get
            {
                return extensions;
            }
        }

        /// <summary>
        /// Proccess
        /// </summary>
        /// <param name="uri">uri</param>
        /// <param name="doc">doc</param>
        /// <returns></returns>
        protected override Resource Proccess(Uri uri, HtmlDocument doc)
        {
            var res = base.Proccess(uri, doc);
            if (res == null)
	        {
		        return null;
	        }

            var extension = Path.GetExtension(uri.AbsolutePath);
            if (string.Equals(".axd", extension, StringComparison.OrdinalIgnoreCase))
            {
                var type = HttpUtility.ParseQueryString(uri.Query).Get("t");
                res.ResourceType = type == "Css" 
                    ? ResourceType.Stylesheet 
                    : string.Equals("Javascript", type, StringComparison.OrdinalIgnoreCase) 
                        ? ResourceType.Javascript 
                        : ResourceType.File;
            }
            return res;
        }

    }
}
