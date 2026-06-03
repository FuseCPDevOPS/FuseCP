<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VdcHome.ascx.cs" Inherits="FuseCP.Portal.VPS.VdcHome" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Menu.ascx" TagName="Menu" TagPrefix="fcp" %>
<%@ Register Src="UserControls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/Quota.ascx" TagName="Quota" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/CollapsiblePanel.ascx" TagName="CollapsiblePanel" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/SearchBox.ascx" TagName="SearchBox" TagPrefix="fcp" %>
   	
	    <div class="card">
			    <div class="card-header">
				    <asp:Image ID="imgIcon" SkinID="Servers48" runat="server" />
				    <asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Virtual Private Servers"></asp:Localize>
			    </div>
<div class="FormButtonsBar right">
                            <asp:LinkButton id="btnCreate" CssClass="btn btn-success" runat="server" OnClick="btnCreate_Click" CausesValidation="False"> <i class="bi bi-plus-lg me-1"></i><asp:Localize runat="server" meta:resourcekey="btnCreateText"/> </asp:LinkButton>&nbsp;
                            <asp:LinkButton id="btnImport" CssClass="btn btn-warning" runat="server" OnClick="btnImport_Click" CausesValidation="False"> <i class="bi bi-download me-1"></i><asp:Localize runat="server" meta:resourcekey="btnImportText"/> </asp:LinkButton>
</div>
			    <div class="card-body form-horizontal">
                    <fcp:Menu id="menu" runat="server" SelectedItem="" />
               <div class="card tab-content">
                <div class="card-body form-horizontal">  
                    <fcp:SimpleMessageBox id="messageBox" runat="server" />
                    <div class="row">
                        <div class="col-md-4 offset-md-4">
		                        <asp:Label runat="server" Text="Page size:" CssClass="Normal"></asp:Label>
		                        <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="True"    
			                        onselectedindexchanged="ddlPageSize_SelectedIndexChanged">   
			                        <asp:ListItem>10</asp:ListItem>   
			                        <asp:ListItem Selected="True">20</asp:ListItem>   
			                        <asp:ListItem>50</asp:ListItem>   
			                        <asp:ListItem>100</asp:ListItem>   
		                        </asp:DropDownList>
	                        </div>
                        <div class="col-md-4">
                            <fcp:SearchBox ID="searchBox" runat="server" />
                        </div>
                    </div>

			        <asp:GridView ID="gvServers" runat="server" AutoGenerateColumns="False" EnableViewState="true"
				        EmptyDataText="gvServers" CssSelectorClass="NormalGridView"
				        AllowPaging="True" AllowSorting="True" DataSourceID="odsServersPaged" PageSize="20"
                        onrowcommand="gvServers_RowCommand">
				        <Columns>
					        <asp:TemplateField HeaderText="gvServersName" SortExpression="ItemName" meta:resourcekey="gvServersName">
						        <ItemStyle></ItemStyle>
						        <ItemTemplate>
						            <asp:Image runat="server" SkinID="Vps16" />
							        <asp:hyperlink id="lnk1" runat="server"
								        NavigateUrl='<%# GetServerEditUrl(Eval("ItemID").ToString()) %>'>
								        <%# Eval("ItemName") %>
							        </asp:hyperlink>
						        </ItemTemplate>
					        </asp:TemplateField>
					        <asp:BoundField HeaderText="gvServersExternalIP" meta:resourcekey="gvServersExternalIP"
					            DataField="ExternalIP" SortExpression="ExternalIP" />
					        <asp:BoundField HeaderText="gvServersPrivateIP" meta:resourcekey="gvServersPrivateIP"
					            DataField="IPAddress" SortExpression="IPAddress" />
					        <asp:TemplateField HeaderText="gvServersSpace" meta:resourcekey="gvServersSpace" SortExpression="PackageName" >
						        <ItemTemplate>
							        <asp:hyperlink id="lnkSpace" runat="server" NavigateUrl='<%# GetSpaceHomeUrl(Eval("PackageID").ToString()) %>'>
								        <%# Eval("PackageName") %>
							        </asp:hyperlink>
						        </ItemTemplate>
					        </asp:TemplateField>
					        <asp:TemplateField HeaderText="gvServersUser" meta:resourcekey="gvServersUser" SortExpression="Username"  >						        
						        <ItemTemplate>
							        <asp:hyperlink id="lnkUser" runat="server" NavigateUrl='<%# GetUserHomeUrl((int)Eval("UserID")) %>'>
                                        <%# Eval("UserName") %>
							        </asp:hyperlink>
						        </ItemTemplate>
					        </asp:TemplateField>
						    <asp:TemplateField>
							    <ItemTemplate>
								    <asp:LinkButton id="cmdDelete" CssClass="btn btn-danger" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("ItemID") %>'> <i class="bi bi-trash"></i> </asp:LinkButton>
							    </ItemTemplate>
						    </asp:TemplateField>
                            <asp:TemplateField>
			                    <ItemTemplate>
				                    <asp:LinkButton id="cmdMove" CssClass="btn btn-warning" runat="server" CommandName="Move" CommandArgument='<%# Eval("ItemID") %>'> <i class="bi bi-copy me-1"></i><asp:Localize runat="server" meta:resourcekey="cmdMoveText"/> </asp:LinkButton>
					                &nbsp;
				                    <asp:LinkButton ID="cmdDetach" runat="server" 
 					                    CommandName="Detach" CommandArgument='<%# Eval("ItemID") %>'
					                    CssClass="btn btn-warning" OnClientClick="return confirm('Remove this item?');">
                                        <i class="bi bi-link-45deg me-1"></i><asp:Localize runat="server" meta:resourcekey="cmdDetachText"/>
                                    </asp:LinkButton>
			                    </ItemTemplate>
                            </asp:TemplateField>
				        </Columns>
			        </asp:GridView>
				    <asp:ObjectDataSource ID="odsServersPaged" runat="server" EnablePaging="True"
						    SelectCountMethod="GetVirtualMachinesCount"
						    SelectMethod="GetVirtualMachines"
						    SortParameterName="sortColumn"
						    TypeName="FuseCP.Portal.VirtualMachinesHelper"
						    OnSelected="odsServersPaged_Selected">
					    <SelectParameters>
						    <asp:QueryStringParameter Name="packageId" QueryStringField="SpaceID" DefaultValue="0" />						    
                            <asp:ControlParameter Name="filterColumn" ControlID="searchBox"  PropertyName="FilterColumn" />
                            <asp:ControlParameter Name="filterValue" ControlID="searchBox" PropertyName="FilterValue" />
					    </SelectParameters>
				    </asp:ObjectDataSource>
				    <br />
    				
				    <fcp:CollapsiblePanel id="secQuotas" runat="server"
                        TargetControlID="QuotasPanel" meta:resourcekey="secQuotas" Text="Quotas">
                    </fcp:CollapsiblePanel>
                    <asp:Panel ID="QuotasPanel" runat="server" Height="0" style="overflow:hidden">
                    
                        <table class="table table-borderless align-middle mb-0">
                            <tr>
                                <td><asp:Localize ID="locVpsQuota" runat="server" meta:resourcekey="locVpsQuota" Text="Number of VPS:"></asp:Localize></td>
                                <td><fcp:Quota ID="vpsQuota" runat="server" QuotaName="VPS.ServersNumber" /></td>
                            </tr>
                            <tr>
                                <td><asp:Localize ID="locRamQuota" runat="server" meta:resourcekey="locRamQuota" Text="RAM, MB:"></asp:Localize></td>
                                <td><fcp:Quota ID="ramQuota" runat="server" QuotaName="VPS.Ram" /></td>
                            </tr>
                            <tr>
                                <td><asp:Localize ID="locHddQuota" runat="server" meta:resourcekey="locHddQuota" Text="HDD, GB:"></asp:Localize></td>
                                <td><fcp:Quota ID="hddQuota" runat="server" QuotaName="VPS.Hdd" /></td>
                            </tr>
                        </table>
                    </asp:Panel>
			    </div>
                </div>
                </div>
                </div>
