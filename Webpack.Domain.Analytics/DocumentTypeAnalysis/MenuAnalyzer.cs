// <copyright file="MenuAnalyzer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.DocumentTypeAnalysis
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlMapping;
    using Webpack.Domain.Model.Entities;
    using Webpack.Domain.Model.Logic;

    public class MenuAnalyzer : IMenuAnalyzer
    {
        private readonly IPropertyFactory propertyFactory;

        private readonly string[] xpathMenuLocations;

        private string[] menuPropertyNames;

        public MenuAnalyzer(IPropertyFactory propertyFactory, params string[] xpathMenuLocations)
        {
            if (propertyFactory == null)
            {
                throw new ArgumentNullException("prepertyFactory");
            }
            if (xpathMenuLocations == null)
            {
                throw new ArgumentNullException("xpathMenuLocations");
            }

            this.propertyFactory = propertyFactory;
            this.xpathMenuLocations = xpathMenuLocations;
        }

        public string[] MenuPropertyNames
        {
            get { return menuPropertyNames; }
        }

        public virtual List<PropertyDTO> IdentifyMenus(HtmlNode skeleton)
        {
            if (skeleton == null)
            {
                throw new ArgumentNullException("skeleton");
            }

            var menuProperties = new List<PropertyDTO>();
            var doc = skeleton.OwnerDocument;
            foreach (var menuNode in xpathMenuLocations
                .SelectMany(xpath => skeleton.SelectNodes(xpath))
                .Where(n => n != null && n.ParentNode != null))
            {
                var property = propertyFactory.GetNew();
                menuProperties.Add(property);
                
                var propertyNode = doc.CreateTextNode(property.TemplateReference);
                menuNode.ParentNode.ReplaceChild(propertyNode, menuNode);
            }

            menuPropertyNames = menuProperties.Select(p => p.Name).ToArray();

            return menuProperties;
        }
    }
}
