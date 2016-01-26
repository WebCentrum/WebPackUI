// <copyright file="CrawlerModel.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Tomáš Pouzar</author>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Webpack.Domain.Analytics.Crawler;

namespace WebpackUI.Models
{
    /// <summary>
    /// Represents a Crawler configuration
    /// </summary>  
    [DataContract(Name = "crawlerModel", Namespace = "")]
    public class CrawlerModel
    {
        public CrawlerModel()
        {
            SiteUrl = "http://";
            CountLimit = 500;
            DepthLimit = 500;
            IgnoredPaths = new string[] { };
            IgnoredPrefixes = new string[] { };
            Directory = "webpack";
        }

        //[DataMember(Name = "name")]
        //public string SiteName { get; set; }

        [DataMember(Name = "siteUrl")]
        public string SiteUrl { get; set; }

        [DataMember(Name = "countLimit")]
        public int CountLimit { get; set; }

        [DataMember(Name = "depthLimit")]
        public int DepthLimit { get; set; }

        [DataMember(Name = "ignoredPaths")]
        public string[] IgnoredPaths { get; set; }

        [DataMember(Name = "ignoredPrefixes")]
        public string[] IgnoredPrefixes { get; set; }

        [DataMember(Name = "directory")]
        public string Directory { get; set; }
    }
}