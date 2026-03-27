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

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;

namespace FuseCP.Portal
{
    public partial class CalendarControl : FuseCPControlBase
    {
        public DateTime SelectedDate
        {
            get
            {
                DateTime dt = DateTime.Now;
                try
                {
                    dt = DateTime.Parse(txtDate.Text);
                }
                catch (System.Exception swallowedEx) when (!(swallowedEx is System.OutOfMemoryException) && !(swallowedEx is System.StackOverflowException) && !(swallowedEx is System.AccessViolationException))
                {
                    System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message);
                }
                return dt;
            }
            set
            {
                txtDate.Text = value.ToString("d");
            }
        }

        public string CalendarCtrlClientID
        {
            get
            {
                this.EnsureChildControls();
               return Calendar.ClientID;

            }
        }

        public bool ValidationEnabled
        {
            get { return dateValidator.Enabled; }
            set { dateValidator.Enabled = value; }
        }

        public string ValidationGroup
        {
            get { return dateValidator.ValidationGroup; }
            set { dateValidator.ValidationGroup = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}
