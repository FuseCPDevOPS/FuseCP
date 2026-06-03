<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditBlackBerryUser.ascx.cs" Inherits="FuseCP.Portal.BlackBerry.EditBlackBerryUser" %>
<%@ Register Src="../ExchangeServer/UserControls/UserSelector.ascx" TagName="UserSelector"
    TagPrefix="fcp" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox"
    TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport"
    TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register src="../ExchangeServer/UserControls/MailboxSelector.ascx" tagname="MailboxSelector" tagprefix="uc1" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="../UserControls/CollapsiblePanel.ascx" %>
<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server" />
<div id="ExchangeContainer">
    <div class="Module">
        <div class="Left">
        </div>
        <div class="Content">
            <div class="Center">
                <div class="Title">
                    <asp:Image ID="Image1" SkinID="BlackBerryUsersLogo" runat="server" />
                    <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle"></asp:Localize>
                </div>
                <div class="card-body form-horizontal">
                    
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />

                    <fcp:CollapsiblePanel id="secPassword" runat="server"
                        TargetControlID="pnlSetPassword" meta:resourcekey="secPassowrd">
                    </fcp:CollapsiblePanel>
                    
                    <asp:Panel runat="server" ID="pnlSetPassword">
                    <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <table class="table table-borderless align-middle mb-0 w-100">
                        <tr>
                            <td><asp:RadioButton runat="server" ID="rbSpecifyPassword" OnCheckedChanged="rbSpecifyPassword_OnCheckedChanged" Checked="true" AutoPostBack="true" meta:resourcekey="rbSpecifyPassword" GroupName="Password"/></td>
                        </tr>
                        <tr>
                            <td><asp:RadioButton OnCheckedChanged="rbGeneratePassword_OnCheckedChanged"  AutoPostBack="true" runat="server" ID="rbGeneratePassword" meta:resourcekey="rbGeneratePassword" GroupName="Password" /></td>
                        </tr>
                    </table>
                    
                    <table runat="server" id="tblPassword" visible="true">
                        <tr>
                            <td class="FormLabel150"><asp:Localize runat="server" ID="locPassword" meta:resourcekey="locPassword"></asp:Localize></td>
                            <td><asp:TextBox runat="server" ID="txtPassword" CssClass="form-control" TextMode="Password" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator> 
                            </td>
                        </tr>
                        <tr>
                            <td  class="FormLabel150"><asp:Localize runat="server" ID="locTime" meta:resourcekey="locTime"/></td>
                            <td><asp:TextBox runat="server" ID="txtTime" CssClass="form-control" Text="48" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtTime" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:RangeValidator runa="server" ControlToValidate="txtTime"  ErrorMessage="*" Type="Integer" MinimumValue="1" MaximumValue="720" />
                                
                            </td>
                        </tr>
                    </table>                        
                        </ContentTemplate>
                    </asp:UpdatePanel>                    
                    <br />
                    <asp:LinkButton id="btnSetPassword" CssClass="btn btn-primary" runat="server" OnClick="btnSetPassword_Click"> <i class="bi bi-lock me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSetPassword"/> </asp:LinkButton>              
                    </asp:Panel>
                    <br />                    
                   
                    
                    
                    <asp:GridView runat="server" ID="dvStats" AutoGenerateColumns="False" EnableViewState="true"
					     CssSelectorClass="NormalGridView" ShowHeader="true" ShowFooter="false">
                        <Columns>
                            <asp:BoundField DataField="Name" ItemStyle-Wrap="false" />
                            <asp:BoundField DataField="Value" />
                        </Columns>
                    </asp:GridView>
                        
					<div class="card-footer text-end">
					<asp:LinkButton id="btnDelete" CssClass="btn btn-danger" runat="server" OnClick="btnDelete_Click" CausesValidation="false"> <i class="bi bi-trash me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteText"/> </asp:LinkButton>&nbsp; 
                    <asp:LinkButton id="btnDeleteData" CssClass="btn btn-danger" runat="server" OnClick="btnDeleteData_Click" CausesValidation="false"> <i class="bi bi-database me-1"></i><asp:Localize runat="server" meta:resourcekey="btnDeleteDataText"/> </asp:LinkButton>&nbsp; 
                    <asp:LinkButton id="btnSaveExit" CssClass="btn btn-success" runat="server" OnClick="btnSaveExit_Click"> <i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveExitText"/> </asp:LinkButton>
				    </div>	
                </div>
            </div>
        </div>
    </div>
</div>
