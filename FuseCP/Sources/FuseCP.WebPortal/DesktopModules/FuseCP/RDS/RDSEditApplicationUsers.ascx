<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RDSEditApplicationUsers.ascx.cs" Inherits="FuseCP.Portal.RDS.RDSEditApplicationUsers" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register Src="UserControls/RDSCollectionUsers.ascx" TagName="CollectionUsers" TagPrefix="fcp"%>
<%@ Register Src="UserControls/RDSCollectionTabs.ascx" TagName="CollectionTabs" TagPrefix="fcp" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="../UserControls/ItemButtonPanel.ascx" TagName="ItemButtonPanel" TagPrefix="fcp" %>
<script type="text/javascript" src="/JavaScript/jquery-1.4.4.min.js"></script>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

<div class="card-header">
					<asp:Image ID="imgEditRDSCollection" SkinID="EnterpriseRDSCollections48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Edit RDS Collection"></asp:Localize>
                    -
					<asp:Literal ID="litCollectionName" runat="server" Text="" />
				</div>
                <div class="card-body form-horizontal">
				<fcp:CollectionTabs id="tabs" runat="server" SelectedTab="rds_collection_edit_apps" />
                <div class="card tab-content">
                <div class="card-body form-horizontal">
				    <fcp:SimpleMessageBox id="SimpleMessageBox1" runat="server" />
                    <fcp:CollapsiblePanel id="secRdsApplicationEdit" runat="server"
                        TargetControlID="panelRdsApplicationEdit" meta:resourcekey="secRdsApplicationEdit" Text="">
                    </fcp:CollapsiblePanel>		
                    
                    <asp:Panel runat="server" ID="panelRdsApplicationEdit">                                                
                        <div class="fcp-p-10">
                            <table>
                                <tr>
                                    <td class="FormLabel150 text-nowrap" colspan="2" >
                                        <asp:Localize ID="locLblApplicationName" runat="server" meta:resourcekey="locLblApplicationName" Text="Application Name"/>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtApplicationName" runat="server" CssClass="TextBox300" />
                                        <asp:RequiredFieldValidator ID="valApplicationName" runat="server" ErrorMessage="*" ControlToValidate="txtApplicationName" ValidationGroup="SaveRDSCollection"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <asp:RadioButton ID="chNotAllow" GroupName="commandLineParameters" meta:resourcekey="chNotAllow" runat="server" Text=""/>                                        
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <asp:RadioButton ID="chAllowAny" GroupName="commandLineParameters" meta:resourcekey="chAllowAny" runat="server" Text=""/>                                        
                                    </td>
                                </tr>
                                <tr>
                                    <td  />
                                    <td colspan="2">
                                        <asp:Localize ID="locAllowAny" runat="server" meta:resourcekey="locAllowAny" Text=""/>                                        
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="3">
                                        <asp:RadioButton ID="chAllow" meta:resourcekey="chAllow" GroupName="commandLineParameters" runat="server" Text=""/>                                        
                                    </td>
                                </tr>
                                <tr>
                                    <td class="FormLabel150 text-nowrap" colspan="2" >
                                        <asp:Localize ID="locCommandLine" runat="server" meta:resourcekey="locCommandLine" Text="Command-line parameters"/>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCommandLine" runat="server" CssClass="TextBox300" Enabled="False" />                                        
                                    </td>
                                </tr>
                            </table>
                        </div>                            
                    </asp:Panel>
                    					
                    <fcp:CollapsiblePanel id="secRdsApplicationUsers" runat="server"
                        TargetControlID="panelRdsApplicationUsers" meta:resourcekey="secRdsApplicationUsers" Text="">
                    </fcp:CollapsiblePanel>		
                    
                    <asp:Panel runat="server" ID="panelRdsApplicationUsers">                                                
                        <div class="fcp-p-10">
                            <fcp:CollectionUsers id="users" runat="server" />
                        </div>                            
                    </asp:Panel>
                    <div class="text-end">
                        <asp:LinkButton id="btnExit" CssClass="btn btn-warning" runat="server" OnClick="btnExit_Click" OnClientClick="ShowProgressDialog('Loading ...');"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnExitText"/> </asp:LinkButton>&nbsp;
                        <asp:LinkButton id="btnSaveExit" CssClass="btn btn-success" runat="server" OnClick="btnSaveExit_Click" OnClientClick="ShowProgressDialog('Updating ...');"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveExitText"/> </asp:LinkButton>&nbsp;
                        <asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" OnClientClick="ShowProgressDialog('Updating ...');"> <i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveText"/> </asp:LinkButton>
			        </div>
				</div>	
			</div>
		</div>
