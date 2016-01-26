// <copyright file="HierarchyOrganizer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.ModelAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Model.Entities;

    public class HierarchyOrganizer : Organizer
    {
        public override IEnumerable<Page> Organize(IEnumerable<RawPage> rawPages, Page parent)
        {
            if (rawPages == null)
            {
                throw new ArgumentNullException("rawPages");
            }
            var root = new Page() { Name = string.Empty };

            foreach (var rawPage in rawPages)
            {
                var segments = rawPage.Url.Segments;

                var lastPage = root;
                for (int i = 0; i < segments.Length; i++)
                {
                    var segment = segments[i];
                    var foundPage = lastPage.GetChild(segment);
                    if (foundPage == null)
                    {
                        if (lastPage.RawPage == null)
                        {
                            var combinedSegments = segments.Skip(i).ToArray();
                            foundPage = lastPage.GetChild(combinedSegments);
                        }
                        if (foundPage == null)
                        {
                            foundPage = new Page()
                            {
                                Name = segment,
                                Parent = lastPage,
                                ID = Guid.NewGuid() 
                            };
                        }
                        lastPage.Children.Add(foundPage);
                    }

                    lastPage = foundPage;
                }

                lastPage.RawPage = rawPage;
            }


            var level = 0;
            var queue = new Queue<Tuple<Page, int>>();
            queue.Enqueue(Tuple.Create(root, 0));
            while (queue.Any())
            {
                var item = queue.Dequeue();
                var page = item.Item1;
                var currentLevel = item.Item2;
                if (page.RawPage == null)
                {
                    foreach (var child in page.Children)
                    {
                        queue.Enqueue(Tuple.Create(child, currentLevel + 1));
                    }   
                } else
	            {
                    level = currentLevel;
                    break;
	            }
            }

            var descendants = root.GetDescendants(level).ToArray();
            foreach (var child in descendants)
	        {
                child.Parent = parent;
		        yield return child;
	        }
        }
    }
}
