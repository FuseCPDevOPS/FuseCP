<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VpsDetailsInsertDvd.ascx.cs" Inherits="FuseCP.Portal.VPSForPC.VpsDetailsInsertDvd" %>
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
                        
			            <p class="SubTitle">
			                <asp:Localize ID="locSubTitle" runat="server" meta:resourcekey="locSubTitle" Text="Browse Media Library" />
			            </p>
			            
			            <asp:GridView ID="gvDisks" runat="server" AutoGenerateColumns="False" EnableViewState="true"
				            EmptyDataText="gvDisks" CssSelectorClass="NormalGridView"
                            onrowcommand="gvDisks_RowCommand">
				            <Columns>
					            <asp:TemplateField HeaderText="gvTitle" meta:resourcekey="gvTitle">
						            <ItemTemplate>
						                <asp:Image ID="Image2" SkinID="Dvd48" runat="server" CssClass="float-start" />
						                <div class="fw-bold fcp-p-3 fcp-ms-50">
						                    <%# Eval("Name") %>
						                </div>
						                <div class="fcp-p-3 fcp-ms-50">
							                <%# Eval("Description") %>
							            </div>
						            </ItemTemplate>
					            </asp:TemplateField>
					            <asp:TemplateField>
						            <ItemTemplate>
							            <asp:Button ID="btnInsert" runat="server" Text="Insert" meta:resourcekey="btnInsert"
							                CommandName="insert" CommandArgument='<%# Eval("Path") %>' CssClass="btn btn-primary btn-sm">
							            </asp:Button>
						            </ItemTemplate>
					            </asp:TemplateField>
				            </Columns>
			            </asp:GridView>
			            <br />
			            <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>
			    </div>
		    </div>
            </div>
            </div>
		    <div class="Right">
			    <asp:Localize ID="FormComments" runat="server" meta:resourcekey="FormComments"></asp:Localize>
		    </div>
