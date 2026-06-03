<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RDSUserSessions.ascx.cs" Inherits="FuseCP.Portal.RDS.RDSUserSessions" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<%@ Register Src="UserControls/RDSCollectionTabs.ascx" TagName="CollectionTabs" TagPrefix="fcp" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>
<%@ Register Src="../UserControls/ItemButtonPanel.ascx" TagName="ItemButtonPanel" TagPrefix="fcp" %>
<script type="text/javascript" src="/JavaScript/jquery-1.4.4.min.js"></script>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>
<div class="card-header">
    <asp:Image ID="imgEditRDSCollection" SkinID="EnterpriseRDSCollections48" runat="server" />
    <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Edit RDS Collection"></asp:Localize>
    -
    <asp:Literal ID="litCollectionName" runat="server" Text="" />
</div>
<div class="card-body form-horizontal">
    <fcp:CollectionTabs id="tabs" runat="server" SelectedTab="rds_collection_user_sessions" />
    <div class="card tab-content">
        <div class="card-body form-horizontal">
            <fcp:SimpleMessageBox id="messageBox" runat="server" />  
            <asp:UpdatePanel ID="RDAppsUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <div class="FormButtonsBarCleanRight">
                        <div class="FormButtonsBarClean">
                            <asp:LinkButton id="btnRefresh" CssClass="btn btn-warning" runat="server" OnClick="btnRefresh_Click" OnClientClick="ShowProgressDialog('Loading'); return true;">
                                <i class="bi bi-arrow-clockwise me-1"></i>
                                <asp:Localize runat="server" meta:resourcekey="btnRefreshText"/>
                            </asp:LinkButton>
                            &nbsp;
                            <asp:LinkButton id="btnRecentMessages" CssClass="btn btn-primary" runat="server" OnClick="btnRecentMessages_Click" OnClientClick="ShowProgressDialog('Loading'); return true;">
                                <i class="bi bi-chat-dots me-1"></i>
                                <asp:Localize runat="server" meta:resourcekey="btnRecentMessagesText"/>
                            </asp:LinkButton>
                            &nbsp;
                            <asp:LinkButton id="btnSendMessage" CssClass="btn btn-success" runat="server" OnClick="btnSendMessage_Click">
                                <i class="bi bi-chat-left-text me-1"></i>
                                <asp:Localize runat="server" meta:resourcekey="cmdSendMessageText"/>
                            </asp:LinkButton>
                        </div>
                    </div>
                    <fcp:CollapsiblePanel id="secRdsUserSessions" runat="server" TargetControlID="panelRdsUserSessions" meta:resourcekey="secRdsUserSessions" Text=""></fcp:CollapsiblePanel>
                    <asp:Panel runat="server" ID="panelRdsUserSessions">
                        <div class="fcp-p-10">
                            <asp:GridView ID="gvRDSUserSessions" runat="server" AutoGenerateColumns="False" EnableViewState="true" EmptyDataText="No Sessions available" CssSelectorClass="NormalGridView" OnRowCommand="gvRDSCollections_RowCommand" AllowPaging="True" AllowSorting="True" meta:resourcekey="gvRDSUserSessions">
                                <Columns>
                                    <asp:TemplateField meta:resourcekey="gvUserName" HeaderText="gvUserName">
                                        <ItemStyle Wrap="false"/>
                                        <ItemTemplate>
                                            <asp:Image ID="vipImage" runat="server" ImageUrl='<%# GetAccountImage(Convert.ToBoolean(Eval("IsVip"))) %>' ImageAlign="AbsMiddle" AlternateText='<%# Convert.ToBoolean(Eval("IsVip")) ? "VIP user" : "Standard user" %>' ToolTip='<%# Convert.ToBoolean(Eval("IsVip")) ? "VIP user" : "Standard user" %>'/>
                                            <asp:Literal ID="litUserName" runat="server" Text='<%# Eval("UserName") %>'/>
                                            <asp:HiddenField ID="hfUnifiedSessionId" runat="server"  Value='<%# Eval("UnifiedSessionId") %>'/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField meta:resourcekey="gvHostServer" HeaderText="gvHostServer">
                                        <ItemStyle Wrap="false"/>
                                        <ItemTemplate>
                                            <asp:Literal ID="litHostServer" runat="server" Text='<%# Eval("HostServer") %>'/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField meta:resourcekey="gvSessionState" HeaderText="gvSessionState">
                                        <ItemStyle Wrap="false"/>
                                        <ItemTemplate>
                                            <asp:Literal ID="litSessionState" runat="server" Text='<%# Eval("SessionState") %>'/>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkViewSession" runat="server" CssClass="btn btn-primary" Text="View" CommandName="View" CommandArgument='<%# Eval("UnifiedSessionId") + ";" + Eval("HostServer") %>' meta:resourcekey="cmdViewSession" OnClientClick="ShowProgressDialog('Loading'); return true;">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkControlSession" runat="server" CssClass="btn btn-primary" Text="Control" CommandName="Control" CommandArgument='<%# Eval("UnifiedSessionId") + ";" + Eval("HostServer") %>' meta:resourcekey="cmdControlSession" OnClientClick="ShowProgressDialog('Loading'); return true;">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkLogOff" runat="server" CssClass="btn btn-danger" Text="Log Off" CommandName="LogOff" CommandArgument='<%# Eval("UnifiedSessionId") + ";" + Eval("HostServer") %>' meta:resourcekey="cmdLogOff" OnClientClick="return confirm('Are you sure you want to log off selected user?')">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkSendMessage" runat="server" CssClass="btn btn-primary" Text="Send Message" CommandName="SendMessage" CommandArgument='<%# Eval("HostServer") + ":" + Eval("UserName") + ":" + Eval("UnifiedSessionId") %>' meta:resourcekey="cmdSendMessage">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </asp:Panel>
                    <div class="text-end">
                        <fcp:ItemButtonPanel id="buttonPanel" runat="server" ValidationGroup="SaveRDSCollection" OnSaveClick="btnSave_Click" OnSaveExitClick="btnSaveExit_Click" />
                    </div>
                    <asp:Panel ID="MessagesHistoryPanel" runat="server" style="display:none">
                        <div class="widget">
                            <div class="widget-header clearfix">
                                <h3>
                                    <i class="bi bi-envelope"></i>
                                    <asp:Localize ID="headerMessagesHistory" runat="server" meta:resourcekey="headerMessagesHistory">
                                    </asp:Localize>
                                </h3>
                            </div>
                            <div class="widget-content Popup">
                                <asp:UpdatePanel ID="MessagesHistoryUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                                    <ContentTemplate>
                                        <div class="Popup-Scroll">
                                            <asp:GridView ID="gvMessagesHistory" runat="server" meta:resourcekey="gvMessagesHistory" AutoGenerateColumns="False" CssSelectorClass="NormalGridView" DataKeyNames="Id">
                                                <Columns>
                                                    <asp:TemplateField meta:resourcekey="gvMessageText">
                                                        <ItemStyle/>
                                                        <ItemTemplate>
                                                            <asp:Literal ID="litMessage" runat="server" Text='<%# Eval("MessageText") %>'></asp:Literal>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField meta:resourcekey="gvUser" HeaderText="gvUser">
                                                        <ItemStyle Wrap="false"/>
                                                        <ItemTemplate>
                                                            <asp:Literal ID="litUserName" runat="server" Text='<%# Eval("UserName") %>'/>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litDate" runat="server" Text='<%# Convert.ToDateTime(Eval("Date")).ToShortDateString() %>'/>
                                                    </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="popup-buttons text-end">
                                <asp:LinkButton id="btnCancelMessagesHistory" CssClass="btn btn-warning" runat="server" CausesValidation="False">
                                    <i class="bi bi-x-lg me-1"></i>
                                    <asp:Localize runat="server" meta:resourcekey="btnCancelText"/>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="EnterMessagePanel" runat="server" style="display:none">
                        <div class="widget">
                            <div class="widget-header clearfix">
                                <h3>
                                    <i class="bi bi-pencil"></i>
                                    <asp:Localize ID="headerEnterMessage" runat="server" meta:resourcekey="headerEnterMessage"></asp:Localize>
                                </h3>
                            </div>
                            <div class="widget-content Popup">
                                <asp:TextBox id="txtMessage" TextMode="multiline" Columns="70" Rows="15" runat="server" CssClass="form-control" />
                            </div>
                            <div class="popup-buttons text-end">
                                <asp:LinkButton id="btnAddMessage" CssClass="btn btn-success" runat="server" OnClick="btnAddMessage_Click" CausesValidation="false">
                                    <i class="bi bi-check-lg me-1"></i>
                                    <asp:Localize runat="server" meta:resourcekey="btnAddText"/>
                                </asp:LinkButton>
                                &nbsp;
                                <asp:LinkButton id="btnCancelEnterMessage" CssClass="btn btn-warning" runat="server" CausesValidation="False">
                                    <i class="bi bi-x-lg me-1"></i>
                                    <asp:Localize runat="server" meta:resourcekey="btnCancelText"/>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Button ID="btnEnterMessageFake" runat="server" style="display:none" />
                    <ajaxToolkit:ModalPopupExtender ID="EnterMessageModal" runat="server" TargetControlID="btnEnterMessageFake" PopupControlID="EnterMessagePanel" BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelEnterMessage"/>
                    <asp:Button ID="btnMessagesHistoryFake" runat="server" style="display:none" />
                    <ajaxToolkit:ModalPopupExtender ID="MessagesHistoryModal" runat="server" TargetControlID="btnMessagesHistoryFake" PopupControlID="MessagesHistoryPanel" BackgroundCssClass="modalBackground" DropShadow="false" CancelControlID="btnCancelMessagesHistory"/>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</div>
