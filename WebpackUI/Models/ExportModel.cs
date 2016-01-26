// <copyright file="ExportModel.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Tomáš Pouzar</author>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebpackUI.Models
{
    /// <summary>
    /// Represents an Export configuration
    /// </summary>  
    [DataContract(Name = "exportModel", Namespace = "")]
    public class ExportModel
    {
        public ExportModel()
        {
            Xsl = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<xsl:stylesheet version=\"1.0\"\nxmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">\n\t<xsl:template match=\"/\">\n\t\t<xsl:copy-of select=\".\"/>\n\t</xsl:template>\n</xsl:stylesheet>";
        }

        [DataMember(Name = "xml")]
        public string Xml { get; set; }

        [DataMember(Name = "xsl")]
        public string Xsl { get; set; }

        [DataMember(Name = "xmlPreview")]
        public string XmlPreview { get; set; }
    }
}