// <copyright file="PageModel.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.ModelAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Webpack.Domain.Model.Entities;

    /// <summary>
    /// Page Model
    /// </summary>
    [DebuggerDisplay("name = {name}")]
    public class PageModel
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<PageModel> children = new List<PageModel>();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<RawPage> metPages = new List<RawPage>();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<RawPage> unhandledPages = new List<RawPage>();
        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Condition<RawPage> meetCondition = Condition<RawPage>.Never();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Condition<RawPage> fallThroughCondition = Condition<RawPage>.Never();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Organizer organizer = new Organizer();
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<string> macroXpaths = new List<string>();

        private readonly string name;

        /// <summary>
        /// Page Model
        /// </summary>
        /// <param name="name">name</param>
        /// <returns></returns>
        public PageModel(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            this.name = name;
        }

        /// <summary>
        /// Page Model
        /// </summary>
        /// <param name="pageModel">page Model</param>
        /// <returns></returns>
        private PageModel(PageModel pageModel)
        {
            this.meetCondition = pageModel.meetCondition;
            this.fallThroughCondition = pageModel.fallThroughCondition;
            this.organizer = pageModel.organizer;
            this.name = pageModel.name;

            this.PageType = pageModel.PageType;
        }

        public PageType PageType { get; set; }

        public Condition<RawPage> Meets
        {
            get { return meetCondition; }
            set { meetCondition = value ?? meetCondition; }
        }

        public Condition<RawPage> FallsThrough
        { 
            get { return fallThroughCondition; }
            set { fallThroughCondition = value ?? fallThroughCondition; }
        }

        public Organizer Organizer
        {
            get { return organizer; }
            set { organizer = value ?? organizer; }
        }

        public List<string> MacroXpaths
        {
            get { return macroXpaths; }
            set { macroXpaths = value ?? new List<string>(); }
        }

        public List<PageModel> Children
        {
            get { return children; }
        }

        public List<RawPage> MetPages
        {
            get { return metPages; }
        }

        public string Name
        {
            get { return name; }
        }

        public IEnumerable<RawPage> AllMetPages
        {
            get 
            {
                return MetPages.Concat(Children.SelectMany(c => c.AllMetPages));
            }
        }

        public List<RawPage> UnhandledPages
        {
            get { return unhandledPages; }
        }

        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="pages">pages</param>
        /// <returns></returns>
        public void Handle(IEnumerable<RawPage> pages)
        {
            var fallenThroughPages = new List<RawPage>();
            foreach (var page in pages)
            {
                if (Meets.Evaluate(page))
                {
                    page.IsHandled = true;
                    MetPages.Add(page);
                }
                else if(FallsThrough.Evaluate(page))
                {
                    fallenThroughPages.Add(page);
                }
            }

            foreach (var child in Children)
            {
                var notYetHandled = fallenThroughPages
                    .Where(p => !p.IsHandled)
                    .ToArray();
                child.Handle(notYetHandled);
            }
            
            UnhandledPages
                .AddRange(fallenThroughPages.Where(p => !p.IsHandled));
        }

        /// <summary>
        /// Add Children
        /// </summary>
        /// <param name="children">children</param>
        /// <returns></returns>
        public PageModel AddChildren(params PageModel[] children)
        {
            if (children == null || children.Any(c => c == null))
            {
                throw new ArgumentNullException("children");
            }
            Children.AddRange(children);
            
            return this;
        }

        /// <summary>
        /// Get Pages
        /// </summary>
        /// <param name="parent">parent</param>
        /// <returns></returns>
        public IEnumerable<Page> GetPages(Page parent)
        {
            if (MetPages.Count == 0 || MetPages.Count == 1)
            {
                var hasName = MetPages.Count == 1 && MetPages.Single().Url.LastSegment != null;
                var pageName = hasName ? MetPages.Single().Url.LastSegment : name;
                var page = new Page { Parent = parent, Name = pageName, PageType = PageType, ID = Guid.NewGuid() };
                if (MetPages.Count == 1)
                {
                    page.RawPage = MetPages.First();
                }
                PageType.Pages.Add(page);

                var childPages = new List<Page>();
                foreach (var modelChild in Children)
                {
                    childPages.AddRange(modelChild.GetPages(page));
                }
                page.Children.AddRange(childPages);
                
                return Enumerable.Repeat(page, 1);
            }
            else //tady je nekde chyba (asi)
            {
                var pages = organizer.Organize(MetPages, parent).ToList();
                foreach (var page in pages.SelectMany(p => p.GetDescendantsAndSelf()))
                {
                    page.PageType = PageType;
                    PageType.Pages.Add(page);
                }
                return pages;
            }
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public PageModel Clone()
        {
            var clone = new PageModel(this);         
            clone.Children.AddRange(Children.Select(c => c.Clone()));
            return clone;
        }
    }
}
