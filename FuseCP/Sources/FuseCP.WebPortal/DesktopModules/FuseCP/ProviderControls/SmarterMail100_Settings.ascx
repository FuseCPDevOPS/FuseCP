<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SmarterMail100_Settings.ascx.cs" Inherits="FuseCP.Portal.ProviderControls.SmarterMail100_Settings" %>
<%@ Register Src="../UserControls/SelectIPAddress.ascx" TagName="SelectIPAddress" TagPrefix="uc1" %>
<%@ Import Namespace="FuseCP.Portal" %>
<table class="table table-borderless align-middle mb-0 w-100">
	<tr>
	 <td class="SubHead text-nowrap">
		    <asp:Label ID="lblServiceUrl" runat="server" meta:resourcekey="lblServiceUrl" Text="Web Services URL:"></asp:Label>
		</td>
		<td><asp:TextBox Runat="server" ID="txtServiceUrl" CssClass="form-control" Width="200px"></asp:TextBox></td>
	</tr>
	<tr>
		<td class="SubHead">
		    <asp:Label ID="lblPublicIP" runat="server" meta:resourcekey="lblPublicIP" Text="Public IP Address:"></asp:Label>
		</td>
		<td>
            <uc1:SelectIPAddress ID="ipAddress" runat="server" ServerIdParam="ServerID" UseAddressValueAsKey="true" />
        </td>
	</tr>
	<tr>
		<td class="SubHead">
		    <asp:Label ID="lblDomainsPath" runat="server" meta:resourcekey="lblDomainsPath" Text="Domains Root Folder:"></asp:Label>
		</td>
		<td>
		    <asp:TextBox Runat="server" ID="txtDomainsFolder" CssClass="form-control" Width="200px"></asp:TextBox>
		</td>
	</tr>
	<tr>
		<td class="SubHead">
		    <asp:Label ID="lblAdminLogin" runat="server" meta:resourcekey="lblAdminLogin" Text="Admin Login:"></asp:Label>
		</td>
		<td>
		    <asp:TextBox Runat="server" ID="txtUsername" CssClass="form-control" Width="200px"></asp:TextBox>
		</td>
	</tr>
	<tr id="rowPassword" runat="server">
		<td class="SubHead">
		    <asp:Label ID="lblCurrPassword" runat="server" meta:resourcekey="lblCurrPassword" Text="Current Admin Password:"></asp:Label>
		</td>
		<td class="Normal">*******
		</td>
	</tr>
	<tr>
		<td class="SubHead">
		    <asp:Label ID="lblAdminPassword" runat="server" meta:resourcekey="lblAdminPassword" Text="Admin Password:"></asp:Label>
		</td>
		<td>
		    <asp:TextBox Runat="server" ID="txtPassword" CssClass="form-control" Width="200px" TextMode="Password"></asp:TextBox>
	    </td>	    
	</tr> 
	<tr>
	    <td></td>
	    <td><asp:CheckBox runat="server" ID="cbImportDomainAdmin" meta:resourcekey="cbImportDomainAdmin"/></td>
	</tr>
	<tr>
	    <td></td>
	    <td><asp:CheckBox runat="server" ID="cbInheritDefaultLimits" meta:resourcekey="cbInheritDefaultLimits"/></td>
	</tr>
	<tr>
	    <td></td>
	    <td><asp:CheckBox runat="server" ID="cbEnableDomainAdmin" meta:resourcekey="cbEnableDomainAdmin"/></td>
	</tr>
		
</table>

<div id="SeDiv" runat="server">
<fieldset>
    <legend>
        <asp:Label ID="lblRouteFromSE" runat="server" meta:resourcekey="lblRouteFromSE"
            Text="Automatic Mail Filter" CssClass="NormalBold"></asp:Label>
    </legend>
    <asp:CheckBox ID="chkSEEnable" runat="server" meta:resourcekey="chkEnableSE" Text="Enable Automatic Mail Filtering (SpamExperts) for domains managed by this provider" />
    <asp:UpdatePanel ID="GeneralUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <asp:GridView id="gvSEDestinations" runat="server"  EnableViewState="true" AutoGenerateColumns="false"
	        EmptyDataText="" CssSelectorClass="NormalGridView" OnRowCommand="gvSEDestinations_RowCommand" >
	        <Columns>
		        <asp:TemplateField HeaderText="Destinations">
			        <ItemStyle></ItemStyle>
			        <ItemTemplate>
				        <asp:Label id="lblSEDestination" runat="server" EnableViewState="true" ><%# PortalAntiXSS.Encode((string)Container.DataItem)%></asp:Label>
                    </ItemTemplate>
		        </asp:TemplateField>
                <asp:TemplateField>
                    <ItemStyle Width="65px" HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:LinkButton id="imgDelRouteFromSE" CssClass="btn btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# (string)Container.DataItem %>' OnClientClick="return confirm('Are you sure you want to delete selected route?')"> 
                            <i class="bi bi-trash"></i> 
                        </asp:LinkButton>
                    </ItemTemplate>
		        </asp:TemplateField>
	        </Columns>
	        </asp:GridView>
            <br />
            <div class="input-group col-sm-6">
             <asp:TextBox ID="tbSEDestinations" CssClass="form-control" runat="server"></asp:TextBox>
                <span class="d-flex">
                <asp:LinkButton ID="bntAddSEDestination" runat="server" CssClass="btn btn-primary" OnClick="bntAddSEDestination_Click" CausesValidation="False">
                <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="bntAddSEDestination" />
                </asp:LinkButton>
                </span>
            </div>
            <br />
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
    <br />
</fieldset>
</div>
