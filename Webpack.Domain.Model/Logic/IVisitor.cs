// <copyright file="IVisitor.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model.Logic
{
    /// <summary>
    /// An interface that allows traversal of every page in a website.
    /// </summary>
    /// <typeparam name="TType">The type that can be visited.</typeparam>
    public interface IVisitor<TType>
    {
        /// <summary>
        /// Visitor does whatever he wants with the host.
        /// </summary>
        /// <param name="page">The host.</param>
        void Visit(TType page);
    }
}
