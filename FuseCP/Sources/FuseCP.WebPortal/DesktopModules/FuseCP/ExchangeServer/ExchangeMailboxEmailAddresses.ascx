<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangeMailboxEmailAddresses.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangeMailboxEmailAddresses" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/EmailAddress.ascx" TagName="EmailAddress" TagPrefix="fcp" %>
<%@ Register Src="UserControls/MailboxTabs.ascx" TagName="MailboxTabs" TagPrefix="fcp" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>

<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/email-selection.js"></script>

				<div class="card-header">
                    <h3 class="card-title">
					<asp:Image ID="Image1" SkinID="ExchangeMailbox48" runat="server" AlternateText="Exchange mailbox" ToolTip="Exchange mailbox" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Edit Mailbox"></asp:Localize>
					-
					<asp:Literal ID="litDisplayName" runat="server" Text="John Smith" />
                        </h3>
                </div>
				<div class="card-body form-horizontal fcp-modern-page">
				<fcp:MailboxTabs id="tabs" runat="server" SelectedTab="mailbox_addresses" />
                    <div class="card tab-content">
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    
					<fieldset>
					    <legend>
					        <h3><i class="bi bi-envelope"></i> <asp:Label ID="lblAddEmail" runat="server" Text="Add New E-mail Address" meta:resourcekey="lblAddEmail" CssClass="NormalBold"></asp:Label></h3>
					    </legend>
                        <br /><br />
					   <div class="row fcp-p-20">
                           <div class="col-sm-2 fcp-lh-25">
							   <asp:Localize ID="locAccount" runat="server" meta:resourcekey="locAccount" Text="E-mail Address:"></asp:Localize>
                           </div>
                           <div class="input-group col-sm-10">
                               <fcp:EmailAddress id="email" runat="server" class="form-control" ValidationGroup="AddEmail"></fcp:EmailAddress>
                               <span class="d-flex"><asp:LinkButton id="btnAddEmail" CssClass="btn btn-success" runat="server" OnClick="btnAddEmail_Click" ValidationGroup="AddEmail"> <i class="bi bi-envelope me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddEmail"/> </asp:LinkButton></span>
                           </div>
					   </div>
					</fieldset>
					<br />
					<br />
					
					<fcp:CollapsiblePanel id="secExistingAddresses" runat="server"
                        TargetControlID="ExistingAddresses" meta:resourcekey="secExistingAddresses" Text="Existing E-mail Addresses">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="ExistingAddresses" runat="server" Height="0" style="overflow:hidden">
                        <br />
				        <asp:GridView ID="gvEmails" runat="server" AutoGenerateColumns="False"
					        EmptyDataText="gvEmails" CssSelectorClass="NormalGridView" DataKeyNames="EmailAddress">
					        <Columns>
					            <asp:TemplateField>
					                <HeaderTemplate>
					                    <asp:CheckBox ID="chkSelectAll" runat="server" onclick="checkAll(this);" />
					                </HeaderTemplate>
					                <ItemTemplate>
					                    <asp:CheckBox ID="chkSelect" runat="server" onclick="unCheckSelectAll(this);" Enabled='<%# !(bool)Eval("IsPrimary")%>' />
					                </ItemTemplate>
                                    <ItemStyle Width="10px" />
					            </asp:TemplateField>
						        <asp:TemplateField HeaderText="gvEmailAddress">
							        <ItemStyle></ItemStyle>
							        <ItemTemplate>
								        <%# Eval("EmailAddress") %>
							        </ItemTemplate>
						        </asp:TemplateField>
						        <asp:TemplateField HeaderText="gvPrimaryEmail">
							        <ItemTemplate>
							            <div>
							                &nbsp;
								            <asp:Image ID="Image1" runat="server" SkinID="Checkbox16" Visible='<%# (bool)Eval("IsPrimary")%>' AlternateText="Primary e-mail address" ToolTip="Primary e-mail address" />
								        </div>
							        </ItemTemplate>
						        </asp:TemplateField>
					        </Columns>
				        </asp:GridView>
				        
				        <br />
				        <asp:Localize ID="locTotal" runat="server" meta:resourcekey="locTotal" Text="Total E-mail Addresses:"></asp:Localize>
				        <asp:Label ID="lblTotal" runat="server" CssClass="NormalBold">1</asp:Label>
				        
					    <br />
					    <br />
				        <asp:LinkButton id="btnSetAsPrimary" CssClass="btn btn-success" runat="server"  CausesValidation="false" OnClick="btnSetAsPrimary_Click"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSetAsPrimary"/> </asp:LinkButton>&nbsp;
				        <asp:LinkButton id="btnDeleteAddresses" CssClass="btn btn-danger" runat="server" OnClick="btnDeleteAddresses_Click" CausesValidation="false"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteAddresses"/> </asp:LinkButton>
					</asp:Panel>					
					<br />
					<table class="fcp-mt-10">
					    <tr>
					        <td class="text-center">
					            <asp:CheckBox ID="chkPmmAllowed" Visible="false" runat="server" meta:resourcekey="chkPmmAllowed" AutoPostBack="true"
					                Text="Allow these settings to be managed from Personal Mailbox Manager" OnCheckedChanged="chkPmmAllowed_CheckedChanged" />
					        </td>
					    </tr>
					</table>
					</div>
				</div>
