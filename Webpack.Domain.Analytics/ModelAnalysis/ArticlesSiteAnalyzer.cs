// <copyright file="ArticlesSiteAnalyzer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.ModelAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Articles Site Analyzer
    /// </summary>
    public class ArticlesSiteAnalyzer : ModelAnalyzer
    {
        /// <summary>
        /// Build
        /// </summary>
        /// <returns></returns>
        public override void Build()
        {
            AddLanguageVersion(Rule(c => c.Path.StartsWith("/cs") || c.Path == "/"));
            AddLanguageVersion(Rule(c => c.Path.StartsWith("/en")));

            var landing = new PageModel("landing") 
            { 
                Meets = Rule(c => c.Path == "/" || c.Path == "/en"), 
            };
            
            var articles = new PageModel("article") 
            { 
                Organizer = new HierarchyOrganizer(),
                Meets = Allways
            };

            var system = new PageModel("system")
            {
                FallsThrough = Rule(c => c.Url.SegmentPredicate(2, s => s == "system") && c.Url.SegmentExists(3))
            };

            var @event = new PageModel("event")
            {
                Meets = Allways
            };

            var eventsList = new PageModel("eventList")
            {
                Meets = Rule(p => p.Url.LastSegment.StartsWith("aktuality")),
                FallsThrough = Rule(p => p.Url.LastSegment == "detail")
            };
            eventsList.AddChildren(@event);

            Hook.AddChildren(landing, articles, system, eventsList);
        }
    }
}
