<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VpsDetailsSnapshots.ascx.cs" Inherits="FuseCP.Portal.VPS2012.VpsDetailsSnapshots" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/ServerTabs.ascx" TagName="ServerTabs" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>
<%@ Register Src="UserControls/FormTitle.ascx" TagName="FormTitle" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

	    <div class="Content">
		    <div class="Center">
			    <div class="card-body form-horizontal">
			        <fcp:ServerTabs id="tabs" runat="server" SelectedTab="vps_snapshots" />	
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    
				    
				    <table >
				        <tr>
				            <td class="align-top">
				            
                                <div class="FormButtonsBarClean">
                                    <asp:LinkButton id="btnTakeSnapshot" CssClass="btn btn-success" runat="server" onclick="btnTakeSnapshot_Click" meta:resourcekey="btnTakeSnapshot"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnTakeSnapshotText"/> </asp:LinkButton>
                                </div>
                                <br />
                                
				                <asp:TreeView ID="SnapshotsTree" runat="server" 
                                    onselectednodechanged="SnapshotsTree_SelectedNodeChanged" ShowLines="True">
                                    <SelectedNodeStyle CssClass="SelectedTreeNode" />
                                    <Nodes>
                                    </Nodes>
                                    <NodeStyle CssClass="TreeNode" />
				                </asp:TreeView>
				                
				                <div id="NoSnapshotsPanel" runat="server" class="fcp-p-5">
				                    <asp:Localize ID="locNoSnapshots" runat="server" meta:resourcekey="locNoSnapshots" Text="No snapshots"></asp:Localize>
				                </div>
                                
                                <br />
				                <br />
				                <asp:Localize ID="locQuota" runat="server" meta:resourcekey="locQuota"
				                    Text="Number of Snapshots:"></asp:Localize>
				                &nbsp;&nbsp;&nbsp;
				                <fcp:QuotaViewer ID="snapshotsQuota" runat="server" QuotaTypeId="2" />
				    
				            </td>
				            <td class="align-top" id="SnapshotDetailsPanel" runat="server">
				                <asp:Image ID="imgThumbnail" runat="server" Height="120" />
				                <p>
				                    <asp:Localize ID="locCreated" runat="server" meta:resourcekey="locCreated"
				                        Text="Created:"></asp:Localize>
				                    <asp:Literal ID="litCreated" runat="server"></asp:Literal>
				                </p>
				                <ul class="ActionButtons">
				                    <li>
				                        <asp:LinkButton ID="btnApply" runat="server" CausesValidation="false" CssClass="ActionButtonApplySnapshot2012"
				                            meta:resourcekey="btnApply" Text="Apply" onclick="btnApply_Click"></asp:LinkButton>
				                    </li>
				                    <li>
				                        <asp:LinkButton ID="btnRename" runat="server" CausesValidation="false" CssClass="ActionButtonRename2012"
				                            meta:resourcekey="btnRename" Text="Rename"></asp:LinkButton>
				                    </li>
				                    <li>
				                        <asp:LinkButton ID="btnDelete" runat="server" CausesValidation="false" CssClass="ActionButtonDeleteSnapshot2012"
				                            meta:resourcekey="btnDelete" Text="Delete" onclick="btnDelete_Click"></asp:LinkButton>
				                    </li>
				                    <li>
				                        <asp:LinkButton ID="btnDeleteSubtree" runat="server" CausesValidation="false" CssClass="ActionButtonDeleteSnapshotTree2012"
				                            meta:resourcekey="btnDeleteSubtree" Text="Delete subtree" 
                                            onclick="btnDeleteSubtree_Click"></asp:LinkButton>
				                    </li>
				                </ul>
				            </td>
				        </tr>
				    </table>
				    
			    </div>
		    </div>
	    </div>

<asp:Panel ID="RenamePanel" runat="server" style="display:none">
	 <div class="widget">
             <div class="widget-header clearfix">
                           <h3><i class="bi bi-i-cursor"></i>  <asp:Localize ID="locRenameSnapshot" runat="server" Text="Rename snapshot" meta:resourcekey="locRenameSnapshot"></asp:Localize></h3>
			</div>
                    <div class="widget-content Popup">
			<table class="table table-borderless align-middle mb-0">
			    <tr>
			        <td>
			            <asp:TextBox ID="txtSnapshotName" runat="server" CssClass="form-control"></asp:TextBox>
			            
			            <asp:RequiredFieldValidator ID="SnapshotNameValidator" runat="server" Text="*" Display="Dynamic"
                                ControlToValidate="txtSnapshotName" meta:resourcekey="SnapshotNameValidator" SetFocusOnError="true"
                                ValidationGroup="RenameSnapshot">*</asp:RequiredFieldValidator>
			        </td>
			    </tr>
			</table>                       
			</div>
					<div class="popup-buttons text-end">
		    <asp:LinkButton id="btnCancelRename" CssClass="btn btn-warning" runat="server" CausesValidation="False"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancelRenameText"/> </asp:LinkButton>&nbsp;
            <asp:LinkButton id="btnRenameSnapshot" CssClass="btn btn-success" runat="server" OnClick="btnRenameSnapshot_Click" ValidationGroup="RenameSnapshot"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnRenameSnapshotText"/> </asp:LinkButton>    
		</div>
	</div>
</asp:Panel>

<ajaxToolkit:ModalPopupExtender ID="RenameSnapshotModal" runat="server" BehaviorID="RenameSnapshotModal"
	TargetControlID="btnRename" PopupControlID="RenamePanel"
	BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelRename" />
