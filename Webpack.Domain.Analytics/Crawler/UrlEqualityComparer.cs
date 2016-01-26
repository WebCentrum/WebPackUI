// <copyright file="UrlComparer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Model;

    public class UrlEqualityComparer : IEqualityComparer<Url>
    {
        public bool Equals(Url x, Url y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.ToString() == y.ToString();
        }

        public int GetHashCode(Url obj)
        {
            if (obj == null)
            {
                return base.GetHashCode();
            }

            return obj.ToString().GetHashCode();
        }
    }
}
