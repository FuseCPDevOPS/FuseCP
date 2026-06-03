<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VpsDetailsDvd.ascx.cs" Inherits="FuseCP.Portal.VPSForPC.VpsDetailsDvd" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/ServerTabs.ascx" TagName="ServerTabs" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>
<%@ Register Src="UserControls/FormTitle.ascx" TagName="FormTitle" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
	    <div class="card">
			    <div class="card-header">
				    <asp:Image ID="imgIcon" SkinID="DvdDrive48" runat="server" />
				    <fcp:FormTitle ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="DVD" />
			    </div>
			    <div class="card-body form-horizontal">
                    <fcp:Menu id="menu" runat="server" SelectedItem="" />
                <div class="card tab-content">
                <div class="card-body form-horizontal">
			        <fcp:ServerTabs id="tabs" runat="server" SelectedTab="vps_dvd" />
	
                        <fcp:SimpleMessageBox id="messageBox" runat="server" />
                        
			            <table class="fcp-m-dvd-table">
			                <tr>
			                    <td><asp:Localize ID="locDvdDrive" runat="server" meta:resourcekey="locDvdDrive" Text="DVD Drive:"></asp:Localize></td>
			                </tr>
			                <tr>
			                    <td>
			                        <asp:TextBox ID="txtInsertedDisk" runat="server" Width="400px"
			                            CssClass="form-control" ReadOnly="true"></asp:TextBox>
			                    </td>
			                </tr>
			                <tr>
			                    <td>
			                        <br />
			                        <br />
			                        <asp:LinkButton id="btnEjectDisk" CssClass="btn btn-warning" runat="server" OnClick="btnEjectDisk_Click" CausesValidation="false"> <i class="bi bi-stop-circle me-1"></i><asp:Localize runat="server" meta:resourcekey="btnEjectDiskText"/> </asp:LinkButton>&nbsp;
                                    <asp:LinkButton id="btnInsertDisk" CssClass="btn btn-success" runat="server" OnClick="btnInsertDisk_Click" CausesValidation="false"> <i class="bi bi-play-circle me-1"></i><asp:Localize runat="server" meta:resourcekey="btnInsertDiskText"/> </asp:LinkButton>
			                    </td>
			                </tr>
			            </table>
			    </div>
		    </div>
            </div>
            </div>
		    <div class="Right">
			    <asp:Localize ID="FormComments" runat="server" meta:resourcekey="FormComments"></asp:Localize>
		    </div>
