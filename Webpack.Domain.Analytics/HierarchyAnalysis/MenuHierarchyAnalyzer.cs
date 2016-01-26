// <copyright file="MenuHierarchyAnalyzer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.HierarchyAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Model.Entities;
    using Webpack.Domain.Model.Logic;

    /// <summary>
    /// Menu Hierarchy Analyzer
    /// </summary>
    public class MenuHierarchyAnalyzer : IHierarchyAnalyzer
    {
        private readonly MenuHierarchyFinder menuHierarchyFinder;
        private readonly string[] propertyNames;

        private readonly List<Page> unmappedPages = new List<Page>();

        /// <summary>
        /// Menu Hierarchy Analyzer
        /// </summary>
        /// <param name="menuHierarchyFinder">menu Hierarchy Finder</param>
        /// <param name="propertyNames">property Names</param>
        /// <returns></returns>
        public MenuHierarchyAnalyzer(MenuHierarchyFinder menuHierarchyFinder, params string[] propertyNames)
        {
            if (menuHierarchyFinder == null)
            {
                throw new ArgumentNullException("menuHierarchyFinder");
            }
            if (propertyNames == null)
            {
                throw new ArgumentNullException("propertyNames");
            }

            this.menuHierarchyFinder = menuHierarchyFinder;
            this.propertyNames = propertyNames;
        }

        public List<Page> UnmappedPages
        {
            get { return unmappedPages; }
        }

        /// <summary>
        /// Determine Hierarchy
        /// </summary>
        /// <param name="pages">pages</param>
        /// <returns></returns>
        public PagesTree DetermineHierarchy(List<Page> pages)
        {
            if (pages == null)
            {
                throw new ArgumentNullException("pages");
            }

            var propertyData = pages.ToDictionary(p => p, p => propertyNames.ToDictionary(prop => prop, prop => p[prop]));
            var urlStructure = menuHierarchyFinder.FindHierarchy(propertyData);
            return new PagesTree(urlStructure, pages, unmappedPages);
        }
    }
}
