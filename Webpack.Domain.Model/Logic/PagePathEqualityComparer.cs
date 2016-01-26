// <copyright file="PagePathEqualityComparer.cs" company="ÚVT MU">
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
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Page Path Equality Comparer
    /// </summary>
    public class PagePathEqualityComparer : IEqualityComparer<Page>
    {
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns></returns>
        public bool Equals(Page x, Page y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.RawPage == null && y.RawPage == null)
            {
                return true;
            }

            if (x.RawPage == null || y.RawPage == null)
            {
                return false;
            }

            return StringComparer.InvariantCulture.Equals(x.RawPage.Path, y.RawPage.Path);
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns></returns>
        public int GetHashCode(Page obj)
        {
            if (obj == null || obj.RawPage == null)
            {
                return 0;
            }

            unchecked
            {
                return StringComparer.InvariantCulture.GetHashCode(obj.RawPage.Path);
            }
        }
    }
}
