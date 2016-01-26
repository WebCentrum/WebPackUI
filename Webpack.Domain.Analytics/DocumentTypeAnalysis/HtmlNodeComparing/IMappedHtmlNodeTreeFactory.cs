using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlNodeComparing
{
    public interface IMappedHtmlNodeTreeFactory
    {
        MappedHtmlNodeTree Create(HtmlNode skeleton, string html);
    }
}
