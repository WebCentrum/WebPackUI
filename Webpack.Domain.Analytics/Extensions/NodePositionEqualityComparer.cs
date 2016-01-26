// <copyright file="NodePositionEqualityComparer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.Extensions
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Node Position Equality Comparer
    /// </summary>
    public class NodePositionEqualityComparer : IEqualityComparer<HtmlNode>
    {
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns></returns>
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

            return x.XPath == y.XPath;
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns></returns>
        public int GetHashCode(HtmlNode obj)
        {
            if (obj == null)
            {
                return 0;
            }

            unchecked
            {
                return obj.XPath.GetHashCode();
            }
        }
    }
}
