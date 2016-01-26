// <copyright file="PageDivisionInfo.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlMapping
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Page Division Info
    /// </summary>
    public class PageDivisionInfo
    {
        private readonly HtmlNode template;

        private readonly List<PropertyDTO> properties;

        /// <summary>
        /// Page Division Info
        /// </summary>
        /// <param name="template">template</param>
        /// <param name="properties">properties</param>
        /// <returns></returns>
        public PageDivisionInfo(HtmlNode template, List<PropertyDTO> properties)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            this.template = template;
            this.properties = properties;
        }

        public HtmlNode Template
        {
            get { return template; }
        }

        public List<PropertyDTO> Properties
        {
            get { return properties; }
        }
    }
}
