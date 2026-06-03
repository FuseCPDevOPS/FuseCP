<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApplyEnableHardQuotaFeature.ascx.cs" Inherits="FuseCP.Portal.ApplyEnableHardQuotaFeature" %>
<%@ Register Src="UserControls/SimpleMessageBox.ascx" TagName="messageBox" TagPrefix="uc4" %>

<div class="card-body form-horizontal">
    <table class="table table-borderless mb-0 w-100">
        <tr>

            <td class="Normal">
                <uc4:messageBox id="messageBox" runat="server" >                   
                </uc4:messageBox>
            </td>
        </tr>
    </table>
    
</div>
<div class="card-footer text-end">
    <asp:LinkButton id="btnUpdate" CssClass="btn btn-success" runat="server" OnClick="btnUpdate_Click"> <i class="bi bi-arrow-clockwise me-1"></i><asp:Localize runat="server" meta:resourcekey="btnUpdate"/> </asp:LinkButton>
</div>
