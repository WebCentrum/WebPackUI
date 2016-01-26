// <copyright file="WebpackApplication.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Tomáš Pouzar</author>using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.businesslogic;
using umbraco.interfaces;

namespace WebpackUI.Sections
{
    /// <summary>
    /// WebPack application
    /// </summary>
    [Application("Webpack", "Webpack", "icon-box-open", 15)]
    public class WebpackApplication : IApplication { }
}