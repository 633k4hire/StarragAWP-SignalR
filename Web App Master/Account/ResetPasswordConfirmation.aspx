<%@ Page Title="Password Changed" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ResetPasswordConfirmation.aspx.cs" Inherits="Web_App_Master.Account.ResetPasswordConfirmation" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="bg-shaded"></div>
    <div class="row">
        <div class="col-md-offset-4 col-md-4">
            <h2 class="fg-white shadow-metro-black"><%: Title %>.</h2>
            <div class="fg-white shadow-metro-black">
               <h4> Your password has been changed. Click <asp:HyperLink ID="login" runat="server" NavigateUrl="~/Account/Login" CssClass="fg-starrag shadow-metro-white"><b>here</b></asp:HyperLink> to login </h4>
            </div>
        </div>
    </div>
   
</asp:Content>
