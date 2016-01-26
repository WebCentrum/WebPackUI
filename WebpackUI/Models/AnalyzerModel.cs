// <copyright file="AnalyzerModel.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Tomáš Pouzar</author>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebpackUI.Models
{
    /// <summary>
    /// Represents an Analyzer configuration
    /// </summary>    
    [DataContract(Name = "analyzerModel", Namespace = "")]
    public class AnalyzerModel
    {
        public AnalyzerModel()
        {
            Types = new List<PageTypeModel>();            
            RawPages = new List<RawPageModel>();

            LanguageVersions = new string[] { "always", "never" }; //, "specify"
            LanguageVersionRule = "always";
            LanguageVersionCustomRules = new string[] { };

            Hook = new string[] { };
        }
        
        [DataMember(Name = "types")]
        public List<PageTypeModel> Types { get; set; }

        [DataMember(Name = "languageVersions")]
        public string[] LanguageVersions { get; set; }

        [DataMember(Name = "languageVersionRule")]
        public string LanguageVersionRule { get; set; }

        [DataMember(Name = "languageVersionCustomRules")]
        public string[] LanguageVersionCustomRules { get; set; }

        [DataMember(Name = "hook")]
        public string[] Hook { get; set; }

        [DataMember(Name = "rawPages")]
        public List<RawPageModel> RawPages { get; set; }
    }

    /// <summary>
    /// Represents an unmodified page
    /// </summary>  
    [DataContract(Name = "rawPageModel", Namespace = "")]
    public class RawPageModel
    {        
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "sitePath")]
        public string SitePath { get; set; }


        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "selected")]
        public bool Selected { get; set; }
    }

    /// <summary>
    /// Represents a new type
    /// </summary>  
    [DataContract(Name = "pageType", Namespace = "")]
    public class PageTypeModel
    {
        public PageTypeModel()
        {
            RawPages = new List<RawPageModel>();
            AllowedDescendants = new string[] { };
        }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "allowedDescendants")]
        public string[] AllowedDescendants { get; set; }

        [DataMember(Name = "rawPages")]
        public List<RawPageModel> RawPages { get; set; }
    }
}