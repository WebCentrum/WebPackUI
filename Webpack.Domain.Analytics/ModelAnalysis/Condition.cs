// <copyright file="ArticlesSiteAnalyzer.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Analytics.ModelAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Condition
    /// </summary>
    public class Condition<TSource>
    {
        private readonly List<Func<TSource, bool>> predicates = new List<Func<TSource, bool>>();

        private readonly Func<bool, bool, bool> agregator;

        /// <summary>
        /// Condition
        /// </summary>
        /// <param name="agregator">agregator</param>
        /// <returns></returns>
        private Condition(Func<bool, bool, bool> agregator = null)
        {
            this.agregator = agregator ?? ((b1, b2) => b1 && b2);
        }

        /// <summary>
        /// Rule
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns></returns>
        public Condition<TSource> Rule(Func<TSource, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            predicates.Add(predicate);
            return this;
        }

        /// <summary>
        /// Any
        /// </summary>
        /// <param name="applyRules">apply Rules</param>
        /// <returns></returns>
        public Condition<TSource> Any(Action<Condition<TSource>> applyRules)
        {
            if (applyRules == null)
            {
                throw new ArgumentNullException("applyRules");
            }

            var predicate = new Condition<TSource>((b1, b2) => b1 || b2);
            applyRules(predicate);

            predicates.Add(predicate.Evaluate);
            return this;
        }

        /// <summary>
        /// All
        /// </summary>
        /// <param name="applyRules">apply Rules</param>
        /// <returns></returns>
        public Condition<TSource> All(Action<Condition<TSource>> applyRules)
        {
            if (applyRules == null)
            {
                throw new ArgumentNullException("applyRules");
            }

            var predicate = Create();
            applyRules(predicate);

            predicates.Add(predicate.Evaluate);
            return this;
        }

        /// <summary>
        /// Member
        /// </summary>
        /// <param name="selector">selector</param>
        /// <param name="applyRules">apply Rules</param>
        /// <returns></returns>
        public Condition<TSource> Member<TMember>(Func<TSource, TMember> selector, Action<Condition<TMember>> applyRules)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            if (applyRules == null)
            {
                throw new ArgumentNullException("applyRules");
            }

            var predicate = new Condition<TMember>();
            applyRules(predicate);

            predicates.Add(p => predicate.Evaluate(selector(p)));
            return this;
        }

        /// <summary>
        /// Member Any
        /// </summary>
        /// <param name="selector">selector</param>
        /// <param name="applyRules">apply Rules</param>
        /// <returns></returns>
        public Condition<TSource> MemberAny<TMember>(Func<TSource, TMember> selector, Action<Condition<TMember>> applyRules)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            if (applyRules == null)
            {
                throw new ArgumentNullException("applyRules");
            }

            var predicate = new Condition<TMember>((b1, b2) => b1 || b2);
            applyRules(predicate);

            predicates.Add(p => predicate.Evaluate(selector(p)));
            return this;
        }

        /// <summary>
        /// Member Regex
        /// </summary>
        /// <param name="selector">selector</param>
        /// <param name="patern">patern</param>
        /// <returns></returns>
        public Condition<TSource> MemberRegex<TMember>(Func<TSource, string> selector, string patern)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            if (patern == null)
            {
                throw new ArgumentNullException("applyRules");
            }

            predicates.Add(p => Regex.IsMatch(selector(p), patern));
            return this;
        }

        /// <summary>
        /// Evaluate
        /// </summary>
        /// <param name="argument">argument</param>
        /// <returns></returns>
        public bool Evaluate(TSource argument)
        {
            return predicates.Select(p => p(argument)).Aggregate(agregator);
        }

        /// <summary>
        /// Allways
        /// </summary>
        /// <returns></returns>
        public static Condition<TSource> Allways()
        {
            return Create().Rule(c => true);
        }

        /// <summary>
        /// Never
        /// </summary>
        /// <returns></returns>
        public static Condition<TSource> Never()
        {
            return Create().Rule(c => false);
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <returns></returns>
        public static Condition<TSource> Create()
        {
            return new Condition<TSource>();
        }
    }
}
