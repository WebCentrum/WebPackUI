// <copyright file="RawPageEqualityComparer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.Crawler
{
    using System;
    using System.Collections.Generic;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Compares two instances of <seealso cref="RawPage"/> by the hash code of their content.
    /// </summary>
    public class RawPageEqualityComparer : EqualityComparer<RawPage>
    {
        /// <summary>
        /// Field used for comparing two <c>string</c> instances.
        /// </summary>
        private readonly IEqualityComparer<string> comparer = StringComparer.InvariantCulture;

        /// <summary>
        /// Compares two instances of <seealso cref="RawPage"/> by the hash code of their content.
        /// </summary>
        /// <param name="x">First instance to be compared.</param>
        /// <param name="y">Second instance to be compared.</param>
        /// <returns><c>true</c> if equal, <c>false</c> otherwise.</returns>
        public override bool Equals(RawPage x, RawPage y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            
            if (x == null || y == null)
            {
                return false;
            }

            return comparer.Equals(x.TextData, y.TextData);
        }

        /// <summary>
        /// Returns a hash code based on content hash code.
        /// </summary>
        /// <param name="obj">The object to generate the hash code for.</param>
        /// <returns>Generated hash code for the specified RawPage</returns>
        public override int GetHashCode(RawPage obj)
        {
            if (obj == null)
            {
                return base.GetHashCode();
            }

            return comparer.GetHashCode(obj.TextData);
        }
    }
}
