// <copyright file="IVisitable.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model.Logic
{
    /// <summary>
    /// Can be visited by a <seealso cref="IVisitor"/>
    /// </summary>
    /// <typeparam name="TType">The type of the visited object.</typeparam>
    public interface IVisitable<TType>
    {
        /// <summary>
        /// Accept a visitor.
        /// </summary>
        /// <param name="visitor">The visitor.</param>
        void Accept(IVisitor<TType> visitor);
    }
}
