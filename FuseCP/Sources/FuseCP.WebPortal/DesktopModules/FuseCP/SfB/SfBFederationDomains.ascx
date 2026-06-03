<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SfBFederationDomains.ascx.cs" Inherits="FuseCP.Portal.SfB.SfBFederationDomains" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>


				<div class="card-header">
					<asp:Image ID="Image1" SkinID="SfBFederationDomains48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Domain Names"></asp:Localize>
				</div>
  <div class="FormButtonsBar right">
                        <asp:LinkButton id="btnAddDomain" CssClass="btn btn-success" runat="server" OnClick="btnAddDomain_Click"> <i class="bi bi-globe me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddDomainText"/> </asp:LinkButton>
                    </div>
			
				    <fcp:SimpleMessageBox id="messageBox" runat="server" />
				    
                  

				    <asp:GridView ID="gvDomains" runat="server" AutoGenerateColumns="False" EnableViewState="true"
					    EmptyDataText="gvDomains" CssSelectorClass="NormalGridView" OnRowCommand="gvDomains_RowCommand">
					    <Columns>
						    <asp:TemplateField HeaderText="gvDomainsName">
							    <ItemStyle></ItemStyle>
							    <ItemTemplate>
								    <asp:hyperlink id="lnkEditZone" runat="server" EnableViewState="false">
									    <%# Eval("DomainName") %>
								    </asp:hyperlink>
							    </ItemTemplate>
						    </asp:TemplateField>
						    <asp:TemplateField>
							    <ItemTemplate>
									<asp:LinkButton id="imgDelDomain" CssClass="btn btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("DomainName") %>' OnClientClick="return confirm('Are you sure you want to delete selected domain?')"> <i class="bi bi-trash"></i> </asp:LinkButton>
							    </ItemTemplate>
						    </asp:TemplateField>
					    </Columns>
				    </asp:GridView>
