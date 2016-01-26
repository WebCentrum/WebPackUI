// <copyright file="IMenuAnalyzer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.DocumentTypeAnalysis
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlMapping;

    public interface IPropertyFactory
    {
        List<PropertyDTO> AllProperties { get; }
        Regex PropertyTemplateReferenceRegex { get; }

        PropertyDTO Copy(PropertyDTO property, string value = null);
        PropertyDTO GetNew(string value = null);
        PropertyDTO Get(int number, string value = null);
        PropertyDTO ParseTemplateReference(HtmlNode node);
        PropertyDTO ParseTemplateReference(string text);
        string[] SplitText(string text);
    }
}
