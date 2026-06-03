<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangeMailboxMobile.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangeMailboxMobile" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/MailboxTabs.ascx" TagName="MailboxTabs" TagPrefix="fcp" %>



				<div class="card-header">
                    <h3 class="card-title">
					<asp:Image ID="Image1" SkinID="ExchangeMailbox48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Edit Mailbox"></asp:Localize>
					-
					<asp:Literal ID="litDisplayName" runat="server" Text="John Smith" />
                        </h3>
                </div>
				<div class="card-body form-horizontal fcp-modern-page">
				<fcp:MailboxTabs id="tabs" runat="server" SelectedTab="mailbox_mobile" />
                    <div class="card tab-content">
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    
                    <asp:GridView ID="gvMobile" runat="server" AutoGenerateColumns="False" EnableViewState="true"
					    EmptyDataText="gvUsers" CssSelectorClass="NormalGridView"
					    AllowSorting="False" onrowcommand="gvMobile_RowCommand" 
                        onrowdatabound="gvMobile_RowDataBound" meta:resourcekey="gvMobile" 
                        onrowdeleting="gvMobile_RowDeleting" onrowediting="gvMobile_RowEditing">
					    <Columns>						     						   						    
						    <asp:TemplateField HeaderText=""  SortExpression="DeviceUserAgent" meta:resourcekey="deviceUserAgentColumn" >
							    <ItemStyle></ItemStyle>
							    <ItemTemplate>							       
								    <asp:hyperlink id="lnkDeviceUserAgent" runat="server" NavigateUrl='<%# GetEditUrl(Eval("Id").ToString()) %>' >
									    <%# Eval("DeviceUserAgent") %>
								    </asp:hyperlink>
							    </ItemTemplate>
						    </asp:TemplateField>
						    <asp:BoundField HeaderText="" DataField="DeviceType" meta:resourcekey="deviceTypeColumn" />
						    	   
						    <asp:TemplateField HeaderText="" SortExpression="LastSuccessSync" meta:resourcekey="lastSyncTimeColumn" >
							    <ItemStyle Wrap="false"></ItemStyle>
							    <ItemTemplate>							       
								    <asp:Label runat="server" ID="lblLastSyncTime" Text='<%# Eval("LastSuccessSync") %>' />								    
							    </ItemTemplate>
						    </asp:TemplateField>
						    
						   <asp:TemplateField HeaderText="" SortExpression="Status"  meta:resourcekey="deviceStatus" >
							    <ItemStyle  Wrap="false"></ItemStyle>
							    <ItemTemplate>							       
								     <asp:Label runat="server" ID="lblStatus" Text='<%# Eval("Status") %>' />								    
							    </ItemTemplate>
						    </asp:TemplateField>
						    
						    <asp:TemplateField>
							    <ItemTemplate>
								    <asp:LinkButton id="cmdDelete" CssClass="btn btn-danger" runat="server" CommandName="Delete" CommandArgument='<%# Eval("ID") %>' OnClientClick="return confirm('Remove this item?');"> <i class="bi bi-trash"></i> </asp:LinkButton>
							    </ItemTemplate>
						    </asp:TemplateField>
					    </Columns>
				    </asp:GridView>
					

                        </div>
				</div>
				    <div class="card-footer text-end">
					   
				    </div>
