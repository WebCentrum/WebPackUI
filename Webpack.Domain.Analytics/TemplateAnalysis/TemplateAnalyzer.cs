// <copyright file="TemplateAnalyzer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.TemplateAnalysis
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Template Analyzer
    /// </summary>
    public class TemplateAnalyzer
    {
        /// <summary>
        /// Analyze
        /// </summary>
        /// <param name="templates">templates</param>
        /// <returns></returns>
        public Template Analyze(List<Template> templates)
        {
            var nodes = templates.Select(t => 
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(t.Text);
                    return doc.DocumentNode;
                }).ToList();

            var length = templates.Select(t => t.Text.Length).Min();
            int i = 0;
            for (; i < length; i++)
			{
			    var allSame = templates.Select(t => t.Text[i]).Distinct().Count() == 1;
                if (!allSame)
	            {
		            break;
	            }
			}



            return new Template
            {
                Name = "BASE",
                Text = ""
            };
        }
    }
}
