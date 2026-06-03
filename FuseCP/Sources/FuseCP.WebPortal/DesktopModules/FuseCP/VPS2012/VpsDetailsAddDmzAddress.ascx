<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VpsDetailsAddDmzAddress.ascx.cs" Inherits="FuseCP.Portal.VPS2012.VpsDetailsAddDmzAddress" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/ServerTabs.ascx" TagName="ServerTabs" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>
<%@ Register Src="UserControls/FormTitle.ascx" TagName="FormTitle" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>

	    <div class="Content">
		    <div class="Center">
			    <div class="card-body form-horizontal">
			        <fcp:ServerTabs id="tabs" runat="server" SelectedTab="vps_network" />
			        	
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    
                    <asp:ValidationSummary ID="validatorsSummary" runat="server" 
                        ValidationGroup="AddAddress" ShowMessageBox="True" ShowSummary="False" />
                    
		            <p class="SubTitle">
		                <asp:Localize ID="locSubTitle" runat="server" meta:resourcekey="locSubTitle"
		                    Text="Add DMZ IP Addresses" />
		            </p>
                    
                    <table class="table table-borderless align-middle mb-0" id="tableDmzNetwork" runat="server" >
                        <tr>
                            <td>
                                <asp:RadioButton ID="radioDmzRandom" runat="server" AutoPostBack="true"
                                    meta:resourcekey="radioDmzRandom" Text="Randomly select next available IP addresses to the addresses format" 
                                    Checked="True" GroupName="DmzAddress" />
                            </td>
                        </tr>
                        <tr id="DmzAddressesNumberRow" runat="server">
                            <td class="fcp-ps-30">
                                <asp:Localize ID="locDmzAddresses" runat="server"
                                        meta:resourcekey="locDmzAddresses" Text="Number of IP addresses:"></asp:Localize>

                                <asp:TextBox ID="txtDmzAddressesNumber" runat="server" CssClass="form-control" Text="1"></asp:TextBox>
                                
                                <asp:RequiredFieldValidator ID="DmzAddressesValidator" runat="server" Text="*" Display="Dynamic"
                                        ControlToValidate="txtDmzAddressesNumber" meta:resourcekey="DmzAddressesValidator" SetFocusOnError="true"
                                        ValidationGroup="AddAddress">*</asp:RequiredFieldValidator>
                                        
                                <asp:Literal ID="litMaxDmzAddresses" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:RadioButton ID="radioDmzSelected" runat="server" AutoPostBack="true"
                                    meta:resourcekey="radioDmzSelected" Text="Assign specified IP addresses" 
                                    GroupName="DmzAddress" />
                            </td>
                        </tr>
                        <tr id="DmzAddressesListRow" runat="server">
                            <td class="fcp-ps-30">
                                <asp:TextBox ID="txtDmzAddressesList" runat="server" TextMode="MultiLine"
                                    CssClass="form-control" Rows="5"></asp:TextBox>
                                <br />
                                <asp:Localize ID="locOnePerLine" runat="server"
                                        meta:resourcekey="locOnePerLine" Text="* Type one IP address per line"></asp:Localize>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <br />
                                <asp:CheckBox ID="chkCustomGateway" runat="server" AutoPostBack="true" Checked="false"
                                    meta:resourcekey="chkCustomGateway" Text="Custom Gateway and DNS" />
                            </td>
                        </tr>
                        <tr id="trCustomGateway" runat="server">
                            <td class="fcp-ps-30">
                                <asp:RequiredFieldValidator ID="GatewayValidator" runat="server" Text="*" Display="Dynamic"
                                    ControlToValidate="txtGateway" meta:resourcekey="GatewayValidator" SetFocusOnError="true"
                                    ValidationGroup="Vps">*</asp:RequiredFieldValidator>
                                <asp:Localize ID="locGateway" runat="server"
                                    meta:resourcekey="locGateway" Text="Gateway:"></asp:Localize>
                                <asp:TextBox ID="txtGateway" runat="server" CssClass="form-control" Text=""></asp:TextBox>

                                <asp:RequiredFieldValidator ID="DNSValidator" runat="server" Text="*" Display="Dynamic"
                                    ControlToValidate="txtDNS1" meta:resourcekey="DNSValidator" SetFocusOnError="true"
                                    ValidationGroup="Vps">*</asp:RequiredFieldValidator>
                                <asp:Localize ID="locDNS1" runat="server"
                                    meta:resourcekey="locDNS1" Text="Preferred DNS server:"></asp:Localize>
                                <asp:TextBox ID="txtDNS1" runat="server" CssClass="form-control" Text=""></asp:TextBox>

                                <asp:Localize ID="locDNS2" runat="server"
                                    meta:resourcekey="locDNS2" Text="Alternate DNS server:"></asp:Localize>
                                <asp:TextBox ID="txtDNS2" runat="server" CssClass="form-control" Text=""></asp:TextBox>

                                <asp:RequiredFieldValidator ID="MaskValidator" runat="server" Text="*" Display="Dynamic"
                                    ControlToValidate="txtMask" meta:resourcekey="MaskValidator" SetFocusOnError="true"
                                    ValidationGroup="Vps">*</asp:RequiredFieldValidator>
                                <asp:Localize ID="locMask" runat="server"
                                    meta:resourcekey="locMask" Text="Subnet mask:"></asp:Localize>
                                <asp:TextBox ID="txtMask" runat="server" CssClass="form-control" Text=""></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    
                    <p>
                        <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>&nbsp;
                        <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click" ValidationGroup="AddAddress"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAdd"/> </asp:LinkButton>
                        (<asp:LinkButton id="btnAddByInject" CssClass="btn btn-success" runat="server" OnClick="btnAddByInject_Click" ValidationGroup="AddAddress"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAddByInject"/> </asp:LinkButton>)
                    </p>

				    
			    </div>
		    </div>
	    </div>
