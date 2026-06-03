<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RDSCreateCollection.ascx.cs" Inherits="FuseCP.Portal.RDS.RDSCreateCollection" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register Src="UserControls/RDSCollectionServers.ascx" TagName="CollectionServers" TagPrefix="fcp"%>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>
<script type="text/javascript" src="/JavaScript/jquery-1.4.4.min.js"></script>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>


				<div class="card-header">
					<asp:Image ID="imgAddRDSServer" SkinID="EnterpriseRDSCollections48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Create New RDS Collection"></asp:Localize>
				</div>
				<div class="card-body form-horizontal">
				    <fcp:SimpleMessageBox id="messageBox" runat="server" />

					<table>
					    <tr>
						    <td class="FormLabel150" ><asp:Localize ID="locCollectionName" runat="server" meta:resourcekey="locCollectionName" Text="Collection Name"></asp:Localize></td>
						    <td>
                                <asp:TextBox ID="txtCollectionName" runat="server" CssClass="TextBox300" />
                                <asp:RequiredFieldValidator ID="valCollectionName" runat="server" ErrorMessage="*" ControlToValidate="txtCollectionName" ValidationGroup="SaveRDSCollection"></asp:RequiredFieldValidator>
						    </td>                            
					    </tr>                        
					</table>                                                                              

                    <fieldset id="RDSServersPanel" runat="server">
                        <legend><asp:Localize ID="locRDSServersSection" runat="server" meta:resourcekey="locRDSServersSection" Text="RDS Servers"></asp:Localize></legend>
                        <div class="fcp-p-10">
                            <fcp:CollectionServers id="servers" runat="server" />
                        </div>  
                    </fieldset>
                      

				</div>
				    <div class="card-footer text-end">
					    <asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" OnClientClick="ShowProgressDialog('Adding collection...');" ValidationGroup="SaveRDSCollection"> <i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveText"/> </asp:LinkButton>
				    </div>
