<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VpsDetailsTools.ascx.cs" Inherits="FuseCP.Portal.VPSForPC.VpsDetailsTools" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/ServerTabs.ascx" TagName="ServerTabs" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>
<%@ Register Src="UserControls/FormTitle.ascx" TagName="FormTitle" TagPrefix="fcp" %>


	    <div class="card">
			    <div class="card-header">
				    <asp:Image ID="imgIcon" SkinID="Server48" runat="server" />
				    <fcp:FormTitle ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Tools" />
			    </div>
			    <div class="card-body form-horizontal">
                    <fcp:Menu id="menu" runat="server" SelectedItem="" />
                <div class="card tab-content">
                <div class="card-body form-horizontal">
			        <fcp:ServerTabs id="tabs" runat="server" SelectedTab="vps_tools" />	
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    
				    <table class="table table-borderless align-middle mb-0">
				        <%-- <tr>
				            <td>
				                <asp:LinkButton id="btnReinstall" CssClass="btn btn-success" runat="server" CausesValidation="false" onclick="btnReinstall_Click"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnReinstallText"/> </asp:LinkButton>
				            </td>
				            <td>
				                <asp:Localize ID="locReinstall" runat="server" meta:resourcekey="locReinstall" Text="Performs..."></asp:Localize>
				            </td>
				        </tr>--%>
				        <tr>
				            <td>
				                <asp:LinkButton id="btnDelete" CssClass="btn btn-danger" runat="server" CausesValidation="false" onclick="btnDelete_Click"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteText"/> </asp:LinkButton>
				            </td>
				            <td>
				                <asp:Localize ID="locDelete" runat="server" meta:resourcekey="locDelete" Text="Performs..."></asp:Localize>
				            </td>
				        </tr>
				    </table>
			    </div>
		    </div>
            </div>
            </div>
		    <div class="Right">
			    <asp:Localize ID="FormComments" runat="server" meta:resourcekey="FormComments"></asp:Localize>
		    </div>
