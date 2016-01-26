// <copyright file="ModelAnalyzer.cs" company="ÚVT MU">
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
    using Webpack.Domain.Model.Logic;

    /// <summary>
    /// Model Analyzer
    /// </summary>
    public abstract class ModelAnalyzer
    {
        private readonly PageModel hook = new PageModel("HOOK") { FallsThrough = Allways };
        private readonly PageModel root = new PageModel("ROOT") { FallsThrough = Allways };
        private readonly PageModel lang = new PageModel("LANG") { FallsThrough = Allways };

        private readonly List<PageModel> langVersions = new List<PageModel>();
        private readonly List<string> macroXpaths = new List<string>();

        protected PageModel Hook
        {
            get { return hook; }
        }

        private PageModel Root
        {
            get { return root; }
        }

        public IEnumerable<RawPage> MetPages
        {
            get
            {
                return Root.AllMetPages;
            }
        }

        /// <summary>
        /// Build
        /// </summary>
        /// <returns></returns>
        public abstract void Build();

        /// <summary>
        /// Build Language Versions
        /// </summary>
        /// <returns></returns>
        private void BuildLanguageVersions()
        {
            foreach (var langVersion in langVersions)
            {
                var copy = Hook.Clone() as PageModel;
                langVersion.Children.AddRange(copy.Children);
                copy.Children.Clear();
                root.Children.Add(langVersion);
            }
        }

        /// <summary>
        /// Prepare
        /// </summary>
        /// <returns></returns>
        public void Prepare()
        {
            Build();
            BuildLanguageVersions();
            BuildPageTypes();
        }

        /// <summary>
        /// Build Page Types
        /// </summary>
        /// <returns></returns>
        private void BuildPageTypes()
        {
            var queue = new Queue<PageModel>();
            queue.Enqueue(root);
            var pageTypes = new Dictionary<string, PageType>();
            while (queue.Any())
            {
                var model = queue.Dequeue();
                PageType pageType;
                if (!pageTypes.TryGetValue(model.Name, out pageType))
                {
                    pageType = new PageType { Name = model.Name, ID = Guid.NewGuid() };
                    pageTypes.Add(model.Name, pageType);
                }
                model.PageType = pageType;

                foreach (var child in model.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        /// <summary>
        /// Analyze
        /// </summary>
        /// <param name="pages">pages</param>
        /// <returns></returns>
        public PagesTree Analyze(IEnumerable<RawPage> pages)
        {
            if (pages == null)
            {
                throw new ArgumentNullException("pages");
            }
            
            Root.Handle(pages);
            var root = Root.GetPages(null).Single();

            return new PagesTree(root);
        }

        /// <summary>
        /// Get Pages Types
        /// </summary>
        /// <returns></returns>
        public List<PageType> GetPagesTypes()
        {
            var queue = new Queue<PageModel>();
            queue.Enqueue(root);
            var result = new HashSet<PageType>();
            while (queue.Any())
            {
                var model = queue.Dequeue();
                
                var pt = model.PageType;
                //pt.MacroXpaths = macroXpaths.Concat(model.MacroXpaths).ToArray();
                result.Add(pt);

                foreach (var child in model.Children)
                {
                    queue.Enqueue(child);
                }
            }
            return result.ToList();
        }

        protected static Condition<RawPage> Allways
        {
            get { return Condition<RawPage>.Allways(); }
        }

        protected static Condition<RawPage> Never
        {
            get { return Condition<RawPage>.Never(); }
        }

        /// <summary>
        /// Rule
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns></returns>
        protected static Condition<RawPage> Rule(Func<RawPage, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            return Condition<RawPage>.Create().Rule(predicate);
        }

        /// <summary>
        /// All
        /// </summary>
        /// <param name="applyRules">apply Rules</param>
        /// <returns></returns>
        protected static Condition<RawPage> All(Action<Condition<RawPage>> applyRules)
        {
            if (applyRules == null)
            {
                throw new ArgumentNullException("applyRules");
            }

            return Condition<RawPage>.Create().All(applyRules);
        }

        /// <summary>
        /// Any
        /// </summary>
        /// <param name="applyRules">apply Rules</param>
        /// <returns></returns>
        protected static Condition<RawPage> Any(Action<Condition<RawPage>> applyRules)
        {
            if (applyRules == null)
            {
                throw new ArgumentNullException("applyRules");
            }

            return Condition<RawPage>.Create().Any(applyRules);
        }

        /// <summary>
        /// Member
        /// </summary>
        /// <param name="selector">selector</param>
        /// <param name="applyRules">apply Rules</param>
        /// <returns></returns>
        protected static Condition<RawPage> Member<TMember>(Func<RawPage, TMember> selector, Action<Condition<TMember>> applyRules)
        {
            if (applyRules == null)
            {
                throw new ArgumentNullException("applyRules");
            }

            return Condition<RawPage>.Create().Member(selector, applyRules);
        }

        /// <summary>
        /// Add Language Version
        /// </summary>
        /// <param name="fallThrough">fall Through</param>
        /// <returns></returns>
        protected void AddLanguageVersion(Condition<RawPage> fallThrough)
        {
            if (fallThrough == null)
            {
                throw new ArgumentNullException("fallThrough");
            }
            
            var newLang = lang.Clone();
            newLang.FallsThrough = fallThrough;

            langVersions.Add(newLang);
        }

        /// <summary>
        /// Add Global Macro Xpath
        /// </summary>
        /// <param name="xpaths">xpaths</param>
        /// <returns></returns>
        protected void AddGlobalMacroXpath(params string[] xpaths)
        {
            if (xpaths == null || !xpaths.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentNullException("xpath");
            }

            macroXpaths.AddRange(xpaths);
        }
    }
}
