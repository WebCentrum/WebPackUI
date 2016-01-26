// <copyright file="Resource.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    [DebuggerDisplay("url = {Url}")]
    public class Resource : EntityBase
    {
        [XmlElement(ElementName = "Path")]
        public string TextData { get; set; }

        public int Number { get; set; }

        public string Name { get; set; }

        public string FileName { get; set; }

        public string TemplateReference { get; set; }

        [XmlIgnore]
        public Url Url { get; set; }

        public string UrlText 
        { 
            get { return Url.ToString(); } 
            set { Url = new Url(new Uri(value)); } 
        }

        [XmlAttribute]
        public ResourceType ResourceType { get; set; }
    }

    public enum ResourceType
    {
        Unknown, Image, Stylesheet, Javascript, File
    }
}
