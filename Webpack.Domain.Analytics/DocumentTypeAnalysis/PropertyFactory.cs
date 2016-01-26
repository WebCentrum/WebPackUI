// <copyright file="PropertyFactory.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.DocumentTypeAnalysis
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlMapping;

    public class PropertyFactory : IPropertyFactory
    {
        public const string DIRECTIVE = "@Raw(";
        public const string MODEL = "Model.";
        public const string NAME_PREFIX = "Property";
        public const string DIRECTIVE_CLOSE = ")";

        private readonly Regex propertyTemplateReferenceRegex = new Regex(@"(@Raw\(Model\.Property\d+\))",
            RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private readonly Regex parseTemplateReferenceRegex = new Regex(@"^(@Raw\(Model\.Property(?<number>\d+)\))$",
            RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private int counter = 0;
        private readonly List<PropertyDTO> allProperties = new List<PropertyDTO>();

        public Regex PropertyTemplateReferenceRegex
        {
            get { return propertyTemplateReferenceRegex; }
        }

        public List<PropertyDTO> AllProperties
        {
            get { return allProperties; }
        }

        public PropertyDTO GetNew(string value = null)
        {
            var result = Get(counter++);
            allProperties.Add(result);
            return result;
        }

        public PropertyDTO Get(int number, string value = null)
        {
            return new PropertyDTO(number, GetName(number), GetTemplateReference(number), value);
        }

        public PropertyDTO Copy(PropertyDTO property, string value = null)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            return new PropertyDTO(property.Number, property.Name, property.TemplateReference, value ?? property.Value);
        }

        public string[] SplitText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            return PropertyTemplateReferenceRegex.Split(text)
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
        }

        public PropertyDTO ParseTemplateReference(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            var match = parseTemplateReferenceRegex.Match(text);
            if (match.Success)
            {
                int number = int.Parse(match.Groups["number"].Value);
                return new PropertyDTO(number, GetName(number), GetTemplateReference(number));
            }
            return null;
        }

        public PropertyDTO ParseTemplateReference(HtmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("text");
            }
            
            return node.NodeType == HtmlNodeType.Text
                ? ParseTemplateReference(node.InnerText)
                : null;
        }

        protected virtual string GetName(int number)
        {
            return NAME_PREFIX + number.ToString();
        }

        protected virtual string GetTemplateReference(int number)
        {
            return DIRECTIVE + MODEL + GetName(number) + DIRECTIVE_CLOSE;
        }
    }
}
