<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VpsDetailsAddExternalAddress.ascx.cs" Inherits="FuseCP.Portal.VPS.VpsDetailsAddExternalAddress" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/ServerTabs.ascx" TagName="ServerTabs" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>
<%@ Register Src="UserControls/FormTitle.ascx" TagName="FormTitle" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>   	
	    <div class="card">
			    <div class="card-header">
				    <asp:Image ID="imgIcon" SkinID="Network48" runat="server" />
				    <fcp:FormTitle ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Network" />
			    </div>
			    <div class="card-body form-horizontal">
                    <fcp:Menu id="menu" runat="server" SelectedItem="" />
                <div class="card tab-content">
                <div class="card-body form-horizontal">
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
			        <fcp:ServerTabs id="tabs" runat="server" SelectedTab="vps_network" />	
                    <asp:ValidationSummary ID="validatorsSummary" runat="server" 
                        ValidationGroup="AddAddress" ShowMessageBox="True" ShowSummary="False" />
                    
		            <p class="SubTitle">
		                <asp:Localize ID="locSubTitle" runat="server" meta:resourcekey="locSubTitle"
		                    Text="Add External IP Addresses" />
		            </p>
		            
                     <div runat="server" ID="EmptyExternalAddressesMessage" class="fcp-p-5" visible="false">
                        <asp:Localize ID="locNotEnoughExternalAddresses" runat="server" Text="Not enough..."
                                meta:resourcekey="locNotEnoughExternalAddresses"></asp:Localize>
                     </div>
                    
                    <table class="table table-borderless align-middle mb-0" id="ExternalAddressesTable" runat="server" >
                        <tr>
                            <td>
                                <asp:RadioButton ID="radioExternalRandom" runat="server" AutoPostBack="true"
                                    meta:resourcekey="radioExternalRandom" Text="Randomly select IP addresses from the pool" 
                                    Checked="True" GroupName="ExternalAddress" />
                            </td>
                        </tr>
                        <tr id="ExternalAddressesNumberRow" runat="server">
                            <td class="fcp-ps-30">
                                <asp:Localize ID="locExternalAddresses" runat="server"
                                        meta:resourcekey="locExternalAddresses" Text="Number of IP addresses:"></asp:Localize>

                                <asp:TextBox ID="txtExternalAddressesNumber" runat="server" CssClass="form-control" Text="1"></asp:TextBox>
                                
                                <asp:RequiredFieldValidator ID="ExternalAddressesValidator" runat="server" Text="*" Display="Dynamic"
                                        ControlToValidate="txtExternalAddressesNumber" meta:resourcekey="ExternalAddressesValidator" SetFocusOnError="true"
                                        ValidationGroup="AddAddress">*</asp:RequiredFieldValidator>
                                
                                <asp:Literal ID="litMaxExternalAddresses" runat="server"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:RadioButton ID="radioExternalSelected" runat="server" AutoPostBack="true"
                                    meta:resourcekey="radioExternalSelected" Text="Select IP addresses from the list" 
                                    GroupName="ExternalAddress" />
                            </td>
                        </tr>
                        <tr id="ExternalAddressesListRow" runat="server">
                            <td class="fcp-ps-30">
                                <asp:ListBox ID="listExternalAddresses" SelectionMode="Multiple" runat="server" Rows="8"
                                    CssClass="form-control" ></asp:ListBox>
                                <br />
                                <asp:Localize ID="locHoldCtrl" runat="server"
                                        meta:resourcekey="locHoldCtrl" Text="* Hold CTRL key to select multiple addresses"></asp:Localize>
                            </td>
                        </tr>
                    </table>
                    
                    <p>
                        <asp:LinkButton id="btnCancel" CssClass="btn btn-warning" runat="server" CausesValidation="False" OnClick="btnCancel_Click"> <i class="bi bi-x-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCancel"/> </asp:LinkButton>
                        <asp:LinkButton id="btnAdd" CssClass="btn btn-success" runat="server" OnClick="btnAdd_Click" ValidationGroup="AddAddress"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnAdd"/> </asp:LinkButton>&nbsp;
                    </p>
			    </div>
                </div>
                </div>
                </div>
