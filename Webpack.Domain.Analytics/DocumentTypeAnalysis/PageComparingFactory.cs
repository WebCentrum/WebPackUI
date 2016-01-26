// <copyright file="PageComparingFactory.cs" company="ÚVT MU">
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
    using Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlMapping;

    public class PageComparingFactory : IPageComparingFactory
    {
        private readonly IEqualityComparer<HtmlNode> comparer;
        private readonly IPropertyFactory propertyFactory;
        private readonly ISkeletonExtractor skeletonExtractor;
        private readonly Builder builder;

        public PageComparingFactory(IEqualityComparer<HtmlNode> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            this.comparer = comparer;
            this.propertyFactory = new PropertyFactory();
            this.skeletonExtractor = new SkeletonExtractor(comparer);
            this.builder = new Builder(comparer, propertyFactory);
        }

        public IEqualityComparer<HtmlNode> Comparer 
        {
            get { return comparer; }
        }

        public virtual Builder Builder
        {
            get { return builder; }
        }

        public virtual ISkeletonExtractor SkeletonExtractor
        {
            get { return skeletonExtractor; }
        }

        public virtual IPropertyFactory PropertyFactory
        {
            get { return propertyFactory; }
        }
    }
}
