<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OCS_Settings.ascx.cs" Inherits="FuseCP.Portal.ProviderControls.OCS_Settings" %>
<table class="table table-borderless align-middle mb-0">
    <tr>
        <td class="Normal" >
            <asp:Localize runat="server" ID="locServerName" meta:resourcekey="locServerName"/>
        </td>
        <td >
            <asp:TextBox runat="server" ID="txtServerName"  CssClass="form-control" Width="200px"/>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Localize runat="server" ID="locEdgeServices" meta:resourcekey="locEdgeServices"/>
        </td>
        <td>
            <asp:DropDownList runat="server" ID="ddlEdgeServers" />
            <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddText"/> </asp:LinkButton>
            <br />
            <asp:GridView ID="gvEdgeServices" runat="server" AutoGenerateColumns="False"  
                EmptyDataText="gvRecords" CssSelectorClass="NormalGridView" 
                onrowcommand="gvEdgeServices_RowCommand"  meta:resourcekey="gvEdgeServices">
                <Columns>                                       
                    <asp:BoundField DataField="ServiceName" meta:resourcekey="locServerNameColumn" HeaderText="gvRecordsData" />
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

