// <copyright file="EntityBase.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The base for all entities
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// Gets or sets the unique identifier for an entity
        /// </summary>
        [XmlAttribute]
        public virtual Guid ID { get; set; }

        /// <summary>
        /// Determines whether two instances of <c>EntityBase</c> are the same or not, based on ID
        /// </summary>
        /// <param name="obj">The other object to compare with</param>
        /// <returns><c>true</c> if they are the same, <c>false</c> otherwise</returns>
        public override bool Equals(object obj)
        {
            EntityBase entity = obj as EntityBase;
            return entity != null && ID != default(Guid)
                && entity.ID != default(Guid) && entity.ID == ID;
        }

        /// <summary>
        /// Calculate a hash code based on ID
        /// </summary>
        /// <returns>the calculated hash code</returns>
        public override int GetHashCode()
        {
            return ID.GetHashCode() * GetType().GetHashCode() * 17;
        }
    }
}