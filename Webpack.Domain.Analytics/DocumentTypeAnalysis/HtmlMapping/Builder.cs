// <copyright file="Builder.cs" company="ÚVT MU">
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
    using System.Threading.Tasks;
    using Webpack.Domain.Analytics.Extensions;

    /// <summary>
    /// Builder
    /// </summary>
    public class Builder
    {
        private readonly List<PropertyDTO> properties = new List<PropertyDTO>();

        private readonly IEqualityComparer<HtmlNode> comparer;
        private readonly IPropertyFactory propertyFactory;

        /// <summary>
        /// Builder
        /// </summary>
        /// <param name="comparer">comparer</param>
        /// <param name="propertyFactory">property Factory</param>
        /// <returns></returns>
        public Builder(IEqualityComparer<HtmlNode> comparer, IPropertyFactory propertyFactory)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (propertyFactory == null)
            {
                throw new ArgumentNullException("propertyFactory");
            }

            this.comparer = comparer;
            this.propertyFactory = propertyFactory;
        }

        public List<PropertyDTO> Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// Build
        /// </summary>
        /// <param name="skeleton">skeleton</param>
        /// <param name="rawPage">raw Page</param>
        /// <returns></returns>
        public PageDivisionInfo Build(HtmlNode skeleton, string rawPage)
        {
            properties.Clear();

            var doc = new HtmlDocument();
            doc.LoadHtml(rawPage);

            var root = BuildMappedTree(skeleton, doc.DocumentNode);
            var flatTree = BuildHtmlTree(root);

            return new PageDivisionInfo(flatTree, properties);
        }

        /// <summary>
        /// Build Html Tree
        /// </summary>
        /// <param name="toFlatten">to Flatten</param>
        /// <returns></returns>
        private HtmlNode BuildHtmlTree(MappedNode toFlatten)
        {
            var doc = new HtmlDocument();
            var root = doc.DocumentNode;

            var queue = new Queue<Tuple<HtmlNode, MappedNode>>(new[] { new Tuple <HtmlNode, MappedNode> (root, toFlatten) });
            while (queue.Any())
            {
                var tuple = queue.Dequeue();
                var htmlNode = tuple.Item1;
                var mappedNode = tuple.Item2;

                foreach (var child in Group(mappedNode.Children))
                {
                    var childHtmlNode = child.Item1;
                    var mappedNodes = child.Item2;
                    htmlNode.AppendChild(childHtmlNode);
                    
                    if (mappedNodes.First().IsProperty)
                    {
                        var value = mappedNodes.Aggregate(new StringBuilder(), (sb, n) => sb.Append(n.Mapee != null ? n.Mapee.WriteTo() : string.Empty));
                        properties.Add(propertyFactory.Copy(mappedNodes.First().Property, value.ToString()));
                    }
                    else
                    {
                        queue.Enqueue(new Tuple<HtmlNode, MappedNode>(childHtmlNode, mappedNodes.Single()));
                    }
                }
            }

            return root;
        }

        /// <summary>
        /// Group
        /// </summary>
        /// <param name="mappedNodes">mapped Nodes</param>
        /// <returns></returns>
        private IEnumerable<Tuple<HtmlNode, List<MappedNode>>> Group(List<MappedNode> mappedNodes)
        {
            var listOfListsOdNodes = mappedNodes.Aggregate(new List<List<MappedNode>>(), (listOfLists, node) =>
            {
                if (listOfLists.Count == 0)
                {
                    var list = new List<MappedNode>();
                    list.Add(node);
                    listOfLists.Add(list);
                }
                else
                {
                    var lastNode = listOfLists.Last().Last();
                    if (lastNode.IsProperty && node.IsProperty)
                    {
                        listOfLists.Last().Add(node);
                    }
                    else
                    {
                        var list = new List<MappedNode>();
                        list.Add(node);
                        listOfLists.Add(list);
                    }
                };

                return listOfLists;
            });
            foreach (var listOfNodes in listOfListsOdNodes)
            {
                var firstNode = listOfNodes.First();
                if (firstNode.IsProperty)
                {
                    var property = firstNode.Property ?? propertyFactory.GetNew();
                    var node = HtmlNode.CreateNode(property.TemplateReference);
                    listOfNodes[0] = new MappedNode(firstNode.Parent, property, firstNode.Mapee);
                    yield return new Tuple<HtmlNode, List<MappedNode>>(node, listOfNodes);
                } 
                else
	            {
                    yield return new Tuple<HtmlNode, List<MappedNode>>(firstNode.Model.CloneNode(false), listOfNodes);
	            }
            }
        }

        /// <summary>
        /// Build Mapped Tree
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="mapee">mapee</param>
        /// <returns></returns>
        private MappedNode BuildMappedTree(HtmlNode model, HtmlNode mapee)
        {
            var root = new MappedNode(null, model, mapee);
            var queue = new Queue<MappedNode>(new[] { root });
            while (queue.Any())
            {
                var parent = queue.Dequeue();
                var map = MapChildren(parent).ToArray();

                queue.EnqueueAll(map, p => !p.IsProperty);
            }

            return root;
        }

        /// <summary>
        /// Map Children
        /// </summary>
        /// <param name="parent">parent</param>
        /// <returns></returns>
        private IEnumerable<MappedNode> MapChildren(MappedNode parent)
        {
            var models = new Queue<HtmlNode>(DistinguishProperties(parent.Model.ChildNodes));
            var mapees = new Queue<HtmlNode>(parent.Mapee != null ? parent.Mapee.ChildNodes : Enumerable.Empty<HtmlNode>());

            while (true)
	        {
                var model = models.FirstOrDefault();
                var mapee = mapees.FirstOrDefault();

                if (model == null && mapee == null)
                {
                    yield break;
                }

                if (model != null && mapee == null)
                {
                    var property = propertyFactory.ParseTemplateReference(model);
                    if (property == null || models.Count > 1)
                    {
                        throw new InvalidOperationException("Skeleton is not a subset of the page");
                    }
                    else
                    {
                        yield return new MappedNode(parent, property, null);
                        yield break;
                    }
                }

                if (model == null && mapee != null)
                {
                    while (mapees.Any())
                    {
                        yield return new MappedNode(parent, (PropertyDTO)null, mapees.Dequeue());
                    }
                    yield break;
                }

                var prop = propertyFactory.ParseTemplateReference(model);
                if (prop != null)
                {
                    models.Dequeue();
                    var nextModel = models.FirstOrDefault();

                    if (!mapees.Any() || AreEqual(nextModel, mapees.Peek()))
                    {
                        yield return new MappedNode(parent, prop, HtmlNode.CreateNode(string.Empty));
                    }
                    else
                    {
                        while (mapees.Any() && !AreEqual(nextModel, mapees.Peek()))
                        {
                            yield return new MappedNode(parent, prop, mapees.Dequeue());
                        }
                    }
                    
                    if (mapees.Any())
                    {
                        continue;
                    }
                    else
                    {
                        yield break;
                    }
                }

                if (AreEqual(model, mapee))
                {
                    yield return new MappedNode(parent, models.Dequeue(), mapees.Dequeue());
                    continue;
                }
                else
                {
                    while (mapees.Any() && !AreEqual(model, mapees.Peek()))
                    {
                        yield return new MappedNode(parent, (PropertyDTO)null, mapees.Dequeue());
                    }
                }
	        }
        }

        /// <summary>
        /// Distinguish Properties
        /// </summary>
        /// <param name="nodes">nodes</param>
        /// <returns></returns>
        private HtmlNode[] DistinguishProperties(HtmlNodeCollection nodes)
        {
            var list = new List<HtmlNode>();
            foreach (var node in nodes)
            {
                if (node.NodeType == HtmlNodeType.Text)
                {
                    list.AddRange(propertyFactory.SplitText(node.InnerText).Select(HtmlNode.CreateNode));
                }
                else
                {
                    list.Add(node);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Are Equal
        /// </summary>
        /// <param name="node1">node1</param>
        /// <param name="node2">node2</param>
        /// <returns></returns>
        private bool AreEqual(HtmlNode node1, HtmlNode node2)
        {
            return comparer.Equals(node1, node2);
        }
    }
}
