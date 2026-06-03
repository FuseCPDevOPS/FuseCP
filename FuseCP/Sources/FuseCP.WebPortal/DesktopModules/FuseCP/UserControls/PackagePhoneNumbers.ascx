<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PackagePhoneNumbers.ascx.cs" Inherits="FuseCP.Portal.UserControls.PackagePhoneNumbers" %>
<%@ Register Src="SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="SearchBox.ascx" TagName="SearchBox" TagPrefix="fcp" %>

<script type="text/javascript" src="/DesktopModules/FuseCP/Scripts/email-selection.js"></script>

<fcp:SimpleMessageBox id="messageBox" runat="server" />

<div class="FormButtonsBarClean">
    <div class="FormButtonsBarCleanLeft">
        <asp:LinkButton id="btnAllocateAddress" CssClass="btn btn-primary" runat="server" OnClick="btnAllocateAddress_Click" CausesValidation="False"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAllocateAddressText"/> </asp:LinkButton>
    </div>
    <div class="FormButtonsBarCleanRight">
        <fcp:SearchBox ID="searchBox" runat="server" />
    </div>
</div>

<asp:GridView ID="gvAddresses" runat="server" AutoGenerateColumns="False"
    EmptyDataText="gvAddresses" CssSelectorClass="NormalGridView"
    AllowPaging="True" AllowSorting="True" DataSourceID="odsExternalAddressesPaged" 
    onrowdatabound="gvAddresses_RowDataBound" DataKeyNames="PackageAddressID" >
    <Columns>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:CheckBox ID="chkSelectAll" runat="server" onclick="checkAll(this);" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="chkSelect" runat="server" onclick="unCheckSelectAll(this);" />&nbsp;
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:BoundField HeaderText="gvAddressesIPAddress" meta:resourcekey="gvAddressesIPAddress"
            DataField="ExternalIP" SortExpression="ExternalIP" />
            
        <asp:TemplateField HeaderText="gvAddressesItemName" meta:resourcekey="gvAddressesItemName" SortExpression="ItemName">						        						        
	        <ItemTemplate>
		         <asp:hyperlink id="lnkEdit" runat="server" NavigateUrl='<%# GetItemEditUrl(Eval("ItemID").ToString()) %>'>
			        <%# Eval("ItemName") %>
		        </asp:hyperlink>&nbsp;
	        </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="gvAddressesSpace" meta:resourcekey="gvAddressesSpace" SortExpression="PackageName" >
	        <ItemTemplate>
		        <asp:hyperlink id="lnkSpace" runat="server" NavigateUrl='<%# GetSpaceHomeUrl(Eval("PackageID").ToString()) %>'>
			        <%# Eval("PackageName") %>
		        </asp:hyperlink>
	        </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="gvAddressesUser" meta:resourcekey="gvAddressesUser" SortExpression="Username"  >						        
	        <ItemTemplate>
		        <%# Eval("UserName") %>
	        </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:ObjectDataSource ID="odsExternalAddressesPaged" runat="server" EnablePaging="True"
	    SelectCountMethod="GetPackageIPAddressesCount"
	    SelectMethod="GetPackageIPAddresses"
	    SortParameterName="sortColumn"
	    TypeName="FuseCP.Portal.VirtualMachinesHelper"
	    OnSelected="odsExternalAddressesPaged_Selected" 
    onselecting="odsExternalAddressesPaged_Selecting">
    <SelectParameters>
	    <asp:QueryStringParameter Name="packageId" QueryStringField="SpaceID" DefaultValue="0" />
        <asp:QueryStringParameter Name="orgId" QueryStringField="ItemID" DefaultValue="0" />						    
	    <asp:Parameter Name="pool" DefaultValue="0" />
        <asp:ControlParameter Name="filterColumn" ControlID="searchBox"  PropertyName="FilterColumn" />
        <asp:ControlParameter Name="filterValue" ControlID="searchBox" PropertyName="FilterValue" />
    </SelectParameters>
</asp:ObjectDataSource>

<div class="fcp-mt-4">
    <asp:Button ID="btnDeallocateAddresses" runat="server" meta:resourcekey="btnDeallocateAddresses"
            Text="Deallocate selected" CssClass="btn btn-primary btn-sm" CausesValidation="False" 
        onclick="btnDeallocateAddresses_Click" />
</div>
