<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServersEditService.ascx.cs"
	Inherits="FuseCP.Portal.ServersEditService" %>
<%@ Register Src="GlobalDnsRecordsControl.ascx" TagName="GlobalDnsRecordsControl"
	TagPrefix="fcp" %>
<%@ Register Src="ServerHeaderControl.ascx" TagName="ServerHeaderControl" TagPrefix="fcp" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport"
	TagPrefix="fcp" %>
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/mail-confirmation.js"></script>
<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server" />
<div class="card-body form-horizontal">
	<fcp:ServerHeaderControl ID="ServerHeaderControl" runat="server" />
	<table class="table table-borderless align-middle mb-0 w-100">
		<tr>
			<td class="SubHead" >
				<asp:Label ID="lblGroup" runat="server" meta:resourcekey="lblGroup" Text="Group:"></asp:Label>
			</td>
			<td class="Normal">
				<asp:Literal ID="litGroup" runat="server"></asp:Literal>
			</td>
		</tr>
		<tr id="textProvider" runat="server">
			<td class="SubHead" height="24">
				<asp:Label ID="lblProvider" runat="server" meta:resourcekey="lblProvider" Text="Provider:"></asp:Label>
			</td>
			<td class="Normal">
				<asp:Literal ID="litProvider" runat="server"></asp:Literal>
			</td>
		</tr>
        <tr id="selectProvider" runat="server">
		    <td class="SubHead align-top">
			    <asp:Label ID="lblServiceProvider" runat="server" meta:resourcekey="lblServiceProvider" Text="Service provider:"></asp:Label></td>
		    <td class="NormalBold align-top">
			    <asp:DropDownList id="ddlProviders" CssClass="form-control" runat="server" DataTextField="DisplayName"
				    DataValueField="ProviderID"></asp:DropDownList>
			    </td>
	    </tr>
		<tr>
			<td class="SubHead">
				<asp:Label ID="lblName" runat="server" meta:resourcekey="lblName" Text="Name:"></asp:Label>
			</td>
			<td>
				<asp:TextBox ID="txtServiceName" runat="server" CssClass="form-control" Width="300px"
					MaxLength="50"></asp:TextBox><asp:RequiredFieldValidator ID="vldServiceNameRequired"
						runat="server" CssClass="NormalBold" ErrorMessage="Please enter service name"
						ControlToValidate="txtServiceName" Display="Dynamic"></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr>
			<td class="SubHead align-top">
				<asp:Label ID="lblComments" runat="server" meta:resourcekey="lblComments" Text="Comments:"></asp:Label>
			</td>
			<td class="Normal">
				<asp:TextBox ID="txtComments" runat="server" Width="300px" CssClass="form-control"
					TextMode="MultiLine" Rows="3"></asp:TextBox>
			</td>
		</tr>
		<tr id="rowInstallResults" runat="server">
			<td class="NormalError" colspan="2">
				<asp:Label ID="lblServiceInstallationResults" runat="server" meta:resourcekey="lblServiceInstallationResults"
					Text="Service installation results:"></asp:Label>
				<asp:BulletedList ID="blInstallResults" runat="server">
				</asp:BulletedList>
			</td>
		</tr>
	</table>

    <h3><asp:Localize ID="SettingsHeader" runat="server" meta:resourcekey="SettingsHeader" Text="Settings"></asp:Localize></h3>
    <asp:PlaceHolder ID="serviceProps" runat="server"></asp:PlaceHolder>

    <fcp:CollapsiblePanel id="DnsRecrodsHeader" runat="server" TargetControlID="DnsRecrodsPanel"
		resourcekey="DnsRecrodsHeader" Text="DNS Records Template">
	</fcp:CollapsiblePanel>
	<asp:Panel ID="DnsRecrodsPanel" runat="server">
					<fcp:GlobalDnsRecordsControl ID="GlobalDnsRecordsControl" runat="server" IPServerIDParam="ServerID" ServiceIdParam="ServiceID" />
	</asp:Panel>
	<asp:Panel ID="pnlQuota" runat="server" Visible="false">
		<table id="tblQuota" runat="server" visible="false">
			<tr>
				<td class="SubHead text-nowrap">
					<asp:Label ID="lblQuotaName" runat="server"></asp:Label>
				</td>
				<td class="Normal">
					<asp:TextBox ID="txtQuotaValue" runat="server" CssClass="form-control" Width="100px" Text="0"></asp:TextBox>
				</td>
			</tr>
		</table>
		<table id="tblCluster" runat="server" visible="false">
			<tr>
				<td class="SubHead text-nowrap">
					<asp:Label ID="lblClusters" runat="server" meta:resourcekey="lblClusters"></asp:Label>
				</td>
				<td class="Normal">
					<asp:DropDownList ID="ddlClusters" runat="server" Width="200px" CssClass="form-control"
						DataTextField="ClusterName" DataValueField="ClusterID">
					</asp:DropDownList>
					<asp:RequiredFieldValidator ID="valCluster" runat="server" ControlToValidate="ddlClusters"
						ErrorMessage="*" ValidationGroup="SelectCluster"></asp:RequiredFieldValidator>
					<asp:LinkButton ID="cmdDeleteCluster" runat="server" ValidationGroup="SelectCluster"
						OnClick="cmdDeleteCluster_Click">Delete Cluster</asp:LinkButton>
				</td>
			</tr>
			<tr>
				<td class="SubHead">
					<asp:Label ID="lblNewCluster" runat="server" meta:resourcekey="lblNewCluster"></asp:Label>
				</td>
				<td class="Normal">
					<asp:TextBox ID="txtClusterName" runat="server" CssClass="form-control" Width="200px"></asp:TextBox>
					<asp:RequiredFieldValidator ID="valNewCluster" runat="server" ControlToValidate="txtClusterName"
						ErrorMessage="*" ValidationGroup="NewCluster"></asp:RequiredFieldValidator>
					<asp:LinkButton ID="cmdAddCluster" runat="server" ValidationGroup="NewCluster" OnClick="cmdAddCluster_Click">Add Cluster</asp:LinkButton>
				</td>
			</tr>
		</table>
	</asp:Panel>
</div>
<div class="card-footer text-end">
	<asp:LinkButton id="btnDelete" CssClass="btn btn-danger" runat="server" CausesValidation="False" OnClick="btnDelete_Click" OnClientClick="return fuseCpConfirmWithProgress('Are you sure you want to delete this Service?', 'Deleting Service...');"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteText"/> </asp:LinkButton>&nbsp;
	<asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
	<asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click" OnClientClick="ShowProgressDialog('Updating Service...');"> <i class="bi bi-arrow-clockwise me-1"></i><asp:Localize runat="server" meta:resourcekey="btnUpdateText"/> </asp:LinkButton>
</div>

