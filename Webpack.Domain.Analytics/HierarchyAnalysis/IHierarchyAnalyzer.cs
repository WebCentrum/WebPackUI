// <copyright file="IHierarchyAnalyzer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.HierarchyAnalysis
{
    using System.Collections.Generic;
    using Webpack.Domain.Model.Entities;
    using Webpack.Domain.Model.Logic;

    /// <summary>
    /// Organizes raw rawPages into a tree-like structure.
    /// </summary>
    public interface IHierarchyAnalyzer
    {
        /// <summary>
        /// Organizes raw rawPages into a tree-like structure of rawPages, sets a rootUrlNode and sets redirects.
        /// </summary>
        /// <param name="rawPages">A list of downloaded rawPages.</param>
        /// <returns>A rootUrlNode page containing </returns>
        PagesTree DetermineHierarchy(List<Page> pages);
    }
}
