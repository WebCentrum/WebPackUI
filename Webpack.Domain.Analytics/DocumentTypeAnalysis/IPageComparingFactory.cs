// <copyright file="IPageComparerFactory.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.DocumentTypeAnalysis
{
    using HtmlAgilityPack;
    using System.Collections.Generic;
    using Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlMapping;

    /// <summary>
    /// Gets an instance of <seealso cref="ISkeletonExtractor"/>.
    /// </summary>
    public interface IPageComparingFactory
    {
        /// <summary>
        /// Gets an instance of <seealso cref="Builder"/> which is used to fill a skeleton with properties.
        /// </summary>
        Builder Builder { get; }

        /// <summary>
        /// Gets an instance of <seealso cref="ISkeletonExtractor"/>.
        /// </summary>
        ISkeletonExtractor SkeletonExtractor { get; }

        /// <summary>
        /// Gets an instance of <seealso cref="IEqualityComparer<>"/> for comparing <seealso cref="HtmlNode"/>.
        /// </summary>
        IEqualityComparer<HtmlNode> Comparer { get; }

        /// <summary>
        /// Gets an instance of <seealso cref="IPropertyFactory"/>.
        /// </summary>
        IPropertyFactory PropertyFactory  { get; }
    }
}
