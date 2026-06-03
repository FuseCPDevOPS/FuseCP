<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreateNewBlackBerryUser.ascx.cs" Inherits="FuseCP.Portal.BlackBerry.CreateNewBlackBerryUser" %>
<%@ Register Src="../ExchangeServer/UserControls/UserSelector.ascx" TagName="UserSelector"
    TagPrefix="fcp" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox"
    TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport"
    TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register src="../ExchangeServer/UserControls/MailboxSelector.ascx" tagname="MailboxSelector" tagprefix="uc1" %>
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
                    <table id="ExistingUserTable"   runat="server"> 					    
					    <tr>
					        <td class="FormLabel150"><asp:Localize ID="Localize1" runat="server" meta:resourcekey="locDisplayName" Text="Display Name:"></asp:Localize></td>
					        <td>
                                <uc1:MailboxSelector ID="mailboxSelector" ContactsEnabled="false" ShowOnlyMailboxes="true" MailboxesEnabled="true"  DistributionListsEnabled="false" runat="server" />
                            </td>
					    </tr>
					    	    					    					    
					</table>
					
					<div class="card-footer text-end">
					    <asp:LinkButton id="btnCreate" CssClass="btn btn-success" runat="server" OnClick="btnCreate_Click"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCreateText"/> </asp:LinkButton>				    
				    </div>			
                </div>
            </div>
        </div>
    </div>
</div>
