<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangePublicFolders.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangePublicFolders" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

				<div class="card-header">
                    <h3 class="card-title">
					<asp:Image ID="Image1" SkinID="ExchangePublicFolder48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Public Folders"></asp:Localize>
                    </h3>
				</div>
			     <div class="FormButtonsBar right">
                        <asp:LinkButton id="btnCreatePublicFolder" CssClass="btn btn-primary" runat="server" OnClick="btnCreatePublicFolder_Click"> <i class="bi bi-folder me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCreatePublicFolderText"/> </asp:LinkButton>
                    </div>	
				<div class="card-body form-horizontal">
				    <fcp:SimpleMessageBox id="messageBox" runat="server" />
				    <asp:TreeView ID="FoldersTree" runat="server">
				    </asp:TreeView>			    
				</div>
                <div class="card-footer">
                    <div class="row">
                        <div class="col-md-6">
				    <asp:Localize ID="locQuota" runat="server" meta:resourcekey="locQuota" Text="Total Public Folders Created:"></asp:Localize>
				    &nbsp;&nbsp;&nbsp;
				    <fcp:QuotaViewer ID="foldersQuota" runat="server" QuotaTypeId="2" />
                            </div>
                        <div class="col-md-6 text-end">
                                <asp:LinkButton id="btnDeleteFolders" CssClass="btn btn-danger" runat="server" OnClick="btnDeleteFolders_Click"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteFoldersText"/> </asp:LinkButton>
                            </div>
                        </div>
				</div>
