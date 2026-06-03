<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VpsDetailsEditConfiguration.ascx.cs" Inherits="FuseCP.Portal.VPSForPC.VpsDetailsEditConfiguration" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/ServerTabs.ascx" TagName="ServerTabs" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>
<%@ Register Src="UserControls/FormTitle.ascx" TagName="FormTitle" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/CollapsiblePanel.ascx" TagName="CollapsiblePanel" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
	    <div class="card">
			    <div class="card-header">
				    <asp:Image ID="imgIcon" SkinID="ServerConfig48" runat="server" />
				    <fcp:FormTitle ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Edit Configuration" />
			    </div>
			    <div class="card-body form-horizontal">
                    <fcp:Menu id="menu" runat="server" SelectedItem="" />
                <div class="card tab-content">
                <div class="card-body form-horizontal">
			        <fcp:ServerTabs id="tabs" runat="server" SelectedTab="vps_config" />	
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    
		            <p class="SubTitle">
		                <asp:Localize ID="locSubTitle" runat="server" meta:resourcekey="locSubTitle" Text="Edit Configuration" />
		            </p>

                    <asp:ValidationSummary ID="validatorsSummary" runat="server" 
                        ValidationGroup="VpsEdit" ShowMessageBox="True" ShowSummary="False" />

                    <fcp:CollapsiblePanel id="secResources" runat="server"
                        TargetControlID="ResourcesPanel" meta:resourcekey="secResources" Text="Resources">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="ResourcesPanel" runat="server" Height="0" CssClass="fcp-p-10" Style="overflow:hidden;">
                        <table class="table table-borderless align-middle mb-0">
                            <tr>
                                <td ><asp:Label ID="lblCpu" runat="server" AssociatedControlID="ddlCpu"
                                        meta:resourcekey="lblCpu" Text="CPU:" CssClass="Medium" /></td>
                                <td>
                                    <asp:DropDownList ID="ddlCpu" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                </td>
                                <td><asp:Localize ID="locCores" runat="server" meta:resourcekey="locCores" Text="cores"/></td>
                            </tr>
                        </table>
                        <table class="table table-borderless align-middle mb-0">
                            <tr>
                                <td ><asp:Label ID="lblRam" runat="server" AssociatedControlID="txtRam"
                                        meta:resourcekey="lblRam" Text="RAM:" CssClass="Medium" /></td>
                                <td>
                                    <asp:TextBox ID="txtRam" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequireRamValidator" runat="server" Text="*" Display="Dynamic"
                                        ControlToValidate="txtRam" meta:resourcekey="RequireRamValidator" SetFocusOnError="true"
                                        ValidationGroup="VpsEdit">*</asp:RequiredFieldValidator>
                                </td>
                                <td><asp:Localize ID="locMB" runat="server" meta:resourcekey="locMB" Text="MB"/></td>
                            </tr>
                        </table>
                        <table class="table table-borderless align-middle mb-0">
                            <tr>
                                <td ><asp:Label ID="lblHdd" runat="server" AssociatedControlID="txtHdd"
                                        meta:resourcekey="lblHdd" Text="HDD:" CssClass="Medium" /></td>
                                <td>
                                    <asp:TextBox ID="txtHdd" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequireHddValidator" runat="server" Text="*" Display="Dynamic"
                                        ControlToValidate="txtHdd" meta:resourcekey="RequireHddValidator" SetFocusOnError="true"
                                        ValidationGroup="VpsEdit">*</asp:RequiredFieldValidator>
                                </td>
                                <td><asp:Localize ID="locGB" runat="server" meta:resourcekey="locGB" Text="GB"/></td>
                            </tr>
                        </table>
                    </asp:Panel>
                    
                    <fcp:CollapsiblePanel id="secSnapshots" runat="server"
                        TargetControlID="SnapshotsPanel" meta:resourcekey="secSnapshots" Text="Snapshots">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="SnapshotsPanel" runat="server" Height="0" CssClass="fcp-p-5" Style="overflow:hidden;">
                        <table class="table table-borderless align-middle mb-0">
                            <tr>
                                <td class="FormLabel150"><asp:Localize ID="locSnapshots" runat="server"
                                    meta:resourcekey="locSnapshots" Text="Number of snapshots:"></asp:Localize></td>
                                <td>
                                    <asp:TextBox ID="txtSnapshots" runat="server" CssClass="form-control"></asp:TextBox>
                                    
                                    <asp:RequiredFieldValidator ID="SnapshotsValidator" runat="server" Text="*" Display="Dynamic"
                                        ControlToValidate="txtSnapshots" meta:resourcekey="SnapshotsValidator" SetFocusOnError="true"
                                        ValidationGroup="VpsEdit">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    
                    <fcp:CollapsiblePanel id="secDvd" runat="server"
                        TargetControlID="DvdPanel" meta:resourcekey="secDvd" Text="DVD">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="DvdPanel" runat="server" Height="0" CssClass="fcp-p-5" Style="overflow:hidden;">
                        <table class="table table-borderless align-middle mb-0">
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkDvdInstalled" runat="server" Checked="true"
                                        Text="DVD drive installed" meta:resourcekey="chkDvdInstalled" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    
                    <fcp:CollapsiblePanel id="secBios" runat="server"
                        TargetControlID="BiosPanel" meta:resourcekey="secBios" Text="BIOS">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="BiosPanel" runat="server" Height="0" CssClass="fcp-p-5" Style="overflow:hidden;">
                        <table class="table table-borderless align-middle mb-0">
                            <tr>
                                <td >
                                    <asp:CheckBox ID="chkBootFromCd" runat="server" Text="Boot from CD" meta:resourcekey="chkBootFromCd" />
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkNumLock" runat="server" Text="Num Lock enabled" meta:resourcekey="chkNumLock" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    
                    <fcp:CollapsiblePanel id="secActions" runat="server"
                        TargetControlID="ActionsPanel" meta:resourcekey="secActions" Text="Allowed actions">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="ActionsPanel" runat="server" Height="0" CssClass="fcp-p-5" Style="overflow:hidden;">
                        <table >
                            <tr>
                                <td >
                                    <asp:CheckBox ID="chkStartShutdown" runat="server" Text="Start, Turn off and Shutdown" meta:resourcekey="chkStartShutdown" />
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkReset" runat="server" Text="Reset" meta:resourcekey="chkReset" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkPauseResume" runat="server" Text="Pause, Resume" meta:resourcekey="chkPauseResume" />
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkReinstall" runat="server" Text="Re-install" meta:resourcekey="chkReinstall" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkReboot" runat="server" Text="Reboot" meta:resourcekey="chkReboot" />
                                </td>
                                <td>
                                    
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    
                    <fcp:CollapsiblePanel id="secNetwork" runat="server"
                        TargetControlID="NetworkPanel" meta:resourcekey="secNetwork" Text="Network">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="NetworkPanel" runat="server" Height="0" CssClass="fcp-p-5" Style="overflow:hidden;">
                        <table class="table table-borderless align-middle mb-0">
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkExternalNetworkEnabled" runat="server"
                                             meta:resourcekey="chkExternalNetworkEnabled" Text="External network enabled" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkPrivateNetworkEnabled" runat="server"
                                                    meta:resourcekey="chkPrivateNetworkEnabled" Text="Private network enabled" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    
                    <p>
                        <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click" meta:resourcekey="btnCancel"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancelText"/> </asp:LinkButton>&nbsp;
                        <asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click"  ValidationGroup="VpsEdit" meta:resourcekey="btnUpdate"> <i class="bi bi-arrow-clockwise me-1"></i><asp:Localize runat="server" meta:resourcekey="btnUpdateText"/> </asp:LinkButton>
                    </p>       

			    </div>
		    </div>
            </div>
            </div>
		    <div class="Right">
			    <asp:Localize ID="FormComments" runat="server" meta:resourcekey="FormComments"></asp:Localize>
		    </div>

