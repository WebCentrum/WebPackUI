// <copyright file="Template.cs" company="ÚVT MU">
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
    using System.Xml;
    using System.Xml.Serialization;

    public class Template : EntityBase
    {
        [XmlIgnore]
        public string Text { get; set; }
        
        [XmlAttribute]
        public string Name { get; set; }
        
        //public Template Base { get; set; }

        [XmlText]
        public XmlNode[] CDataValue
        {
            get
            {
                var dummy = new XmlDocument();
                return new XmlNode[] { dummy.CreateCDataSection(Text) };
            }
            set
            {
                if (value == null)
                {
                    Text = null;
                    return;
                }

                if (value.Length != 1)
                {
                    throw new InvalidOperationException(
                        String.Format(
                            "Invalid array length {0}", value.Length));
                }

                Text = value[0].Value;
            }
        }
    }
}
