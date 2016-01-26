using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlNodeComparing
{
    public class MappedHtmlNodeTree : IEnumerable<MappedHtmlNode>
    {
        private readonly MappedHtmlNodeToReferenceNode root;

        private readonly int depth;

        public MappedHtmlNodeToReferenceNode Root
        {
            get { return root; }
        }

        public int Depth
        {
            get { return depth; }
        }

        public MappedHtmlNodeTree(HtmlNode referenceNode, HtmlNode comparedNode, IEqualityComparer<HtmlNode> comparer, PropertyNameState propertyNameState)
        {
            if (referenceNode == null)
            {
                throw new ArgumentNullException("referenceNode");
            }
            if (comparedNode == null)
            {
                throw new ArgumentNullException("comparedNode");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (propertyNameState == null)
            {
                throw new ArgumentNullException("propertyNameState");
            }

            root = new MappedHtmlNodeToReferenceNode(null, comparedNode, referenceNode, comparer, propertyNameState);
            depth = root.Depth;
        }

        /// <summary>
        /// Enumerates through the whole tree.
        /// </summary>
        /// <returns>Nodes from top to bottom.</returns>
        public IEnumerator<MappedHtmlNode> GetEnumerator()
        {
            var queue = new Queue<MappedHtmlNode>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var page = queue.Dequeue();
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

        public IDictionary<string, string> GetProperties()
        {
            return this.Where(mhn => !string.IsNullOrWhiteSpace(mhn.PropertyName))
                .ToDictionary(mhn => mhn.PropertyName, mhn => mhn.MappedNodeText);
        }

        public HtmlNode ReconstructSkeleton()
        {
            return Root.CreateSkeletonNode();
        }
    }
}
