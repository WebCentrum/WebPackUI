// <copyright file="WebsiteAnalyzer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Tomáš Pouzar</author>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webpack.Domain.Analytics.ModelAnalysis;
using Webpack.Domain.Model.Entities;
using WebpackUI.Models;

namespace WebpackUI.Analyzer
{    
    /// <summary>
    /// Analyzer model defining a way of analyzing chosen website.
    /// </summary>
    public class WebsiteAnalyzer : ModelAnalyzer
    {
        private AnalyzerModel config;

        /// <summary>
        /// WebsiteAnalyzer constructor
        /// </summary>        
        /// <returns></returns> 
        public WebsiteAnalyzer(AnalyzerModel _config)
        {
            config = _config;
        }

        /// <summary>
        /// Build Analyzer model
        /// </summary>        
        /// <returns></returns>       
        public override void Build()
        {

            //Language rules
            switch (config.LanguageVersionRule)
            {
                case "always":
                    AddLanguageVersion(Allways);
                    break;
                case "never":
                    AddLanguageVersion(Never);
                    break;
                //case "specify":
                //    foreach(var rule in config.LanguageVersionCustomRules)
                //        AddLanguageVersion(rule);
                //    break;
            }

            //Assigning types to pages:
            Dictionary<string, Webpack.Domain.Analytics.ModelAnalysis.PageModel> dictionary = new Dictionary<string, Webpack.Domain.Analytics.ModelAnalysis.PageModel>();
            foreach (var type in config.Types)
            {
                dictionary.Add(type.Name, new Webpack.Domain.Analytics.ModelAnalysis.PageModel(type.Name));
                List<Func<RawPage, bool>> meetPredicates = new List<Func<RawPage, bool>>();
                List<Func<RawPage, bool>> fallsThroughPredicates = new List<Func<RawPage, bool>>();

                foreach (var page in type.RawPages)
                {
                    //Rule 1: If the paths equals, we found our page.
                    meetPredicates.Add(new Func<RawPage, bool>(c => c.Path == page.SitePath));
                    //Rule 2: If the path is longer, we know we must continue.
                    fallsThroughPredicates.Add(new Func<RawPage, bool>(c => c.Path.StartsWith(page.SitePath + "/")));
                }

                //Add all pages rules of type 1
                dictionary[type.Name].Meets = Rule(c =>
                {
                    foreach (var predicate in meetPredicates)
                    {
                        if (predicate(c))
                            return true;
                    }

                    return false;
                });

                //Add all pages rules of type 2
                dictionary[type.Name].FallsThrough = Rule(c =>
                {
                    foreach (var predicate in meetPredicates)
                    {
                        if (predicate(c))
                            return false;
                    }

                    foreach (var predicate in fallsThroughPredicates)
                    {
                        if (predicate(c))
                            return true;
                    }

                    return false;
                });
            }

            //Hiearchy rules:
            foreach (var type in config.Types)
            {
                //if (config.Hook.Contains(type.Name))
                Hook.AddChildren(dictionary[type.Name]);

                foreach (var childType in type.AllowedDescendants)
                {
                    //If one type may have descendant of the same type, add page of the type HiearchyOrganizer
                    if (type.Name != childType)
                        dictionary[type.Name].AddChildren(dictionary[childType]);
                    else
                        dictionary[type.Name].Organizer = new HierarchyOrganizer();
                }
            }
        }
    }
}