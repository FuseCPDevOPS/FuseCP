<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VpsCheckPoints.ascx.cs" Inherits="FuseCP.Portal.VPSForPC.VpsCheckPoints" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/ServerTabs.ascx" TagName="ServerTabs" TagPrefix="fcp" %>
<%@ Register Src="UserControls/FormTitle.ascx" TagName="FormTitle" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>

	    <div class="card">
			    <div class="card-header">
				    <asp:Image ID="imgIcon" SkinID="Monitoring48" runat="server" />
                    <fcp:FormTitle ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Snapshots" />
			    </div>
			    <div class="card-body form-horizontal">
                    <fcp:Menu id="menu" runat="server" SelectedItem="" />
                <div class="card tab-content">
                <div class="card-body form-horizontal">
                    <fcp:ServerTabs id="tabs" runat="server" SelectedTab="vps_checkpoints" />	
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />

                    <asp:TreeView runat="server" ID="treeCheckPoints"></asp:TreeView>
                <div class="FormButtonsBar" >
                    <asp:LinkButton id="btnRestoreCheckPoint" CssClass="btn btn-warning" runat="server" OnClick="btnRestoreCheckPoint_Click" CausesValidation="False"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnRestoreText"/> </asp:LinkButton>&nbsp;        
                    <asp:LinkButton id="btnCreateCheckPoint" CssClass="btn btn-success" runat="server" OnClick="btnCreateCheckPoint_Click" CausesValidation="False"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCreateText"/> </asp:LinkButton>
                </div> 
            </div>
                    </div>
                    </div>
            </div>
	        <div class="Right">
		        <asp:Localize ID="FormComments" runat="server" meta:resourcekey="FormComments"></asp:Localize>
	        </div>
