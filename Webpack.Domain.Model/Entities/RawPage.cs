// <copyright file="RawPage.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Represents an unmodified page
    /// </summary>
    [DebuggerDisplay("path = {Path}")]
    public class RawPage : EntityBase
    {
        /// <summary>
        /// Backing field for <see cref="TextData"/> property.
        /// </summary>
        private readonly string textData;

        /// <summary>
        /// Backing field for <see cref="Path"/> property.
        /// </summary>
        private readonly string path;

        /// <summary>
        /// Backing field for <see cref="AlternativePaths"/> property.
        /// </summary>
        private readonly List<string> alternativePaths = new List<string>();

        /// <summary>
        /// Backing field for <see cref="Links"/> property.
        /// </summary>
        private readonly List<Uri> links;

        /// <summary>
        /// Backing field for <see cref="Url"/> property.
        /// </summary>
        private readonly Url url;

        /// <summary>
        /// Backing field for <see cref="IsHandled"/> property.
        /// </summary>
        private bool isHandled = false;

        /// <summary>
        /// Backing field for <see cref="Reference"/> property.
        /// </summary>
        private readonly string reference;

        /// <summary>
        /// Initializes a new instance of the <see cref="RawPage" /> class.
        /// </summary>
        /// <param name="uri">The uri to the page on the site</param>
        /// <param name="textData">Raw text of the page.</param>
        /// <param name="links">Links recovered from this page.</param>
        /// <param name="alternativePaths">The path the request was redirected to.</param>
        public RawPage(Uri uri, string textData, string reference, IEnumerable<Uri> links, params string[] alternativePaths)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }
            
            if (string.IsNullOrWhiteSpace(textData))
            {
                throw new ArgumentNullException("textData");
            }

            if (links == null)
            {
                throw new ArgumentNullException("links");
            }

            this.path = uri.PathAndQuery;
            this.url = new Url(uri);
            this.textData = textData;
            this.links = links.ToList();
            var alternative = (alternativePaths ?? Enumerable.Empty<string>())
                .Where(p => p != path);
            this.alternativePaths.AddRange(alternative);
            this.reference = reference;
        }

        /// <summary>
        /// Gets the code to this raw page.
        /// </summary>
        public virtual string TextData
        {
            get { return textData; }
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        public virtual string Path 
        {
            get { return path; }
        }

        /// <summary>
        /// Gets the responce path.
        /// </summary>
        public virtual List<string> AlternativePaths
        {
            get { return alternativePaths; }
        }

        /// <summary>
        /// Gets the paths that link to this page.
        /// </summary>
        public virtual IEnumerable<string> Paths
        {
            get 
            { 
                yield return Path;
                foreach (var path in AlternativePaths)
	            {
		            yield return path;
	            }
            }
        }

        /// <summary>
        /// Gets a list of links found on this page.
        /// </summary>
        public virtual List<Uri> Links
        {
            get { return links; }
        }

        /// <summary>
        /// Gets the url.
        /// </summary>
        public virtual Url Url
        {
            get { return url; }
        }

        public virtual bool IsHandled
        {
            get { return isHandled; }
            set { isHandled = value; }
        }

        public virtual string Reference
        {
            get { return reference; }
        }
    }
}