// <copyright file="Site.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    [Serializable]
    public class Site
    {
        [XmlElement("Pages")]
        public Page Root { get; set; }

        [XmlArray]
        public PageType[] PageTypes { get; set; }

        [XmlArray]
        public Template[] Templates { get; set; }

        [XmlArray]
        public Resource[] Resources { get; set; }

        public string Name { get; set; }
    }
}
