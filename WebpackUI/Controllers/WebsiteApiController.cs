// <copyright file="WebsiteApiController.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Tomáš Pouzar</author>
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using Webpack.Domain.Analytics;
using Webpack.Domain.Analytics.Crawler;
using Webpack.Domain.Analytics.Crawler.Handlers;
using Webpack.Domain.Analytics.ModelAnalysis;
using Webpack.Domain.Model;
using Webpack.Domain.Model.Entities;
using WebpackUI.Analyzer;
using WebpackUI.Helpers;
using WebpackUI.Import;
using WebpackUI.Models;

namespace WebpackUI.Controllers
{
    /// <summary>
    /// API controller that contains database and WebPack service
    /// </summary>
    [PluginController("Webpack")]
    public class WebsiteApiController : UmbracoAuthorizedJsonController
    {
        /// <summary>
        /// Returns all websites from databse
        /// </summary>
        /// <returns>
        /// Collection of websites.
        /// </returns>
        [HttpGet]
        public IEnumerable<WebsiteMirror> GetAll()
        {
            var query = new Sql().Select("*").From("UMBRACOv2.webpackWebsites");
            return DatabaseContext.Database.Fetch<WebsiteMirror>(query);
        }

        /// <summary>
        /// Returns a specific website from databse
        /// </summary>
        /// <param name="id">Website's id</param>
        /// <returns>
        /// Website
        /// </returns>
        [HttpGet]
        public WebsiteMirror GetById(int id)
        {
            var query = new Sql().Select("*").From("UMBRACOv2.webpackWebsites").Where<WebsiteMirror>(x => x.Id == id);
            return DatabaseContext.Database.Fetch<WebsiteMirror>(query).FirstOrDefault();
        }

        /// <summary>
        /// Directly saves a website mirror to database
        /// </summary>
        /// <param name="website">Website mirror to be saved</param>
        /// <returns>
        /// Saved website mirror
        /// </returns>
        [HttpPost]
        public WebsiteMirror PostSaveMirror(WebsiteMirror website)
        {
            website.Config = JsonConvert.SerializeObject(new WebsiteModel(website.Name));
            
            DatabaseContext.Database.Save(website);            
            return website;
        }

        /// <summary>
        /// Maps website's config to database mirror and saves it
        /// </summary>
        /// <param name="config">Website's config to be saved</param>
        /// <returns></returns>
        [HttpPost]
        public void PostSaveWebsiteConfig(int id, WebsiteModel config)
        {
            WebpackApiController apiHelper = new WebpackApiController();
            WebsiteMirror website = GetById(id);

            website = apiHelper.MapModelToMirror(website, config);
            apiHelper.Save(website);
        }

        /// <summary>
        /// Deletes a website from database by id
        /// </summary>
        /// <param name="id">Website's id</param>
        /// <returns>
        /// Rows affected
        /// </returns>
        [HttpDelete]
        public int DeleteById(int id)
        {
            return DatabaseContext.Database.Delete<WebsiteMirror>(id);
        }

        /// <summary>
        /// Returns website's config from database
        /// </summary>
        /// <param name="id">Website's id</param>
        /// <returns>
        /// Website's config
        /// </returns>
        [HttpGet]
        public WebsiteModel GetWebsiteConfigById(int id)
        {
            WebsiteMirror website = GetById(id);
            if (website != null)
            {
                var config = (WebsiteModel)JsonConvert.DeserializeObject(website.Config, typeof(WebsiteModel));
                config.ExportConfig.Xml = website.Xml;
                config.ExportConfig.Xsl = website.Xsl;

                return config;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Runs Crawler process
        /// </summary>
        /// <param name="config">Website's config</param>
        /// <returns>
        /// Website's config
        /// </returns>
        [HttpPost]        
        public WebsiteModel PostRunCrawler(WebsiteModel config)
        {
            CrawlerHelper crawlHelper = new CrawlerHelper();

            return crawlHelper.CrawlWebsite(config);
        }

        /// <summary>
        /// Runs analyzer process
        /// </summary>
        /// <param name="config">Website's config</param>
        /// <returns>
        /// Website's config
        /// </returns>
        [HttpPost]
        [HttpGet]
        public WebsiteModel PostRunAnalyzer(WebsiteModel config)
        {
            AnalyzerHelper analyzerHelper = new AnalyzerHelper();

            return analyzerHelper.AnalyzeWebsite(config);
        }

        /// <summary>
        /// Transforms XML with XSL transformation
        /// </summary>
        /// <param name="config">Website's config</param>
        /// <returns>
        /// Website's config
        /// </returns>
        [HttpPost]
        public WebsiteModel PostTransformXml(WebsiteModel config)
        {
            OrganizerHelper orgHelper = new OrganizerHelper();

            config.ExportConfig.XmlPreview = config.ExportConfig.Xsl != string.Empty ? orgHelper.XslTransform(config.ExportConfig.Xml, config.ExportConfig.Xsl) : config.ExportConfig.Xml;
            
            return config;
        }

        /// <summary>
        /// Transforms XML with XSL transformation and imports website to Umbraco.
        /// </summary>
        /// <param name="config">Website's config</param>
        /// <returns>
        /// Website's config
        /// </returns>
        [HttpPost]
        public WebsiteModel PostRunImport(WebsiteModel config)
        {
            OrganizerHelper orgHelper = new OrganizerHelper();

            //apply XSL
            string finalXml;
            if(config.ExportConfig.Xsl == null || config.ExportConfig.Xsl.Length == 0)
            {
                finalXml = config.ExportConfig.Xml;
            }
            else
            {
                finalXml = orgHelper.XslTransform(config.ExportConfig.Xml, config.ExportConfig.Xsl);
            }

            // na serializer zavolat novou metodu, která deserializuje jen XML - do objektu Site
            Site site;

            using (var sw = new StringReader(finalXml))
            {
                var serializer = new XmlSerializer(typeof(Site));
                site = (Site)serializer.Deserialize(sw);
            }

            WebpackImporter importer = new WebpackImporter();
            importer.Import(site,ApplicationContext.Current);

            //ApplicationContext.Services.ContentTypeService
             
            return config;
        }

        /// <summary>
        /// Saves XML to database.
        /// </summary>
        /// <param name="config">Website's config to be saved</param>
        /// <returns></returns>
        [HttpPost]
        public void PostSaveXml(int id, WebsiteModel config)
        {
            WebpackApiController apiHelper = new WebpackApiController();
            WebsiteMirror website = GetById(id);

            website.Xml = config.ExportConfig.Xml;
            website.Xsl = config.ExportConfig.Xsl;

            apiHelper.Save(website);
        }
    }
}