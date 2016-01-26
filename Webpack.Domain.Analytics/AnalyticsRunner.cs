// <copyright file="AnalyticsRunner.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Analytics.Crawler;
    using Webpack.Domain.Analytics.DocumentTypeAnalysis;
    using Webpack.Domain.Analytics.Extensions;
    using Webpack.Domain.Analytics.ModelAnalysis;
    using Webpack.Domain.Analytics.PageTypeAnalysis;
    using Webpack.Domain.Analytics.TemplateAnalysis;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Analytics Runner
    /// </summary>
    public class AnalyticsRunner
    {
        private ModelAnalyzer modelAnalyzer;
        private readonly ICrawler crawler;
        private readonly PageTypeAnalyzer pageTypeAnalyzer = new PageTypeAnalyzer();
        private readonly TemplateAnalyzer templateAnalyzer = new TemplateAnalyzer();
        private readonly IEqualityComparer<HtmlNode> comparer = 
            new HtmlNodeEqualityComparer(new HtmlAttributeEqualityComparer());

        /// <summary>
        /// Analytics Runner
        /// </summary>
        /// <param name="modelAnalyzer">model Analyzer</param>
        /// <returns></returns>
        public AnalyticsRunner(ICrawler crawler, ModelAnalyzer modelAnalyzer = null)
        {
            if (crawler == null)
            {
                throw new ArgumentNullException("crawler");
            }
            this.crawler = crawler;
            this.modelAnalyzer = modelAnalyzer ?? new ArticlesSiteAnalyzer();
        }

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="rawPages">raw Pages</param>
        /// <returns></returns>
        public Site Run(List<RawPage> rawPages = null)
        {
            if(rawPages == null)
                rawPages = crawler.Crawl();

            modelAnalyzer.Prepare();

            var pagesTree = modelAnalyzer.Analyze(rawPages);
            var pages = pagesTree.ToList();
            var pageTypes = modelAnalyzer.GetPagesTypes();
            PerformReferenceSubstitutions(rawPages, pagesTree);

            foreach (var pageType in pageTypes)
            {
                pageTypeAnalyzer.Analyze(pageType);
            }

            FindBaseTemplate(pageTypes);

            return new Site
            {
                Root = pagesTree.Root,
                PageTypes = pageTypes.ToArray(),
                Templates = pageTypes.Select(pt => pt.Template).ToArray(),
                Resources = crawler.Resources
            };
        }

        /// <summary>
        /// Replaces URLs with their references.
        /// </summary>
        /// <param name="rawPages">raw pages</param>
        /// <param name="pagesTree">pages tree</param>
        private void PerformReferenceSubstitutions(List<RawPage> rawPages, Model.Logic.PagesTree pagesTree)
        {
            var pageSubstitutions = rawPages.SelectMany(rp => rp.AlternativePaths.Prepend(rp.Url.Uri.PathAndQuery), (rp, ap) => new
            {
                Reference = "href=\"" + rp.Reference + "\"",
                Path = "href=\"" + ap + "\""
            });

            var resourceSubstitutions = crawler.Resources.Select(r => 
            {
                if (r.ResourceType == ResourceType.Image || r.ResourceType == ResourceType.Javascript)
	            {   
		            return new 
                    {
                        Reference = r.TemplateReference,
                        Path = r.Url.Uri.PathAndQuery
                    };
	            }
                else
                {
                    return new
                    {
                        Reference = r.TemplateReference,
                        Path = r.Url.Uri.PathAndQuery
                    };
                }
            });

            var substitutions = new Dictionary<string, string>();
            foreach (var item in pageSubstitutions)
            {
                if (!substitutions.ContainsKey(item.Reference))
                {
                    substitutions.Add(item.Reference, item.Path);   
                }
            }
            foreach (var item in resourceSubstitutions)
            {
                if (!substitutions.ContainsKey(item.Reference))
                {
                    substitutions.Add(item.Reference, item.Path);
                }
            }

            pagesTree.Accept(new UriSubstitutionVisitor(substitutions));
        }

        /// <summary>
        /// Find Base Template
        /// </summary>
        /// <param name="pageTypes">page Types</param>
        /// <returns></returns>
        private void FindBaseTemplate(List<PageType> pageTypes)
        {
            
        }
    }
}
