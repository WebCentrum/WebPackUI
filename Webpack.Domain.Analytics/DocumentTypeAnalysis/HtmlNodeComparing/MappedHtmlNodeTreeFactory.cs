using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlNodeComparing
{
    public class MappedHtmlNodeTreeFactory: IMappedHtmlNodeTreeFactory
    {
        private readonly PropertyNameState propertyNameState;

        public MappedHtmlNodeTreeFactory(PropertyNameState propertyNameState)
        {
            if (propertyNameState == null)
            {
                throw new ArgumentNullException("propertyNameState");
            }

            this.propertyNameState = propertyNameState;
        }
        
        public MappedHtmlNodeTree Create(HtmlNode skeleton, string html)
        {
            if (skeleton == null)
            {
                throw new ArgumentNullException("skeleton");
            }
            if (string.IsNullOrWhiteSpace(html))
            {
                throw new ArgumentNullException("html");
            }

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var htmlRootNode = htmlDoc.DocumentNode;
            
            return new MappedHtmlNodeTree(skeleton, htmlRootNode, new HtmlNodeEqualityComparer(new HtmlAttributeEqualityComparer()), propertyNameState);
        }
    }
}
