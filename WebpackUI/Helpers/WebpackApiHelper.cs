// <copyright file="WebpackApiHelper.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Tomáš Pouzar</author>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WebpackUI.Models;
using WebpackUI.Analyzer;

using Umbraco.Web.Mvc;
using Umbraco.Web.Editors;
using Umbraco.Core.Persistence;

using Webpack.Domain.Analytics;
using Webpack.Domain.Analytics.Crawler;
using Webpack.Domain.Analytics.Crawler.Handlers;
using Webpack.Domain.Analytics.ModelAnalysis;
using Webpack.Domain.Model;
using Webpack.Domain.Model.Entities;

using Newtonsoft.Json;
using System.IO;
using System.Xml.Serialization;

namespace WebpackUI.Helpers
{
    /// <summary>
    /// Helper that provides utility methods for WebsiteApiController
    /// </summary>
    public class WebpackApiController : UmbracoAuthorizedJsonController
    {
        /// <summary>
        /// Saves website to database
        /// </summary>
        /// <param name="website">Website's mirror</param>
        /// <returns>
        /// Website's mirror
        /// </returns>
        public WebsiteMirror Save(WebsiteMirror website)
        {
            if (website.Config == "" || website.Config == "null")
            {
                website.Config = JsonConvert.SerializeObject(new WebsiteModel(website.Name));
            }

            if (website.Id > 0)
            {
                DatabaseContext.Database.Update(website);
            }
            else
            {
                DatabaseContext.Database.Save(website);
            }

            return website;
        }

        /// <summary>
        /// Updates website's config if needed and maps it to database mirror
        /// </summary>
        /// <param name="website">Website's mirror for XML comparsion</param>
        /// <param name="config">Website's config</param>
        /// <returns>
        /// Database mirror
        /// </returns>
        public WebsiteMirror MapModelToMirror(WebsiteMirror website, WebsiteModel config)
        {
            OrganizerHelper orgHelper = new OrganizerHelper();

            website.Name = config.Name;

            website.Xml = config.ExportConfig.Xml;
            website.Xsl = config.ExportConfig.Xsl;

            config.ExportConfig.Xml = string.Empty;
            config.ExportConfig.Xsl = string.Empty;

            // If XML is downloaded, retrieve it from the database, and deserialize and update each page of config.OrganizerModel
            if (website.Xml != string.Empty && website.Xml != null)
            {
                Site site;

                // Deserialize XML
                using (var sw = new StringReader(website.Xml))
                {
                    var serializer = new XmlSerializer(typeof(Site));
                    site = (Site)serializer.Deserialize(sw);
                }

                // Find changes in pages and properties (names, properties) and handle them
                orgHelper.UpdateChildren(site.Root, site, config);                

                // Turn the object into XML
                using (var sw = new StringWriter())
                {
                    var serializer = new XmlSerializer(typeof(Site));
                    serializer.Serialize(sw, site);

                    website.Xml = sw.ToString();
                }

                // Update each page of config.OrganizerModel
                config.OrganizerConfig.Pages = orgHelper.AddChildren(site.Root, site);
            }

            // Finally, update website's config
            website.Config = JsonConvert.SerializeObject(config);

            return website;
        }
    }
}