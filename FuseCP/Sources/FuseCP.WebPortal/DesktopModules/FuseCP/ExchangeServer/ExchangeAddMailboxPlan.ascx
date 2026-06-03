<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangeAddMailboxPlan.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangeAddMailboxPlan" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/SizeBox.ascx" TagName="SizeBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/DaysBox.ascx" TagName="DaysBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/CollapsiblePanel.ascx" TagName="CollapsiblePanel" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaEditor.ascx" TagName="QuotaEditor" TagPrefix="uc1" %>

<%@ Import Namespace="FuseCP.Portal" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

				<div class="card-header">
                    <h3 class="card-title">
					<asp:Image ID="Image1" SkinID="ExchangeMailboxPlans48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" Text="Add Mailboxplan"></asp:Localize>
				</h3>
                        </div>
				<div class="card-body form-horizontal">
				    <fcp:SimpleMessageBox id="messageBox" runat="server" />

                    <asp:HiddenField runat="server" ID="hfArchivingPlan" />

					<fcp:CollapsiblePanel id="secMailboxPlan" runat="server"
                        TargetControlID="MailboxPlan" meta:resourcekey="secMailboxPlan" Text="Mailboxplan">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="MailboxPlan" runat="server" Height="0" style="overflow:hidden">
					    <table>
						    <tr>
							    <td class="FormLabel200 text-end">
									
								</td>
							    <td>
									<asp:TextBox ID="txtMailboxPlan" runat="server" CssClass="form-control" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="valRequireMailboxPlan" runat="server" meta:resourcekey="valRequireMailboxPlan" ControlToValidate="txtMailboxPlan"
									ErrorMessage="Enter name" ValidationGroup="CreateMailboxPlan" Display="Dynamic" Text="*" SetFocusOnError="True"></asp:RequiredFieldValidator>
								</td>
						    </tr>
					    </table>
					    <br />
					</asp:Panel>

					<fcp:CollapsiblePanel id="secMailboxFeatures" runat="server"
                        TargetControlID="MailboxFeatures" meta:resourcekey="secMailboxFeatures" Text="Mailbox Features">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="MailboxFeatures" runat="server" Height="0" style="overflow:hidden">
					    <table>
						    <tr>
							    <td>
								    <asp:CheckBox ID="chkPOP3" runat="server" meta:resourcekey="chkPOP3" Text="POP3"></asp:CheckBox>
							    </td>
						    </tr>
						    <tr>
							    <td>
								    <asp:CheckBox ID="chkIMAP" runat="server" meta:resourcekey="chkIMAP" Text="IMAP"></asp:CheckBox>
							    </td>
						    </tr>
						    <tr>
							    <td>
								    <asp:CheckBox ID="chkOWA" runat="server" meta:resourcekey="chkOWA" Text="OWA/HTTP"></asp:CheckBox>
							    </td>
						    </tr>
						    <tr>
							    <td>
								    <asp:CheckBox ID="chkMAPI" runat="server" meta:resourcekey="chkMAPI" Text="MAPI"></asp:CheckBox>
							    </td>
						    </tr>
						    <tr>
							    <td>
								    <asp:CheckBox ID="chkActiveSync" runat="server" meta:resourcekey="chkActiveSync" Text="ActiveSync"></asp:CheckBox>
							    </td>
						    </tr>
						</table>
						<br />
					</asp:Panel>

					<fcp:CollapsiblePanel id="secMailboxGeneral" runat="server"
                        TargetControlID="MailboxGeneral" meta:resourcekey="secMailboxGeneral" Text="Mailbox General">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="MailboxGeneral" runat="server" Height="0" style="overflow:hidden">
					    <table>
						    <tr>
							    <td>
								    <asp:CheckBox ID="chkHideFromAddressBook" runat="server" meta:resourcekey="chkHideFromAddressBook" Text="Hide from Addressbook"></asp:CheckBox>
							    </td>
						    </tr>
                            <tr>
							    <td>
								    <asp:CheckBox ID="chkIsForJournaling" runat="server" meta:resourcekey="chkIsForJournaling" Text="For journaling mailboxes only" Checked="false"></asp:CheckBox>
							    </td>
						    </tr>
						</table>
					</asp:Panel>
				
					<fcp:CollapsiblePanel id="secStorageQuotas" runat="server"
                        TargetControlID="StorageQuotas" meta:resourcekey="secStorageQuotas" Text="Storage Quotas">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="StorageQuotas" runat="server" Height="0" style="overflow:hidden">
						<table>
                            <tr>
								<td class="FormLabel200 text-end">
                                    <asp:Localize ID="locAutoReplyEnabled" runat="server" meta:resourcekey="locAutoReplyEnabled" Text="Automatic Replies via FuseCP"></asp:Localize>
								</td>
                                <td class="fcp-ps-5">
                                    <asp:CheckBox ID="chkAutoReplyEnabled" runat="server" Text="" />
                                </td>
							</tr>
							<tr>
								<td class="FormLabel200 text-end"><asp:Localize ID="locMailboxSize" runat="server" meta:resourcekey="locMailboxSize" Text="Mailbox size:"></asp:Localize></td>
								<td class="fcp-ps-5">
                                        <uc1:QuotaEditor id="mailboxSize" runat="server"
                                            QuotaTypeID="2"
                                            QuotaValue="0"
                                            ParentQuotaValue="-1">
                                        </uc1:QuotaEditor>
								</td>
							</tr>
							<tr>
								<td class="FormLabel200 text-end"><asp:Localize ID="locMaxRecipients" runat="server" meta:resourcekey="locMaxRecipients" Text="Maximum Recipients:"></asp:Localize></td>
								<td class="fcp-ps-5">
                                        <uc1:QuotaEditor id="maxRecipients" runat="server"
                                            QuotaTypeID="2"
                                            QuotaValue="0"
                                            ParentQuotaValue="-1">
                                        </uc1:QuotaEditor>
								</td>
							</tr>
							<tr>
								<td class="FormLabel200 text-end"><asp:Localize ID="locMaxSendMessageSizeKB" runat="server" meta:resourcekey="locMaxSendMessageSizeKB" Text="Maximum Send Message Size (Kb):"></asp:Localize></td>
								<td class="fcp-ps-5">
                                        <uc1:QuotaEditor id="maxSendMessageSizeKB" runat="server"
                                            QuotaTypeID="2"
                                            QuotaValue="0"
                                            ParentQuotaValue="-1">
                                        </uc1:QuotaEditor>
								</td>
							</tr>
							<tr>
								<td class="FormLabel200 text-end"><asp:Localize ID="locMaxReceiveMessageSizeKB" runat="server" meta:resourcekey="locMaxReceiveMessageSizeKB" Text="Maximum Receive Message Size (Kb):"></asp:Localize></td>
								<td class="fcp-ps-5">
                                        <uc1:QuotaEditor id="maxReceiveMessageSizeKB" runat="server"
                                            QuotaTypeID="2"
                                            QuotaValue="0"
                                            ParentQuotaValue="-1">
                                        </uc1:QuotaEditor>
								</td>
							</tr>
                            <tr>
								<td class="fcp-pb-20"></td>
                            </tr>
							<tr>
								<td class="FormLabel200" colspan="2"><asp:Localize ID="locWhenSizeExceeds" runat="server" meta:resourcekey="locWhenSizeExceeds" Text="When the mailbox size exceeds the indicated amount:"></asp:Localize></td>
							</tr>
							<tr>
								<td class="FormLabel200 text-end"><asp:Localize ID="locIssueWarning" runat="server" meta:resourcekey="locIssueWarning" Text="Issue warning at:"></asp:Localize></td>
								<td class="fcp-ps-5">
									<fcp:SizeBox id="sizeIssueWarning" runat="server" ValidationGroup="CreateMailboxPlan" DisplayUnitsKB="false" DisplayUnitsMB="false" DisplayUnitsPct="true" RequireValidatorEnabled="true"/>
								</td>
							</tr>
							<tr>
								<td class="FormLabel200 text-end"><asp:Localize ID="locProhibitSend" runat="server" meta:resourcekey="locProhibitSend" Text="Prohibit send at:"></asp:Localize></td>
								<td class="fcp-ps-5">
									<fcp:SizeBox id="sizeProhibitSend" runat="server" ValidationGroup="CreateMailboxPlan"  DisplayUnitsKB="false" DisplayUnitsMB="false" DisplayUnitsPct="true" RequireValidatorEnabled="true"/>
								</td>
							</tr>
							<tr>
								<td class="FormLabel200 text-end"><asp:Localize ID="locProhibitSendReceive" runat="server" meta:resourcekey="locProhibitSendReceive" Text="Prohibit send and receive at:"></asp:Localize></td>
								<td class="fcp-ps-5">
									<fcp:SizeBox id="sizeProhibitSendReceive" runat="server" ValidationGroup="CreateMailboxPlan" DisplayUnitsKB=false DisplayUnitsMB="false" DisplayUnitsPct="true" RequireValidatorEnabled="true"/>
								</td>
							</tr>
						</table>
					</asp:Panel>
					
					<fcp:CollapsiblePanel id="secDeleteRetention" runat="server"
                        TargetControlID="DeleteRetention" meta:resourcekey="secDeleteRetention" Text="Delete Item Retention">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="DeleteRetention" runat="server" Height="0" style="overflow:hidden">
						<table>
							<tr>
								<td class="FormLabel200 text-end"><asp:Localize ID="locKeepDeletedItems" runat="server" meta:resourcekey="locKeepDeletedItems" Text="Keep deleted items for:"></asp:Localize></td>
								<td class="fcp-ps-5">
									<fcp:DaysBox id="daysKeepDeletedItems" runat="server" ValidationGroup="CreateMailboxPlan" />
								</td>
							</tr>
						</table>
					</asp:Panel>

					<fcp:CollapsiblePanel id="secLitigationHold" runat="server"
                        TargetControlID="LitigationHold" meta:resourcekey="secLitigationHold" Text="LitigationHold">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="LitigationHold" runat="server" Height="0" style="overflow:hidden">
						<table>
						    <tr>
							    <td>
								    <asp:CheckBox ID="chkEnableLitigationHold" runat="server" meta:resourcekey="chkEnableLitigationHold" Text="Enabled Litigation Hold"></asp:CheckBox>
							    </td>
						    </tr>
							<tr>
								<td class="FormLabel200 text-end"><asp:Localize ID="locRecoverableItemsSpace" runat="server" meta:resourcekey="locRecoverableItemsSpace" Text="Recoverable Items Space (MB):"></asp:Localize></td>
								<td class="fcp-ps-5">
                                        <uc1:QuotaEditor id="recoverableItemsSpace" runat="server"
                                            QuotaTypeID="2"
                                            QuotaValue="6144"
                                            QuotaMinValue="6144"
                                            QuotaMaxValue="-1"
                                            ParentQuotaValue="-1">
                                        </uc1:QuotaEditor>
								</td>
							</tr>
							<tr>
								<td class="FormLabel200 text-end"><asp:Localize ID="locRecoverableItemsWarning" runat="server" meta:resourcekey="locRecoverableItemsWarning" Text="Issue warning at:"></asp:Localize></td>
								<td class="fcp-ps-5">
									<fcp:SizeBox id="recoverableItemsWarning" runat="server" ValidationGroup="CreateMailboxPlan" DisplayUnitsKB="false" DisplayUnitsMB="false" DisplayUnitsPct="true" RequireValidatorEnabled="true"/>
								</td>
							</tr>
                            <tr>
                                <td class="SubHead" ><asp:Label ID="lblLitigationHoldUrl" runat="server" meta:resourcekey="lblLitigationHoldUrl" Text="Url:"></asp:Label></td>
                                <td class="Normal fcp-ps-5" >
                                    <asp:TextBox ID="txtLitigationHoldUrl" runat="server" CssClass="form-control" MaxLength="255"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="SubHead align-top"><asp:Label ID="lblLitigationHoldMsg" runat="server" meta:resourcekey="lblLitigationHoldMsg" Text="Page Content:"></asp:Label></td>
                                <td class="Normal align-top fcp-ps-5" >
                                    <asp:TextBox ID="txtLitigationHoldMsg" runat="server" Rows="10" TextMode="MultiLine" CssClass="form-control" Wrap="False" MaxLength="511"></asp:TextBox></td>
                            </tr>

						</table>
					</asp:Panel>

		            <fcp:CollapsiblePanel id="secArchiving" runat="server"
                        TargetControlID="Archiving" meta:resourcekey="secArchiving" Text="Archiving">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="Archiving" runat="server" Height="0" style="overflow:hidden">
			            <table>
				            <tr>
					            <td class="FormLabel200">
						            <asp:CheckBox ID="chkEnableArchiving" runat="server" meta:resourcekey="chkEnableArchiving" Text="Archiving"></asp:CheckBox>
					            </td>
                                <td></td>
				            </tr>
				            <tr>
					            <td class="FormLabel200 text-end"><asp:Localize ID="locArchiveQuota" runat="server" meta:resourcekey="locArchiveQuota" Text="Archive quota:"></asp:Localize></td>
					            <td class="fcp-ps-5">
                                    <div class="Right">
                                        <uc1:QuotaEditor id="archiveQuota" runat="server"
                                            QuotaTypeID="2"
                                            QuotaValue="0"
                                            ParentQuotaValue="-1">
                                        </uc1:QuotaEditor>
                                    </div>
					            </td>
				            </tr>
				            <tr>
					            <td class="FormLabel200 text-end"><asp:Localize ID="locArchiveWarningQuota" runat="server" meta:resourcekey="locArchiveWarningQuota" Text="Archive warning quota:"></asp:Localize></td>
					            <td class="fcp-ps-5">
						            <fcp:SizeBox id="archiveWarningQuota" runat="server" DisplayUnitsKB="false" DisplayUnitsMB="false" DisplayUnitsPct="true" RequireValidatorEnabled="true"/>
					            </td>
				            </tr>
                            <tr>
					            <td class="FormLabel200" colspan="2">
						            <asp:CheckBox ID="chkEnableForceArchiveDeletion" runat="server" meta:resourcekey="chkEnableForceArchiveDeletion" Text="Force Archive on Mailbox Deletion"></asp:CheckBox>
					            </td>
				            </tr>
			            </table>
			            <br />
		            </asp:Panel>


                    <fcp:CollapsiblePanel id="secRetentionPolicyTags" runat="server"
                        TargetControlID="RetentionPolicyTags" meta:resourcekey="secRetentionPolicyTags" Text="Retention policy tags">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="RetentionPolicyTags" runat="server" Height="0" style="overflow:hidden">
                        <asp:UpdatePanel ID="GeneralUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>
                            <asp:GridView id="gvPolicy" runat="server"  EnableViewState="true" AutoGenerateColumns="false"
		                    EmptyDataText="" CssSelectorClass="NormalGridView" OnRowCommand="gvPolicy_RowCommand" >
		                    <Columns>
			                    <asp:TemplateField HeaderText="Tag">
				                    <ItemStyle></ItemStyle>
				                    <ItemTemplate>
					                    <asp:Label id="displayPolicy" runat="server" EnableViewState="true" ><%# PortalAntiXSS.Encode((string)Eval("TagName"))%></asp:Label>
                                    </ItemTemplate>
			                    </asp:TemplateField>
                                <asp:TemplateField>
				                    <ItemTemplate>
					                    <asp:LinkButton id="imgDelPolicy" CssClass="btn btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("TagId") %>' OnClientClick="return confirm('Are you sure you want to delete selected policy tag?')"> <i class="bi bi-trash"></i> </asp:LinkButton>
				                    </ItemTemplate>
			                    </asp:TemplateField>
		                    </Columns>
	                        </asp:GridView>
                            <br />

                            <asp:DropDownList ID="ddTags" runat ="server"></asp:DropDownList>
                            <asp:Button ID="bntAddTag" runat="server" Text="Add tag" CssClass="btn btn-primary" meta:resourcekey="bntAddTag" OnClick="bntAddTag_Click" CausesValidation="false"/>
                            <br />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </asp:Panel>

				
				</div>
				    <div class="card-footer text-end">
					    <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click" ValidationGroup="CreateMailboxPlan" OnClientClick="ShowProgressDialog('Creating ...');"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddText"/> </asp:LinkButton>&nbsp;
					    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" ValidationGroup="CreateMailboxPlan" />
				    </div>
