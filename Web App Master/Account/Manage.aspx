<%@ Page Title="Manage Account" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="Web_App_Master.Account.Manage" %>

<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
                
 <div class="bg-shaded"></div>
    <div class="row">
        <div class="col-md-offset-2 col-md-8">
                    <div class="col-md-12">
              <div class="row fg-white shadow-metro-black">
        <div>
        <asp:PlaceHolder runat="server" ID="successMessage" Visible="false" ViewStateMode="Disabled">
            <p class="text-success"><%: SuccessMessage %></p>
        </asp:PlaceHolder>
    </div>
            <h2 class="fg-white shadow-metro-black"><%: Title %>.</h2>
            <div class="form-horizontal">
                <h2 class="fg-white shadow-metro-black">Change your account settings</h2>
                <hr />
                <dl class="dl-horizontal">
                    <dt ><strong class="fg-white shadow-metro-black">Password:</strong></dt>
                    <dd>
                        <asp:HyperLink CssClass="fg-white shadow-metro-black" NavigateUrl="/Account/ManagePassword" Text="[Change]" Visible="false" ID="ChangePassword" runat="server" />
                        <asp:HyperLink CssClass="fg-white" NavigateUrl="/Account/ManagePassword" Text="[Create]" Visible="false" ID="CreatePassword" runat="server" />
                    </dd>
                    <dd>
                        <% if (TwoFactorEnabled)
                          { %> 
                        <%--
                        Enabled
                        <asp:LinkButton Text="[Disable]" runat="server" CommandArgument="false" OnClick="TwoFactorDisable_Click" />
                        --%>
                        <% }
                          else
                          { %> 
                        <%--
                        Disabled
                        <asp:LinkButton Text="[Enable]" CommandArgument="true" OnClick="TwoFactorEnable_Click" runat="server" />
                        --%>
                        <% } %>
                    </dd>
                </dl>
            </div>
        </div>
    </div>

        </div>
    </div>				
    

  

  
        

</asp:Content>
