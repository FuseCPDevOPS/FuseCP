<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LyncUserPlans.ascx.cs" Inherits="FuseCP.Portal.Lync.LyncUserPlans" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/CollapsiblePanel.ascx" TagName="CollapsiblePanel" TagPrefix="fcp" %>
<%@ Register Src="UserControls/LyncUserPlanSelector.ascx" TagName="LyncUserPlanSelector" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

			<div class="card-header">
                    <h3 class="card-title">
					<asp:Image ID="Image1" SkinID="LyncUserPlan48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Domain Names"></asp:Localize>
				</h3>
                        </div>
				<div class="card-body form-horizontal">
				    <fcp:SimpleMessageBox id="messageBox" runat="server" />
				    
                    <div class="FormButtonsBarClean">
                        <asp:LinkButton id="btnAddPlan" CssClass="btn btn-primary" runat="server" OnClick="btnAddPlan_Click"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddPlanText"/> </asp:LinkButton>
                    </div>

				    <asp:GridView ID="gvPlans" runat="server" AutoGenerateColumns="False" EnableViewState="true"
					    EmptyDataText="gvPlans" CssSelectorClass="NormalGridView" OnRowCommand="gvPlan_RowCommand">
					    <Columns>
						    <asp:TemplateField>
							    <ItemTemplate>							        
								    <asp:Image ID="img2" runat="server" Width="16px" Height="16px" ImageUrl='<%# GetPlanType((int)Eval("LyncUserPlanType")) %>' ImageAlign="AbsMiddle" />
							    </ItemTemplate>
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="gvPlan">
							    <ItemStyle></ItemStyle>
							    <ItemTemplate>
								    <asp:hyperlink id="lnkDisplayPlan" runat="server" EnableViewState="false"
									    NavigateUrl='<%# GetPlanDisplayUrl(Eval("LyncUserPlanId").ToString()) %>'>
									    <%# Eval("LyncUserPlanName")%> 
								    </asp:hyperlink>
							    </ItemTemplate>
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="gvPlanDefault">
							    <ItemTemplate>
							        <div class="text-center">
								        <input type="radio" name="DefaultPlan" value='<%# Eval("LyncUserPlanId") %>' <%# IsChecked((bool)Eval("IsDefault")) %> />
								    </div>
							    </ItemTemplate>
						    </asp:TemplateField>
						    <asp:TemplateField>
							    <ItemTemplate>
									<asp:LinkButton id="imgDelMailboxPlan" CssClass="btn btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("LyncUserPlanId") %>' OnClientClick="return confirm('Are you sure you want to delete selected plan?')"> <i class="bi bi-trash"></i> </asp:LinkButton>
							    </ItemTemplate>
						    </asp:TemplateField>
					    </Columns>
				    </asp:GridView>
				    <br />
				    <div class="text-center">
				        <asp:LinkButton id="btnSetDefaultPlan" CssClass="btn btn-success" runat="server" OnClick="btnSetDefaultPlan_Click"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSetDefaultPlanText"/> </asp:LinkButton>
                    </div>
				    
                    <fcp:CollapsiblePanel id="secMainTools" runat="server" IsCollapsed="true" TargetControlID="ToolsPanel" meta:resourcekey="secMainTools" Text="Lync user plan maintenance">
					</fcp:CollapsiblePanel>
					<asp:Panel ID="ToolsPanel" runat="server" Height="0" Style="overflow: hidden;">
						<table class="table table-borderless align-middle mb-0" id="tblMaintenance" runat="server">
					        <tr>
					            <td class="FormLabel150"><asp:Localize ID="lblSourcePlan" runat="server" meta:resourcekey="locSourcePlan" Text="Replace"></asp:Localize></td>
					            <td>                                
                                    <fcp:LyncUserPlanSelector ID="lyncUserPlanSelectorSource" runat="server" AddNone="true"/>
                                </td>
					        </tr>
					        <tr>
					            <td class="FormLabel150"><asp:Localize ID="lblTargetPlan" runat="server" meta:resourcekey="locTargetPlan" Text="With"></asp:Localize></td>
					            <td>                                
                                    <fcp:LyncUserPlanSelector ID="lyncUserPlanSelectorTarget" runat="server" AddNone="false"/>
                                </td>
					        </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtStatus" runat="server"  CssClass="form-control" MaxLength="128" ReadOnly="true"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                            </tr>
						</table>
				        <div class="card-footer text-end">
					        <asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" OnClientClick = "ShowProgressDialog('Stamping mailboxes, this might take a while ...');"> <i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveText"/> </asp:LinkButton>
				        </div>
					</asp:Panel>
				</div>
