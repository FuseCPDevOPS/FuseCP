<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangeDisclaimers.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangeDisclaimers" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

				<div class="card-header">
                    <h3 class="card-title">
					<asp:Image ID="Image1" SkinID="ExchangeDisclaimers48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Lists"></asp:Localize>
                        </h3>
				</div>
				<div class="FormButtonsBar right">
                            <asp:LinkButton id="btnCreateList" CssClass="btn btn-primary" runat="server" OnClick="btnCreateList_Click"> <i class="bi bi-file-earmark-text me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCreateList"/> </asp:LinkButton>
                        </div>
				<div class="card-body form-horizontal">
				    <fcp:SimpleMessageBox id="messageBox" runat="server" />
				    <asp:GridView ID="gvLists" runat="server" AutoGenerateColumns="False" EnableViewState="true"
					    EmptyDataText="gvLists" CssSelectorClass="NormalGridView"
					    OnRowCommand="gvLists_RowCommand" AllowPaging="True" AllowSorting="True"
					    PageSize="20">
					    <Columns>
						    <asp:TemplateField HeaderText="gvListsDisplayName" SortExpression="DisplayName">
							    <ItemStyle></ItemStyle>
							    <ItemTemplate>
								    <asp:hyperlink id="lnk1" runat="server"
									    NavigateUrl='<%# GetListEditUrl(Eval("ExchangeDisclaimerId").ToString()) %>'>
									    <%# Eval("DisclaimerName")%>
								    </asp:hyperlink>
							    </ItemTemplate>
						    </asp:TemplateField>
						    <asp:TemplateField>
							    <ItemTemplate>
								    <asp:LinkButton id="cmdDelete" CssClass="btn btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("ExchangeDisclaimerId") %>' OnClientClick="return confirm('Remove this item?');"> <i class="bi bi-trash"></i> </asp:LinkButton>
							    </ItemTemplate>
						    </asp:TemplateField>
					    </Columns>
				    </asp:GridView>
				    <br />
				    <asp:Localize ID="locQuota" runat="server" meta:resourcekey="locQuota" Text="Total Distribution Lists Created:"></asp:Localize>
				    &nbsp;&nbsp;&nbsp;
				    <fcp:QuotaViewer ID="listsQuota" runat="server" QuotaTypeId="2" />
				    
				    
				</div>
