<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SfBAddFederationDomain.ascx.cs" Inherits="FuseCP.Portal.SfBAddFederationDomain" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server" />

<div id="ExchangeContainer">
    <div class="Module">
        <div class="Left">
        </div>
        <div class="Content">
            <div class="Center">
                <div class="Title">
                    <asp:Image ID="Image1" SkinID="SfBLogo" runat="server" />
                    <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle"></asp:Localize>
                </div>
                <div class="card-body form-horizontal">
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    <table id="AddFederationDomain"   runat="server"> 					    
                        <tr>
                            <td class="FormLabel150">
                                <asp:Localize ID="locDomainName" runat="server" meta:resourcekey="locDomainName" Text="Domain Name: *"></asp:Localize>
                            </td>
                            <td>                                
                                <asp:TextBox ID="DomainName" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator id="DomainRequiredValidator" runat="server" meta:resourcekey="DomainRequiredValidator"
                                    ControlToValidate="DomainName" Display="Dynamic" ValidationGroup="Domain" SetFocusOnError="true"></asp:RequiredFieldValidator>
		                        <asp:RegularExpressionValidator id="DomainFormatValidator" runat="server" meta:resourcekey="DomainFormatValidator"
		                            ControlToValidate="DomainName" Display="Dynamic" ValidationGroup="Domain" SetFocusOnError="true"
		                            ValidationExpression="^([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.){1,10}[a-zA-Z]{2,15}$"></asp:RegularExpressionValidator>
                            </td>
					    </tr>
                        <tr>
                            <td class="FormLabel150">
                                <asp:Localize ID="locProxyFQDN" runat="server" meta:resourcekey="locProxyFQDN" Text="Proxy FQDN: "></asp:Localize>
                            </td>
                            <td>                                
                                <asp:TextBox ID="ProxyFQDN" runat="server" CssClass="form-control"></asp:TextBox>
		                        <asp:RegularExpressionValidator id="ProxyFqdnFormatValidator" runat="server" meta:resourcekey="ProxyFqdnFormatValidator"
		                            ControlToValidate="ProxyFQDN" Display="Dynamic" ValidationGroup="Domain" SetFocusOnError="true"
		                            ValidationExpression="^([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.){1,10}[a-zA-Z]{2,15}$"></asp:RegularExpressionValidator>
                            </td>
					    </tr>
                    </table>
                    <br />
                    <div class="FormButtonsBarClean">
                        <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
                        <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click" OnClientClick="ShowProgressDialog('Adding Domain...');" ValidationGroup="Domain"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddText"/> </asp:LinkButton>&nbsp;
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="Domain" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
