// <copyright file="Extensions.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// To Expando
        /// </summary>
        /// <param name="source">source</param>
        /// <returns></returns>
        public static ExpandoObject ToExpando(this IList<Property> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var result = new ExpandoObject() as IDictionary<string, object>;
            foreach (var item in source)
            {
                result.Add(item.Definition.Name, item.Value);
            }

            return result as ExpandoObject;
        }

        /// <summary>
        /// Prepend
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="element">element</param>
        /// <returns></returns>
        public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            yield return element;
            foreach (var item in source)
            {
                yield return item;
            }
        }
    }
}
