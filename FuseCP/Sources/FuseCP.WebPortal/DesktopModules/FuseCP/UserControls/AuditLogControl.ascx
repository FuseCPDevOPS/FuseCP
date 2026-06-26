<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuditLogControl.ascx.cs" Inherits="FuseCP.Portal.UserControls.AuditLogControl" %>
<%@ Register Src="PopupHeader.ascx" TagName="PopupHeader" TagPrefix="fcp" %>
<%@ Import Namespace="FuseCP.Portal" %>

<table class="table">
    <tr>
        <td >
        <asp:Calendar ID="calPeriod" runat="server"
            SelectionMode="DayWeekMonth"
            DayNameFormat="Shortest"
            Height="180px" Width="200px" OnSelectionChanged="calPeriod_SelectionChanged">
        </asp:Calendar></td>
        <td>
            <table class="table">
                <tr>
                    <td class="Big" colspan="2">
                        <asp:Literal ID="litPeriod" runat="server"></asp:Literal>
                        <asp:HiddenField ID="hidStartDate" runat="server" />
                        <asp:HiddenField ID="hidEndDate" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="SubHead text-nowrap">
                        <asp:Label id="lblSeverity" runat="server" meta:resourcekey="lblSeverity" Text="Severity"></asp:Label>
                    </td>
                    <td class="Normal">
                        <asp:DropDownList ID="ddlSeverity" runat="server" CssClass="form-control" resourcekey="ddlSeverity" AutoPostBack="true">
                            <asp:ListItem Value="-1">All</asp:ListItem>
                            <asp:ListItem Value="0">Information</asp:ListItem>
                            <asp:ListItem Value="1">Warning</asp:ListItem>
                            <asp:ListItem Value="2">Error</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr id="SourceRow" runat="server">
                    <td class="SubHead text-nowrap">
                        <asp:Label id="lblSource" runat="server" meta:resourcekey="lblSource" Text="Source"></asp:Label>
                    </td>
                    <td class="Normal">
                        <asp:DropDownList ID="ddlSource" runat="server" CssClass="form-control"
                            AutoPostBack="True" OnSelectedIndexChanged="ddlSource_SelectedIndexChanged">
                        </asp:DropDownList></td>
                </tr>
                <tr>
                    <td class="SubHead text-nowrap" style="height: 24px">
                        <asp:Label id="lblTask" runat="server" meta:resourcekey="lblTask" Text="Task"></asp:Label>
                    </td>
                    <td class="Normal" style="height: 24px">
                        <asp:DropDownList ID="ddlTask" runat="server" CssClass="form-control" AutoPostBack="true">
                        </asp:DropDownList></td>
                </tr>
                <tr id="ItemNameRow" runat="server">
                    <td class="SubHead text-nowrap" style="height: 24px">
                        <asp:Label id="lblItemName" runat="server" meta:resourcekey="lblItemName" Text="Item Name"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtItemName" runat="server" CssClass="form-control"></asp:TextBox>
                    </td>
                </tr>
                <tr id="FilterButtonsRow" runat="server">
                    <td colspan="2">
                        <asp:Button ID="btnDisplay" runat="server" Text="Display Records" meta:resourcekey="btnDisplay"
                            CssClass="btn btn-success" OnClick="btnDisplay_Click" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>



<div class="FormButtonsBar">
	<div class="FormButtonsBarCleanLeft">
	    <asp:Button ID="btnExportLog" runat="server" Text="Export Log" meta:resourcekey="btnExportLog"
		    CssClass="btn btn-primary" OnClick="btnExportLog_Click" />
		<asp:Button ID="btnClearLog" runat="server" Text="Clear Log" meta:resourcekey="btnClearLog"
			CssClass="btn btn-danger" OnClick="btnClearLog_Click" OnClientClick="return confirm('Clear Log?');" />
	</div>
	<div class="FormButtonsBarCleanRight">
		<asp:UpdateProgress ID="recordsProgress" runat="server"
			AssociatedUpdatePanelID="updatePanelLog" DynamicLayout="false">
			<ProgressTemplate>
                <asp:Image ID="imgSep" runat="server" SkinID="AjaxIndicator" CssClass="my-1" />
			</ProgressTemplate>
		</asp:UpdateProgress>
	</div>
</div>


<asp:UpdatePanel runat="server" ID="updatePanelLog" UpdateMode="Conditional" ChildrenAsTriggers="true">
    <ContentTemplate>


<asp:GridView ID="gvLog" runat="server" AutoGenerateColumns="False"
    EmptyDataText="gvLog" CssSelectorClass="NormalGridView" EnableViewState="false"
    AllowSorting="True" DataSourceID="odsLog" AllowPaging="True"
    DataKeyNames="RecordID">
    <Columns>
        <asp:TemplateField SortExpression="SeverityID" HeaderText="gvLogSeverity">
            <ItemStyle Wrap="False" />
            <ItemTemplate>
                <asp:Image ID="imgIcon" runat="server" CssClass="align-middle me-1" ImageUrl='<%# GetIconUrl((int)Eval("SeverityID")) %>' />
	            <%# GetAuditLogRecordSeverityName((int)Eval("SeverityID")) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField SortExpression="StartDate" HeaderText="gvLogStartDate">
            <ItemStyle Wrap="False" />
            <ItemTemplate>
	            <%# ((DateTime)Eval("StartDate")).ToShortDateString() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="gvLogStartTime">
            <ItemStyle Wrap="False" />
            <ItemTemplate>
	            <%# ((DateTime)Eval("StartDate")).ToShortTimeString() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="gvLogFinishTime">
            <ItemStyle Wrap="False" />
            <ItemTemplate>
	            <%# ((DateTime)Eval("FinishDate")).ToShortTimeString() %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField SortExpression="SourceName" HeaderText="gvLogSource">
            <ItemStyle Wrap="False" />
            <ItemTemplate>
		         <%# GetAuditLogSourceName((string)Eval("SourceName")) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField SortExpression="TaskName" HeaderText="gvLogTask">
            <ItemStyle />
            <ItemTemplate>
                <a href="javascript:void(0)" class="audit-log-detail" data-recordid='<%# Eval("RecordID") %>'>
		            <%# GetAuditLogTaskName((string)Eval("SourceName"), (string)Eval("TaskName"))%>
		        </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField SortExpression="ItemName" HeaderText="gvLogItemName" >
            <ItemStyle Wrap="false" />
            <ItemTemplate>
		         <%# PortalAntiXSS.Encode((string)Eval("ItemName"))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField SortExpression="Username" HeaderText="gvLogUser">
            <ItemStyle Wrap="False" />
            <ItemTemplate>
		         <asp:HyperLink ID="lnkUser" runat="server" NavigateUrl='<%# NavigateURL("UserID", Eval("EffectiveUserID").ToString())%>'>
		            <%# Eval("Username")%>
		         </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:ObjectDataSource ID="odsLog" runat="server" EnablePaging="True" SelectCountMethod="GetAuditLogRecordsPagedCount"
    SelectMethod="GetAuditLogRecordsPaged" SortParameterName="sortColumn" TypeName="FuseCP.Portal.AuditLogHelper" OnSelected="odsLog_Selected">
    <SelectParameters>
        <asp:ControlParameter Name="sStartDate" ControlID="hidStartDate" PropertyName="Value" />
        <asp:ControlParameter Name="sEndDate" ControlID="hidEndDate" PropertyName="Value" />
        <asp:QueryStringParameter Name="packageId" QueryStringField="SpaceID" Type="int32" DefaultValue="0" />
        <asp:QueryStringParameter Name="itemId" QueryStringField="ItemId" Type="int32" DefaultValue="0" />
        <asp:ControlParameter Name="itemName" ControlID="txtItemName" PropertyName="Text" />
        <asp:ControlParameter Name="severityId" ControlID="ddlSeverity" PropertyName="SelectedValue" Type="Int32" />
        <asp:ControlParameter Name="sourceName" ControlID="ddlSource" PropertyName="SelectedValue" />
        <asp:ControlParameter Name="taskName" ControlID="ddlTask" PropertyName="SelectedValue" />
    </SelectParameters>
</asp:ObjectDataSource>

<!-- Hidden trigger for audit log detail postback -->
<asp:HiddenField ID="hidRecordId" runat="server" />
<asp:Button ID="btnShowDetail" runat="server" style="display:none" OnClick="btnShowDetail_Click" />

<!-- Bootstrap 5 Modal for Task Details (inside UpdatePanel so content is refreshed on partial postback) -->
<div class="modal fade" id="taskDetailsModal" tabindex="-1" aria-labelledby="taskDetailsModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="taskDetailsModalLabel">
                    <i class="bi bi-book me-2"></i>
                    <asp:Localize ID="TaskDetailsHeader" runat="server" Text="Task Details" meta:resourcekey="TaskDetailsHeader"></asp:Localize>
                </h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <div class="row">
                    <div class="col-md-6">
                        <table class="table table-sm">
                            <tr>
                                <td class="SubHead">
                                    <asp:Label ID="lblSourceName" runat="server" meta:resourcekey="lblSourceName" Text="Source:"></asp:Label>
                                </td>
                                <td class="Normal">
                                    <asp:Literal ID="litSourceName" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead">
                                    <asp:Label ID="lblTaskName" runat="server" meta:resourcekey="lblTaskName" Text="Task Name:"></asp:Label>
                                </td>
                                <td class="Normal">
                                    <asp:Literal ID="litTaskName" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead">
                                    <asp:Label ID="lblItemName1" runat="server" meta:resourcekey="lblItemName1" Text="Item Name:"></asp:Label>
                                </td>
                                <td class="Normal">
                                    <asp:Literal ID="litItemName" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead">
                                    <asp:Label ID="lblRecordUser" runat="server" meta:resourcekey="lblRecordUser" Text="User:"></asp:Label>
                                </td>
                                <td class="Normal">
                                    <asp:Literal ID="litUsername" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="col-md-6">
                        <table class="table table-sm">
                            <tr>
                                <td class="SubHead">
                                    <asp:Label ID="lblStarted" runat="server" meta:resourcekey="lblStarted" Text="Started:"></asp:Label>
                                </td>
                                <td class="Normal">
                                    <asp:Literal ID="litStarted" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead">
                                    <asp:Label ID="lblFinished" runat="server" meta:resourcekey="lblFinished" Text="Finished:"></asp:Label>
                                </td>
                                <td class="Normal">
                                    <asp:Literal ID="litFinished" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead">
                                    <asp:Label ID="lblDuration" runat="server" meta:resourcekey="lblDuration" Text="Duration:"></asp:Label>
                                </td>
                                <td class="Normal">
                                    <asp:Literal ID="litDuration" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead">
                                    <asp:Label ID="lblResultSeverity" runat="server" meta:resourcekey="lblResultSeverity" Text="Severity:"></asp:Label>
                                </td>
                                <td class="Normal">
                                    <asp:Literal ID="litSeverity" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <hr />
                <div class="SubHead mb-2">
                    <asp:Label ID="lblExecutionLog" runat="server" meta:resourcekey="lblExecutionLog" Text="Execution Log:"></asp:Label>
                </div>
                <div class="fcp-log-scroll" style="max-height:300px;overflow-y:auto;">
                    <asp:Literal ID="litLog" runat="server"></asp:Literal>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-primary" data-bs-dismiss="modal">
                    <i class="bi bi-x-lg me-1"></i>
                    <asp:Localize ID="btnCloseTaskDetailsText" runat="server" meta:resourcekey="btnCloseTaskDetailsText" Text="Close"></asp:Localize>
                </button>
            </div>
        </div>
    </div>
</div>

</ContentTemplate>
<Triggers>
    <asp:AsyncPostBackTrigger ControlID="btnShowDetail" EventName="Click" />
</Triggers>
</asp:UpdatePanel>

<script type="text/javascript">
    (function () {
        // Delegate click handler for audit log detail links (works after UpdatePanel refresh)
        document.addEventListener('click', function (e) {
            var link = e.target.closest('.audit-log-detail');
            if (!link) return;
            e.preventDefault();
            var recordId = link.getAttribute('data-recordid');
            var hidField = document.getElementById('<%= hidRecordId.ClientID %>');
            var btnShow = document.getElementById('<%= btnShowDetail.ClientID %>');
            if (hidField && btnShow) {
                hidField.value = recordId;
                btnShow.click();
            }
        });
    })();
</script>
