// <copyright file="HtmlAttributeEqualityComparer.cs" company="ÚVT MU">
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
    /// Compares 2 attributes by their name and value ignoring case.
    /// </summary>
    public class HtmlAttributeEqualityComparer : IEqualityComparer<HtmlAttribute>
    {
        /// <summary>
        /// Holds t
        /// </summary>
        private string[] caseSensitiveAttributeNames = { "id", "class", "value" };
        
        /// <summary>
        /// Compares 2 attributes by their name and value ignoring case.
        /// </summary>
        /// <param name="x">First attribute to compare.</param>
        /// <param name="y">Second attribute to compare.</param>
        /// <returns><c>true</c> if equal, <c>false</c> otherwise.</returns>
        public bool Equals(HtmlAttribute x, HtmlAttribute y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (string.Equals(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                if (caseSensitiveAttributeNames.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase))
                {
                    return string.Equals(x.Value, y.Value, StringComparison.InvariantCulture);
                }
                else
                {
                    return string.Equals(x.Value, y.Value, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a hash code for the html attribute.
        /// </summary>
        /// <param name="obj">An html attribute to generate a hash code for.</param>
        /// <returns>The generated hash code.</returns>
        public int GetHashCode(HtmlAttribute obj)
        {
            if (obj == null)
            {
                return 0;
            }

            var hashcode = 17;
            unchecked
            {
                hashcode = hashcode * obj.Name.ToUpperInvariant().GetHashCode();
                hashcode = hashcode * obj.Value.ToUpperInvariant().GetHashCode();
            }

            return hashcode;
        }
    }
}
