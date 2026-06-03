<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PackageVLANs.ascx.cs" Inherits="FuseCP.Portal.UserControls.PackageVLANs" %>
<%@ Register Src="SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>

<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/email-selection.js"></script>

<fcp:SimpleMessageBox id="messageBox" runat="server" />

<div class="FormButtonsBarClean d-flex flex-wrap justify-content-between align-items-center gap-2 mb-2">
    <div class="FormButtonsBarCleanLeft d-flex align-items-center gap-2">
        <asp:LinkButton id="btnAllocateVLAN" CssClass="btn btn-primary" runat="server" OnClick="btnAllocateVLAN_Click" CausesValidation="False"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAllocateVLANText"/> </asp:LinkButton>
    </div>
    <div class="FormButtonsBarCleanRight">
        <div class="d-flex align-items-center gap-2">
			<asp:Label runat="server" Text="Page size:" CssClass="Normal"></asp:Label>
            <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="True" CssClass="form-select"
				onselectedindexchanged="ddlPageSize_SelectedIndexChanged">   
				<asp:ListItem>10</asp:ListItem>   
				<asp:ListItem Selected="True">20</asp:ListItem>   
				<asp:ListItem>50</asp:ListItem>   
				<asp:ListItem>100</asp:ListItem>   
			</asp:DropDownList> 
		</div>
	</div>
</div>

<asp:GridView ID="gvVLANs" runat="server" AutoGenerateColumns="False"
    EmptyDataText="gvVLANs" CssSelectorClass="NormalGridView"
    AllowPaging="True" AllowSorting="True" DataSourceID="odsVLANsPaged" PageSize="20"
    onrowdatabound="gvVLANs_RowDataBound" DataKeyNames="PackageVlanID" >
    <Columns>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:CheckBox ID="chkSelectAll" runat="server" onclick="checkAll(this);" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="chkSelect" runat="server" onclick="unCheckSelectAll(this);" />&nbsp;
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:BoundField HeaderText="gvVLANsVLAN" meta:resourcekey="gvVLANsVLAN"
            DataField="Vlan" SortExpression="Vlan" />

        <asp:TemplateField HeaderText="gvVLANsSpace" meta:resourcekey="gvVLANsSpace" SortExpression="PackageName" >
	        <ItemTemplate>
		        <asp:hyperlink id="lnkSpace" runat="server" NavigateUrl='<%# GetSpaceHomeUrl(Eval("PackageID").ToString()) %>'>
			        <%# Eval("PackageName") %>
		        </asp:hyperlink>
	        </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="gvVLANsUser" meta:resourcekey="gvVLANsUser" SortExpression="Username"  >						        
	        <ItemTemplate>
		        <%# Eval("UserName") %>
	        </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:CheckBox ID="cbIsDmz" Visible="false" runat="server"/>
<asp:ObjectDataSource ID="odsVLANsPaged" runat="server" EnablePaging="True"
	    SelectCountMethod="GetPackageVLANsCount"
	    SelectMethod="GetPackageVLANs"
	    SortParameterName="sortColumn"
	    TypeName="FuseCP.Portal.VirtualMachines2012Helper"
	    OnSelected="odsVLANsPaged_Selected">
    <SelectParameters>
	    <asp:QueryStringParameter Name="packageId" QueryStringField="SpaceID" DefaultValue="0" />
        <asp:ControlParameter Name="isDmz" ControlID="cbIsDmz" PropertyName="Checked" Type="Boolean" DefaultValue=false />
    </SelectParameters>
</asp:ObjectDataSource>

<div class="fcp-mt-4 text-end">
    <asp:Button ID="btnDeallocateVLANs" runat="server" meta:resourcekey="btnDeallocateVLANs"
            Text="Deallocate selected" CssClass="btn btn-danger" CausesValidation="False" 
        onclick="btnDeallocateVLANs_Click" />
</div>
