// <copyright file="MappedNode.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlMapping
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Mapped Node
    /// </summary>
    [DebuggerDisplay("model = {Model.Name}, mapee = {Mapee.Name}")]
    public class MappedNode
    {
        private readonly List<MappedNode> children = new List<MappedNode>();

        private readonly MappedNode parent;

        private readonly HtmlNode model;

        private readonly HtmlNode mapee;

        private readonly PropertyDTO property;

        /// <summary>
        /// Mapped Node
        /// </summary>
        /// <param name="parent">parent</param>
        /// <returns></returns>
        private MappedNode(MappedNode parent)
        {
            this.parent = parent;
            if (parent != null)
            {
                parent.Children.Add(this);
            }
        }

        /// <summary>
        /// Mapped Node
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="model">model</param>
        /// <param name="mapee">mapee</param>
        /// <returns></returns>
        public MappedNode(MappedNode parent, HtmlNode model, HtmlNode mapee)
            : this(parent)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            if (mapee == null)
            {
                throw new ArgumentNullException("mapee");
            }

            this.model = model;
            this.mapee = mapee;
        }

        /// <summary>
        /// Mapped Node
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="property">property</param>
        /// <param name="mapee">mapee</param>
        /// <returns></returns>
        public MappedNode(MappedNode parent, PropertyDTO property, HtmlNode mapee)
            : this(parent)
        {
            if (property == null && mapee == null)
            {
                throw new ArgumentNullException(property == null ? "property" : "mapee");
            }

            this.property = property;
            this.mapee = mapee;
        }

        public HtmlNode Model 
        {
            get { return model; }
        }

        public HtmlNode Mapee 
        {
            get { return mapee; }
        }

        public MappedNode Parent 
        {
            get { return parent; }
        }

        public List<MappedNode> Children
        {
            get { return children; }
        }

        public PropertyDTO Property
        {
            get { return property; }
        }

        public bool IsProperty
        {
            get { return Model == null; }
        }
    }
}
