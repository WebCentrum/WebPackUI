// <copyright file="LinqHelper.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.Extensions
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Model.Logic;

    /// <summary>
    /// Linq Helper
    /// </summary>
    public static class LinqHelper
    {
        /// <summary>
        /// Take Until
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="until">until</param>
        /// <returns></returns>
        public static IEnumerable<TType> TakeUntil<TType>(this Queue<TType> source, Predicate<TType> until)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (until == null)
            {
                throw new ArgumentNullException("until");
            }
            while (source.Count > 0)
            {
                var item = source.Dequeue();
                yield return item;

                if (until(item))
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Enqueue All
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="collection">collection</param>
        /// <returns></returns>
        public static void EnqueueAll<TType>(this Queue<TType> source, IEnumerable<TType> collection)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            foreach (var item in collection)
            {
                source.Enqueue(item);
            }
        }

        /// <summary>
        /// Enqueue All
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="collection">collection</param>
        /// <param name="predicate">predicate</param>
        /// <returns></returns>
        public static void EnqueueAll<TType>(this Queue<TType> source, IEnumerable<TType> collection, Predicate<TType> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            source.EnqueueAll(collection.Where(i => predicate(i)));
        }

        /// <summary>
        /// Is Empty
        /// </summary>
        /// <param name="source">source</param>
        /// <returns></returns>
        public static bool IsEmpty<TType>(this IEnumerable<TType> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.Count() == 0;
        }

        /// <summary>
        /// To Node String
        /// </summary>
        /// <param name="source">source</param>
        /// <returns></returns>
        public static string ToNodeString(this IEnumerable<HtmlNode> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.Aggregate(new StringBuilder(), (sb, node) => sb.Append(node.WriteTo())).ToString();
        }

        /// <summary>
        /// Accept
        /// </summary>
        /// <param name="hosts">hosts</param>
        /// <param name="visitor">visitor</param>
        /// <returns></returns>
        public static void Accept<TVisitable>(this IEnumerable<TVisitable> hosts, IVisitor<TVisitable> visitor)
            where TVisitable : IVisitable<TVisitable>
        {
            if (hosts == null)
            {
                throw new ArgumentNullException("hosts");
            }
            if (visitor == null)
            {
                throw new ArgumentNullException("visitor");
            }
            foreach (var host in hosts)
            {
                host.Accept(visitor);
            }
        }

        /// <summary>
        /// Try Max
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="selector">selector</param>
        /// <returns></returns>
        public static TResult TryMax<TSource, TResult>(this ICollection<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            return source.Count > 0 ? source.Max(selector) : default(TResult);
        }

        /// <summary>
        /// Dequeue While
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="condition">condition</param>
        /// <returns></returns>
        public static TSource DequeueWhile<TSource>(this Queue<TSource> source, Predicate<TSource> condition)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (condition == null)
            {
                throw new ArgumentNullException("condition");
            }

            while (source.Any())
            {
                var item = source.Dequeue();
                if (!condition(item))
                {
                    return item;
                }
            }
            return default(TSource);
        }

        /// <summary>
        /// Descendants
        /// </summary>
        /// <param name="node">node</param>
        /// <param name="level">level</param>
        /// <returns></returns>
        public static IEnumerable<HtmlNode> Descendants(this HtmlNode node, int level)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (level < 1)
            {
                throw new ArgumentOutOfRangeException("level");
            }

            var nodes = new[] { node }.AsEnumerable();
            for (int i = 0; i < level; i++)
            {
                nodes = nodes.SelectMany(n => n.ChildNodes);
            }

            return nodes;
        }

        /// <summary>
        /// Level
        /// </summary>
        /// <param name="node">node</param>
        /// <param name="ancestor">ancestor</param>
        /// <returns></returns>
        public static int Level(this HtmlNode node, HtmlNode ancestor)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (ancestor == null)
            {
                throw new ArgumentNullException("ancestor");
            }

            return node.AncestorsAndSelf().TakeWhile(n => n != ancestor).Count();
        }

        /// <summary>
        /// Group By Common Ancestor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="root">root</param>
        /// <returns></returns>
        public static HtmlNode GroupByCommonAncestor(this IEnumerable<HtmlNode> source, HtmlNode root)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (!source.Any())
            {
                return null;
            }

            var commonXpath = source.Select(n => n.XPath)
                .Aggregate((acc, xpath) => string.Join(string.Empty, acc.Zip(xpath, (a, b) => new { Acc = a, XPath = b })
                    .TakeWhile(a => a.Acc == a.XPath)
                    .Select(a => a.Acc)));

            var lastSeparator = commonXpath.LastIndexOf('/');
            if (lastSeparator != -1)
            {
                return root.SelectSingleNode(commonXpath.Substring(0, lastSeparator));
            }
            return root;
            
        }

        /// <summary>
        /// Group By Parent
        /// </summary>
        /// <param name="source">source</param>
        /// <returns></returns>
        public static IEnumerable<IGrouping<HtmlNode, HtmlNode>> GroupByParent(this IEnumerable<HtmlNode> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            return source.GroupBy(n => n.ParentNode, new NodePositionEqualityComparer());
        }

        /// <summary>
        /// Prepend
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="element">element</param>
        /// <returns></returns>
        public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            yield return element;
            foreach (var item in source)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Enumerate DFS
        /// </summary>
        /// <param name="root">root</param>
        /// <param name="childSelector">child Selector</param>
        /// <param name="predicate">predicate</param>
        /// <returns></returns>
        public static IEnumerable<TSource> EnumerateDFS<TSource>(this TSource root, Func<TSource, IEnumerable<TSource>> childSelector, Predicate<TSource> predicate = null)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            if (childSelector == null)
            {
                throw new ArgumentNullException("childSelector");
            }

            predicate = predicate ?? new Predicate<TSource>(n => true);

            var stack = new Stack<TSource>();
            stack.Push(root);

            while (stack.Any())
            {
                var item = stack.Pop();
                foreach (var child in childSelector(item).Reverse())
                {
                    stack.Push(child);
                }

                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Cat
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="selector">selector</param>
        /// <returns></returns>
        public static string Cat<TSource>(this IEnumerable<TSource> source, Func<TSource, string> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            return source.Aggregate(new StringBuilder(), (sb, e) => sb.Append(selector(e)), sb => sb.ToString());
        }

        /// <summary>
        /// Concat
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="second">second</param>
        /// <returns></returns>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> source, IEnumerable<TSource> second)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }
            
            foreach (var item in source)
            {
                yield return item;
            }
            foreach (var item in second)
            {
                yield return item;
            }
        }
    }
}
