<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Apache_Settings.ascx.cs" Inherits="FuseCP.Portal.ProviderControls.Apache_Settings" %>
<%@ Register Src="../UserControls/SelectIPAddress.ascx" TagName="SelectIPAddress" TagPrefix="uc1" %>
<table class="table table-borderless align-middle mb-0 w-100">
		<tr>
			<td class="SubHead text-nowrap" >
			    <asp:Label ID="lblSharedIP" runat="server" meta:resourcekey="lblSharedIP" Text="Web Sites Shared IP Address:"></asp:Label>
			</td>
			<td><uc1:SelectIPAddress ID="ipAddress" runat="server" ServerIdParam="ServerID" /></td>
		</tr>
		<tr>
			<td class="SubHead text-nowrap" >
			    <asp:Label ID="lblPublicSharedIP" runat="server" meta:resourcekey="lblPublicSharedIP" Text="Web Sites Public Shared IP Address:"></asp:Label>
			</td>
			<td><asp:TextBox ID="txtPublicSharedIP" runat="server" CssClass="form-control"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="SubHead text-nowrap" >
			    <asp:Label ID="lblConfigPath" runat="server" meta:resourcekey="lblConfigPath" Text="Apache Configuration Path:"></asp:Label>
			</td>
			<td><asp:TextBox Runat="server" ID="txtConfigPath" Width="300px" CssClass="form-control"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="SubHead text-nowrap" >
			    <asp:Label ID="lblConfigFile" runat="server" meta:resourcekey="lblConfigFile" Text="Apache Configuration File:"></asp:Label>
			</td>
			<td><asp:TextBox Runat="server" ID="txtConfigFile" Width="300px" CssClass="form-control"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="SubHead text-nowrap" >
			    <asp:Label ID="lblBinPath" runat="server" meta:resourcekey="lblBinPath" Text="Apache Bin Path:"></asp:Label>
			</td>
			<td><asp:TextBox Runat="server" ID="txtBinPath" Width="300px" CssClass="form-control"></asp:TextBox></td>
		</tr>
</table>

