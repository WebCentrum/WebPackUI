// <copyright file="PropertyIdentifier.cs" company="ÚVT MU">
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
    using Webpack.Domain.Model.Entities;
    using Webpack.Domain.Model.Logic;
    using Webpack.Domain.Analytics.Extensions;
    using Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlMapping;
    using System.Text.RegularExpressions;

    public class PropertyIdentifier : IVisitor<Page>
    {
        private HtmlNode skeleton;
        
        private readonly Builder builder;

        private readonly Dictionary<string, Definition> propertyCache = new Dictionary<string, Definition>();

        private event Action<Definition> NewDefinition;

        public PropertyIdentifier(HtmlNode skeleton, Builder builder)
        {
            if (skeleton == null)
            {
                throw new ArgumentNullException("skeleton");
            }
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            this.skeleton = skeleton;
            this.builder = builder;
        }

        public HtmlNode PopulatedSkeleton
        {
            get { return skeleton; }
        }

        public List<Definition> Definitions
        {
            get { return propertyCache.Values.ToList(); }
        }

        public void Visit(Page page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            if (page.RawPage == null)
            {
                return;
            }

            var pageDivisionInfo = builder.Build(skeleton, page.RawPage.TextData);
            skeleton = pageDivisionInfo.Template;
               
            var propertyDTOs = pageDivisionInfo.Properties;
            UpdateCache(propertyDTOs);

            page.Properties = propertyDTOs
                .Select(p => new Property { Definition = propertyCache[p.Name], Name = p.Name, Value = p.Value, ID = Guid.NewGuid() })
                .ToList();

            NewDefinition += def => page.Properties.Add(new Property { Definition = def, Name = def.Name, Value = string.Empty, ID = Guid.NewGuid() });
        }

        private void UpdateCache(List<PropertyDTO> propertyDTOs)
        {
            var newAdditions = propertyDTOs
                .Where(dto => !propertyCache.ContainsKey(dto.Name))
                .Select(dto => new Definition { Number = dto.Number, Name = dto.Name, TemplateReference = dto.TemplateReference });
            foreach (var item in newAdditions)
	        {
		        propertyCache.Add(item.Name, item);
                if (NewDefinition != null)
                {
                    NewDefinition(item);
                }
	        }
        }
    }
}
