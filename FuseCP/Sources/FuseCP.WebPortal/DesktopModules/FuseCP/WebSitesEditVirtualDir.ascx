<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebSitesEditVirtualDir.ascx.cs" Inherits="FuseCP.Portal.WebSitesEditVirtualDir" %>
<%@ Register Src="WebSitesCustomErrorsControl.ascx" TagName="WebSitesCustomErrorsControl" TagPrefix="uc4" %>
<%@ Register Src="WebSitesMimeTypesControl.ascx" TagName="WebSitesMimeTypesControl" TagPrefix="uc5" %>
<%@ Register Src="VirtualDirectoryHomeFolderControl.ascx" TagName="VirtualDirectoryHomeFolderControl" TagPrefix="uc1" %>
<%@ Register Src="WebSitesCustomHeadersControl.ascx" TagName="WebSitesCustomHeadersControl" TagPrefix="uc6" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/websites-edit-site.js"></script>

<fcp:EnableAsyncTasksSupport ID="asyncTasks" runat="server" />

<div class="card-body form-horizontal fcp-modern-page">
    <table>
        <tr>
            <td class="align-top">
                <table>
                    <tr>
                        <td class="Big">
                            <asp:HyperLink ID="lnkSiteName" runat="server" NavigateUrl="#" Target="_blank">domain.com/vdir</asp:HyperLink>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <div class="widget">
                    <div class="fcp-modern-tabs">
                        <ul class="fcp-modern-nav-tabs nav nav-tabs" role="tablist">
                            <asp:DataList ID="dlTabs" runat="server" RepeatDirection="Horizontal"
                                OnSelectedIndexChanged="dlTabs_SelectedIndexChanged" RepeatLayout="Flow">
                                <ItemStyle Wrap="False" />
                                <ItemTemplate>
                                    <li class="nav-item" role="presentation">
                                        <asp:LinkButton ID="cmdSelectTab" runat="server" CommandName="select" CssClass="nav-link">
                                            <%# Eval("Name") %>
                                        </asp:LinkButton>
                                    </li>
                                </ItemTemplate>
                                <SelectedItemStyle Wrap="False" />
                                <SelectedItemTemplate>
                                    <li class="nav-item" role="presentation">
                                        <asp:LinkButton ID="cmdSelectTab" runat="server" CommandName="select" CssClass="nav-link active" aria-current="page">
                                            <%# Eval("Name") %>
                                        </asp:LinkButton>
                                    </li>
                                </SelectedItemTemplate>
                            </asp:DataList>
                        </ul>
                    </div>
                    <div class="widget-content tab-content">
                        <div class="card-body form-horizontal">
                            <asp:MultiView ID="tabs" runat="server" ActiveViewIndex="0">
                                <asp:View ID="tabHomeFolder" runat="server">
                                    <uc1:VirtualDirectoryHomeFolderControl ID="VirtualDirectoryHomeFolderControl" runat="server" IsVirtualDirectory="true" />
                                </asp:View>

                                <%--<asp:View ID="tabErrors" runat="server">
                                    <uc4:WebSitesCustomErrorsControl ID="webSitesCustomErrorsControl" runat="server" />
                                </asp:View>

                                <asp:View ID="tabHeaders" runat="server">
                                    <uc6:WebSitesCustomHeadersControl ID="webSitesCustomHeadersControl" runat="server" />
                                </asp:View>

                                <asp:View ID="tabMimes" runat="server">
                                    <uc5:WebSitesMimeTypesControl ID="webSitesMimeTypesControl" runat="server" />
                                </asp:View>--%>

                            </asp:MultiView>
                        </div>
                        <div class="card-footer text-end">
                            <asp:LinkButton ID="btnDelete" CssClass="btn btn-danger" runat="server" OnClick="btnDelete_Click" CausesValidation="false" OnClientClick="return confirm('Delete this virtual directory?');">
                                <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteText" />
                            </asp:LinkButton>
                            &nbsp;
                    <asp:LinkButton ID="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click">
                        <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel" />
                    </asp:LinkButton>
                            &nbsp;
                    <asp:LinkButton ID="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click" OnClientClick="ShowProgressDialog('Please Wait! Updating virtual directory...');">
                        <i class="bi bi-arrow-clockwise me-1"></i><asp:Localize runat="server" meta:resourcekey="btnUpdateText" />
                    </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</div>
