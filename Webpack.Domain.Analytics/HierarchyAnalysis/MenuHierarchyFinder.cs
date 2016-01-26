// <copyright file="MenuHierarchyFinder.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.HierarchyAnalysis
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Analytics.DocumentTypeAnalysis.HtmlMapping;
    using Webpack.Domain.Analytics.Extensions;
    using Webpack.Domain.Model.Entities;
    using Webpack.Domain.Model.Logic;

    public class MenuHierarchyFinder
    {
        private readonly IEqualityComparer<HtmlNode> comparer;

        public MenuHierarchyFinder(IEqualityComparer<HtmlNode> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            this.comparer = comparer;
        }

        public UrlNode FindHierarchy(Dictionary<Page, Dictionary<string, string>> pageProperties)
        {
            if (pageProperties == null)
            {
                throw new ArgumentNullException("pageProperties");
            }
            var root = UrlNode.CreateRoot();
            var urlTrees = new HashSet<UrlNode>(new UrlTreeEqualityComparer());

            foreach (var pair in pageProperties)
            {
                var page = pair.Key;
                var dataset = pair.Value
                    .Where(ds => !string.IsNullOrWhiteSpace(ds.Value));
                
                foreach (var propertyData in dataset)
                {
                    var data = propertyData.Value;
                    var urlTree = ExtractHierarchyFromMenu(data);
                    var lines = urlTree.Lines().Select(line => line.Skip(1)).ToList(); // remove roots;
                    root.MergeLines(lines);
                    urlTrees.Add(urlTree);

                    AssignPageToMacro(page, propertyData.Key);
                }
            }

            return root;
        }

        private void AssignPageToMacro(Page page, string propertyName)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }
        }

        public UrlNode ExtractHierarchyFromMenu(string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            var rootUrlNode = UrlNode.CreateRoot();

            var rootHtmlNode = HtmlNode.CreateNode(data);  // should only be one rootHtmlNode
            if (rootHtmlNode == null)
            {
                return rootUrlNode;
            }


            var anchorLevels = rootHtmlNode
                .EnumerateDFS(n => n.ChildNodes, IsAnchor)
                .Select(a => new {
                    Level = a.Level(rootHtmlNode),
                    Path = a.Attributes["href"].Value
                });
            
            var addedNodes = new List<Tuple<UrlNode, int>>();
            foreach (var anchor in anchorLevels)
            {
                var existing = rootUrlNode.FindByPath(anchor.Path);
                if (existing != null)
                {
                    continue;
                }

                var parent = addedNodes.LastOrDefault(n => n.Item2 < anchor.Level);
                var newNode = parent == null
                    ? rootUrlNode.AppendChild(anchor.Path)
                    : parent.Item1.AppendChild(anchor.Path);

                addedNodes.Add(new Tuple<UrlNode, int>(newNode, anchor.Level));
            }

            return rootUrlNode;
        }

        private static bool IsAnchor(HtmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            return node.Attributes.Contains("href")
                && !node.Attributes["href"].Value.Contains(".")
                && !node.Attributes["href"].Value.StartsWith("#");
        }
    }
}
