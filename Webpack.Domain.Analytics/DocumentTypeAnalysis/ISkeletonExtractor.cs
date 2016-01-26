// <copyright file="IPageComparer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.DocumentTypeAnalysis
{
    using HtmlAgilityPack;
    using Webpack.Domain.Model.Entities;
    using Webpack.Domain.Model.Logic;

    /// <summary>
    /// Compares rawPages.
    /// </summary>
    public interface ISkeletonExtractor : IVisitor<Page>
    {
        HtmlNode Result { get; }
    }
}
