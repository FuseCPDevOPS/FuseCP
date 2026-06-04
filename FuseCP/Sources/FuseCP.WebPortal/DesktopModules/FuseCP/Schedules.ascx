<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Schedules.ascx.cs" Inherits="FuseCP.Portal.Schedules" %>
<%@ Import Namespace="FuseCP.Portal" %>
<%@ Register Src="UserControls/UserDetails.ascx" TagName="UserDetails" TagPrefix="uc2" %>
<%@ Register Src="UserControls/SearchBox.ascx" TagName="SearchBox" TagPrefix="uc1" %>
<%@ Register Src="UserControls/Quota.ascx" TagName="Quota" TagPrefix="uc4" %>
<%@ Import Namespace="FuseCP.Portal" %>

<asp:Timer runat="server" Interval="15000" Enabled="false" ID="tasksTimer" OnTick="tasksTimer_Tick" />
<div class="fcp-schedules-page">
	<div class="fcp-schedules-toolbar">
		<div class="fcp-schedules-toolbar-main">
			<div class="fcp-schedules-recursive">
				<asp:CheckBox ID="chkRecursive" runat="server" AutoPostBack="True" CssClass="Normal"
					Text="Recursive" meta:resourcekey="chkRecursive" />
			</div>
			<div class="fcp-schedules-search">
				<uc1:SearchBox ID="searchBox" runat="server" />
			</div>
		</div>
		<div class="fcp-schedules-actions">
			<asp:CheckBox ID="chkAutoRefresh" runat="server" AutoPostBack="True" CssClass="Normal me-2"
				Text="Auto Refresh" />
			<asp:LinkButton ID="btnRefresh" runat="server" CssClass="btn btn-outline-secondary me-2">
				<i class="bi bi-arrow-repeat me-1"></i><asp:Localize runat="server" meta:resourcekey="btnRefresh" />
			</asp:LinkButton>
			<asp:LinkButton ID="btnAddItem" runat="server" CssClass="btn btn-primary" OnClick="btnAddItem_Click">
				<i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddItem" />
			</asp:LinkButton>
		</div>
	</div>
	<div class="fcp-scheduler-hero row g-3">
		<div class="col-12 col-lg-4">
			<div id="cardCronStatus" runat="server" class="fcp-scheduler-stat fcp-scheduler-stat-ok">
				<div class="fcp-scheduler-stat-icon"><i class="bi bi-check2"></i></div>
				<div>
					<div class="fcp-scheduler-stat-value"><asp:Literal ID="litCronStatusValue" runat="server" /></div>
					<div class="fcp-scheduler-stat-label"><asp:Literal ID="litCronStatusLabel" runat="server" /></div>
				</div>
			</div>
		</div>
		<div class="col-12 col-lg-4">
			<div class="fcp-scheduler-stat fcp-scheduler-stat-last">
				<div class="fcp-scheduler-stat-icon"><i class="bi bi-calendar-event"></i></div>
				<div>
					<div class="fcp-scheduler-stat-value"><asp:Literal ID="litCronLastInvocationValue" runat="server" /></div>
					<div class="fcp-scheduler-stat-label">Last Cron Invocation</div>
				</div>
			</div>
		</div>
		<div class="col-12 col-lg-4">
			<div class="fcp-scheduler-stat fcp-scheduler-stat-next">
				<div class="fcp-scheduler-stat-icon"><i class="bi bi-calendar2-check"></i></div>
				<div>
					<div class="fcp-scheduler-stat-value"><asp:Literal ID="litCronNextRunValue" runat="server" /></div>
					<div class="fcp-scheduler-stat-label">Next Daily Task Run</div>
				</div>
			</div>
		</div>
	</div>
	<div class="alert alert-info fcp-schedules-overview" role="status">
		<asp:Literal ID="litScheduleOverview" runat="server" />
	</div>
	<div class="row g-3 mb-3" runat="server" id="rowSchedulerDashboard">
		<div class="col-12 col-xl-8">
			<div class="card h-100">
				<div class="card-header">
					Scheduler Health
				</div>
				<div class="card-body">
					<asp:Literal ID="litSchedulerHealth" runat="server" />
					<asp:Literal ID="litSchedulerExecution" runat="server" />
				</div>
			</div>
		</div>
		<div class="col-12 col-xl-4">
			<div class="card h-100">
				<div class="card-header">
					Auto-Tune Snapshot
				</div>
				<div class="card-body">
					<asp:Literal ID="litSchedulerAutotune" runat="server" />
					<div id="pnlSchedulerOverrides" runat="server" class="fcp-scheduler-overrides mt-3">
						<div class="fcp-scheduler-overrides-title">Manual Runtime Overrides</div>
						<div class="small text-muted mb-2">Applies live to the scheduler worker on this EnterpriseServer runtime.</div>
						<div class="row g-2">
							<div class="col-12 col-md-6">
								<asp:Label ID="lblPerAffinityConcurrency" runat="server" CssClass="form-label" AssociatedControlID="txtPerAffinityConcurrency" Text="Per-affinity max" />
								<asp:TextBox ID="txtPerAffinityConcurrency" runat="server" CssClass="form-control" MaxLength="4" />
							</div>
							<div class="col-12 col-md-6">
								<asp:Label ID="lblGlobalConcurrency" runat="server" CssClass="form-label" AssociatedControlID="txtGlobalConcurrency" Text="Global max" />
								<asp:TextBox ID="txtGlobalConcurrency" runat="server" CssClass="form-control" MaxLength="4" />
							</div>
						</div>
						<div class="d-flex align-items-center gap-2 mt-2">
							<asp:LinkButton ID="btnApplySchedulerOverrides" runat="server" CssClass="btn btn-sm btn-primary" OnClick="btnApplySchedulerOverrides_Click">
								<i class="bi bi-sliders me-1"></i>Apply
							</asp:LinkButton>
							<asp:Literal ID="litSchedulerOverrideResult" runat="server" />
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>

<asp:UpdatePanel runat="server" ID="schedulesUpdatePanel" UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="tasksTimer" EventName="Tick" />
    </Triggers>
    <ContentTemplate>
<div class="fcp-schedules-grid-wrap table-responsive">
<asp:GridView id="gvSchedules" runat="server" AutoGenerateColumns="False"
	DataSourceID="odsSchedules" AllowPaging="True" AllowSorting="True" EmptyDataText="gvSchedules"
	OnRowCommand="gvSchedules_RowCommand" CssSelectorClass="NormalGridView fcp-schedules-grid" DataKeyNames="ScheduleID">
	<PagerStyle CssClass="fcp-schedules-pager" />
	<Columns>
		<asp:TemplateField SortExpression="ScheduleName" HeaderText="gvSchedulesName">
			<ItemStyle></ItemStyle>
			<HeaderStyle Wrap="false" />
			<ItemTemplate>
				<asp:hyperlink id="lnkEdit" runat="server" NavigateUrl='<%# EditUrl("ScheduleID", Eval("ScheduleID").ToString(), "edit", "SpaceID=" + PanelSecurity.PackageId) %>'>
					<%# PortalAntiXSS.Encode((string)Eval("ScheduleName")) %>
				</asp:hyperlink>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField SortExpression="ScheduleTypeID" HeaderText="gvSchedulesType"
		    ItemStyle-Wrap="false" HeaderStyle-Wrap="false">
			<ItemTemplate>
    			<%# GetSharedLocalizedString("ScheduleType." + Eval("ScheduleTypeID").ToString()) %>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField SortExpression="NextRun" HeaderText="gvSchedulesNextRun"
		    ItemStyle-Wrap="false" HeaderStyle-Wrap="false">
			<ItemTemplate>
				<%# GetNextRunDisplay(Eval("ScheduleID"), Eval("NextRun")) %>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField DataField="LastRun" SortExpression="LastRun" HeaderText="gvSchedulesLastRun"
		    ItemStyle-Wrap="false" HeaderStyle-Wrap="false"></asp:BoundField>
		<asp:TemplateField HeaderText="gvSchedulesStatus" ItemStyle-Wrap="false">
			<ItemTemplate>
                <asp:ImageButton ID="cmdStart" runat="server" ToolTip="Start" SkinID="StartMedium" Visible='<%# !IsScheduleActive((int)Eval("StatusID")) %>'
                    CommandName="start" CommandArgument='<%# Eval("ScheduleID") %>' />
                <asp:ImageButton ID="cmdStop" runat="server" ToolTip="Stop" SkinID="StopMedium" Visible='<%# IsScheduleActive((int)Eval("StatusID")) %>'
                    CommandName="stop" CommandArgument='<%# Eval("ScheduleID") %>' />
                <%# GetScheduleStatus((int)Eval("StatusID")) %>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField HeaderText="gvSchedulesResult" HeaderStyle-Wrap="false">
			<ItemTemplate>
    			<%# GetAuditLogRecordSeverityName((int)Eval("LastResult"))%>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:TemplateField SortExpression="ServerName" HeaderText="gvSchedulesServer">
		    <ItemStyle Wrap="False"></ItemStyle>
		    <ItemTemplate>
		        <%# Eval("ServerName") %>
		    </ItemTemplate>
		    <HeaderStyle Wrap="False" />
		</asp:TemplateField>
        <asp:TemplateField SortExpression="PackageName" HeaderText="gvSchedulesSpace">
            <ItemStyle Wrap="False"></ItemStyle>
            <ItemTemplate>
	            <asp:hyperlink id="lnkSpace" runat="server"
	                NavigateUrl='<%# GetSpaceHomePageUrl((int)Eval("PackageID")) %>'>
		            <%# Eval("PackageName") %>
	            </asp:hyperlink>
            </ItemTemplate>
        </asp:TemplateField>
		<asp:TemplateField SortExpression="Username" HeaderText="gvSchedulesUser">
		    <ItemStyle Wrap="False"></ItemStyle>
			<ItemTemplate>
				<asp:hyperlink id="lnkUser" runat="server"
				    NavigateUrl='<%# GetUserHomePageUrl((int)Eval("UserID")) %>'>
					<%# Eval("Username") %>
				</asp:hyperlink>
			</ItemTemplate>
            <HeaderStyle Wrap="False" />
		</asp:TemplateField>
	</Columns>
</asp:GridView>
</div>
<div class="GridFooter fcp-schedules-footer">
    <asp:Label ID="lblScheduledTasks" runat="server" meta:resourcekey="lblScheduledTasks" Text="Scheduled Tasks:"></asp:Label>
    <uc4:Quota ID="quotaTasks" runat="server" QuotaName="OS.ScheduledTasks" />
</div>
    </ContentTemplate>
</asp:UpdatePanel>
</div>

<asp:ObjectDataSource ID="odsSchedules" runat="server" EnablePaging="True" SelectCountMethod="GetSchedulesPagedCount"
    SelectMethod="GetSchedulesPaged" SortParameterName="sortColumn" TypeName="FuseCP.Portal.SchedulesHelper" OnSelected="odsSchedules_Selected">
    <SelectParameters>
        <asp:ControlParameter ControlID="chkRecursive" Name="recursive" PropertyName="Checked" />
        <asp:ControlParameter ControlID="searchBox" Name="filterColumn" PropertyName="FilterColumn" />
         <asp:ControlParameter ControlID="searchBox" Name="filterValue" PropertyName="FilterValue" />
    </SelectParameters>
</asp:ObjectDataSource>
