<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DnsZoneRecords.ascx.cs" Inherits="FuseCP.Portal.DnsZoneRecords" %>
<%@ Register Src="UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>
<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/mail-confirmation.js"></script>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server" />
<asp:Panel ID="pnlRecords" runat="server">
	<div class="card-body form-horizontal">
        <div class="Huge fcp-p-10" style="border: solid 1px #e5e5e5; background-color: #f5f5f5">
			<asp:Literal ID="litDomainName" runat="server"></asp:Literal>
		</div>
	</div>
    <div class="FormButtonsBar">
        <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" meta:resourcekey="btnAdd" OnClick="btnAdd_Click" CausesValidation="False">
            <i class="bi bi-plus-lg me-1"></i>
            <asp:Localize runat="server" meta:resourcekey="btnAddText" Text="Add record" />
        </asp:LinkButton>
    </div>
    <asp:GridView ID="gvRecords" runat="server" AutoGenerateColumns="False" EmptyDataText="gvRecords"
        CssSelectorClass="NormalGridView FixedGrid"
        OnRowEditing="gvRecords_RowEditing" OnRowDeleting="gvRecords_RowDeleting"
        AllowSorting="True" DataSourceID="odsDnsRecords">
        <Columns>
            <asp:TemplateField>
                <ItemStyle Width="25px" HorizontalAlign="Center" />
                <ItemTemplate>
                    <asp:ImageButton ID="cmdEdit" runat="server" SkinID="EditSmall" CommandName="edit" AlternateText="Edit record">
                    </asp:ImageButton>
                    <asp:Literal ID="litMxPriority" runat="server" Text='<%# Eval("MxPriority") %>' Visible="false"></asp:Literal>
                    <asp:Literal ID="litRecordName" runat="server" Text='<%# Eval("RecordName") %>' Visible="false"></asp:Literal>
                    <asp:Literal ID="litRecordType" runat="server" Text='<%# Eval("RecordType") %>' Visible="false"></asp:Literal>
                    <asp:Literal ID="litRecordTTL" runat="server" Text='<%# Eval("RecordTTL") %>' Visible="false"></asp:Literal>
                    <asp:Literal ID="litRecordData" runat="server" Text='<%# Eval("RecordData") %>' Visible="false"></asp:Literal>
                    <asp:Literal ID="litSrvPriority" runat="server" Text='<%# Eval("SrvPriority") %>' Visible="false"></asp:Literal>
                    <asp:Literal ID="litSrvWeight" runat="server" Text='<%# Eval("SrvWeight") %>' Visible="false"></asp:Literal>
                    <asp:Literal ID="litSrvPort" runat="server" Text='<%# Eval("SrvPort") %>' Visible="false"></asp:Literal>
                </ItemTemplate>
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="RecordName" SortExpression="RecordName" HeaderText="gvRecordsName" />
            <asp:BoundField DataField="RecordType" SortExpression="RecordType" HeaderText="gvRecordsType" />
            <asp:BoundField DataField="RecordTTL" SortExpression="RecordTTL" HeaderText="gvRecordsTTL" Visible="false"/>
            <asp:TemplateField SortExpression="RecordData" HeaderText="gvRecordsData" >
                <ItemStyle />
                <ItemTemplate>
                    <%# GetRecordFullData((string)Eval("RecordType"), (string)Eval("RecordData"), (int)Eval("MxPriority"), (int)Eval("SrvPort"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemStyle Width="65px" HorizontalAlign="Center" />
                <ItemTemplate>
                    <asp:LinkButton id="cmdDelete" CssClass="btn btn-danger" runat="server" CommandName="delete" OnClientClick="return fuseCpConfirmWithProgress('Are you sure you want to delete this DNS Zone Record?', 'Deleting DNS Zone Record...');"> 
                    <i class="bi bi-trash"></i> 
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <div class="card-footer text-end">
        <asp:LinkButton id="btnBack" CssClass="btn btn-warning" runat="server" OnClick="btnBack_Click"> <i class="bi bi-arrow-left me-1"></i><asp:Localize runat="server" meta:resourcekey="btnBack"/> </asp:LinkButton>
    </div>
</asp:Panel>
<asp:ObjectDataSource ID="odsDnsRecords" runat="server" SelectMethod="GetRawDnsZoneRecords" TypeName="FuseCP.Portal.ServersHelper" OnSelected="odsDnsRecords_Selected">
    <SelectParameters>
        <asp:QueryStringParameter DefaultValue="0" Name="domainId" QueryStringField="DomainID" />
    </SelectParameters>
</asp:ObjectDataSource>
<asp:Panel ID="pnlEdit" runat="server" DefaultButton="btnSave">
    <div class="card-body form-horizontal">
        <div class="row">
            <div class="mb-3 col-sm-8">
                <asp:Label runat="server" CssClass="form-label col-sm-3" AssociatedControlID="ddlRecordType">
                    <asp:Localize ID="lblRecordType" runat="server" meta:resourcekey="lblRecordType" Text="Record Type:"></asp:Localize>
                </asp:Label>
                <div class="d-flex flex-wrap gap-2 align-items-center">
                    <asp:DropDownList ID="ddlRecordType" runat="server" SelectedValue='<%# Bind("RecordType") %>' CssClass="form-control" Width="110px" AutoPostBack="True" OnSelectedIndexChanged="ddlRecordType_SelectedIndexChanged">
                        <asp:ListItem>A</asp:ListItem>
	                    <asp:ListItem>AAAA</asp:ListItem>
                        <asp:ListItem>CAA</asp:ListItem>
                        <asp:ListItem>MX</asp:ListItem>
                        <asp:ListItem>NS</asp:ListItem>
                        <asp:ListItem>TXT</asp:ListItem>
                        <asp:ListItem>CNAME</asp:ListItem>
                        <asp:ListItem>SRV</asp:ListItem>
                        <asp:ListItem>PTR</asp:ListItem>
                    </asp:DropDownList>
                    <asp:Literal ID="litRecordType" runat="server"></asp:Literal>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="mb-3 col-sm-8">
                <asp:Label runat="server" CssClass="form-label col-sm-3" AssociatedControlID="txtRecordName">
                    <asp:Label ID="lblRecordName" runat="server" meta:resourcekey="lblRecordName" Text="Record Name:"></asp:Label>
                </asp:Label>
                <div class="d-flex flex-wrap gap-2 align-items-center">
                    <asp:TextBox ID="txtRecordName" runat="server" CssClass="form-control" Width="300px"></asp:TextBox><asp:Label ID="lblDomainName" runat="server" meta:resourcekey="lblDomainName" Text=""></asp:Label>
                </div>
            </div>
        </div>
        <div class="row" id="rowData" runat="server">
            <div class="mb-3 col-sm-8">
                <asp:Label runat="server" CssClass="form-label col-sm-3" AssociatedControlID="txtRecordData">
                    <asp:Label ID="lblRecordData" runat="server" meta:resourcekey="lblRecordData" Text="Record Data:"></asp:Label>
                </asp:Label>
                <div class="d-flex flex-wrap gap-2 align-items-center">
                    <asp:TextBox ID="txtRecordData" runat="server" CssClass="form-control" Width="300px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valRequireData" runat="server" ControlToValidate="txtRecordData" ErrorMessage="*" ValidationGroup="DnsZoneRecord" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="IPValidator" runat="server" ControlToValidate="txtRecordData" ValidationGroup="DnsZoneRecord" Display="Dynamic" Text="Please enter a valid IP" OnServerValidate="Validate" meta:resourcekey="IPValidator" />
                </div>
            </div>
        </div>
        <div class="row" id="rowMXPriority" runat="server">
            <div class="mb-3 col-sm-8">
                <asp:Label runat="server" CssClass="form-label col-sm-3" AssociatedControlID="txtMXPriority">
                    <asp:Label ID="lblMXPriority" runat="server" meta:resourcekey="lblMXPriority" Text="MX Priority:"></asp:Label>
                </asp:Label>
                <div class="d-flex flex-wrap gap-2 align-items-center">
                    <asp:TextBox ID="txtMXPriority" runat="server" CssClass="form-control" Width="60px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valRequireMxPriority" runat="server" ControlToValidate="txtMXPriority" ErrorMessage="*" ValidationGroup="DnsZoneRecord" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="valRequireCorrectPriority" runat="server" ControlToValidate="txtMXPriority" ErrorMessage="*" ValidationExpression="\d{1,3}" ValidationGroup="DnsZoneRecord" />
                </div>
            </div>
        </div>
        <div class="row" id="rowSRVPriority" runat="server">
            <div class="mb-3 col-sm-8">
                <asp:Label runat="server" CssClass="form-label col-sm-3" AssociatedControlID="txtSRVPriority">
                    <asp:Label ID="lblSRVPriority" runat="server" meta:resourcekey="lblSRVPriority" Text="Priority:"></asp:Label>
                </asp:Label>
                <div class="d-flex flex-wrap gap-2 align-items-center">
                    <asp:TextBox ID="txtSRVPriority" runat="server" CssClass="form-control" Width="60px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valRequireSrvPriority" runat="server" ControlToValidate="txtSRVPriority" ErrorMessage="*" ValidationGroup="DnsZoneRecord" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="valRequireCorrectSrvPriority" runat="server" ControlToValidate="txtSRVPriority" ErrorMessage="*" ValidationExpression="\d{1,3}"></asp:RegularExpressionValidator>
                </div>
            </div>
        </div>
        <div class="row" id="rowSRVWeight" runat="server">
            <div class="mb-3 col-sm-8">
                <asp:Label runat="server" CssClass="form-label col-sm-3" AssociatedControlID="txtSRVWeight">
                    <asp:Label ID="lblSRVWeight" runat="server" meta:resourcekey="lblSRVWeight" Text="Weight:"></asp:Label>
                </asp:Label>
                <div class="d-flex flex-wrap gap-2 align-items-center">
                    <asp:TextBox ID="txtSRVWeight" runat="server" CssClass="form-control" Width="60px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valRequireSrvWeight" runat="server" ControlToValidate="txtSRVWeight" ErrorMessage="*" ValidationGroup="DnsZoneRecord" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="valRequireCorrectSrvWeight" runat="server" ControlToValidate="txtSRVWeight" ErrorMessage="*" ValidationExpression="\d{1,3}"></asp:RegularExpressionValidator>
                </div>
            </div>
        </div>
        <div class="row" id="rowSRVPort" runat="server">
            <div class="mb-3 col-sm-8">
                <asp:Label runat="server" CssClass="form-label col-sm-3" AssociatedControlID="ddlRecordType">
                    <asp:Label ID="lblSRVPort" runat="server" meta:resourcekey="lblSRVPort" Text="Port Number:"></asp:Label>
                </asp:Label>
                <div class="d-flex flex-wrap gap-2 align-items-center">
                    <asp:TextBox ID="txtSRVPort" runat="server" CssClass="form-control" Width="115px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valRequireSrvPort" runat="server" ControlToValidate="txtSRVPort" ErrorMessage="*" ValidationGroup="DnsZoneRecord" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="valRequireCorrectSrvPort" runat="server" ControlToValidate="txtSRVPort" ErrorMessage="*" ValidationExpression="\d{1,3}"></asp:RegularExpressionValidator>
                </div>
            </div>
        </div>
        <div class="row" id="rowRecordTTL" runat="server">
            <div class="mb-3 col-sm-8">
                <asp:Label runat="server" CssClass="form-label col-sm-3" AssociatedControlID="txtRecordTTL">
                    <asp:Label ID="lblRecordTTL" runat="server" meta:resourcekey="lblRecordTTL" Text="Record TTL:"></asp:Label>
                </asp:Label>
                <div class="d-flex flex-wrap gap-2 align-items-center">
                    <asp:TextBox ID="txtRecordTTL" runat="server" CssClass="form-control" Width="300px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="valRequireRecordTTL" runat="server" ControlToValidate="txtRecordTTL" ErrorMessage="*" ValidationGroup="DnsZoneRecord" Display="Dynamic"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="valRequireCorrectRecordTTL" runat="server" ControlToValidate="txtRecordTTL" ErrorMessage="*" ValidationExpression="^(\d+)|(?=\s*)$"></asp:RegularExpressionValidator>
                </div>
            </div>
        </div>
    </div>
    <div class="card-footer text-end">
        <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click">
            <i class="bi bi-x-lg me-1"></i>
            <asp:Localize runat="server" meta:resourcekey="btnCancel"/>
        </asp:LinkButton>
        &nbsp;
        <asp:LinkButton id="btnSave" CssClass="btn btn-success" runat="server" OnClick="btnSave_Click" OnClientClick="ShowProgressDialog('Saving DNS Zone Record ...');" ValidationGroup="DnsZoneRecord">
            <i class="bi bi-floppy me-1"></i>
            <asp:Localize runat="server" meta:resourcekey="btnSaveText"/>
        </asp:LinkButton>
    </div>
</asp:Panel>
