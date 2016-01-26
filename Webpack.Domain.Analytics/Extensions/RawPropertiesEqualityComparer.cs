// <copyright file="RawPropertiesEqualityComparer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// List Of Property Definition Equality Comparer
    /// </summary>
    public class ListOfPropertyDefinitionEqualityComparer : IEqualityComparer<List<Definition>>
    {
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns></returns>
        public bool Equals(List<Definition> x, List<Definition> y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.SequenceEqual(y, new PropertyDefinitionEqualityComparer());
            //return x.Keys.All(k => y.Keys.Contains(k)) && y.Keys.All(k => x.Keys.Contains(k));
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns></returns>
        public int GetHashCode(List<Definition> obj)
        {
            if (obj == null)
            {
                return 0;
            }

            unchecked
            {
                return obj.Aggregate(17, (h, k) => h * 23 + k.GetHashCode());
            }
        }
    }

    /// <summary>
    /// Property Definition Equality Comparer
    /// </summary>
    internal class PropertyDefinitionEqualityComparer : IEqualityComparer<Definition>
    {
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns></returns>
        public bool Equals(Definition x, Definition y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Number == y.Number;
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns></returns>
        public int GetHashCode(Definition obj)
        {
            if (obj == null)
            {
                return 0;
            }

            unchecked
            {
                return obj.Number.GetHashCode();
            }
        }
    }

}
