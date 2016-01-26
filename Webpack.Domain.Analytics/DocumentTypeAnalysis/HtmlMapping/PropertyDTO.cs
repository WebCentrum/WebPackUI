// <copyright file="PropertyDTO.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlMapping
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Property DTO
    /// </summary>
    public class PropertyDTO
    {
        private readonly string name;
        private readonly string templateReference;
        private readonly int number;
        private readonly string value;

        /// <summary>
        /// Property DTO
        /// </summary>
        /// <param name="number">number</param>
        /// <param name="name">name</param>
        /// <param name="templateReference">template Reference</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public PropertyDTO(int number, string name, string templateReference, string value = null)
        {
            if (number < 0)
            {
                throw new ArgumentOutOfRangeException("number");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            if (string.IsNullOrWhiteSpace(templateReference))
            {
                throw new ArgumentNullException("templateReference");
            }

            this.number = number;
            this.name = name;
            this.templateReference = templateReference;
            this.value = value;
        }

        public string Name 
        {
            get { return name; } 
        }

        public string TemplateReference 
        {
            get { return templateReference; } 
        }

        public string Value 
        {
            get { return value; } 
        }

        public int Number
        {
            get { return number; }
        }
    }
}
