using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webpack.Domain.Analytics.Extensions;
using Webpack.Domain.Model.Logic;

namespace Webpack.Domain.Analytics.DocumentTypeAnalysis
{
    [DebuggerDisplay("Path = {Path}")]
    public class UrlNode
    {
        private readonly List<UrlNode> children = new List<UrlNode>();

        protected readonly string path;

        protected readonly UrlNode parent;

        private UrlNode(UrlNode parent, string path)
        {
            this.parent = parent;
            this.path = path;

            if (parent != null)
	        {
                parent.Children.Add(this);
	        }
        }

        public UrlNode Parent 
        {
            get { return parent; } 
        }

        public List<UrlNode> Children 
        {
            get { return children; } 
        }

        public string Path 
        {
            get { return path; } 
        }
        
        public int Count
        {
            get { return DescendantsAndSelf().Count(); }
        }

        private bool IsRoot
        {
            get { return Path != null; }
        }

        public static UrlNode CreateRoot()
        {
            return new UrlNode(null, null);
        }

        public UrlNode AppendChild(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("path");
            }

            return new UrlNode(this, path);
        }

        public bool ContainsDescendantWithPath(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            return DescendantsAndSelf().Any(d => d.Path == path);
        }

        public UrlNode FindByPath(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            return DescendantsAndSelf().FirstOrDefault(d => d.Path == path);
        }

        public IEnumerable<UrlNode> DescendantsAndSelf()
        {
            var descendants = Children.SelectMany(c => c.DescendantsAndSelf());
            return Enumerable
                .Repeat(this, 1)
                .Concat(descendants);
        }

        public IEnumerable<UrlNode> Descendants()
        {
            return DescendantsAndSelf().Skip(1);
        }

        public IEnumerable<IEnumerable<UrlNode>> Lines()
        {
            return GetLines();
        }

        public void MergeLine(IEnumerable<UrlNode> line)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            if (!line.Any())
            {
                return;
            }

            UrlNode previous = null;
            foreach (var item in line)
            {
                var found = FindByPath(item.Path);
                if (found == null && previous == null)
                {
                    previous = AppendChild(item.Path);
                }
                else if (found == null)
                {
                    previous = previous.AppendChild(item.Path);
                }
                else
                {
                    previous = found;
                    continue;
                }
            }
        }

        public void MergeLines(IEnumerable<IEnumerable<UrlNode>> lines)
        {
            if (lines == null)
            {
                throw new ArgumentNullException("lines");
            }
            foreach (var line in lines)
            {
                MergeLine(line);
            }
        }

        private IEnumerable<IEnumerable<UrlNode>> GetLines()
        {
            if (!Children.Any())
            {
                yield return new List<UrlNode> { this };
            }
            else
            {
                foreach (var child in Children)
                {
                    foreach (var line in child.GetLines())
                    {
                        yield return line.Prepend(this);
                    }
                }
            }
        }

        public bool ContainsLine(IEnumerable<UrlNode> line, bool canMoveDown = true)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            if (!line.Any())
            {
                return true;
            }

            if (line.First().Path == Path)
            {
                return Children.Any() 
                    ? Children.Any(c => c.ContainsLine(line.Skip(1), false))
                    : true;
            } 
            else
	        {
                return Children.Any() && canMoveDown
                    ? Children.Any(c => c.ContainsLine(line, true))
                    : false;
	        }

        }
    }
}
