<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangePublicFolderEmailAddresses.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangePublicFolderEmailAddresses" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/EmailAddress.ascx" TagName="EmailAddress" TagPrefix="fcp" %>
<%@ Register Src="UserControls/PublicFolderTabs.ascx" TagName="PublicFolderTabs" TagPrefix="fcp" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>

<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/email-selection.js"></script>


				<div class="card-header">
                    <h3 class="card-title">
                    <asp:Image ID="Image1" SkinID="ExchangePublicFolder48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Edit Public Folder"></asp:Localize>
					-
					<asp:Literal ID="litDisplayName" runat="server" Text="John Smith" />
                    </h3>
                </div>
				<div class="card-body form-horizontal fcp-modern-page">
				<fcp:PublicFolderTabs id="tabs" runat="server" SelectedTab="public_folder_addresses" />
                    <div class="card tab-content">
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    
					<fieldset>
					    <legend>
					       <h3><i class="bi bi-envelope"></i>  <asp:Label ID="lblAddEmail" runat="server" Text="Add New E-mail Address" meta:resourcekey="lblAddEmail" CssClass="NormalBold"></asp:Label></h3>
					    </legend>
					    <div class="row fcp-p-20">
                           <div class="col-sm-2 fcp-lh-25">
							   <asp:Localize ID="locAccount" runat="server" meta:resourcekey="locAccount" Text="E-mail Address:"></asp:Localize></td>
						   </div>
                           <div class="input-group col-sm-10">
									<fcp:EmailAddress id="email" runat="server" ValidationGroup="AddEmail"></fcp:EmailAddress>
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
					                    <asp:CheckBox ID="chkSelect" runat="server" onclick="unCheckSelectAll(this);" Enabled='<%# !(bool)Eval("IsPrimary") %>' />
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
							            <div class="text-center">
							                &nbsp;
								            <asp:Image ID="Image2" runat="server" SkinID="Checkbox16" Visible='<%# Eval("IsPrimary") %>' />
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
                        <asp:LinkButton id="btnDeleteAddresses" CssClass="btn btn-danger" runat="server" OnClick="btnDeleteAddresses_Click" CausesValidation="false"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteAddresses"/> </asp:LinkButton>&nbsp;
				        <asp:LinkButton id="btnSetAsPrimary" CssClass="btn btn-success" runat="server"  CausesValidation="false" OnClick="btnSetAsPrimary_Click"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSetAsPrimary"/> </asp:LinkButton>
					</asp:Panel>					
					<br />
					<br />
                        </div>
				</div>
