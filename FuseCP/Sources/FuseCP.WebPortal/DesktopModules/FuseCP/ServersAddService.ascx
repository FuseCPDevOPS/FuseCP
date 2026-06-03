<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServersAddService.ascx.cs" Inherits="FuseCP.Portal.ServersAddService" %>
<%@ Register Src="ServerHeaderControl.ascx" TagName="ServerHeaderControl" TagPrefix="uc1" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

<div class="card-body form-horizontal">
    <uc1:ServerHeaderControl id="ServerHeaderControl" runat="server">
    </uc1:ServerHeaderControl>
    <br />
    <table class="table table-borderless align-middle mb-0 w-100">
	    <tr>
		    <td class="SubHead text-nowrap"><asp:Label ID="lblServiceGroupName" runat="server" meta:resourcekey="lblServiceGroupName" Text="Service group name:"></asp:Label></td>
		    <td class="NormalBold">
			    <asp:Literal ID="litGroupName" Runat="server"></asp:Literal>
		    </td>
	    </tr>
	    <tr>
		    <td class="SubHead" style="height: 34px"><asp:Label ID="lblServiceName" runat="server" meta:resourcekey="lblServiceName" Text="Service name:"></asp:Label></td>
		    <td class="NormalBold" style="height: 34px">
			    <asp:TextBox id="serviceName" runat="server" CssClass="form-control" style="height: 30px"></asp:TextBox>
			    <asp:RequiredFieldValidator id="serviceNameValidator" meta:resourcekey="serviceNameValidator" runat="server" ErrorMessage="Please specify service name" ControlToValidate="serviceName"
				    Display="Dynamic"></asp:RequiredFieldValidator>
		    </td>
	    </tr>
	    <tr>
		    <td class="SubHead align-top">
			    <asp:Label ID="lblServiceProvider" runat="server" meta:resourcekey="lblServiceProvider" Text="Service provider:"></asp:Label></td>
		    <td class="NormalBold align-top">
			    <asp:DropDownList id="ddlProviders" CssClass="form-control" runat="server" DataTextField="DisplayName"
				    DataValueField="ProviderID"></asp:DropDownList>
			    <asp:RequiredFieldValidator id="serviceValidator" meta:resourcekey="serviceValidator" runat="server" Display="Dynamic" ControlToValidate="ddlProviders"
				    ErrorMessage="Please select service provider"></asp:RequiredFieldValidator></td>
	    </tr>
    </table>
</div>
<div class="card-footer text-end">    
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click" OnClientClick="ShowProgressDialog('Adding Service...');"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddText"/> </asp:LinkButton>
</div>

