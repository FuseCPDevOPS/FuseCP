<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangeDomainNames.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangeDomainNames" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

				<div class="card-header">
                    <h3 class="card-title">
					<asp:Image ID="Image1" SkinID="ExchangeDomainName48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Domain Names"></asp:Localize>
                    </h3>
				</div>
                    <div class="FormButtonsBar right">
                        <asp:LinkButton id="btnAddDomain" CssClass="btn btn-primary" runat="server" OnClick="btnAddDomain_Click"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddDomain"/> </asp:LinkButton>
                    </div>
				    <fcp:SimpleMessageBox id="messageBox" runat="server" />
				    <asp:GridView ID="gvDomains" runat="server" AutoGenerateColumns="False" EnableViewState="true"
					    EmptyDataText="gvDomains" CssSelectorClass="NormalGridView" OnRowCommand="gvDomains_RowCommand">
					    <Columns>
						    <asp:TemplateField SortExpression="DomainName" HeaderText="gvDomainsName">
							    <ItemStyle></ItemStyle>
							    <ItemTemplate>
								    <asp:hyperlink id="lnkEditZone" runat="server" EnableViewState="false"
									    NavigateUrl='<%# GetDomainRecordsEditUrl(Eval("DomainID").ToString()) %>' Enabled="true">
									    <%# Eval("DomainName") %>
								    </asp:hyperlink>
							    </ItemTemplate>
						    </asp:TemplateField>
                            <asp:TemplateField HeaderText="gvDomainsType">
							    <ItemTemplate>
							        <div class="text-center">
								        <asp:Label ID="Label1" Text='<%# Eval("DomainType") %>' runat="server"/>
								    </div>
							    </ItemTemplate>
						    </asp:TemplateField>
                             <asp:TemplateField HeaderText="gvDomainsTypeChange">
							    <ItemTemplate>
							        <div class="text-center">
								        <asp:Button ID="btnChangeDomain" text="Change" meta:resourcekey="btnChangeDomain" CssClass="btn btn-secondary btn-sm" runat="server" CommandName="Change" CommandArgument='<%# Eval("DomainId") + "|" + Eval("DomainType") %>'/>
								    </div>
							    </ItemTemplate>
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="gvDomainsDefault">
							    <ItemTemplate>
							        <div class="text-center">
								        <input type="radio" name="DefaultDomain" value='<%# Eval("DomainId") %>' <%# IsChecked((bool)Eval("IsDefault")) %> />
								    </div>
							    </ItemTemplate>
						    </asp:TemplateField>                            
						    <asp:TemplateField>
							    <ItemTemplate>
									<asp:LinkButton id="imgDelDomain" CssClass="btn btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("DomainId") %>' Visible='<%# ((!(bool)Eval("IsDefault"))) && (!CheckDomainUsedByHostedOrganization(Eval("DomainID").ToString())) %>' OnClientClick="return confirm('Are you sure you want to delete selected domain?')"> <i class="bi bi-trash"></i> </asp:LinkButton>&nbsp;
                                    <asp:LinkButton ID="lnkViewUsage" CssClass="btn btn-primary" runat="server" Text="View Usage" Visible='<%# CheckDomainUsedByHostedOrganization(Eval("DomainID").ToString()) %>'
                                        CommandName="ViewUsage" CommandArgument='<%# Eval("DomainId") %>'
                                         />
							    </ItemTemplate>
						    </asp:TemplateField>
					    </Columns>
				    </asp:GridView>
				    <div class="card-footer">
				   <div class="row">
                    <div class="col-md-6">
				 
				    <asp:Localize ID="locQuota" runat="server" meta:resourcekey="locQuota" Text="Total Domains Used:"></asp:Localize>
				    &nbsp;&nbsp;&nbsp;
				    <fcp:QuotaViewer ID="domainsQuota" runat="server" QuotaTypeId="2" />
</div>
                         <div class="col-md-6 text-end">
				        <asp:LinkButton id="btnSetDefaultDomain" CssClass="btn btn-success" runat="server"  CausesValidation="false" OnClick="btnSetDefaultDomain_Click"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSetDefaultDomain"/> </asp:LinkButton>&nbsp;
                    </div>
                       </div>
				</div>
