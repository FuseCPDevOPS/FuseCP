<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServersEditTerminalConnections.ascx.cs" Inherits="FuseCP.Portal.ServersEditTerminalConnections" %>
<%@ Register Src="ServerHeaderControl.ascx" TagName="ServerHeaderControl" TagPrefix="uc1" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/terminal-connections.js"></script>
<uc1:ServerHeaderControl id="ServerHeaderControl1" runat="server">
</uc1:ServerHeaderControl>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

<asp:UpdatePanel ID="ItemsUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:Timer runat="server" Interval="10000" ID="itemsTimer" />
        <asp:GridView ID="gvSessions" runat="server" AutoGenerateColumns="False"
            EmptyDataText="gvSessions"
            CssSelectorClass="NormalGridView" EnableViewState="false"
            DataKeyNames="SessionID" OnRowDeleting="gvSessions_RowDeleting">
            <Columns>
                <asp:BoundField DataField="SessionID" HeaderText="gvSessionsSessionID" />
                <asp:BoundField DataField="Username" HeaderText="gvSessionsUserName" />
                <asp:BoundField DataField="Status" HeaderText="gvSessionsStatus" />
                <asp:TemplateField HeaderText="gvSessionsReset">
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:ImageButton ID="cmdReset" runat="server" SkinID="StopSmall" meta:resourcekey="cmdReset"
                            CommandName="delete" OnClientClick="return confirm('Reset session?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </ContentTemplate>
</asp:UpdatePanel>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click">
        <i class="bi bi-x-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnCancel"/>
    </asp:LinkButton>
</div>
