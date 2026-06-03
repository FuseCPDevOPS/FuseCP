<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SfB_Settings.ascx.cs" Inherits="FuseCP.Portal.ProviderControls.SfB_Settings" %>
<table class="table table-borderless align-middle mb-0">
    <tr>
        <td class="Normal" >
            <asp:Localize runat="server" ID="locServerName" meta:resourcekey="locServerName"/>
        </td>
        <td >
            <asp:TextBox runat="server" ID="txtServerName"  CssClass="form-control" Width="200px"/>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtServerName" ErrorMessage="*" Display="Dynamic" />
        </td>
    </tr>
    <tr>
        <td class="Normal" >
            <asp:Localize runat="server" ID="locSimpleUrlBase" meta:resourcekey="locSimpleUrlBase"/>
        </td>
        <td >
            <asp:TextBox runat="server" ID="txtSimpleUrlBase"  CssClass="form-control" Width="200px"/>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtSimpleUrlBase" ErrorMessage="*" Display="Dynamic" />
        </td>
    </tr>
    <tr>
        <td class="SubHead text-nowrap align-top">
            <asp:Localize ID="locLynServers" runat="server" meta:resourcekey="locLynServers"
                Text="SfB Servers:"></asp:Localize>
        </td>
        <td>
            <asp:DropDownList ID="ddlSfBServers" runat="server" CssClass="form-control">
            </asp:DropDownList>
            <asp:LinkButton id="btnAddSfBServer" CssClass="btn btn-success" runat="server" OnClick="btnAddSfBServer_Click"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddText"/> </asp:LinkButton>
            <br />
            <asp:GridView ID="gvSfBServers" runat="server" AutoGenerateColumns="False" EmptyDataText="gvRecords"
                CssSelectorClass="NormalGridView" OnRowCommand="gvSfBServers_RowCommand" meta:resourcekey="gvSfBServers">
                <Columns>
                    <asp:TemplateField meta:resourcekey="locServerNameColumn" >
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblServiceName" Text='<%#Eval("ServiceName") + "(" + Eval("ServerName") +")"%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton id="cmdDelete" CssClass="btn btn-danger" runat="server" CommandName='RemoveServer' CommandArgument='<%#Eval("ServiceId") %>' OnClientClick="return confirm('Delete?');"> 
                                <i class="bi bi-trash"></i> 
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </td>
    </tr>



</table>

