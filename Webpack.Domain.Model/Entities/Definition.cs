// <copyright file="Definition.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Matej Chudo</author>
namespace Webpack.Domain.Model.Entities
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// A property of a PageType
    /// </summary>
    [DebuggerDisplay("Name = {name}")]
    public class Definition
    {
        public bool IsMacro { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string TemplateReference { get; set; }
    }
}