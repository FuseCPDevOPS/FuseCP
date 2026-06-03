<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserAccountSummaryLetter.ascx.cs" Inherits="FuseCP.Portal.UserAccountSummaryLetter" %>
<%@ Register TagPrefix="fcp" TagName="CollapsiblePanel" Src="UserControls/CollapsiblePanel.ascx" %>

<div class="card-body form-horizontal">

    <fcp:CollapsiblePanel id="secEmail" runat="server"
        TargetControlID="EmailPanel" meta:resourcekey="secEmail" Text="Send via E-Mail">
    </fcp:CollapsiblePanel>
	<asp:Panel ID="EmailPanel" runat="server" Height="0" style="overflow:hidden">
        <table class="table table-borderless align-middle mb-0" id="tblEmail" runat="server">
            <tr>
                <td class="SubHead text-nowrap">
                    <asp:Label ID="lblTo" runat="server" meta:resourcekey="lblTo" Text="To:"></asp:Label>
                </td>
                <td class="Normal">
                    <asp:TextBox ID="txtTo" runat="server" CssClass="form-control" Width="300px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valRequireEmail" runat="server" ControlToValidate="txtTo" Display="Dynamic"
                        ErrorMessage="Enter e-mail" ValidationGroup="SendEmail" meta:resourcekey="valRequireEmail"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="SubHead">
                    <asp:Label ID="lblCC" runat="server" meta:resourcekey="lblCC" Text="CC:"></asp:Label>
                </td>
                <td class="Normal">
                    <asp:TextBox ID="txtCC" runat="server" CssClass="form-control" Width="300px"></asp:TextBox></td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Button ID="btnSend" runat="server" CssClass="btn btn-success" meta:resourcekey="btnSend" Text="Send" OnClick="btnSend_Click" ValidationGroup="SendEmail" /></td>
            </tr>
        </table>
    </asp:Panel>

    <div class="PreviewArea">
        <asp:Literal ID="litContent" runat="server"></asp:Literal>
    </div>

</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnReturn" CssClass="btn btn-warning" runat="server" OnClick="btnReturn_Click" CausesValidation="False"> <i class="bi bi-arrow-left me-1"></i><asp:Localize runat="server" meta:resourcekey="btnReturnText"/> </asp:LinkButton>
</div>
