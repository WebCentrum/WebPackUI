// <copyright file="WebpackTreeController.cs" company="ÚVT MU">
//     Copyright (c) ÚVT MU. All rights reserved.
// </copyright>
// <author>Tomáš Pouzar</author>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using umbraco;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using WebpackUI.Models;

namespace WebpackUI.Controllers
{
    /// <summary>
    /// Tree controller that specifies WebPack node settings
    /// </summary>
    [PluginController("Webpack")]
    [Tree("Webpack", "WebpackTree", "Webpack", iconClosed: "icon-doc")]
    public class WebpackTreeController : TreeController
    {
        /// <summary>
        /// Returns all section tree nodes
        /// </summary>
        /// <param name="id">Node's id.</param>
        /// <param name="queryStrings">Query strings</param>
        /// <returns>Collection of tree nodes</returns>
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();
            var ctrl = new WebsiteApiController();            

            if (id == Constants.System.Root.ToInvariantString())
            {
                var item = CreateTreeNode("0", "-1", queryStrings, "Websites", "icon-folder", true);
                nodes.Add(item);
                
                return nodes;
            }
            else if (id == "0")
            {
                foreach (var website in ctrl.GetAll())
                {
                    var node = CreateTreeNode(
                        website.Id.ToString(),
                        "0",
                        queryStrings,
                        website.ToString(),
                        "icon-document",
                        false);
                    
                    nodes.Add(node);
                }

                return nodes;
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns context menu for each tree node
        /// </summary>
        /// <param name="id">Node's id.</param>
        /// <param name="queryStrings">Query strings</param>
        /// <returns>Collection of menu items</returns>
        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            if (id == Constants.System.Root.ToInvariantString())
            {
                // WebPack node actions:       
                menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
                return menu;
            }
            else if(id == "0")
            {
                // Websites node actions:
                MenuItem createItem = new MenuItem("Create", ActionNew.Instance.Alias);
                createItem.Name = "Create";
                createItem.Icon = "add";
                menu.Items.Add(createItem);
                //menu.Items.Add<CreateChildEntity, ActionNew>(ActionNew.Instance.Alias);
                menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
                return menu;
            }
            else
            {
                // Website node actions:
                menu.Items.Add<ActionDelete>(ui.Text("actions", ActionDelete.Instance.Alias));                
            }
            return menu;
        }
    }
}