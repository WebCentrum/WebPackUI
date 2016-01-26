// <copyright file="PagesTree.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model.Logic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// A collection of web pages organized into a tree simulating a directory structure.
    /// </summary>
    public class PagesTree : IEnumerable<Page>, IVisitable<Page>
    {
        /// <summary>
        /// Delimits pages in a path
        /// </summary>
        public const char DELIMITER = '/';

        /// <summary>
        /// The root of the tree.
        /// </summary>
        private readonly Page root;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagesTree" /> class.
        /// </summary>
        public PagesTree() 
            : this(new Page 
            { 
                Name = string.Empty 
            })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagesTree" /> class.
        /// </summary>
        /// <param name="root">The root webpage of the site.</param>
        public PagesTree(Page root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            this.root = root;
        }

        public PagesTree(UrlNode urlStructure, IEnumerable<Page> pages, List<Page> unmapped) : this()
        {
            if (urlStructure == null)
            {
                throw new ArgumentNullException("urlStructure");
            }
            if (pages == null)
            {
                throw new ArgumentNullException("pages");
            }
            if (unmapped == null)
            {
                throw new ArgumentNullException("unmapped");
            }

            urlStructure.SetUrlOnPages(pages, Root);
            unmapped.AddRange(pages.Except(this, new PagePathEqualityComparer()));
        }

        /// <summary>
        /// Gets the root page of the tree.
        /// </summary>
        public Page Root 
        {
            get { return root; }
        }

        /// <summary>
        /// Enumerates through the whole tree.
        /// </summary>
        /// <returns>Pages from top to bottom.</returns>
        public IEnumerator<Page> GetEnumerator()
        {
            Queue<Page> queue = new Queue<Page>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                Page page = queue.Dequeue();
                foreach (var child in page.Children)
                {
                    queue.Enqueue(child);
                }

                yield return page;
            }
        }

        /// <summary>
        /// Enumerates through the whole tree.
        /// </summary>
        /// <returns>Pages from top to bottom.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Let the visitor visit every page in this tree.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        public void Accept(IVisitor<Page> visitor)
        {
            foreach (var page in this)
            {
                page.Accept(visitor);
            }
        }
    }
}
