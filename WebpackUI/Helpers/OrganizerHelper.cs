// <copyright file="OrganizerHelper.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Tomáš Pouzar</author>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Xsl;
using Webpack.Domain.Model.Entities;
using WebpackUI.Models;

namespace WebpackUI.Helpers
{
    /// <summary>
    /// Helper that provides utility methods for Organizer process
    /// </summary>
    public class OrganizerHelper
    {
        /// <summary>
        /// Recursively updates all pages and their properties
        /// </summary>
        /// <param name="parent">Node from which to start updating</param>
        /// <param name="site">Whole site</param>
        /// <param name="config">Website's config</param>
        /// <returns></returns>
        public void UpdateChildren(Page parent, Site site, WebsiteModel config)
        {
            foreach (var page in parent.Children.Reverse<Page>())
            {
                var pageModel = GetPage(page.ID.ToString(), config);
                if (pageModel != null)
                {
                    page.Name = pageModel.Title;

                    if (page.Properties.Any())
                    {
                        foreach (var property in page.Properties.Reverse<Property>())
                        {
                            var propertyModel = GetProperty(property.ID.ToString(), pageModel);
                            if (propertyModel != null)
                            {
                                if (property.Name != propertyModel.Name)
                                {
                                    RenamePropertyReferences(site, property.Value, property.Name, propertyModel.Name, page.PageTypeID.ToString());
                                    //RenamePropertiesName(site.Root, property.Name, propertyModel.Name, page.PageTypeID.ToString());
                                    property.Name = propertyModel.Name;
                                }
                                property.Value = propertyModel.Html;
                            }
                            else
                            {
                                //RemovePropertiesFromPages(site.Root, property.Name, page.PageTypeID.ToString());
                                //RemovePropertyFromType(site, property.Name, page.PageTypeID.ToString());
                                
                                //RemovePropertyReferences(site, property.Name);
                                RemovePropertyFromTypeDefinition(site, property.Name, page.PageTypeID.ToString());
                                page.Properties.Remove(property);
                            }
                        }
                    }

                    if (page.Children.Any())
                    {
                        UpdateChildren(page, site, config);
                    }
                }
                else
                {
                    parent.Children.Remove(page);
                }
            }
        }

        /// <summary>
        /// Renames all references in XML
        /// </summary>
        /// <param name="site">Whole site</param>
        /// <param name="propertyText">Original property text</param>
        /// <param name="oldName">Old reference to be removed</param>
        /// <param name="newName">New reference</param>
        /// <param name="typeId">Type id</param>
        /// <returns></returns>
        public void RenamePropertyReferences(Site site, string propertyText, string oldName, string newName, string typeId)
        {
            //propertyText.Replace("@Raw(Model." + oldName + ")", "@Raw(Model." + newName + ")");

            foreach (var type in site.PageTypes)
            {
                if (type.ID.ToString() == typeId)
                {
                    foreach (var definition in type.Definitions)
                    {
                        if (definition.Name == oldName)
                        {
                            definition.Name = newName;
                            definition.TemplateReference = "@Raw(Model." + newName + ")";

                        }
                    }
                }
            
                foreach (var template in site.Templates)
                {
                    if (template != null && template.ID == type.TemplateID)
                    {
                        template.Text = template.Text.Replace("@Raw(Model." + oldName + ")", "@Raw(Model." + newName + ")");
                    }
                }
            }

        }

        /// <summary>
        /// Renames property names
        /// </summary>
        /// <param name="parent">Parent page</param>
        /// <param name="oldName">Old name to be replaced</param>
        /// <param name="newName">New name</param>
        /// <param name="typeId">Type id</param>
        /// <returns></returns>
        public void RenamePropertiesName(Page parent, string oldName, string newName, string typeId)
        {
            foreach (var page in parent.Children)
            {
                if (page.Properties.Any() && page.PageTypeID.ToString() == typeId)
                {
                    foreach (var property in page.Properties)
                    {
                        if (property.Name == oldName)
                        {
                            property.Name = newName;
                        }
                    }
                }


                if (page.Children.Any())
                {
                    RenamePropertiesName(page, oldName, newName, typeId);
                }
            }
        }

        /// <summary>
        /// Remove properties
        /// </summary>
        /// <param name="parent">Parent page</param>
        /// <param name="name">Property name</param>
        /// <param name="typeId">Type id</param>
        /// <returns></returns>
        public void RemovePropertiesFromPages(Page parent, string name, string typeId)
        {
            foreach (var page in parent.Children)
            {
                if (page.Properties.Any() && page.PageTypeID.ToString() == typeId)
                {
                    foreach (var property in page.Properties.Reverse<Property>())
                    {
                        if (property.Name == name)
                        {
                            page.Properties.Remove(property);
                        }
                    }
                }

                if (page.Children.Any())
                {
                    RemovePropertiesFromPages(page, name, typeId);
                }
            }
        }

        /// <summary>
        /// Remove property from types definition
        /// </summary>
        /// <param name="site">Site</param>
        /// <param name="name">Property name</param>
        /// <param name="typeId">Type id</param>
        /// <returns></returns>
        public void RemovePropertyFromTypeDefinition(Site site, string name, string typeId)
        {
            foreach (var type in site.PageTypes)
            {
                if (type.ID.ToString() == typeId)
                {
                    foreach (var definition in type.Definitions.Reverse<Definition>())
                    {
                        if(definition.Name == name)
                        {
                            type.Definitions.Remove(definition);
                        }
                    }
                }

                // Remove references
                foreach (var template in site.Templates)
                {
                    if (template != null && template.ID == type.TemplateID)
                    {
                        template.Text = template.Text.Replace("@Raw(Model." + name + ")", "");
                    }
                }
            }
        }


        /// <summary>
        /// Returns property
        /// </summary>
        /// <param name="id">Property id</param>
        /// <param name="pageModel">Page containing the property</param>
        /// <returns>
        /// Property
        /// </returns>
        public PagePropertyModel GetProperty(string id, Models.PageModel pageModel)
        {
            foreach (var property in pageModel.Properties)
            {
                if (property.Id == id)
                {
                    return property;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns page
        /// </summary>
        /// <param name="id">Page id</param>
        /// <param name="config">Website's config</param>
        /// <returns>
        /// Page
        /// </returns>
        public WebpackUI.Models.PageModel GetPage(string id, WebsiteModel config)
        {
            foreach (var page in config.OrganizerConfig.Pages)
            {
                if (page.Id == id)
                {
                    return page;
                }
                else if (page.Children.Any())
                {
                    var child = GetChildPage(id, page);
                    if (child != null)
                    {
                        return child;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns child page of a parent page
        /// </summary>
        /// <param name="id">Page id</param>
        /// <param name="parent">Parent page</param>
        /// <returns>
        /// Page
        /// </returns>
        public WebpackUI.Models.PageModel GetChildPage(string id, WebpackUI.Models.PageModel parent)
        {
            foreach (var page in parent.Children)
            {
                if (page.Id == id)
                {
                    return page;
                }
                else if (page.Children.Any())
                {
                    var child = GetChildPage(id, page);
                    if (child != null)
                    {
                        return child;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Recursively creates list of pages starting with parent
        /// </summary>
        /// <param name="parent">Parent page</param>
        /// <param name="site">Site</param>
        /// <returns>
        /// List of pages
        /// </returns>
        public List<WebpackUI.Models.PageModel> AddChildren(Page parent, Site site)
        {
            var children = new List<WebpackUI.Models.PageModel>();
            foreach (var page in parent.Children)
            {
                var pageModel = new WebpackUI.Models.PageModel();
                pageModel.Id = page.ID.ToString();
                pageModel.ParentId = parent.ID.ToString();
                pageModel.Title = page.Name;
                foreach (var type in site.PageTypes)
                {
                    if (type.ID == page.PageTypeID)
                    {
                        pageModel.Type = type.Name;
                        break;
                    }
                }

                foreach (var property in page.Properties)
                {
                    var propertyModel = new PagePropertyModel();
                    propertyModel.Id = property.ID.ToString();
                    propertyModel.Name = property.Name;
                    propertyModel.Html = property.Value;

                    pageModel.Properties.Add(propertyModel);
                }

                pageModel.Children = AddChildren(page, site);

                children.Add(pageModel);
            }

            return children;
        }

        /// <summary>
        /// Transforms XML
        /// </summary>
        /// <param name="xml">XML to be transformed</param>
        /// <param name="xsl">XSL transformation</param>
        /// <returns>
        /// Transformed XML
        /// </returns>
        public string XslTransform(string xml, string xsl)
        {
            string transformedXml = "";

            if (xsl == null || xml == null)
            {
                return transformedXml;
            }

            using (StringReader srxml = new StringReader(xml))
            using (StringReader srxsl = new StringReader(xsl))
            {
                using (XmlReader xrxml = XmlReader.Create(srxml))
                using (XmlReader xrxsl = XmlReader.Create(srxsl))
                {
                    XslCompiledTransform xslt = new XslCompiledTransform();
                    xslt.Load(xrxsl);

                    using (StringWriter sw = new StringWriter())
                    using (XmlWriter xwo = XmlWriter.Create(sw, xslt.OutputSettings))
                    {
                        xslt.Transform(xrxml, xwo);
                        transformedXml = sw.ToString();
                    }
                }
            }

            return transformedXml;
        }    
    }
}