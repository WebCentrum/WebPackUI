// <copyright file="HtmlNodeEqualityComparer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.DocumentTypeAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HtmlAgilityPack;

    /// <summary>
    /// Compares 2 nodes.
    /// </summary>
    public class HtmlNodeEqualityComparer : IEqualityComparer<HtmlNode>
    {
        /// <summary>
        /// Used for comparing html attributes.
        /// </summary>
        private readonly IEqualityComparer<HtmlAttribute> htmlAttributeComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNodeEqualityComparer"/> class.
        /// </summary>
        /// <param name="htmlAttributeComparer">Used for comparing attributes within element nodes.</param>
        public HtmlNodeEqualityComparer(IEqualityComparer<HtmlAttribute> htmlAttributeComparer)
        {
            if (htmlAttributeComparer == null)
            {
                throw new ArgumentNullException("htmlAttributeComparer");
            }

            this.htmlAttributeComparer = htmlAttributeComparer;
        }

        /// <summary>
        /// Compares 2 nodes.
        /// </summary>
        /// <param name="x">First item to compare.</param>
        /// <param name="y">Second item to compare.</param>
        /// <returns><c>true</c> if equal, <c>false</c> otherwise.</returns>
        public bool Equals(HtmlNode x, HtmlNode y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }
            
            bool type = x.NodeType == y.NodeType;
            bool name = string.Equals(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase);

            if (type && x.NodeType == HtmlNodeType.Text)
            {
                return x.InnerText == y.InnerText;
            }
            
            bool attributes = true;
            if (x.Attributes.Count != y.Attributes.Count)
            {
                attributes = false;
            }
            else
            {
                var nodeXAttributes = x.Attributes;
                var nodeYAttributes = y.Attributes;

                var attributeIntersection = nodeXAttributes.Intersect(nodeYAttributes, htmlAttributeComparer);

                attributes = attributeIntersection.Count() == nodeXAttributes.Count;
            }

            return type && name && attributes;
        }

        /// <summary>
        /// Returns a hash code for the html item.
        /// </summary>
        /// <param name="obj">An html item to generate a hash code for.</param>
        /// <returns>The generated hash code.</returns>
        public int GetHashCode(HtmlNode obj)
        {
            if (obj == null)
            {
                return 0;
            }

            int hashcode = 17;
            unchecked
            {
                hashcode = hashcode * obj.Name.ToUpperInvariant().GetHashCode();
                hashcode = hashcode * obj.NodeType.GetHashCode();
                foreach (var attribute in obj.Attributes)
                {
                    hashcode = hashcode * htmlAttributeComparer.GetHashCode(attribute);
                }
            }

            return hashcode;
        }
    }
}
