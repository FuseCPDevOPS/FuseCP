<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddRDSServer.ascx.cs" Inherits="FuseCP.Portal.RDS.AddRDSServer" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<script type="text/javascript" src="/JavaScript/jquery-1.4.4.min.js"></script>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

<div class="card-header">
					<asp:Image ID="imgAddRDSServer" SkinID="EnterpriseRDSServers48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Add Server To Organization"></asp:Localize>
				</div>
				<div class="card-body form-horizontal">
				    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                  
					<table>
					    <tr>
						    <td class="FormLabel150"><asp:Localize ID="locServer" runat="server" meta:resourcekey="locServer" Text="Server:"></asp:Localize></td>
						    <td>
							    <asp:DropDownList ID="ddlServers" runat="server" CssClass="form-control align-middle" Width="150px"  />
						    </td>
					    </tr>
					</table> 
                      </div>
				    <div class="card-footer text-end">
					    <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" ValidationGroup="AddRDSServer" OnClick="btnAdd_Click" OnClientClick="ShowProgressDialog('Adding server...');"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddText"/> </asp:LinkButton>
					    <asp:ValidationSummary ID="valSummary" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="AddRDSServer" />
				    </div>
