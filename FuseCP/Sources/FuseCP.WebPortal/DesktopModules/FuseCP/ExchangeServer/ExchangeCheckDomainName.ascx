<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExchangeCheckDomainName.ascx.cs" Inherits="FuseCP.Portal.ExchangeServer.ExchangeCheckDomainName" %>
<%@ Register Src="../UserControls/SimpleMessageBox.ascx" TagName="SimpleMessageBox" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/QuotaViewer.ascx" TagName="QuotaViewer" TagPrefix="fcp" %>
<%@ Register Src="../UserControls/EnableAsyncTasksSupport.ascx" TagName="EnableAsyncTasksSupport" TagPrefix="fcp" %>

<fcp:EnableAsyncTasksSupport id="asyncTasks" runat="server"/>


				<div class="card-header">
                    <h3 class="card-title">
					<asp:Image ID="Image1" SkinID="ExchangeDomainName48" runat="server" />
					<asp:Localize ID="locTitle" runat="server" meta:resourcekey="locTitle" Text="Domain Names"></asp:Localize>
					-
					<asp:Literal ID="litDomainName" runat="server"></asp:Literal>
                        </h3>
				</div>

				<asp:Literal ID="TopComments" runat="server" meta:resourcekey="TopComments"></asp:Literal>

				<div class="card-body form-horizontal">
				    <fcp:SimpleMessageBox id="messageBox" runat="server" />
				    <br />

				    <asp:GridView ID="gvObjects" runat="server" AutoGenerateColumns="False" EnableViewState="true"
					    CssSelectorClass="NormalGridView" OnRowCommand="gvObjects_RowCommand">
					    <Columns>
						    <asp:TemplateField HeaderText="gvObjectsDisplayName">
							    <ItemStyle></ItemStyle>
							    <ItemTemplate>
							        <asp:Image ID="img1" runat="server" ImageUrl='<%# GetObjectImage(Eval("ObjectName").ToString(),(int)Eval("ObjectType")) %>' ImageAlign="AbsMiddle" />
								    <asp:hyperlink id="lnk1" runat="server"
									    NavigateUrl='<%# GetEditUrl(Eval("ObjectName").ToString(),(int)Eval("ObjectType"),Eval("ObjectID").ToString(),Eval("OwnerID").ToString()) %>'>
									    <%# Eval("DisplayName") %>
								    </asp:hyperlink>
							    </ItemTemplate>
						    </asp:TemplateField>
						    <asp:TemplateField HeaderText="gvObjectsObjectType">
							    <ItemStyle></ItemStyle>
							    <ItemTemplate>							        
									<%# GetObjectType(Eval("ObjectName").ToString(),(int)Eval("ObjectType")) %>
							    </ItemTemplate>
						    </asp:TemplateField>

						    <asp:TemplateField HeaderText="gvObjectsView">
							    <ItemStyle></ItemStyle>
							    <ItemTemplate>	
								    <asp:hyperlink id="lnk2" runat="server"
									    NavigateUrl='<%# GetEditUrl(Eval("ObjectName").ToString(),(int)Eval("ObjectType"),Eval("ObjectID").ToString(),Eval("OwnerID").ToString()) %>'>
									    <asp:Literal id="lnkView" runat="server" Text="View" meta:resourcekey="lnkView" />
								    </asp:hyperlink>
							    </ItemTemplate>
						    </asp:TemplateField>

						    <asp:TemplateField HeaderText="gvObjectsDelete">
							    <ItemStyle></ItemStyle>
							    <ItemTemplate>							        
                                    <asp:LinkButton id="lnkDelete" runat="server" Text="Delete" meta:resourcekey="lnkDelete" 
                                        OnClientClick="if(!confirm('Are you sure you want to delete ?')) return false; else ShowProgressDialog('Deleting ...');"
                                        CommandName="DeleteItem" CommandArgument='<%# Eval("OwnerID").ToString() + "," + Eval("ObjectType").ToString() + "," + Eval("DisplayName") %>'
                                        Visible='<%# AllowDelete(Eval("ObjectName").ToString(), (int)Eval("ObjectType")) %>' />
							    </ItemTemplate>
						    </asp:TemplateField>


					    </Columns>
				    </asp:GridView>


				    <br />
              				</div>
				    <div class="card-footer">
                        <asp:LinkButton id="btnBack" CssClass="btn btn-warning" runat="server" OnClick="btnBack_Click"> <i class="bi bi-arrow-left me-1"></i><asp:Localize runat="server" meta:resourcekey="btnBack"/> </asp:LinkButton>
				    </div>
