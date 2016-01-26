// <copyright file="HierarchyAnalyzer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.HierarchyAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Webpack.Domain.Model.Entities;
    using Webpack.Domain.Model.Logic;

    /// <summary>
    /// Organizes raw rawPages into a tree-like structure by comparing paths and hash codes of the entire page.
    /// </summary>
    public class HierarchyAnalyzer : IHierarchyAnalyzer
    {
        /// <summary>
        /// Organizes raw rawPages into a tree-like structure by comparing paths.
        /// </summary>
        /// <param name="rawPages">A list of downloaded rawPages.</param>
        /// <returns>A rootUrlNode page containing </returns>
        public PagesTree DetermineHierarchy(List<Page> pages)
        {
            if (pages == null)
            {
                throw new ArgumentNullException("pages");
            }
            var rawPages = pages.Select(p => p.RawPage);

            var pagesTree = new PagesTree();
            var root = pagesTree.Root;

            foreach (var rawPage in rawPages)
            {
                var path = rawPage.Path;
                var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim());

                var lastPage = root;
                foreach (var segment in segments)
                {
                    var foundPage = lastPage.GetChild(segment);
                    if (foundPage == null)
                    {
                        foundPage = new Page()
                        {
                            Name = segment,
                            Parent = lastPage
                        };
                        lastPage.Children.Add(foundPage);
                    }

                    lastPage = foundPage;
                }

                lastPage.RawPage = rawPage;
            }

            return pagesTree;
        }
    }
}
