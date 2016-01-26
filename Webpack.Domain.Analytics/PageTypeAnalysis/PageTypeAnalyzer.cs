// <copyright file="PageTypeAnalyzer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.PageTypeAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Model.Entities;
    using Webpack.Domain.Analytics.Extensions;
    using Webpack.Domain.Analytics.DocumentTypeAnalysis;
    using HtmlAgilityPack;
    using Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlMapping;

    /// <summary>
    /// Page Type Analyzer
    /// </summary>
    public class PageTypeAnalyzer
    {
        private readonly IEqualityComparer<HtmlNode> comparer;

        private readonly IPageComparingFactory factory;

        /// <summary>
        /// Page Type Analyzer
        /// </summary>
        /// <returns></returns>
        public PageTypeAnalyzer()
        {
            comparer = new HtmlNodeEqualityComparer(new HtmlAttributeEqualityComparer());
            factory =  new PageComparingFactory(comparer);
        }

        /// <summary>
        /// Analyze
        /// </summary>
        /// <param name="pageType">page Type</param>
        /// <returns></returns>
        public void Analyze(PageType pageType)
        {
            if (pageType == null)
            {
                throw new ArgumentNullException("pageType");
            }

            var pages = pageType.Pages;

            var skeletonExtractor = factory.SkeletonExtractor;
            pages.Accept(skeletonExtractor);
            var skeleton = skeletonExtractor.Result;
            if (skeleton == null)
            {
                return;
            }
            IdentifyMacros(skeleton, pageType);

            var builder = factory.Builder;
            var propertyIdentifier = new PropertyIdentifier(skeleton, builder);
            pages.Accept(propertyIdentifier); // sets properties on pages;

            pageType.Definitions.AddRange(propertyIdentifier.Definitions);
            var textTemplate = propertyIdentifier.PopulatedSkeleton.WriteTo();
            pageType.Template = new Template
            {
                Name = pageType.Name,
                Text = textTemplate,
                ID = Guid.NewGuid()
            };
        }

        /// <summary>
        /// Identify Macros
        /// </summary>
        /// <param name="skeleton">skeleton</param>
        /// <param name="pageType">page Type</param>
        /// <returns></returns>
        private void IdentifyMacros(HtmlNode skeleton, PageType pageType)
        {
            var menuProperties = new List<PropertyDTO>();
            var doc = skeleton.OwnerDocument;
            var propertyFactory = factory.PropertyFactory;
            foreach (var menuNode in pageType.MacroXpaths
                .SelectMany(xpath => skeleton.SelectNodes(xpath))
                .Where(n => n != null && n.ParentNode != null))
            {
                var property = propertyFactory.GetNew();
                menuProperties.Add(property);

                var propertyNode = doc.CreateTextNode(property.TemplateReference);
                menuNode.ParentNode.ReplaceChild(propertyNode, menuNode);
            }

            var macros = menuProperties.Select(p => new Definition
            {
                Number = p.Number,
                Name = p.Name,
                TemplateReference = p.TemplateReference,
                IsMacro = true
            });
            pageType.Definitions.AddRange(macros);
        }
    }
}
