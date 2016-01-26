// <copyright file="TupleEqualityComparer.cs" company="ÚVT MU">
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

    /// <summary>
    /// Tuple Equality Comparer
    /// </summary>
    public class TupleEqualityComparer<T1, T2> : IEqualityComparer<Tuple<T1, T2>>
    {
        private readonly IEqualityComparer<T1> firstComparer;

        private readonly IEqualityComparer<T2> secondComparer;

        /// <summary>
        /// Tuple Equality Comparer
        /// </summary>
        /// <param name="firstComparer">first Comparer</param>
        /// <param name="secondComparer">second Comparer</param>
        /// <returns></returns>
        public TupleEqualityComparer(IEqualityComparer<T1> firstComparer, IEqualityComparer<T2> secondComparer)
        {
            if (firstComparer == null)
            {
                throw new ArgumentNullException("firstComparer");
            }
            if (secondComparer == null)
            {
                throw new ArgumentNullException("secondComparer");
            }

            this.firstComparer = firstComparer;
            this.secondComparer = secondComparer;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns></returns>
        public bool Equals(Tuple<T1, T2> x, Tuple<T1, T2> y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return firstComparer.Equals(x.Item1, y.Item1) && secondComparer.Equals(x.Item2, y.Item2);
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns></returns>
        public int GetHashCode(Tuple<T1, T2> obj)
        {
            if (obj == null)
            {
                return 0;
            }
            unchecked
            {
                int hash = 17;
                hash = hash * 37 + firstComparer.GetHashCode(obj.Item1);
                hash = hash * 37 + secondComparer.GetHashCode(obj.Item2);
                return hash; 
            }
        }
    }
}
