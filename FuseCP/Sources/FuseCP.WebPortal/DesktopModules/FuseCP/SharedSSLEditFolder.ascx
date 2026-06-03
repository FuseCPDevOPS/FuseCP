<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharedSSLEditFolder.ascx.cs" Inherits="FuseCP.Portal.SharedSSLEditFolder" %>
<%@ Register Src="WebSitesExtensionsControl.ascx" TagName="WebSitesExtensionsControl" TagPrefix="uc6" %>
<%@ Register Src="WebSitesCustomErrorsControl.ascx" TagName="WebSitesCustomErrorsControl" TagPrefix="uc4" %>
<%@ Register Src="WebSitesMimeTypesControl.ascx" TagName="WebSitesMimeTypesControl" TagPrefix="uc5" %>
<%@ Register Src="WebSitesHomeFolderControl.ascx" TagName="WebSitesHomeFolderControl" TagPrefix="uc1" %>
<%@ Register Src="WebSitesCustomHeadersControl.ascx" TagName="WebSitesCustomHeadersControl" TagPrefix="uc6" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/mail-confirmation.js"></script>

<div class="card-body form-horizontal fcp-modern-page">
    <div class="FormRow">
        <asp:HyperLink ID="lnkSiteName" runat="server" CssClass="Big" NavigateUrl="#" Target="_blank">domain.com/vdir</asp:HyperLink>
    </div>
    <div class="FormRow widget">
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
                <uc1:WebSitesHomeFolderControl ID="webSitesHomeFolderControl" runat="server" IsAppVirtualDirectory="true" />
            </asp:View>
            
            <asp:View ID="tabExtensions" runat="server">
                <uc6:WebSitesExtensionsControl ID="webSitesExtensionsControl" runat="server" IsAppVirtualDirectory="true" />
            </asp:View>
            
            <asp:View ID="tabErrors" runat="server">
                <uc4:WebSitesCustomErrorsControl ID="webSitesCustomErrorsControl" runat="server" />
            </asp:View>
            
            <asp:View ID="tabHeaders" runat="server">
                <uc6:WebSitesCustomHeadersControl ID="webSitesCustomHeadersControl" runat="server" />
            </asp:View>
            
            <asp:View ID="tabMimes" runat="server">
                <uc5:WebSitesMimeTypesControl ID="webSitesMimeTypesControl" runat="server" />
            </asp:View>
            
           </asp:MultiView>
       </div>
        </div>
    </div>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnDelete" CssClass="btn btn-danger" runat="server" CausesValidation="False" OnClick="btnDelete_Click" OnClientClick="fuseCpConfirmWithProgress('Are you sure you want to delete this Shared SSL Folder?', 'Deleting Shared SSL Folder...');"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteText"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click" ValidationGroup="Server" OnclientClick="ShowProgressDialog('Saving Shared SSL Folder...')"> <i class="bi bi-arrow-clockwise me-1"></i><asp:Localize runat="server" meta:resourcekey="btnUpdateText"/> </asp:LinkButton>
</div>
