// <copyright file="Page.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model.Entities
{
    using RazorEngine;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Webpack.Domain.Model.Logic;
    using System.Dynamic;
    using System.Text.RegularExpressions;
    using System.Text;
    using System.Xml.Serialization;
    
    /// <summary>
    /// A page of a website
    /// </summary>
    [DebuggerDisplay("name = {Name}")]
    public class Page : EntityBase, IVisitable<Page>
    {
        /// <summary>
        /// Backing field for <see cref="Children"/>.
        /// </summary>
        private List<Page> children = new List<Page>();

        /// <summary>
        /// Backing field for <see cref="Definitions"/>.
        /// </summary>
        private List<Property> properties = new List<Property>();

        private Guid parentID = Guid.Empty;
        private Page parent = null;
        private Guid pageTypeID = Guid.Empty;
        private PageType pageType = null;
        private string templateReference;

        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>Value of the property if it exists or <c>null</c> if it doesn't.</returns>
        public string this[string propertyName]
        {
            get 
            { 
                var property = Properties.SingleOrDefault(p => p.Name == propertyName);
                return property != null ? property.Value : null;
            }
        }

        /// <summary>
        /// Gets the values of properties.
        /// </summary>
        /// <param name="propertyNames">The names of properties to get.</param>
        /// <returns>Values of properties if they exist in the same order as argument propertyNames. 
        /// If a property name doest exist <c>null</c> null shall be returned instead.</returns>
        public string[] this[params string[] propertyNames]
        {
            get
            {
                return propertyNames.Select(n => this[n]).ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the children in a tree structure of pages
        /// </summary>
        [XmlArray]
        public virtual List<Page> Children
        {
            get { return this.children; }
            set { this.children = value ?? new List<Page>(); }
        }

        /// <summary>
        /// Gets or sets the the property values
        /// </summary>
        public virtual List<Property> Properties
        {
            get { return this.properties; }
            set { this.properties = value ?? new List<Property>(); }
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [XmlAttribute]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the the raw page associated with this page
        /// </summary>
        [XmlIgnore]
        public virtual RawPage RawPage { get; set; }

        [XmlIgnore]
        public virtual PageType PageType 
        {
            get { return PageType; }
            set 
            { 
                pageType = value;
                if (pageType != null)
                {
                    pageTypeID = pageType.ID;    
                }
            }
        }

        public Guid PageTypeID { 
            get { return pageTypeID; }
            set { pageTypeID = value; } /* DO NOT USE */
        }

        public string TemplateReference 
        {
            get { return templateReference ?? (RawPage != null ? RawPage.Reference : string.Empty); }
            set { templateReference = value; }
        }

        /// <summary>
        /// Gets or sets which page is the request for this page redirected to
        /// </summary>
        [XmlIgnore]
        public virtual Page RedirectTo { get; set; }

        /// <summary>
        /// Gets or sets the parent page, the root page has this property set to null
        /// </summary>
        [XmlIgnore]
        public virtual Page Parent { 
            get { return parent; }
            set 
            { 
                parent = value;
                if (parent != null)
                {
                    ParentID = parent.ID;    
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent page GUID,
        /// </summary>
        [XmlAttribute]
        public virtual Guid ParentID 
        {
            get { return parentID; } 
            set { parentID = value; } /*DO NOT USE*/
        }

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <param name="visitor">The visitor to be accepted.</param>
        public void Accept(IVisitor<Page> visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("visitor");
            }

            visitor.Visit(this);
        }

        /// <summary>
        /// Return a child page with the given name.
        /// </summary>
        /// <param name="name">The name of the child page to return.</param>
        /// <returns>A child page with the specified name or <c>null</c> if the page doesn't exist.</returns>
        public Page GetChild(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            return Children.FirstOrDefault(p => p.Name.Equals(name));
        }

        /// <summary>
        /// Return a child page with the given segments.
        /// </summary>
        /// <param name="name">The name of the child page to return.</param>
        /// <returns>A child page with the specified name or <c>null</c> if the page doesn't exist.</returns>
        public Page GetChild(string[] segments)
        {
            if (segments == null || segments.Any(s => s == null))
            {
                throw new ArgumentNullException("segments");
            }

            var length = segments.Length;
            for (int i = length; i > 0 ; i--)
            {
                var query = string.Join("/", segments.Take(i));
                var child = GetChild(query);
                if (child != null)
                {
                    return child;
                }
            }
            return null;
        }

        public IEnumerable<Page> GetDescendants(int level)
        {
            if (level < 0)
            {
                throw new ArgumentOutOfRangeException("level");
            }
            if (level == 0)
            {
                return Enumerable.Repeat(this, 1);
            }
            return Children.SelectMany(c => c.GetDescendants(level - 1));

        }

        public IEnumerable<Page> GetDescendantsAndSelf()
        {
            return Children.SelectMany(c => c.GetDescendantsAndSelf()).Prepend(this);

        }

        public string Render()
        {
            return Render(PageType.Template.Text);
        }

        public string Render(string template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            var model = new ExpandoObject() as IDictionary<string, object>;
            foreach (var property in Properties)
            {
                model.Add(property.Name, property.Value);
            }
            return Razor.Parse(template, model);
        }
    }
}