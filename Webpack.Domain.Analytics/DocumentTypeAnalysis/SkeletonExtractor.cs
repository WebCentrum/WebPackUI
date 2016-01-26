// <copyright file="HtmlAgilityPackComparer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.DocumentTypeAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HtmlAgilityPack;
    using Webpack.Domain.Model.Entities;
    using Webpack.Domain.Analytics.Extensions;

    /// <summary>
    /// Compares rawPages.
    /// </summary>
    public class SkeletonExtractor : ISkeletonExtractor
    {
        /// <summary>
        /// Backing field for <see cref="NodeEqualtyComparer"/>.
        /// </summary>
        private readonly IEqualityComparer<HtmlNode> nodeEqualityCompaper;

        /// <summary>
        /// A page that is going to be built while passing through each page.
        /// </summary>
        private HtmlNode referenceNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletonExtractor" /> class.
        /// </summary>
        /// <param name="nodeEqualityCompaper">Used for comparing two nodes.</param>
        public SkeletonExtractor(IEqualityComparer<HtmlNode> nodeEqualityCompaper)
        {
            if (nodeEqualityCompaper == null)
            {
                throw new ArgumentNullException("nodeEqualtyComparer");
            }

            this.nodeEqualityCompaper = nodeEqualityCompaper;
        }

        /// <summary>
        /// Gets the comparer used for comparing nodes.
        /// </summary>
        private IEqualityComparer<HtmlNode> NodeEqualtyCompaper
        {
            get { return nodeEqualityCompaper; }
        }

        /// <summary>
        /// Gets the resulting skeleton.
        /// </summary>
        public HtmlNode Result
        {
            get { return referenceNode; }
        }

        /// <summary>
        /// Converts rawPages to HTML documents.
        /// </summary>
        /// <param name="page">The page being converted.</param>
        public void Visit(Page page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            if (page.RawPage == null)
            {
                return;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(page.RawPage.TextData);
            var comparedNode = doc.DocumentNode;

            if (referenceNode == null)
            {
                var rdoc = new HtmlDocument();
                rdoc.LoadHtml(page.RawPage.TextData);
                referenceNode = rdoc.DocumentNode;
            }
            else
            {
                var refQueue = new Queue<HtmlNode[]>();
                refQueue.Enqueue(new[] { referenceNode });
                var comQueue = new Queue<HtmlNode[]>();
                comQueue.Enqueue(new[] { comparedNode });

                while (refQueue.Count > 0)
                {
                    var refNodes = refQueue.Dequeue();
                    var comNodes = new Queue<HtmlNode>(comQueue.IsEmpty() ? new HtmlNode[0] : comQueue.Dequeue());

                    foreach (var refNode in refNodes)
                    {
                        var comNode = comNodes.DequeueWhile(n => !NodeEqualtyCompaper.Equals(n, refNode));
                        if (comNode != null)
                        {
                            refQueue.Enqueue(refNode.ChildNodes.ToArray());
                            comQueue.Enqueue(comNode.ChildNodes.ToArray());
                        }
                        else
                        {
                            refNode.Remove();
                        }
                    }
                }
            }
        }
    }
}
