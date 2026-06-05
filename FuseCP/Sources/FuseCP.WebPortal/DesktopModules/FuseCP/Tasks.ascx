<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tasks.ascx.cs" Inherits="FuseCP.Portal.Tasks" %>
<div class="fcp-tasks-page">
<asp:Timer runat="server" Interval="5000" ID="tasksTimer" />
<asp:UpdatePanel runat="server" ID="tasksUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="true">
  <Triggers>
  <asp:AsyncPostBackTrigger ControlID="tasksTimer" EventName="Tick" />
  </Triggers>
  <ContentTemplate>

<div class="fcp-tasks-header">
  <div>
    <h3 class="fcp-tasks-title">Running Tasks</h3>
    <asp:Literal ID="litTasksRoleHint" runat="server" />
  </div>
  <div class="small text-muted">Refreshes every 5 seconds.</div>
</div>

<div class="table-responsive fcp-tasks-grid-wrap">
<asp:GridView ID="gvTasks" runat="server" AutoGenerateColumns="False"
  EmptyDataText="gvTasks" CssSelectorClass="NormalGridView fcp-tasks-grid" EnableViewState="false"
  DataSourceID="odsTasks" OnRowDataBound="gvTasks_RowDataBound" OnRowCommand="gvTasks_RowCommand">
  <Columns>
    <asp:TemplateField HeaderText="gvTasksName">
      <ItemStyle></ItemStyle>
      <ItemTemplate>
	            <asp:hyperlink id="lnkTaskName" runat="server">
	            </asp:hyperlink>
      </ItemTemplate>
    </asp:TemplateField>
    <asp:TemplateField HeaderText="Source">
      <ItemTemplate>
        <span class="fcp-task-source"><%# Eval("Source") %></span>
      </ItemTemplate>
    </asp:TemplateField>
    <asp:BoundField DataField="ItemName" HeaderText="gvTasksItemName"></asp:BoundField>
    <asp:BoundField DataField="StartDate" HeaderText="gvTasksStarted"></asp:BoundField>
		<asp:TemplateField HeaderText="gvTasksDuration">
			<ItemTemplate>
			    <asp:Literal ID="litTaskDuration" runat="server"></asp:Literal>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField HeaderText="gvTasksProgress">
			<ItemTemplate>
        <div class="ProgressBarContainer">
          <asp:Panel id="pnlProgressIndicator" runat="server" CssClass="ProgressBarIndicator"></asp:Panel>
        </div>
        <asp:Literal ID="litProgressPercent" runat="server" />
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField HeaderText="gvTasksActions">
			<ItemTemplate>
			    <asp:LinkButton ID="cmdStop" runat="server" CommandName="stop"
              CssClass="btn btn-sm btn-outline-danger fcp-task-stop-action"
			        CausesValidation="false" Text="Stop" OnClientClick="return confirm('Do you really want to terminate this task?');"></asp:LinkButton>
			</ItemTemplate>
		</asp:TemplateField>
  </Columns>
</asp:GridView>
</div>

<asp:ObjectDataSource ID="odsTasks" runat="server"
    SelectMethod="GetRunningTasks"
    TypeName="FuseCP.Portal.TasksHelper"
    OnSelected="odsTasks_Selected">
    <SelectParameters>
    </SelectParameters>
</asp:ObjectDataSource>

  </ContentTemplate>
</asp:UpdatePanel>
</div>

  <script type="text/javascript">
  (function () {
    if (!window.Sys || !Sys.WebForms || !Sys.WebForms.PageRequestManager) {
      return;
    }

    // This page uses a local refresh class only. The shared busy overlay is
    // skipped for timer-driven requests via fcpGlobalBusy.shouldBypass(...) so
    // the global menu/search bar stays stable while the grid refreshes.
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_initializeRequest(function () {
      var page = document.querySelector('.fcp-tasks-page');
      if (page) {
        page.classList.add('is-refreshing');
      }
    });
    prm.add_endRequest(function () {
      var page = document.querySelector('.fcp-tasks-page');
      if (page) {
        page.classList.remove('is-refreshing');
      }
    });
  })();
  </script>
