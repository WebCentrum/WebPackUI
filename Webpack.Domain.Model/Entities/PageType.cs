// <copyright file="Sjte.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model.Entities
{
    using RazorEngine;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;


    /// <summary>
    /// Page Type
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public class PageType : EntityBase
    {
        private readonly List<Page> pages = new List<Page>();
        private List<Definition> definitions = new List<Definition>();
        private string[] macroXpaths = new string[0];

        private Template template;

        [XmlAttribute]
        public string Name { get; set; }

        [XmlIgnore]
        public Template Template {
            get { return template; } 
            set 
            {
                template = value;
                if (template != null)
                {
                    TemplateID = template.ID;
                }
            } 
        }

        public Guid TemplateID { get; set; }
        

        [XmlIgnore]
        public string[] MacroXpaths 
        {
            get { return macroXpaths; }
            set { macroXpaths = value ?? new string[0]; }
        }

        [XmlIgnore]
        public List<Page> Pages 
        { 
            get { return pages; } 
        }

        [XmlArray]
        public List<Definition> Definitions 
        { 
            get { return definitions; }
            set { definitions = value ?? new List<Definition>(); }
        }
    }
}
