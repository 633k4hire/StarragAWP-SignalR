<%@ Page Title="Customers" Async="true" Language="C#" MasterPageFile="~/Site.Master" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="Customers.aspx.cs" Inherits="Web_App_Master.Pages.Customers" %>
<asp:Content ID="CustomerPage" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../Browser/BrowserContent/browser.css" rel="stylesheet" />
    <link href="../Browser/BrowserContent/L0XX0R.css" rel="stylesheet" />
    <script type="text/javascript">    
        function CP_EditCustomer(namepostal) {
            $.ajax({
                type: 'POST',
                url: '/Account/AssetController.aspx/GetCustomer',
                data: "{'customer':'" + namepostal + "'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: ShowCustomer
            });     
        }
        function ShowCustomer(customer) {
            $("#CustAddr").val(customer.Address);
            $("#CustAddr2").val(customer.Address2);
            $("#CustCty").val(customer.City);
            $("#CustState").val(customer.State);
            $("#CustPostal").val(customer.Postal);
            $("#CustCountry").val(customer.Country);
            $("#CustName").val(customer.CompanyName);
            $("#CustEmail").val(customer.Email);
            $("#CustPhone").val(customer.Phone);
            $("#IsCustEdit").val("true");
            ShowDiv('CreateCustomerModal');
        }
    </script>
    <asp:UpdatePanel ID="MasterCustomerUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="false" runat="server">
        <ContentTemplate>
                <asp:UpdatePanel runat="server"  ID="CustomerUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" >
                    <ContentTemplate>
                        <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                            <span class="fg-black shadow-metro-black"><strong><h3>Customers</h3></strong></span>
                        </div>
                        <asp:Repeater ID="CustomersRepeater" ClientIDMode="Static" runat="server" OnItemCommand="CustomersRepeater_ItemCommand" OnItemDataBound="CustomersRepeater_ItemDataBound">
                                        <ItemTemplate>
                                            <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                    <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                        <asp:Button ClientIDMode="AutoID" Height="25" Width="15" ToolTip="Delete Customer" ID="DeleteCustomerBtn" CssClass="btn btn-sm" runat="server" Text="X" Font-Bold="true" CommandName='<%#Eval("CompanyName")%>' CommandArgument='<%#Eval("Postal")%>' OnClick="DeleteCustomerBtn_Click" />
                                                        <a title="Edit Customers" onclick="CP_EditCustomer('<%# Eval("CompanyName") %>-dd-<%# Eval("Postal") %>')" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" href="#">(Edit)</a>    
                                                        <span ><%# Eval("CompanyName") %>&nbsp-&nbsp<%# Eval("Address") %> ,<%# Eval("Postal") %> </span>
                                                        <a href='mailto:<%# Eval("Email") %>'>Email: <%# Eval("Email") %></a>
                                                    </div>  
                                            </div>
                                        </ItemTemplate>
                                                    
                                    </asp:Repeater>
                    </ContentTemplate>
                </asp:UpdatePanel>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
