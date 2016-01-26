// <copyright file="IMenuAnalyzer.cs" company="ÚVT MU">
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
    using Webpack.Domain.Model.Entities;
    using Webpack.Domain.Model.Logic;

    public interface IMenuAnalyzer
    {
        List<PropertyDTO> IdentifyMenus(HtmlNode skeleton);
    }
}
