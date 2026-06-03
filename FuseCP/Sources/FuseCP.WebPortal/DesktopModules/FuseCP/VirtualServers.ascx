<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VirtualServers.ascx.cs" Inherits="FuseCP.Portal.VirtualServers" %>
<%@ Import Namespace="FuseCP.Portal" %>
<div class="buttons-in-panel-header">
    <asp:LinkButton ID="btnAddItem"  runat="server" CssClass="btn btn-primary" OnClick="btnAddItem_Click">
        <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddItem" />
    </asp:LinkButton>
</div>
<section>
    <div class="card-body">
        <div class="row g-3">
            <asp:Repeater ID="dlServers" Runat="server">
                <ItemTemplate>
                    <div class="col-sm-12 col-md-6 col-lg-4">
                        <div class="card border-info server-panel matchHeight h-100">
                            <div class="card-header d-flex justify-content-between align-items-center gap-2">
                                <h3 class="card-title m-0 d-flex align-items-center gap-2" title="<%# PortalAntiXSS.EncodeOld((((string)Eval("ServerName")) ?? string.Empty).Trim()) %>">
                                    <i class="bi bi-server" aria-hidden="true"></i>
                                    <span><%# PortalAntiXSS.EncodeOld((((string)Eval("ServerName")) ?? string.Empty).Trim()) %></span>
                                </h3>
                                <asp:hyperlink id=lnkEdit runat="server" CssClass="btn btn-secondary btn-sm flex-shrink-0" NavigateUrl='<%# EditUrl("ServerID", Eval("ServerID").ToString(), "edit_server") %>'>
                                    <i class="bi bi-cogs" aria-hidden="true"></i>
                                    <span class="ms-1">Settings</span>
                                </asp:hyperlink>
                            </div>
                            <ul class="list-group list-group-flush">
                                <li class="list-group-item">
                                    <%# PortalAntiXSS.EncodeOld((string)Eval("Comments")) %>
                                </li>
                                <li class="list-group-item">
                                    <%# Eval("ServicesNumber") %>
                                    <asp:Localize ID="locServices" runat="server" meta:resourcekey="locServices" Text="services" />
                                </li>
                            </ul>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</section>
<table id="tblEmptyList" runat="server" class="table table-borderless mb-0 w-100">
    <tr>
        <td class="Normal text-center">
            <asp:Label ID="lblEmptyList" runat="server" meta:resourcekey="lblEmptyList" Text="Empty list..."></asp:Label>
        </td>
    </tr>
</table>
<div class="card-footer text-end">
    <asp:LinkButton ID="StyleButton1"  runat="server" CssClass="btn btn-primary" OnClick="btnAddItem_Click">
        <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddItem" />
    </asp:LinkButton>
</div>
