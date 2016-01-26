// <copyright file="Property.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the value of a property
    /// </summary>
    [DebuggerDisplay("{Name} = {Value}")]
    public class Property : EntityBase
    {
        /// <summary>
        /// Gets the property of a page type this property data is associated with.
        /// </summary>
        [XmlIgnore]
        public Definition Definition { get; set; }
        
        /// <summary>
        /// Gets the name.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        [XmlIgnore]
        public string Value { get; set; }

        [XmlText]
        public XmlNode[] CDataValue
        {
            get
            {
                var dummy = new XmlDocument();
                return new XmlNode[] { dummy.CreateCDataSection(Value) };
            }
            set
            {
                if (value == null)
                {
                    Value = null;
                    return;
                }

                if (value.Length != 1)
                {
                    throw new InvalidOperationException(
                        String.Format(
                            "Invalid array length {0}", value.Length));
                }

                Value = value[0].Value;
            }
        }
    }
}