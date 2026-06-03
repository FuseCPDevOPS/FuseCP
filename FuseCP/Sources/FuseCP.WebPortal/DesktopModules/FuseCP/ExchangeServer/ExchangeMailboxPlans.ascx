<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangeMailboxPlans.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangeMailboxPlans" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/CollapsiblePanel.ascx" TagName="CollapsiblePanel" TagPrefix="fcp" %>
<%@ Register Src="UserControls/MailboxPlanSelector.ascx" TagName="MailboxPlanSelector" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>


				<div class="card-header">
                    <h3 class="card-title">
					<asp:Image ID="Image1" SkinID="ExchangeMailboxPlans48" runat="server" />
					<asp:Localize ID="locTitle" runat="server"></asp:Localize>
                        </h3>
				</div>
       <div class="FormButtonsBar right">
                        <asp:LinkButton id="btnAddMailboxPlan" CssClass="btn btn-primary" runat="server" OnClick="btnAddMailboxPlan_Click"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddMailboxPlan"/> </asp:LinkButton>
                    </div>
				<div class="card-body form-horizontal">
				    <fcp:SimpleMessageBox id="messageBox" runat="server" />
				    
             </div>

				    <asp:GridView ID="gvMailboxPlans" runat="server" AutoGenerateColumns="False" EnableViewState="true"
					    EmptyDataText="gvMailboxPlans" CssSelectorClass="NormalGridView" OnRowCommand="gvMailboxPlan_RowCommand">
					    <Columns>
						    <asp:TemplateField>
							    <ItemTemplate>							        
								    <asp:Image ID="img2" runat="server" Width="16px" Height="16px" ImageUrl='<%# GetPlanType((int)Eval("MailboxPlanType")) %>' ImageAlign="AbsMiddle" />
							    </ItemTemplate>
						    </asp:TemplateField>

						    <asp:TemplateField HeaderText="gvMailboxPlan">
							    <ItemStyle></ItemStyle>
							    <ItemTemplate>
								    <asp:hyperlink id="lnkDisplayMailboxPlan" runat="server" EnableViewState="false"
									    NavigateUrl='<%# GetMailboxPlanDisplayUrl(Eval("MailboxPlanId").ToString()) %>'>
									    <%# Eval("MailboxPlan")%> 
								    </asp:hyperlink>
							    </ItemTemplate>
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="gvMailboxPlanDefault">
							    <ItemTemplate>
							        <div class="text-center">
								        <input type="radio" name="DefaultMailboxPlan" value='<%# Eval("MailboxPlanId") %>' <%# IsChecked((bool)Eval("IsDefault")) %> />
								    </div>
							    </ItemTemplate>
						    </asp:TemplateField>
						    <asp:TemplateField >
							    <ItemTemplate>
									<asp:LinkButton id="imgDelMailboxPlan" CssClass="btn btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("MailboxPlanId") %>' OnClientClick="return confirm('Are you sure you want to delete selected?')"> <i class="bi bi-trash"></i> </asp:LinkButton>
							    </ItemTemplate>
						    </asp:TemplateField>
					    </Columns>
				    </asp:GridView>
				    <br />
				    <div class="card-body text-end">
				        <asp:LinkButton id="btnSetDefaultMailboxPlan" CssClass="btn btn-success" runat="server"  CausesValidation="false" OnClick="btnSetDefaultMailboxPlan_Click"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSetDefaultMailboxPlan"/> </asp:LinkButton>
                    </div>
<div class="card-body">
					<fcp:CollapsiblePanel id="secMainTools" runat="server" IsCollapsed="true" TargetControlID="ToolsPanel" meta:resourcekey="secMainTools" Text="Mailbox plan maintenance">
					</fcp:CollapsiblePanel>
					<asp:Panel ID="ToolsPanel" runat="server" Height="0" Style="overflow: hidden;" CssClass="card">
                        <div class="card-body">
						<table id="tblMaintenance" runat="server" class="table table-borderless align-middle mb-0">
					        <tr>
					            <td class="FormLabel150"><asp:Localize ID="lblSourcePlan" runat="server" meta:resourcekey="locSourcePlan" Text="Replace"></asp:Localize></td>
					            <td>                                
                                    <fcp:MailboxPlanSelector ID="mailboxPlanSelectorSource" runat="server" AddNone="true"/>
                                </td>
					        </tr>
					        <tr>
					            <td class="FormLabel150"><asp:Localize ID="lblTargetPlan" runat="server" meta:resourcekey="locTargetPlan" Text="With"></asp:Localize></td>
					            <td>                                
                                    <fcp:MailboxPlanSelector ID="mailboxPlanSelectorTarget" runat="server" AddNone="false"/>
                                </td>
					        </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtStatus" runat="server" CssClass="TextBox400" MaxLength="128" ReadOnly="true"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                            </tr>
						</table>
                            </div>
                        				        <div class="card-footer text-end">
					        <asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" OnClientClick = "ShowProgressDialog('Stamping mailboxes, this might take a while ...');"> <i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveText"/> </asp:LinkButton>
				        </div>
					</asp:Panel>
				</div>
