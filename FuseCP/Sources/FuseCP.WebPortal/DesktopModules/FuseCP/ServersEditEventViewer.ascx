<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ServersEditEventViewer.ascx.cs" Inherits="FuseCP.Portal.ServersEditEventViewer" %>
<%@ Register Src="UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Import Namespace="FuseCP.Portal" %>
<%@ Register Src="ServerHeaderControl.ascx" TagName="ServerHeaderControl" TagPrefix="uc1" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

<uc1:ServerHeaderControl id="ServerHeaderControl1" runat="server">
</uc1:ServerHeaderControl>

<asp:UpdatePanel runat="server" ID="updatePanelUsers">
    <ContentTemplate>
        <div class="card-body d-flex flex-wrap gap-2 align-items-center">
            <div class="input-group">
                <asp:DropDownList ID="ddlLogNames" runat="server" CssClass="form-control" onSelectedIndexChanged="LogNameSelected" AutoPostBack="true"></asp:DropDownList>
            </div>
            <div class="input-group float-end">
                <asp:LinkButton id="btnClearLog" CssClass="btn btn-danger" runat="server" meta:resourcekey="btnClearLog" OnClick="btnClearLog_Click">
                    <i class="bi bi-trash me-1"></i>
                </asp:LinkButton>
            </div>
        </div>
   	    <fcp:SimpleMessageBox id="messageBox" runat="server" />
        <asp:GridView ID="gvEntries" runat="server" AutoGenerateColumns="False"
            EmptyDataText="gvEntries" AllowPaging="true" DataSourceID="odsLogEntries"
            CssSelectorClass="NormalGridView" EnableViewState="false" ondatabound="gvEntries_DataBound">
            <Columns>
                <asp:TemplateField HeaderText="gvEntriesType">
                    <ItemTemplate>
                        <asp:Image ID="imgType" runat="server"
                            ImageUrl='<%# PortalUtils.GetThemedImage(Eval("EntryType").ToString() + "_icon_small.gif") %>'
                            ImageAlign="AbsMiddle" />
                        <%# GetLocalizedString("EventType." + Eval("EntryType").ToString()) %>
                    </ItemTemplate>
                    <HeaderStyle Wrap="False" />
                </asp:TemplateField>
                <asp:BoundField DataField="Created" HeaderText="gvEntriesCreated" >
                </asp:BoundField>
                <asp:BoundField DataField="Source" HeaderText="gvEntriesSource" >
                </asp:BoundField>
                <asp:BoundField DataField="Category" HeaderText="gvEntriesCategory" >
                </asp:BoundField>
                <asp:BoundField DataField="EventID" HeaderText="gvEntriesEvent" >
                </asp:BoundField>
                <asp:BoundField DataField="UserName" HeaderText="gvEntriesUserName" >
                </asp:BoundField>
                <asp:BoundField DataField="MachineName" HeaderText="gvEntriesMachineName" >
                </asp:BoundField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <a data-bs-toggle="collapse" href="#infoRow-<%# Container.DataItemIndex %>" class="accordion-toggle collapsed"><i class="bi bi-plus-circle"></i></a>
                        <tr>
                            <td colspan="8" class="hiddenRow">
                                <div class="accordion-body collapse" id="infoRow-<%# Container.DataItemIndex %>">
                                    <pre><%# Eval("Message") %></pre>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="odsLogEntries" runat="server" SelectMethod="GetEventLogEntriesPaged" SelectCountMethod="GetEventLogEntriesPagedCount" TypeName="FuseCP.Portal.ServersHelper" OnSelected="odsLogEntries_Selected" EnablePaging="true">
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlLogNames" Name="logName" PropertyName="SelectedValue" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </ContentTemplate>
</asp:UpdatePanel>
<div class="card-footer text-end">
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click">
        <i class="bi bi-x-lg me-1"></i>
        <asp:Localize runat="server" meta:resourcekey="btnCancel"/>
    </asp:LinkButton>
</div>

<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/updatepanel-endrequest-close-progress.js"></script>
