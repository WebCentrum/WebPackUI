// <copyright file="ResourceHandler.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.Crawler
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Webpack.Domain.Model;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Resource Handler
    /// </summary>
    public abstract class ResourceHandler
    {
        private readonly ResourceHandler nextHandler;

        protected static readonly Regex templateReferenceRegex = new Regex(@"(@Raw\(Model\.Resource\d+\))",
            RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        protected static readonly string templateReferenceFormat = "@Raw(Model.Resource{0})";
        private static int resourceCounter = 0;
        private readonly string dirName;

        protected abstract ResourceType ResourceType { get; }
        protected string DirName { get { return Path.GetTempPath() + dirName; } }

        protected virtual string[] Extensions { get { return new string[0]; } }

        /// <summary>
        /// Resource Handler
        /// </summary>
        /// <param name="dirName">dir Name</param>
        /// <param name="nextHandler">next Handler</param>
        /// <returns></returns>
        public ResourceHandler(string dirName, ResourceHandler nextHandler = null)
        {
            if (dirName == null)
            {
                throw new ArgumentNullException("dirName");
            }

            this.nextHandler = nextHandler;
            this.dirName = dirName;
        }

        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="uri">uri</param>
        /// <param name="doc">doc</param>
        /// <returns></returns>
        public Resource Handle(Uri uri, HtmlDocument doc)
        {
            var resource = Proccess(uri, doc);
            if (resource != null)
	        {
                return resource;
	        }
            return nextHandler != null ? nextHandler.Handle(uri, doc) : null;            
        }

        /// <summary>
        /// Proccess
        /// </summary>
        /// <param name="uri">uri</param>
        /// <param name="doc">doc</param>
        /// <returns></returns>
        protected virtual Resource Proccess(Uri uri, HtmlDocument doc)
        {
            var extension = Path.GetExtension(uri.AbsolutePath);
            if (!string.IsNullOrWhiteSpace(extension) && Extensions.Contains(extension))
            {
                //zkusit z uri vytahnout jmena css
                var fileName = Path.GetRandomFileName() + extension;
                var web = new WebClient();
                string path = null;
                try
                {
                    path = Path.Combine(Path.GetTempPath(), fileName);
                    web.DownloadFile(uri,path);
                    //using (var sw = File.CreateText(path))
                    //{
                    //    sw.Write(response);
                    //}
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(uri);
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine();
                    return null;
                }
                var res = CreateResource(uri);
                res.TextData = path;
                return res;
            }
            return null;
        }

        /// <summary>
        /// Create Resource
        /// </summary>
        /// <param name="uri">uri</param>
        /// <returns></returns>
        protected virtual Resource CreateResource(Uri uri)
        {
            return new Resource
            {
                ID = Guid.NewGuid(),
                Number = ++resourceCounter,
                TemplateReference = string.Format(templateReferenceFormat, resourceCounter),
                ResourceType = ResourceType,
                Url = new Url(uri),
                FileName = Path.GetFileName(uri.AbsolutePath)
            };
        }
    }

    /// <summary>
    /// Resource Equality Comparer
    /// </summary>
    public class ResourceEqualityComparer : IEqualityComparer<Resource>
    {
        private readonly IEqualityComparer<Url> comparer;

        /// <summary>
        /// Resource Equality Comparer
        /// </summary>
        /// <param name="comparer">comparer</param>
        /// <returns></returns>
        public ResourceEqualityComparer(IEqualityComparer<Url> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            this.comparer = comparer;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns></returns>
        public bool Equals(Resource x, Resource y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return comparer.Equals(x.Url, y.Url);
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns></returns>
        public int GetHashCode(Resource obj)
        {
            if (obj == null)
            {
                return base.GetHashCode();
            };
            return comparer.GetHashCode(obj.Url);
        }
    }
}
