// <copyright file="UrlNodePathEqualityComparer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Url Node Path Equality Comparer
    /// </summary>
    public class UrlNodePathEqualityComparer : IEqualityComparer<UrlNode>
    {
        private readonly IEqualityComparer<string> comparer = StringComparer.InvariantCulture;

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
            if (x == null || y == null)
            {
                return false;
            }

            return comparer.Equals(x.Path, y.Path);
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns></returns>
        public int GetHashCode(UrlNode obj)
        {
            if (obj == null || obj.Path == null)
            {
                return 0;
            }

            return comparer.GetHashCode(obj.Path);
        }
    }
}
