// <copyright file="EntityComparer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines rules for comparing entities
    /// </summary>
    /// <typeparam name="TEntity">Entity type which instances are getting compared</typeparam>
    public class EntityComparer<TEntity> : EqualityComparer<TEntity>
        where TEntity : EntityBase
    {
        /// <summary>
        /// Compares two entities
        /// </summary>
        /// <param name="x">first entity</param>
        /// <param name="y">second entity</param>
        /// <returns><c>true</c> if they are equal, <c>false</c> otherwise</returns>
        public override bool Equals(TEntity x, TEntity y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Generates a hash code for the entity
        /// </summary>
        /// <param name="obj">entity to generate hash code for</param>
        /// <returns>generated hash code</returns>
        public override int GetHashCode(TEntity obj)
        {
            return obj.GetHashCode();
        }
    }
}
