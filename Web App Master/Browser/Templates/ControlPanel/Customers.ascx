<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Customers.ascx.cs" Inherits="Web_App_Master.Browser.Templates.ControlPanel.Customers" %>
<style type="text/css"> 


</style>
<nav class="navbar navbar-inverse  bg-grayDark" style="border-bottom-left-radius:8px !important;border-bottom-right-radius:0px !important;border-top-left-radius:0px !important;border-top-right-radius:0px !important; position:fixed; padding-right:25px; width:auto; right:0px; text-align:left !important; margin-top:-11px !important; margin-left:7px!important; z-index:25000; vertical-align:top">
  <div class="container-fluid">
    <div class="navbar-header">
      <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#CustomerMenu" style="float:left !important">
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>                        
      </button>
      
    </div>
    <div id="CustomerMenu" class="collapse navbar-collapse" >
      <ul class="nav navbar-nav starrag-menu">
          <li class="dropdown">
            <a class="dropdown-toggle" data-toggle="dropdown" href="#">Sort
            <span class="caret"></span></a>
            <ul class="dropdown-menu starrag-menu">
                <li><a href="#" >Default</a></li>
                
            </ul>
          </li>
          <li style="margin-top:10px;">
               <div class="  text" data-role="input" style="width:250px !important; vertical-align:middle!important">
                   <asp:TextBox ID="avSearchString" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
               </div>
          </li>
          <li>             
               <asp:LinkButton ID ="AssetSearchBtn" ClientIDMode="Static" runat="server"  OnClick="SearchButton_Click"><span class="glyphicon glyphicon-search""></span></asp:LinkButton>
          </li>     
          
          
      </ul>   
    </div>
  </div>
</nav>

<div style="height:40px"></div>

        <asp:UpdatePanel runat="server" ID="CustomerUpdatePanel" UpdateMode="Conditional">
            <ContentTemplate>
                <div class=" border-bottom-blue" style="margin:0px !important">
                    <span class="fg-black shadow-metro-black"><strong><h4>Customers</h4></strong></span>
                </div>
                <asp:Repeater ID="CustomersRepeater" ClientIDMode="Static" runat="server">
                                <ItemTemplate>
                                    <div class="border-bottom-blue" style="overflow:hidden">                                        
                                            <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                <asp:Button Height="25" Width="15" ToolTip="Delete Customer" ID="DeleteCustomerBtn" CssClass="btn btn-sm" runat="server" Text="X" Font-Bold="true" CommandName='<%#Eval("CompanyName")%>' CommandArgument='<%#Eval("Postal")%>'  OnCommand="DeleteCustomerBtn_Command"/>
                                                <a title="Edit Customers" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" href="#">(Edit)</a>    
                                                <span ><%# Eval("CompanyName") %>&nbsp-&nbsp<%# Eval("Address") %> ,<%# Eval("Postal") %> </span>
                                                <a href='mailto:<%# Eval("Email") %>'>Email: <%# Eval("Email") %></a>
                                            </div>  
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="AssetSearchBtn" EventName="click" />
            </Triggers>
        </asp:UpdatePanel>

