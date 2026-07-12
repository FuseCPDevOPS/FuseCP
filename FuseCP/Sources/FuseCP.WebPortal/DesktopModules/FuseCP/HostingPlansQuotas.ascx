<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HostingPlansQuotas.ascx.cs" Inherits="FuseCP.Portal.HostingPlansQuotas" %>
<%@ Register Src="UserControls/QuotaEditor.ascx" TagName="QuotaEditor" TagPrefix="uc1" %>

<div class="hosting-plans-quotas" id="hostingPlansQuotasRoot">
    <asp:Repeater ID="dlGroups" runat="server">
        <ItemTemplate>
            <asp:Panel ID="GroupPanel" runat="server" CssClass="card border-info">
                <div class="card-header">
                    <div class="row">
                        <div class="col-6">
                            <asp:CheckBox ID="chkEnabled" runat="server" Checked='<%# (bool)Eval("Enabled") & (bool)Eval("ParentEnabled") %>' Enabled='<%# Eval("ParentEnabled") %>' Text='<%# GetSharedLocalizedString("ResourceGroup." + (string)Eval("GroupName")) %>' onclick="fusecpToggleQuotaGroup(this);" />
                            <asp:Literal ID="groupId" runat=server Text='<%# Eval("GroupID") %>' Visible=false></asp:Literal>
                        </div>
                        <div class="col-6 text-end">
                            <asp:CheckBox ID="chkCountDiskspace" runat="server" meta:resourcekey="chkCountDiskspace" Checked='<%# Eval("CalculateDiskspace") %>' Visible='<%# IsPlan %>' Text="Count Diskspace" />&nbsp;
                            <asp:CheckBox ID="chkCountBandwidth" runat="server" meta:resourcekey="chkCountBandwidth" Checked='<%# Eval("CalculateBandwidth") %>' Visible='<%# IsPlan %>' Text="Count Bandwidth" />&nbsp;
                        </div>
                    </div>
                </div>
                <asp:Panel ID="QuotaPanel" runat="server" CssClass="card-body">
                    <asp:DataList ID="dlQuotas" runat="server" CssClass="table table-hover" DataSource='<%# GetGroupQuotas((int)Eval("GroupID")) %>' RepeatColumns="1">
                        <ItemTemplate>
                            <div class="row">
                                <div class="col-6 col-md-3">
                                    <%# GetSharedLocalizedStringNotEmpty((string)Eval("QuotaName"), Eval("QuotaDescription"))%>:
                                </div>
                                <div class="col-6 col-md-9">
                                    <uc1:QuotaEditor id="quotaEditor" runat="server" QuotaID='<%# Eval("QuotaID") %>' QuotaTypeID='<%# Eval("QuotaTypeID") %>' QuotaValue='<%# Eval("QuotaValue") %>' ParentQuotaValue='<%# Eval("ParentQuotaValue") %>'>
                                    </uc1:QuotaEditor>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:DataList>
                </asp:Panel>
            </asp:Panel>
        </ItemTemplate>
    </asp:Repeater>
</div>

<script type="text/javascript">
    (function() {
        function findGroupContainer(element) {
            while (element && element.nodeType === 1) {
                if (element.classList && element.classList.contains("card")) {
                    return element;
                }
                element = element.parentElement;
            }
            return null;
        }

        function toggleGroup(checkbox) {
            if (!checkbox) {
                return;
            }

            var group = findGroupContainer(checkbox);
            if (!group) {
                return;
            }

            var showQuotas = checkbox.checked;
            var quotaPanel = group.querySelector("[id$='QuotaPanel']");
            if (quotaPanel) {
                quotaPanel.style.display = showQuotas ? "" : "none";
            }

            var diskspaceCheckbox = group.querySelector("input[id$='chkCountDiskspace']");
            if (diskspaceCheckbox) {
                diskspaceCheckbox.disabled = !showQuotas;
            }

            var bandwidthCheckbox = group.querySelector("input[id$='chkCountBandwidth']");
            if (bandwidthCheckbox) {
                bandwidthCheckbox.disabled = !showQuotas;
            }
        }

        function initializeGroups() {
            var root = document.getElementById("hostingPlansQuotasRoot");
            if (!root) {
                return;
            }

            var enabledCheckboxes = root.querySelectorAll("input[id$='chkEnabled']");
            for (var i = 0; i < enabledCheckboxes.length; i++) {
                toggleGroup(enabledCheckboxes[i]);
            }
        }

        window.fusecpToggleQuotaGroup = toggleGroup;

        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", initializeGroups);
        } else {
            initializeGroups();
        }

        if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(initializeGroups);
        }
    })();
</script>
