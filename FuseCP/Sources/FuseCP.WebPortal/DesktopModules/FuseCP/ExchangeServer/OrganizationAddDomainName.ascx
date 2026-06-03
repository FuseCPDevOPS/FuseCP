<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrganizationAddDomainName.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.OrganizationAddDomainName" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>


				<div class="card-header">
                    <h3 class="card-title">
					<asp:Image ID="Image1" SkinID="ExchangeDomainNameAdd48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Add Domain"></asp:Localize>
				</h3>
                        </div>
				<div class="card-body form-horizontal">
				    <fcp:SimpleMessageBox id="messageBox" runat="server" />
					<table>
						<tr>
							<td class="FormLabel150"><asp:Localize ID="locDomainName" runat="server" meta:resourcekey="locDomainName" Text="Domain Name:"></asp:Localize></td>
							<td>
                            <asp:DropDownList id="ddlDomains" runat="server" CssClass="form-control align-middle" DataTextField="DomainName" DataValueField="DomainID" ></asp:DropDownList>
							</td>
						</tr>
					</table>
					<br />

				</div>
				    <div class="card-footer text-end">
                        <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
                        <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click" OnClientClick="ShowProgressDialog('Creating Domain...');" ValidationGroup="CreateDomain"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddText"/> </asp:LinkButton>&nbsp;
					    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="CreateDomain" />
				    </div>
