using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webpack.Domain.Analytics.Extensions;

namespace Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlNodeComparing
{
    public abstract class MappedHtmlNode : IEnumerable<MappedHtmlNode>
    {
        private readonly MappedHtmlNode parent;

        private readonly int level;

        protected MappedHtmlNode[] children;

        public MappedHtmlNode(MappedHtmlNode parent)
        {
            this.parent = parent;
            level = parent != null ? parent.Level + 1 : 0;
        }

        public MappedHtmlNode Parent
        {
            get { return parent; }
        }

        public MappedHtmlNode[] Children
        {
            get { return children; }
        }

        public int Level
        {
            get { return level; }
        }

        public virtual string PropertyName
        {
            get { return string.Empty; }
        }

        public abstract HtmlNode ReferenceNode
        {
            get;
        }

        public abstract List<HtmlNode> MappedNodes
        {
            get;
        }

        public virtual string ReferenceNodeText
        {
            get { return ReferenceNode.WriteTo(); }
        }

        public virtual string MappedNodeText
        {
            get
            {
                return MappedNodes.Aggregate(new StringBuilder(), (sb, n) => sb.Append(n.WriteTo())).ToString();
            }
        }
        
        public int Depth
        {
            get { return Children.Length > 0 ? 1 + Children.Max(n => n.Depth) : 0; }
        }

        public IEnumerator<MappedHtmlNode> GetEnumerator()
        {
            var queque = new Queue<MappedHtmlNode>();
            queque.Enqueue(this);

            while (!queque.IsEmpty())
            {
                var node = queque.Dequeue();
                queque.EnqueueAll(node.Children);
                
                yield return node;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public virtual HtmlNode CreateSkeletonNode()
        {
            var node = ReferenceNode.CloneNode(false);

            foreach (var child in children)
            {
                node.AppendChild(child.CreateSkeletonNode());
            }

            return node;
        }

        
    }
}
