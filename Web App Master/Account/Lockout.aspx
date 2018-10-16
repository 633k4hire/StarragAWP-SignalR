<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Lockout.aspx.cs" Inherits="Web_App_Master.Account.Lockout" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="bg-shaded"></div>
    <div class="row">
        <div class="col-md-offset-4 col-md-4">
            <hgroup>
                    <h1 class="fg-white shadow-metro-black">Locked out.</h1>
                    <h2 class=" fg-amber shadow-metro-black">This account has been locked out, please try again later.</h2>
            </hgroup>
        </div>
    </div>
    
</asp:Content>
