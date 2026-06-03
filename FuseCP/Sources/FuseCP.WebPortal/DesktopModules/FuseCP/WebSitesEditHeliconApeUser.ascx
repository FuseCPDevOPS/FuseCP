<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebSitesEditHeliconApeUser.ascx.cs" Inherits="FuseCP.Portal.WebSitesEditHeliconApeUser" %>
<%@ Register Src="UserControls/UsernameControl.ascx" TagName="UsernameControl" TagPrefix="uc3" %>
<%@ Register Src="UserControls/PasswordControl.ascx" TagName="PasswordControl" TagPrefix="uc2" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>
<script src="/JavaScript/jquery-1.4.4.min.js" type="text/javascript">

</script>
<script src="/DesktopModules/FuseCP/Scripts/helicon-ape-user.js" type="text/javascript"></script>
<div class="card-body form-horizontal">
<table class="table table-borderless align-middle mb-0 w-100">
	<tr>
		<td>
            <table class="table table-borderless align-middle mb-0 w-100">
	            <tr>
		            <td class="SubHead" >
						<asp:Label ID="lblUserName" runat="server" meta:resourcekey="lblUserName" Text="User Name:"></asp:Label>
					</td>
		            <td class="NormalBold">
                        <uc3:UsernameControl ID="usernameControl" runat="server" />
                    </td>
	            </tr>
	            <tr>
		            <td class="SubHead align-top">
						<asp:Label ID="lblAuthType" runat="server" meta:resourcekey="lblAuthType" Text="Auth Type"></asp:Label>
					</td>
		            <td>
                        <asp:RadioButtonList ID="rblAuthType" runat="server" CssClass="AuthType"></asp:RadioButtonList>
                    </td>
	            </tr>
	            <tr class="EncType">
		            <td class="SubHead align-top">
						<asp:Label ID="lblEncType" runat="server" meta:resourcekey="lblEncType" Text="Encryption Type"></asp:Label>
					</td>
		            <td>
                        <asp:RadioButtonList ID="rblEncType" runat="server"></asp:RadioButtonList>
                    </td>
	            </tr>
	            <tr class="DigestRealm" style="display: none">
		            <td class="SubHead align-top">
						<asp:Label ID="lblDigestRealm" runat="server" meta:resourcekey="lblDigestRealm" Text="Digest Realm"></asp:Label>
					</td>
		            <td>
                        <asp:TextBox ID="tbDigestRealm" runat="server"></asp:TextBox>
                    </td>
	            </tr>
	            <tr>
		            <td class="SubHead align-top">
                        <asp:Label ID="lblUserPassword" runat="server" meta:resourcekey="lblUserPassword" Text="User Password:"></asp:Label></td>
		            <td class="Normal align-top">
                        <uc2:PasswordControl ID="passwordControl" runat="server" />
		            </td>
	            </tr>
            </table>
            
            <fcp:CollapsiblePanel id="secGroups" runat="server"
                TargetControlID="GroupsPanel" meta:resourcekey="secGroups" Text="Member Of">
            </fcp:CollapsiblePanel>
	        <asp:Panel ID="GroupsPanel" runat="server" Height="0" style="overflow:hidden">
                <table class="table table-borderless align-middle mb-0 w-100" id="tblGroups" runat="server">
	                <tr>
		                <td colspan="2">
			                <asp:checkboxlist id="dlGroups" RepeatColumns="2" CssClass="NormalBold" DataTextField="Name"
				                DataValueField="Name" Runat="server"></asp:checkboxlist>
		                </td>
	                </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click" > <i class="bi bi-arrow-clockwise me-1"></i><asp:Localize runat="server" meta:resourcekey="btnUpdate"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
    <asp:LinkButton id="btnSaveAndAddAnother" CssClass="btn btn-success" runat="server" OnClick="btnSaveAndAddAnother_Click"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveAndAddAnother"/> </asp:LinkButton>
</div>

