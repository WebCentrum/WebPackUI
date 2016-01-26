// <copyright file="OrganizerModel.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Tomáš Pouzar</author>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebpackUI.Models
{
    /// <summary>
    /// Represent a an Organizer configuration
    /// </summary>  
    [DataContract(Name = "organizerModel", Namespace = "")]
    public class OrganizerModel
    {
        public OrganizerModel()
        {
            Pages = new List<PageModel>();
        }

        [DataMember(Name = "pages")]
        public List<PageModel> Pages { get; set; }
    }

    /// <summary>
    /// Represent a single page stored in XML document
    /// </summary>  
    [DataContract(Name = "page", Namespace = "")]
    public class PageModel
    {
        public PageModel()
        {
            Properties = new List<PagePropertyModel>();
            Children = new List<PageModel>();
        }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "properties")]
        public List<PagePropertyModel> Properties { get; set; }


        [DataMember(Name = "parentId")]
        public string ParentId { get; set; }

        [DataMember(Name = "children")]
        public List<PageModel> Children { get; set; }
    }

    /// <summary>
    /// Represent page's property values
    /// </summary>  
    [DataContract(Name = "pageProperty", Namespace = "")]
    public class PagePropertyModel
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "html")]
        public string Html { get; set; }
    }
}