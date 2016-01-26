// <copyright file="Analysis.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The result of analysis of a website.
    /// </summary>
    public class Analysis : EntityBase
    {
        /// <summary>
        /// Backing field for property Sites
        /// </summary>
        private IList<Site> sites = new List<Site>();

        /// <summary>
        /// Backing field for property <c>Rawpages</c>
        /// </summary>
        private IList<RawPage> rawPages = new List<RawPage>();

        /// <summary>
        /// Gets or sets sites that are the result of each step in the analysis process
        /// </summary>
        public virtual IList<Site> Sites
        {
            get { return this.sites; }
            set { this.sites = value ?? new List<Site>(); }
        }

        /// <summary>
        /// Gets or sets the unmodified raw pages as downloaded at beginning of the analysis process
        /// </summary>
        public virtual IList<RawPage> RawPages
        {
            get { return this.rawPages; }
            set { this.rawPages = value ?? new List<RawPage>(); }
        }

        /// <summary>
        /// Gets or sets the domain of the analyzed site
        /// </summary>
        public virtual string Domain { get; set; }

        /// <summary>
        /// Gets or sets the time when the analysis was created
        /// </summary>
        public virtual DateTime Created { get; set; }
    }
}
