using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlNodeComparing
{
    public class MappedHtmlNodeToProperty : MappedHtmlNode
    {
        private readonly string propertyNameForTemplate;
        private readonly List<HtmlNode> mappedNodes;
        private readonly HtmlNode referenceNode;
        private readonly string propertyName;

        public MappedHtmlNodeToProperty(MappedHtmlNode parent, string propertyNameForTemplate, string propertyName, params HtmlNode[] mappedNodes)
            : base(parent)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }
            if (string.IsNullOrWhiteSpace(propertyNameForTemplate))
            {
                throw new ArgumentNullException("propertyNameForTemplate");
            }
            if (mappedNodes == null)
            {
                throw new ArgumentNullException("mappedNodes");
            }
            this.propertyName = propertyName;
            this.propertyNameForTemplate = propertyNameForTemplate;
            this.mappedNodes = mappedNodes.ToList();
            referenceNode = HtmlTextNode.CreateNode(propertyNameForTemplate);
            base.children = new MappedHtmlNode[0];
        }

        public override string PropertyName
        {
            get { return propertyName; }
        }

        public override HtmlNode ReferenceNode
        {
            get { return referenceNode; }
        }

        public override List<HtmlNode> MappedNodes
        {
            get { return mappedNodes; }
        }

        public override HtmlNode CreateSkeletonNode()
        {
            return ReferenceNode;
        }
    }
}
