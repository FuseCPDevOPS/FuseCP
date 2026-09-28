<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RDSEditCollectionUsers.ascx.cs" Inherits="FuseCP.Portal.RDS.RDSEditCollectionUsers" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register Src="UserControls/RDSCollectionUsers.ascx" TagName="CollectionUsers" TagPrefix="fcp"%>
<%@ Register Src="UserControls/RDSCollectionTabs.ascx" TagName="CollectionTabs" TagPrefix="fcp" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="../UserControls/ItemButtonPanel.ascx" TagName="ItemButtonPanel" TagPrefix="fcp" %>
<script type="text/javascript">
	if (!window.jQuery) {
		document.write('<script src="/JavaScript/jquery-2.1.0.min.js"><\/script>');
	}
</script>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>


                        <div class="card-header">
                            <asp:Image ID="imgEditRDSCollection" SkinID="EnterpriseRDSCollections48" runat="server" />
                            <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Edit RDS Collection"></asp:Localize>
                            -
                            <asp:Literal ID="litCollectionName" runat="server" Text="" />
                        </div>
                        <div class="card-body form-horizontal">
                            <fcp:CollectionTabs id="tabs" runat="server" SelectedTab="rds_collection_edit_users" />   
                        <div class="card tab-content">
                        <div class="card-body form-horizontal">
                            <asp:UpdatePanel runat="server" ID="messageUpdatePanel">
                                <ContentTemplate>
                                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                                </ContentTemplate>                                
                            </asp:UpdatePanel>
         					
                            <fcp:CollapsiblePanel id="secRdsUsers" runat="server"
                                TargetControlID="panelRdsUsers" meta:resourcekey="secRdsUsers" Text="">
                            </fcp:CollapsiblePanel>		
                            <asp:Panel runat="server" ID="panelRdsUsers">                                                
                                <div class="fcp-p-10">
                                    <fcp:CollectionUsers id="users" runat="server" />
                                </div>                            
                            </asp:Panel>
                            <div>
                                <asp:Localize ID="locQuota" runat="server" meta:resourcekey="locQuota" Text="Users Created:"></asp:Localize>
                                &nbsp;&nbsp;&nbsp;
                                <fcp:QuotaViewer ID="usersQuota" runat="server" QuotaTypeId="2" DisplayGauge="true" />
                            </div>
                        
                        </div>
                                    <div class="text-end">
                                <fcp:ItemButtonPanel id="buttonPanel" runat="server" ValidationGroup="SaveRDSCollection" 
                                    OnSaveClick="btnSave_Click" OnSaveExitClick="btnSaveExit_Click" />
                            </div>
                    </div>
                    </div>
