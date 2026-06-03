<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RDSServersEditServer.ascx.cs" Inherits="FuseCP.Portal.RDSServersEditServer" %>
<%@ Register Src="UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>
<script type="text/javascript" src="/JavaScript/jquery-1.4.4.min.js"></script>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

<div class="card-body form-horizontal">
	<fcp:SimpleMessageBox id="messageBox" runat="server" />
    <div class="row mb-3">
        <asp:Label runat="server" CssClass="form-label col-sm-4" AssociatedControlID="lblServerName">
            <asp:Localize ID="locServerName" runat="server" meta:resourcekey="locServerName" Text="Server Fully Qualified Domain Name:"></asp:Localize>
        </asp:Label>
        <div class="col-sm-8">
            <asp:TextBox ID="lblServerName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
        </div>
    </div>
    <div class="row mb-3">
        <asp:Label runat="server" CssClass="form-label col-sm-4" AssociatedControlID="txtServerComments">
            <asp:Localize ID="locServerComments" runat="server" meta:resourcekey="locServerComments" Text="Server Comments:"></asp:Localize>
        </asp:Label>
        <div class="col-sm-8">
            <asp:TextBox ID="txtServerComments" runat="server" CssClass="form-control"></asp:TextBox>
        </div>
    </div>
    <fcp:CollapsiblePanel id="secServerInfo" runat="server" TargetControlID="panelHardwareInfo" meta:resourcekey="secServerInfo" IsCollapsed="true" Text=""/>
    <asp:Panel runat="server" ID="panelHardwareInfo">
        <table>
            <tr>
                <td class="FormLabel150">
                    <asp:Literal ID="locStatus" runat="server" Text="Status:"/>
                </td>
                <td class="FormLabel150">
                    <asp:Literal ID="litStatus" runat="server"/>
                </td>
                <td/>
                <td/>
            </tr>
            <tr>
                <td class="FormLabel150">
                    <asp:Literal ID="locProcessor" runat="server" Text="Processor:"/>
                </td>
                <td class="FormLabel150">
                    <asp:Literal ID="litProcessor" runat="server"/>
                </td>
                <td class="FormLabel150">
                    <asp:Literal ID="locLoadPercentage" Text="Load Percentage:" runat="server"/>
                </td>
                <td class="FormLabel150">
                    <asp:Literal ID="litLoadPercentage" runat="server"/>
                </td>
            </tr>
            <tr>
                <td class="FormLabel150">
                    <asp:Literal ID="locMemoryAllocated" runat="server" Text="Allocated Memory:"/>
                </td>
                <td class="FormLabel150">
                    <asp:Literal ID="litMemoryAllocated" runat="server"/>
                </td>
                <td class="FormLabel150">
                    <asp:Literal ID="locFreeMemory" Text="Free Memory:" runat="server"/>
                </td>
                <td class="FormLabel150">
                    <asp:Literal ID="litFreeMemory" runat="server"/>
                </td>
            </tr>
        </table>
        <table>
            <asp:Repeater ID="rpServerDrives" runat="server" EnableViewState="false">
                <ItemTemplate>
                    <tr>
                        <td class="FormLabel150">
                            <asp:Localize ID="locDeviceID" runat="server" meta:resourcekey="locDeviceID" />
                        </td>
                        <td class="FormLabel150">
                            <asp:Literal ID="litDeviceId" runat="server" Text='<%# Eval("DeviceId") %>'/>
                        </td>
                        <td class="FormLabel150">
                            <asp:Literal ID="locVolumeName" Text="Volume Name:" runat="server"/>
                        </td>
                        <td class="FormLabel150">
                            <asp:Literal ID="litVolumeName" Text='<%# Eval("VolumeName") %>' runat="server"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="FormLabel150">
                            <asp:Literal ID="locSize" Text="Size:" runat="server"/>
                        </td>
                        <td class="FormLabel150">
                            <asp:Literal ID="litSize" Text='<%# Eval("SizeMb") + " MB" %>' runat="server"/>
                        </td>
                        <td class="FormLabel150">
                            <asp:Literal ID="locFreeSpace" Text="Free Space:" runat="server"/>
                        </td>
                        <td class="FormLabel150">
                            <asp:Literal ID="litFreeSpace" Text='<%# Eval("FreeSpaceMb") + " MB" %>' runat="server"/>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
    </asp:Panel>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click">
        <i class="bi bi-x-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnCancel"/>
    </asp:LinkButton>
    &nbsp;
	<asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click" OnClientClick="ShowProgressDialog('Updating server...');">
        <i class="bi bi-arrow-clockwise">&nbsp;
        </i>&nbsp;<asp:Localize runat="server" meta:resourcekey="btnUpdateText"/>
	</asp:LinkButton>
</div>

