<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VdcAccountVLanAdd.ascx.cs"
    Inherits="FuseCP.Portal.VdcAccountVLanAdd" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>
 
        <div class="card">
                <div class="card-header">
                    <asp:Image ID="imgIcon" SkinID="VLanNetwork" runat="server" />
                    <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Add VLan to user"></asp:Localize>
                </div>
            <div class="card-body form-horizontal">
            <fcp:menu id="menu" runat="server" selecteditem="vdc_account_vlan_network" />
            <div class="card tab-content">
                <div class="card-body form-horizontal">
                    <table class="table table-borderless align-middle mb-0 w-100">
                        <tr>
                            <td class="SubHead" >
                                <asp:Label ID="lblUsername2" runat="server" meta:resourcekey="lblUsername" Text="Username:"></asp:Label>
                            </td>
                            <td class="Huge">
                                <asp:Literal ID="lblUsername" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td class="SubHead align-top">
                                <asp:Label ID="lblVLanID" runat="server" meta:resourcekey="lblVLanID" Text="VLan:" />
                            </td>
                            <td>
                                <asp:TextBox ID="tbVLanID" runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="VLanIDValidator" runat="server" ErrorMessage="*"
                                    Display="Dynamic" ControlToValidate="tbVLanID" />
                                <asp:RegularExpressionValidator  ID="VLanIDRegExpValidator" runat="server" ErrorMessage="*"
                                    Display="Dynamic" ControlToValidate="tbVLanID" ValidationExpression="^\d+$" />
                            </td>
                        </tr>
                        <tr>
                            <td class="SubHead align-top">
                                <asp:Label ID="lblComment" runat="server" meta:resourcekey="lblComment" Text="Comment:" />
                            </td>
                            <td class="NormalBold">
                                <asp:TextBox ID="tbComment" runat="server" TextMode="MultiLine" />
                            </td>
                        </tr>
                    </table>
     
                </div>
               <div class="card-footer text-end">
                        <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
                        <asp:LinkButton id="btAddVLan" CssClass="btn btn-success" runat="server" OnClick="btAddVLan_Click"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddVLan"/> </asp:LinkButton>
                    </div>
            </div>
            </div>
            </div>
            <div class="alert alert-info">
                <asp:Localize ID="FormComments" runat="server" meta:resourcekey="FormComments"></asp:Localize>
            </div>
