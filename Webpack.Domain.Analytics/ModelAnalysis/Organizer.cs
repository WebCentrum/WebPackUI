// <copyright file="Organizer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.ModelAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Organizer
    /// </summary>
    public class Organizer
    {
        /// <summary>
        /// Organize
        /// </summary>
        /// <param name="rawPages">raw Pages</param>
        /// <param name="parent">parent</param>
        /// <returns></returns>
        public virtual IEnumerable<Page> Organize(IEnumerable<RawPage> rawPages, Page parent)
        {
            if (rawPages == null)
            {
                throw new ArgumentNullException("metPages");
            }

            return rawPages.Select(rp => new Page 
            { 
                RawPage = rp,
                Name = rp.Url.LastSegment,
                Parent = parent,
                ID = Guid.NewGuid() 
            });
        }
    }
}
