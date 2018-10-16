<%@ Page Title="Log in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Web_App_Master.Account.Login" Async="true" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
        
    <div class="bg-shaded" style="margin-top:2px !important; text-align:center"></div>

    <div class="row fg-white shadow-metro-black">
        <div class=" col-md-offset-4 col-md-4" >
            <h2 class="fg-white shadow-metro-black"><%: Title %></h2>
            <section id="loginForm">
                <div class=" form-horizontal">
                    
                    <hr />
                    <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="FailureText" />
                        </p>
                    </asp:PlaceHolder>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-2 control-label">Email</asp:Label>
                        <div class="col-md-10">
                            <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Email"
                                CssClass="text-danger" ErrorMessage="The email field is required." />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-2 control-label">Password</asp:Label>
                        <div class="col-md-10">
                            <asp:TextBox  runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="The password field is required." />
                        </div>
                    </div>
                    <div class="form-group">
                           <asp:CheckBox runat="server" ID="RememberMe"  CssClass="col-md-2 control-label" />
                        <div class="col-md-10" style="text-align:left; float:left;">
                                <asp:Label  runat="server" AssociatedControlID="RememberMe"><b class="shadow-metro-black" >Remember me?</b></asp:Label>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-md-12">
                            <asp:Button runat="server" ID="LoginBtn" OnClick="LogIn" Text="Log in" CssClass="btn btn-default" />
                            &nbsp;&nbsp;
                            <asp:Button runat="server" ID="ResendConfirm"  OnClick="SendEmailConfirmationToken" Text="Resend confirmation" Visible="false" CssClass="btn btn-default" />
                        </div>
                    </div>
                </div>
                <div>
                    <asp:HyperLink runat="server" ID="RegisterHyperLink" ViewStateMode="Disabled" CssClass="fg-white">Register as a new user</asp:HyperLink>
                    </div><div>
                    <asp:HyperLink runat="server" ID="ForgotPasswordHyperLink" ViewStateMode="Disabled" CssClass="fg-white">Forgot your password?</asp:HyperLink>
                    </div>
            </section>
        </div>

      <!--  <div class="col-md-4">
            <section id="socialLoginForm">
                <uc:OpenAuthProviders runat="server" ID="OpenAuthLogin" />
            </section>
        </div>-->
    </div>
</asp:Content>
