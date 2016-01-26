using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webpack.Domain.Analytics.Extensions;

namespace Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlNodeComparing
{
    public class MappedHtmlNodeToReferenceNode : MappedHtmlNode
    {
        private readonly HtmlNode referenceNode;
        private readonly HtmlNode htmlNode;
        private readonly IEqualityComparer<HtmlNode> comparer;
        private readonly PropertyNameState propertyNameState;
        
        private bool isProperty = false;

        public MappedHtmlNodeToReferenceNode(MappedHtmlNode parent, HtmlNode htmlNode, HtmlNode referenceNode, IEqualityComparer<HtmlNode> comparer, PropertyNameState propertyNameState)
            : base(parent)
        {
            if (htmlNode == null)
            {
                throw new ArgumentNullException("htmlNode");
            }

            if (referenceNode == null)
            {
                throw new ArgumentNullException("referenceNode");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            if (propertyNameState == null)
            {
                throw new ArgumentNullException("propertyNameState");
            }

            this.htmlNode = htmlNode;
            this.referenceNode = referenceNode;
            this.comparer = comparer;
            this.propertyNameState = propertyNameState;

            base.children = MapChildren();


        }

        private MappedHtmlNode[] MapChildren()
        {
            var children = new List<MappedHtmlNode>();
            var htmlChildren = new Queue<HtmlNode>(htmlNode.ChildNodes);
            var referenceChildren = new Queue<HtmlNode>(referenceNode.ChildNodes);

            var list = new List<HtmlNode>();
            while (!htmlChildren.IsEmpty())
            {
                if (referenceChildren.IsEmpty())
                {
                    if (list.Count > 0)
	                {
		                throw new InvalidOperationException("The node passed on as a reference is not a subset of the html node");
	                }
                    CreateProperty(htmlChildren.ToList(), children);
                    break;
                }
                else if (propertyNameState.Check(referenceChildren))
                {
                    isProperty = true;
                    continue;
                }

                var htmlChild = htmlChildren.Dequeue();
                if (comparer.Equals(htmlChild, referenceChildren.Peek()))
                {
                    CreateProperty(list, children);
                    children.Add(new MappedHtmlNodeToReferenceNode(this, htmlChild, referenceChildren.Dequeue(), comparer, propertyNameState));
                } 
                else
	            {
                    list.Add(htmlChild);
	            }
            }
            if (!referenceChildren.IsEmpty() && propertyNameState.Check(referenceChildren))
            {
                isProperty = true;
                CreateProperty(list, children);
            }

            return children.ToArray();
        }

        private void CreateProperty(List<HtmlNode> list, List<MappedHtmlNode> children)
        {
            if (list.Count > 0 || isProperty)
            {
                var propertyDTO = propertyNameState.GetProperty();
                children.Add(new MappedHtmlNodeToProperty(this, propertyDTO.TemplateReference, propertyDTO.Name, list.ToArray()));

                isProperty = false;
            }
            list.Clear();
            propertyNameState.Reset();
        }

        public override HtmlNode ReferenceNode
        {
            get { return referenceNode; }
        }

        public override List<HtmlNode> MappedNodes
        {
            get { return new List<HtmlNode>() { htmlNode }; }
        }
    }
}
