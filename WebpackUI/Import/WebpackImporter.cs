// <copyright file="WebpackImporter.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
// <author>Tomáš Pouzar</author>
namespace WebpackUI.Import
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Hosting;
    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using Webpack.Domain.Model;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Hooks up the importing process to Umbraco
    /// </summary>
    public class WebpackImporter : ApplicationEventHandler
    {
        private Dictionary<string, string> references = new Dictionary<string, string>();

        private string templatePrefix = "@inherits UmbracoTemplatePage" + Environment.NewLine 
            + "@{" + Environment.NewLine + "    Layout = null;" + Environment.NewLine + "}" + Environment.NewLine;

        /// <summary>
        /// Import
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="umbApplication">umb Application</param>
        /// <param name="appContext">app Context</param>
        /// <returns></returns>
        public void Import(Site site, ApplicationContext appContext)
        {
            references.Clear();
            
            var mediaService = appContext.Services.MediaService;
            var cts = appContext.Services.ContentTypeService;
            
            var folderTypeID = cts.GetMediaType(Constants.Conventions.MediaTypes.Folder).Id;
            var folders = mediaService.GetMediaOfMediaType(folderTypeID);
            foreach (var folder in folders)
            {
                if (folder.Name == site.Name)
                {
                    foreach (var media in mediaService.GetChildren(folder.Id))
                    {
                        mediaService.Delete(media);
                    }

                    mediaService.Delete(folder);
                    break;
                }
            }
            
            var serializer = new SiteSerializer();

            //Accents removal
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(site.Name.ToLowerInvariant());            
            string siteAlias = System.Text.Encoding.UTF8.GetString(tempBytes);
            siteAlias = siteAlias.Replace(" ", string.Empty);
            //site.Root.Name = site.Name;

            var fileService = appContext.Services.FileService;
                        
            var scriptPath = HostingEnvironment.MapPath("/scripts") + Path.DirectorySeparatorChar;
            var cssPath = HostingEnvironment.MapPath("/css") + Path.DirectorySeparatorChar;
            var resourcePath = site.Name + '/';
            
            var mediaParent = mediaService.CreateMediaWithIdentity(site.Name, -1, Constants.Conventions.MediaTypes.Folder);

            foreach (var resource in site.Resources)
            {
                var filePath = resource.TextData;
                
                if (resource.ResourceType == ResourceType.Javascript)
                {
                    try
                    {
                        var scripts = fileService.GetScripts(resource.TextData.ToLowerInvariant());
                        if (scripts != null && scripts.Any())
                        {
                            foreach (var item in scripts)
                            {
                                fileService.DeleteScript(item.Name);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }

                    var newPath = scriptPath + Path.GetFileName(filePath);
                    if (!newPath.EndsWith(".js"))
                    {
                        newPath = newPath + ".js";
                    }
                    
                    var script = new Script(filePath);
                    fileService.SaveScript(script);
                    System.IO.File.Copy(filePath, newPath, true);
                    AddToReferences(resource.TemplateReference, newPath, true);
                }
                else if (resource.ResourceType == ResourceType.Stylesheet)
                {
                    try
                    {
                        var stylesheets = fileService.GetStylesheets(resource.TextData.ToLowerInvariant());
                        if (stylesheets != null && stylesheets.Any())
                        {
                            foreach (var item in stylesheets)
                            {
                                fileService.DeleteStylesheet(item.Name);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }

                    var newPath = cssPath + Path.GetFileName(filePath);
                    if (!newPath.EndsWith(".css"))
                    {
                        newPath = newPath + ".css";
                    }
                    var stylesheet = new Stylesheet(newPath);
                    fileService.SaveStylesheet(stylesheet);
                    System.IO.File.Copy(filePath, newPath, true);
                    AddToReferences(resource.TemplateReference, newPath, true);
                }
                else if (resource.ResourceType == ResourceType.Image)
                {                    
                    var media = mediaService.CreateMedia(resource.FileName, mediaParent.Id, Constants.Conventions.MediaTypes.Image);
                    using (var fs = System.IO.File.OpenRead(filePath))
                    {
                        media.SetValue(Constants.Conventions.Media.File, resource.FileName, fs);
                    }
                    mediaService.Save(media);
                    AddToReferences(resource.TemplateReference, media.GetValue<string>(Constants.Conventions.Media.File));
                }
                else if (resource.ResourceType == ResourceType.File)
                {
                    var media = mediaService.CreateMedia(resource.FileName, mediaParent.Id, Constants.Conventions.MediaTypes.File);
                    using (var fs = System.IO.File.OpenRead(filePath))
                    {
                        media.SetValue(Constants.Conventions.Media.File, resource.FileName, fs);
                    }
                    mediaService.Save(media);
                    AddToReferences(resource.TemplateReference, media.GetValue<string>(Constants.Conventions.Media.File));
                }
            }


            var rootCT = new ContentType(-1);
            rootCT.Name = site.Name;
            rootCT.Alias = siteAlias;

            var oldCT = cts.GetContentType(rootCT.Alias);

            if (oldCT != null)
            {
                foreach (var child in cts.GetContentTypeChildren(oldCT.Id))
                {
                    cts.Delete(child);
                }

                cts.Delete(oldCT);
            }

            cts.Save(rootCT);
            
            var templatePathFormat = HostingEnvironment
                    .MapPath("/Views") + Path.DirectorySeparatorChar + "{0}.cshtml";

            var templates = new Dictionary<Guid, ITemplate>();
            var templateTexts = new Dictionary<Guid, string>();
            foreach (var template in site.Templates.WhereNotNull())
            {
                var templateAlias = siteAlias + template.Name.ToLowerInvariant();
                if (fileService.GetTemplate(templateAlias) != null)
	            {
                    fileService.DeleteTemplate(templateAlias);
	            }

                var templatePath = string.Format(templatePathFormat, templateAlias);
                var templateModel = new Umbraco.Core.Models.Template(templatePath, template.Name, templateAlias);

                fileService.SaveTemplate(templateModel);

                templateTexts.Add(template.ID, template.Text);
                templates.Add(template.ID, templateModel);
            }
                        
            var contentTypes = new Dictionary<Guid, ContentType>();
            foreach (var pageType in site.PageTypes)
            {
                var contentType = new ContentType(rootCT);
                contentType.Name = pageType.Name;
                contentType.Alias = siteAlias + pageType.Name;
                contentType.Icon = "icon-umb-content";

                cts.Save(contentType);

                contentType.AddPropertyGroup(site.Name);
                var propertyGroup = contentType.PropertyGroups[site.Name];

                bool hasTemplate = pageType.TemplateID != Guid.Empty;

                string templateBody = null;
                ITemplate contentTypeTemplate = null; 
                if (hasTemplate)
                {
                    contentTypeTemplate = templates[pageType.TemplateID];
                    templateBody = templateTexts[pageType.TemplateID];   
                }

                foreach (var definition in pageType.Definitions)
                {
                    var def = appContext.Services.DataTypeService
                        .GetDataTypeDefinitionByPropertyEditorAlias(Constants.PropertyEditors.TinyMCEAlias)
                        .First();
                    var pt = new PropertyType(def);
                    pt.Alias = siteAlias + definition.Name;
                    pt.Name = definition.Name;
                    contentType.AddPropertyType(pt, site.Name);
                    if (hasTemplate)
                    {
                        templateBody = templateBody.Replace(definition.TemplateReference, "@Umbraco.Field(\"" + pt.Alias + "\")");   
                    }
                }

                if (hasTemplate)
                {
                    contentTypeTemplate.Content = templateBody;
                    contentType.AllowedTemplates = contentTypeTemplate.AsEnumerableOfOne();
                    contentType.SetDefaultTemplate(contentTypeTemplate);

                    var templatePath = string.Format(templatePathFormat, contentTypeTemplate.Alias);
                    System.IO.File.WriteAllText(templatePath, templateBody, Encoding.UTF8);
                }
                
                cts.Save(contentType);
                contentTypes.Add(pageType.ID, contentType);
            }

            IContent root = null;
            var cs = appContext.Services.ContentService;
            var pages = new Dictionary<Guid, IContent>();
            var pageReferences = new Dictionary<string, IContent>();
            foreach (var page in site.Root.GetDescendantsAndSelf())
            {
                if(page.Name == null)
                {
                    page.Name = "unknown";
                }
                var parentID = page.ParentID == Guid.Empty ? -1 : pages[page.ParentID].Id;
                var cnt = cs.CreateContent(page.Name, parentID, contentTypes[page.PageTypeID].Alias);
                pages.Add(page.ID, cnt);
                foreach (var prop in page.Properties)
                {
                    //Accents removal
                    tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(prop.Name);
                    string propName = System.Text.Encoding.UTF8.GetString(tempBytes);
                    propName = propName.Replace(" ", string.Empty);
                    cnt.Properties[siteAlias + propName].Value = prop.Value;
                }
                cs.Save(cnt);
                if (page.ParentID == Guid.Empty)
	            {
		            root = cnt;
	            }
                if (!string.IsNullOrWhiteSpace(page.TemplateReference))
                {
                    pageReferences.Add(page.TemplateReference, cnt);   
                }
            }

            var pageRegex = new Regex(@"(@Raw\(Model\.Page\d+\))", RegexOptions.Compiled | RegexOptions.Multiline);
            var resourceRegex = new Regex(@"(@Raw\(Model\.Resource\d+\))", RegexOptions.Compiled | RegexOptions.Multiline);
            foreach (var page in root.Descendants().Where(c => c.Template != null))
	        {
		        foreach (var property in page.Properties)
	            {
                    var value = property.Value != null ? property.Value.ToString() : "";
                    value = pageRegex.Replace(value, m => "/{localLink:" + pageReferences[m.Value].Id + "}");
                    property.Value = resourceRegex.Replace(value, m => references[m.Value]);
	            }
                cs.Save(page);
	        }
            cs.PublishWithChildrenWithStatus(root, includeUnpublished: true);
            foreach (var template in templates.Values)
            {
                var sb = new StringBuilder();
                var templatePath = string.Format(templatePathFormat, template.Alias);
                var newContent = pageRegex.Replace(template.Content, m => string.Format("@Umbraco.NiceUrl({0})", pageReferences[m.Value].Id));
                newContent = resourceRegex.Replace(newContent, m => references[m.Value]);
                System.IO.File.WriteAllText(templatePath, templatePrefix + newContent, Encoding.UTF8);
            }
        }

        /// <summary>
        /// Add To References
        /// </summary>
        /// <param name="templateReference">template Reference</param>
        /// <param name="physicalPath">physical Path</param>
        /// <returns></returns>
        private void AddToReferences(string templateReference, string physicalPath, bool includeLeadingForwardSlash = false)
        {
            references.Add(templateReference, (includeLeadingForwardSlash ? "/" : string.Empty) 
                + physicalPath.Replace(HostingEnvironment.ApplicationPhysicalPath, string.Empty)
                    .Replace('\\', '/'));
        }
    }
}