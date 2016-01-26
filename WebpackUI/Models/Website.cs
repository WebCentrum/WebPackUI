// <copyright file="Website.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Tomáš Pouzar</author>
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Helpers;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace WebpackUI.Models
{
    /// <summary>
    /// Represents website configuration
    /// </summary>  
    [DataContract(Name = "websiteModel", Namespace = "")]
    public class WebsiteModel
    {
        public WebsiteModel() 
        {
            LastOpenConfig = 1;
            // vytvořit instance subobjektů- pro případ nové website
            CrawlerConfig = new CrawlerModel();
            AnalyzerConfig = new AnalyzerModel();
            OrganizerConfig = new OrganizerModel();
            ExportConfig = new ExportModel();
        }

        public WebsiteModel(string name) : this()
        {
            Name = name;
        }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "lastOpenConfig")]
        public int LastOpenConfig { get; set; }


        [DataMember(Name = "crawlerConfig")]
        public CrawlerModel CrawlerConfig { get; set; }

        [DataMember(Name = "analyzerConfig")]
        public AnalyzerModel AnalyzerConfig { get; set; }        

        [DataMember(Name = "organizerConfig")]
        public OrganizerModel OrganizerConfig { get; set; }

        [DataMember(Name = "exportConfig")]
        public ExportModel ExportConfig { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Represents database record
    /// </summary>  
    [TableName("webpackWebsites")]        
    [DataContract(Name = "websiteMirror", Namespace = "")]
    public class WebsiteMirror
    {
        public WebsiteMirror()
        {
            Config = JsonConvert.SerializeObject(new WebsiteModel());
        }

        public WebsiteMirror(string name) : this()
        {
            Name = name;
        }
        
        [PrimaryKeyColumn(AutoIncrement = true)]
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
        
        [DataMember(Name = "config")]
        public string Config { get; set; }

        [DataMember(Name = "xml")]
        public string Xml { get; set; }

        [DataMember(Name = "xsl")]
        public string Xsl { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}