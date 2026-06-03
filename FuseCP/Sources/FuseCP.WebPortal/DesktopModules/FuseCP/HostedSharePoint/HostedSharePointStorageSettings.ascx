<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HostedSharePointStorageSettings.ascx.cs" Inherits="FuseCP.Portal.HostedSharePointStorageSettings" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport"
    TagPrefix="fcp" %>
<%@ Register Src="../UserControls/CollapsiblePanel.ascx" TagName="CollapsiblePanel"
    TagPrefix="fcp" %>
<%@ Register Src="../ExchangeServer/UserControls/SizeBox.ascx" TagName="SizeBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox"
    TagPrefix="fcp" %>
<%@ Register Src="../ExchangeServer/UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register src="../UserControls/QuotaEditor.ascx" tagname="QuotaEditor" tagprefix="uc1" %>



<div id="ExchangeContainer">
	<div class="Module">
		<div class="Left">
        </div>
		<div class="Content">
			<div class="Center">
				<div class="Title">
					<asp:Image ID="Image1" SkinID="ExchangeStorageConfig48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" ></asp:Localize>
				</div>
				<div class="card-body form-horizontal">
				    <fcp:SimpleMessageBox id="messageBox" runat="server" />
				    
					<fcp:CollapsiblePanel id="secStorageLimits" runat="server"
                        TargetControlID="StorageLimits" meta:resourcekey="secStorageLimits" >
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="StorageLimits" runat="server" Height="0" style="overflow:hidden">
					    <table class="table table-borderless align-middle mb-0" >
						    
						    <tr>
						    <td class="FormLabel200 text-end"><asp:Localize ID="locMaxStorage" runat="server" meta:resourcekey="locMaxStorage" ></asp:Localize></td>
							    <td>                                    
									<uc1:QuotaEditor QuotaTypeId="2" ID="maxStorageSettingsValue" runat="server" />                                    																	    
								</td>
						    </tr>
						    <tr>
						    <td class="FormLabel200 text-end"><asp:Localize ID="locWarningStorage" runat="server" meta:resourcekey="locWarningStorage" ></asp:Localize></td>
							    <td>
									<uc1:QuotaEditor  QuotaTypeId="2" ID="warningValue" runat="server" />
									
								</td>
						    </tr>
					    </table>
					    <br />
					</asp:Panel>
                   									                    
				    <div class="card-footer text-end">
					    <asp:LinkButton id="btnSave" CssClass="btn btn-warning" runat="server" OnClick="btnSave_Click" ValidationGroup="EditStorageSettings"> <i class="bi bi-floppy me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveText"/> </asp:LinkButton>&nbsp;
						<asp:LinkButton id="btnSaveApply" CssClass="btn btn-success" runat="server" OnClick="btnSaveApply_Click" ValidationGroup="EditStorageSettings"> <i class="bi bi-check-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnSaveApplyText"/> </asp:LinkButton>
				    </div>
				    
				</div>
			</div>
		</div>
	</div>
</div>

