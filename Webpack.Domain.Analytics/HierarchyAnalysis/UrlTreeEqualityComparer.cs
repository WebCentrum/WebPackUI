// <copyright file="UrlTreeEqualityComparer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.HierarchyAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Model.Logic;

    /// <summary>
    /// Url Tree Equality Comparer
    /// </summary>
    public class UrlTreeEqualityComparer : IEqualityComparer<UrlNode>
    {
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns></returns>
        public bool Equals(UrlNode x, UrlNode y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null || x.Count != y.Count)
            {
                return false;
            }

            var xNodes = x.DescendantsAndSelf();
            var yNodes = y.DescendantsAndSelf();
            return xNodes
                .Zip(yNodes, (xNode, yNode) => xNode.Path == yNode.Path)
                .All(b => b);
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns></returns>
        public int GetHashCode(UrlNode obj)
        {
            if (obj == null)
            {
                return 0;
            }

            unchecked
            {
                return obj.DescendantsAndSelf()
                    .Aggregate(37, (hashcode, node) => 
                        hashcode * 23 + (node.Path ?? string.Empty)
                        .GetHashCode());           
            }
        }
    }
}
