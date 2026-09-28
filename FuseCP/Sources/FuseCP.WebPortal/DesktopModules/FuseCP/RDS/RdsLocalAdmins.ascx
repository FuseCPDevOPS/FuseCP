<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RDSLocalAdmins.ascx.cs" Inherits="FuseCP.Portal.RDS.RdsLocalAdmins" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register Src="UserControls/RDSCollectionTabs.ascx" TagName="CollectionTabs" TagPrefix="fcp" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="../UserControls/ItemButtonPanel.ascx" TagName="ItemButtonPanel" TagPrefix="fcp" %>
<%@ Register Src="UserControls/RDSCollectionUsers.ascx" TagName="CollectionUsers" TagPrefix="fcp"%>
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
                <fcp:CollectionTabs id="tabs" runat="server" SelectedTab="rds_collection_local_admins" />
                <div class="card tab-content">
                <div class="card-body form-horizontal">
				    <fcp:SimpleMessageBox id="messageBox" runat="server" />
    <div class="widget-content tab-content"> 
                    <fcp:CollapsiblePanel id="secRdsLocalAdmins" runat="server"
                        TargetControlID="panelRdsLocalAdmins" meta:resourcekey="secRdsLocalAdmins" Text="">
                    </fcp:CollapsiblePanel>		
                    
                    <asp:Panel runat="server" ID="panelRdsLocalAdmins">                                                
                        <div class="fcp-p-10">
                            <fcp:CollectionUsers id="users" runat="server" />
                        </div>                            
                    </asp:Panel>
				</div>
                    <div class="text-end">
                        <fcp:ItemButtonPanel id="buttonPanel" runat="server" ValidationGroup="SaveRDSCollection" 
                            OnSaveClick="btnSave_Click" OnSaveExitClick="btnSaveExit_Click" />
			        </div>
			</div>
            </div>
                    </div>
