// Copyright (C) 2025 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

// Material sourced from the bluePortal project (http://blueportal.codeplex.com).
// Licensed under the Microsoft Public License (available at http://www.opensource.org/licenses/ms-pl.html).

using System;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using BaseMenuAdapter = System.Web.UI.WebControls.Adapters.MenuAdapter;

namespace CSSFriendly
{
    public class MenuAdapter : BaseMenuAdapter
    {
        private WebControlAdapterExtender _extender = null;
        private WebControlAdapterExtender Extender
        {
            get
            {
                if (((_extender == null) && (Control != null)) ||
                    ((_extender != null) && (Control != _extender.AdaptedControl)))
                {
                    _extender = new WebControlAdapterExtender(Control); 
                }

                System.Diagnostics.Debug.Assert(_extender != null, "CSS Friendly adapters internal error", "Null extender instance");
                return _extender;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Extender.AdapterEnabled)
            {
                RegisterScripts();
            }
        }

        private void RegisterScripts()
        {
            Extender.RegisterScripts();
            string folderPath = WebConfigurationManager.AppSettings.Get("CSSFriendly-JavaScript-Path");
            if (String.IsNullOrEmpty(folderPath))
            {
                folderPath = "~/JavaScript";
            }
            string filePath = folderPath.EndsWith("/") ? folderPath + "MenuAdapter.js" : folderPath + "/MenuAdapter.js";
            Page.ClientScript.RegisterClientScriptInclude(GetType(), GetType().ToString(), Page.ResolveUrl(filePath));
        }

        protected override void RenderBeginTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                Extender.RenderBeginTag(writer, "AspNet-Menu2-" + Control.Orientation);
            }
            else
            {
                base.RenderBeginTag(writer);
            }
        }

        protected override void RenderEndTag(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                Extender.RenderEndTag(writer);
            }
            else
            {
                base.RenderEndTag(writer);
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (Extender.AdapterEnabled)
            {
                writer.Indent++;
                BuildItems(Control.Items, true, writer);
                writer.Indent--;
                writer.WriteLine();
            }
            else
            {
                base.RenderContents(writer);
            }
        }

        private void BuildItems(MenuItemCollection items, bool isRoot, HtmlTextWriter writer, string id = null)
        {
            if (items.Count > 0)
            {
                writer.WriteLine();

                writer.WriteBeginTag("ul");
                if (isRoot)
                {
                    writer.WriteAttribute("class", "AspNet-Menu2 main-menu");
                    writer.WriteAttribute("id", Control.ClientID);
                }
                else
                {
                    writer.WriteAttribute("class", "list-unstyled sub-menu collapse");
                    if (!String.IsNullOrEmpty(id))
                    {
                        writer.WriteAttribute("id", id);
                    }
                }
                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent++;
                               
                foreach (MenuItem item in items)
                {
                    BuildItem(item, writer);
                }

                writer.Indent--;
                writer.WriteLine();
                writer.WriteEndTag("ul");
            }
        }

        private void BuildItem(MenuItem item, HtmlTextWriter writer)
        {
            Menu menu = Control;
            if ((menu != null) && (item != null) && (writer != null))
            {
                bool hasChildItems = (item.ChildItems != null) && (item.ChildItems.Count > 0);
                bool canNavigate = IsLink(item) && !hasChildItems;
                string submenuId = hasChildItems ? GetSubMenuId(menu, item) : null;

                writer.WriteLine();
                writer.WriteBeginTag("li");

                string theClass = (item.ChildItems.Count > 0) ? "has-submenu" : "AspNet-Menu2-Leaf";
                string selectedStatusClass = GetSelectStatusClass(item);
                if (!String.IsNullOrEmpty(selectedStatusClass))
                {
                    theClass += " " + selectedStatusClass;
                }
                writer.WriteAttribute("class", theClass);

                writer.Write(HtmlTextWriter.TagRightChar);
                writer.Indent++;
                writer.WriteLine();

                if (((item.Depth < menu.StaticDisplayLevels) && (menu.StaticItemTemplate != null)) ||
                    ((item.Depth >= menu.StaticDisplayLevels) && (menu.DynamicItemTemplate != null)))
                {
                    writer.WriteBeginTag("div");
                    writer.WriteAttribute("class", GetItemClass(menu, item));
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Indent++;
                    writer.WriteLine();

                    MenuItemTemplateContainer container = new MenuItemTemplateContainer(menu.Items.IndexOf(item), item);
                    if ((item.Depth < menu.StaticDisplayLevels) && (menu.StaticItemTemplate != null))
                    {
                        menu.StaticItemTemplate.InstantiateIn(container);
                    }
                    else
                    {
                        menu.DynamicItemTemplate.InstantiateIn(container);
                    }
                    container.DataBind();
                    container.RenderControl(writer);

                    writer.Indent--;
                    writer.WriteLine();
                    writer.WriteEndTag("div");
                }
                else
                {
                    if (canNavigate)
                    {
                        writer.WriteBeginTag("a");
                        if (!String.IsNullOrEmpty(item.NavigateUrl))
                        {
                            writer.WriteAttribute("href", Page.Server.HtmlEncode(menu.ResolveClientUrl(item.NavigateUrl)));
                        }
                        else
                        {
                            writer.WriteAttribute("href", Page.ClientScript.GetPostBackClientHyperlink(menu, "b" +  item.ValuePath.Replace(menu.PathSeparator.ToString(), "\\"), true));
                        }

                        writer.WriteAttribute("class", GetItemClass(menu, item));
                        WebControlAdapterExtender.WriteTargetAttribute(writer, item.Target);

                        if (!String.IsNullOrEmpty(item.ToolTip))
                        {
                            writer.WriteAttribute("title", item.ToolTip);
                        }
                        else if (!String.IsNullOrEmpty(menu.ToolTip))
                        {
                            writer.WriteAttribute("title", menu.ToolTip);
                        }
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Indent++;
                        writer.WriteLine();
                    }
                    else
                    {
                        writer.WriteBeginTag("a"); //changed span to a
                        writer.WriteAttribute("href", "#");
                        writer.WriteAttribute("class", "submenu-toggle"); //GetItemClass(menu, item)
                        writer.WriteAttribute("data-bs-toggle", "collapse");
                        writer.WriteAttribute("data-bs-target", "#" + submenuId);
                        writer.WriteAttribute("aria-controls", submenuId);
                        writer.WriteAttribute("aria-expanded", "false");
                        writer.WriteAttribute("role", "button");
                        writer.Write(HtmlTextWriter.TagRightChar);
                        writer.Indent++;
                        writer.WriteLine();

                       
                    }
                    

                    //Add Icon Set // Modified by kuldeep
                    writer.WriteBeginTag("i");
                     
                    switch (item.Value.Trim().ToUpper())
                    {   case "ACCOUNT MENU":
                            writer.WriteAttribute("class", "bi bi-person");
                            break;
                        case "REPORTING":
                            writer.WriteAttribute("class", "bi bi-bar-chart");
                            break;
                        case "CONFIGURATION":
                            writer.WriteAttribute("class", "bi bi-gear-wide-connected");
                            break;
                        case "HOSTING SPACE MENU":
                            writer.WriteAttribute("class", "bi bi-hdd-rack");
                            break;
                        case "ORGANIZATION MENU":
                            writer.WriteAttribute("class", "bi bi-people");
                            break;
                        case "HOME":
                            writer.WriteAttribute("class", "bi bi-house-door");
                            break;
                        case "VPS MENU":
                            writer.WriteAttribute("class", "bi bi-display");
                            break;
                        //    case "VPS-MEN�":
                        //        writer.WriteAttribute("class", "bi bi-display");
                        //        break;
                        //    case "KONTOMEN�":
                        //        writer.WriteAttribute("class", "bi bi-person");
                        //break;
                        //    case "HOSTING-BEREICH MEN�":
                        //        writer.WriteAttribute("class", "bi bi-hdd-rack");
                        //break;
                        //case "ORGANISATIONSMEN�":
                        //        writer.WriteAttribute("class", "bi bi-people");
                        //break;
                        //    case "KONTO�BERSICHT":
                        //        writer.WriteAttribute("class", "bi bi-house-door");
                        //break;
                        //    case "BERICHTE":
                        //        writer.WriteAttribute("class", "bi bi-bar-chart");
                        //        break;
                        //    case "KONFIGURATION":
                        //        writer.WriteAttribute("class", "bi bi-gear-wide-connected");
                        //        break;
                        //    case "AUFGABENPLANUNG":
                        //        if (item.Parent == null)
                        //            writer.WriteAttribute("class", "bi bi-clock");
                        //        break;
                        case "SPACESCHEDULEDTASKS":
                            if (item.Parent == null)
                                writer.WriteAttribute("class", "bi bi-clock");
                            break;


                    }
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Indent++;
                    writer.WriteLine();
                    writer.WriteEndTag("i");
                    //---------------
                    
                    if (!String.IsNullOrEmpty(item.ImageUrl))
                    {
                        writer.WriteBeginTag("img");
                        writer.WriteAttribute("src", menu.ResolveClientUrl(item.ImageUrl));
                        writer.WriteAttribute("alt", !String.IsNullOrEmpty(item.ToolTip) ? item.ToolTip : (!String.IsNullOrEmpty(menu.ToolTip) ? menu.ToolTip : item.Text));
                        writer.Write(HtmlTextWriter.SelfClosingTagEnd);
                    }

                    //Added By kuldeep---------------
                    writer.WriteBeginTag("span");
                    writer.WriteAttribute("class", "text");
                    writer.Write(HtmlTextWriter.TagRightChar);
                    writer.Indent++;
                    writer.WriteLine();
                    writer.Write(item.Text);
                    writer.WriteEndTag("span");
                    //---------------

                    if (IsLink(item))
                    {
                        writer.Indent--;
                        writer.WriteLine();
                        writer.WriteEndTag("a");
                    }
                    else
                    {
                        writer.Indent--;
                        writer.WriteLine();
                        writer.WriteEndTag("a"); //changed span to a
                    }

                }

                if (hasChildItems)
                {
                    BuildItems(item.ChildItems, false, writer, submenuId);
                }

                writer.Indent--;
                writer.WriteLine();
                writer.WriteEndTag("li");
            }
        }

        private bool IsLink(MenuItem item)
        {
            return (item != null) && item.Enabled && ((!String.IsNullOrEmpty(item.NavigateUrl)) || item.Selectable);
        }

        private string GetSubMenuId(Menu menu, MenuItem item)
        {
            string baseId = (menu != null ? menu.ClientID : "menu") + "_submenu_" + (item != null ? item.ValuePath : "item");
            return Regex.Replace(baseId, "[^A-Za-z0-9_-]", "_");
        }

        private string GetItemClass(Menu menu, MenuItem item)
        {
            string value = "AspNet-Menu2-NonLink";
            if (item != null)
            {
                if (((item.Depth < menu.StaticDisplayLevels) && (menu.StaticItemTemplate != null)) ||
                    ((item.Depth >= menu.StaticDisplayLevels) && (menu.DynamicItemTemplate != null)))
                {
                    value = "AspNet-Menu2-Template";
                }
                else if (IsLink(item))
                {
                    value =  "AspNet-Menu2-Link";
                }
                string selectedStatusClass = GetSelectStatusClass(item);
                if (!String.IsNullOrEmpty(selectedStatusClass))
                {
                    value += " " + selectedStatusClass;
                }
            }
            return value;
        }

        private string GetSelectStatusClass(MenuItem item)
        { 
            string value = "";
            if (item.Selected)
            {
               // value += " AspNet-Menu2-Selected";
                value += " active";
            }
            else if (IsChildItemSelected(item))
            {
                //value += " AspNet-Menu2-ChildSelected";
                value += " active";
            }
            else if (IsParentItemSelected(item))
            {
                //value += " AspNet-Menu2-ParentSelected";
                value += " active";
            }
            return value;
        }

        private bool IsChildItemSelected(MenuItem item)
        {
            bool bRet = false;

            if ((item != null) && (item.ChildItems != null))
            {
                bRet = IsChildItemSelected(item.ChildItems);
            }

            return bRet;
        }

        private bool IsChildItemSelected(MenuItemCollection items)
        {
            bool bRet = false;

            if (items != null)
            {
                foreach (MenuItem item in items)
                {
                    if (item.Selected || IsChildItemSelected(item.ChildItems))
                    {
                        bRet = true;
                        break;
                    }
                }
            }

            return bRet;
        }

        private bool IsParentItemSelected(MenuItem item)
        {
            bool bRet = false;

            if ((item != null) && (item.Parent != null))
            {
                bRet = item.Parent.Selected || IsParentItemSelected(item.Parent);







            }

            return bRet;
        }
    }
}
