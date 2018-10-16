<%@ Page Title="Control Panel" Async="true" EnableEventValidation="false" AsyncTimeout="60" ValidateRequest="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ControlPanel.aspx.cs" Inherits="Web_App_Master.Browser.ControlPanel" %>
<asp:Content ID="ControlPanelContent" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        .inner {
            padding-left:0px !important;
            padding-right:0px !important;
            padding-top:0px !important;
        }
        .panel-heading{
            padding:2px 15px 2px 15px!important;
        }
        a{
            color: black;
        }
        a:hover a:focus{
            text-decoration:none;
            color: rgba(0, 175, 240, 1) !important;
            text-shadow: 0px 0px 5px rgba(0, 0, 0, 1) !important;
        }
    </style>
         <%--<link href="BrowserContent/awp.css" rel="stylesheet" />--%>
    <link href="BrowserContent/browser.css" rel="stylesheet" />
    <link href="BrowserContent/jquery-ui.css" rel="stylesheet" />
    <link href="BrowserContent/jquery.contextMenu.css" rel="stylesheet" />
    <link href="BrowserContent/L0XX0R.css" rel="stylesheet" />

    <script src="BrowserScripts/jquery-ui.js"></script>
    <script src="BrowserScripts/jquery.contextMenu.js"></script>
    <script src="BrowserScripts/jquery.ui.position.js"></script>   
    <script src="BrowserScripts/browser.js"></script>
    <script src="BrowserScripts/jquery-resizable.js"></script>    
    <script type="text/javascript">
        //SuperFunction
        function Super(cmd,argss) {
            $("#SuperButtonCommand").val(cmd);
            $("#SuperButtonArg").val(argss);
            $("#SuperButton").click();
        }

        function UpdateFooterStatus(stat) {
            $("#FooterStatusLabel").val(stat);
        }

        function NoEdit() {
            $("#MainContent_IsCustEdit").val("false");
            $("#MainContent_IsOPEdit").val("false");
            $("#MainContent_IsFPEdit").val("false");
            $("#MainContent_IsStaticEdit").val("false");
            $("#MainContent_IsAssetEdit").val("false");
            return false;
        }

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
        function ShowCustomer(msg) {
            var customer = msg.d;
            $("#MainContent_CustAddr").val(customer.Address);
            $("#MainContent_CustAddr2").val(customer.Address2);
            $("#MainContent_CustCty").val(customer.City);
            $("#MainContent_CustState").val(customer.State);
            $("#MainContent_CustPostal").val(customer.Postal);
            $("#MainContent_CustCountry").val(customer.Country);
            $("#MainContent_CustName").val(customer.Attn);
            $("#MainContent_CustCompany").val(customer.CompanyName);
            $("#MainContent_CustEmail").val(customer.Email);
            $("#MainContent_CustPhone").val(customer.Phone);
            $("#MainContent_IsCustEdit").val("true");
            ShowDiv('CreateCustomerModal');
        }

        function CP_EditOP(email) {
            $.ajax({
                type: 'POST',
                url: '/Account/AssetController.aspx/GetOP',
                data: "{'email':'" + email + "'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: ShowOP
            });   
        }
        function ShowOP(msg) {
            $("#MainContent_IsOPEdit").val("true");
            var namebox = $("#MainContent_COPNameTextBox");
            namebox.val(msg.d.Name);
            $("#MainContent_COPEmailTextBox").val(msg.d.Email);
            ShowDiv('CreateOfficePersonnelModal');

        }

        function CP_EditFP(email) {
            $.ajax({
                type: 'POST',
                url: '/Account/AssetController.aspx/GetFP',
                data: "{'email':'" + email + "'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: ShowFP
            });   
        }
        function ShowFP(op) {

            $("#MainContent_IsFPEdit").val("true");
            $("#MainContent_CFPNameTextBox").val(op.d.Name);
            $("#MainContent_CFPEmailTextBox").val(op.d.Email);
            ShowDiv('CreateFieldPersonnelModal');
        }

        function CP_EditStatic(email) {
            $.ajax({
                type: 'POST',
                url: '/Account/AssetController.aspx/GetStatic',
                data: "{'email':'" + email + "'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: ShowStatic
            });   
        }
        function ShowStatic(op) {

            $("#MainContent_IsStaticEdit").val("true");
            $("#MainContent_CSENameTextBox").val(op.d.Name);
            $("#MainContent_CSEEmailTextBox").val(op.d.Email);
            ShowDiv('CreateStaticEmailModal');
        }
         function CP_EditAsset(num) {
            $.ajax({
                type: 'POST',
                url: '/Account/AssetController.aspx/GetAsset',
                data: "{'num':'" + num + "'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: ShowAsset
            });   
        }
        function ShowAsset(msg) {
            var asset = msg.d;
            
            //$("MainContent_#IsAssetEdit").val("true");
            //$("#MainContent_AssetImgBox").attr("src",(asset.FirstImage));
            //$("#MainContent_AssetName").val(asset.AssetName);
            //$("#MainContent_AssetNumber").val(asset.AssetNumber);
            //$("#MainContent_Weight").val(asset.Weight);
            //$("#MainContent_DescriptionText").val(asset.Description);
            //ShowDiv('CreateAssetModal');
        }

    </script>
        <div id="appcontainer" style="z-index:900000 !important">           
        <div id="toolbar" style="height:0px !important;">       
            <div class="l0x-toolbar">
                <asp:UpdatePanel runat="server" ID="AppToolbarUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="true" >
                    <ContentTemplate>                     
     
                    </ContentTemplate>
                   
                </asp:UpdatePanel>                
            </div>
        </div>
        <div id="browserbox">
            <%--Modals--%>
            <%--CREATE OFFICE PERSONNEL--%>
            <div class="app-modal transition-bottom" id="CreateOfficePersonnelModal" style="display:none">
                <span class="app-modal-closer" onclick="HideDiv('CreateOfficePersonnelModal')" ><strong>X</strong></span>
                 <span><h4>Create Office Personnel</h4></span>
                <div>
                    <input runat="server" id="COPNameTextBox" type="text" class=" form-control" placeholder="Name..." /><br />
                    <input runat="server" id="COPEmailTextBox" type="text" class="form-control" placeholder="Email..." /><br />
                    <input runat="server" id="IsOPEdit" type="hidden" />
                </div>
                <div>
                    <asp:Button OnClientClick="HideDiv('CreateOfficePersonnelModal')"  ID="COPOkBtn" CssClass="btn"  ClientIDMode="Static" runat="server" Text="Ok" OnClick="COPOkBtn_Click" />
                </div>        
            </div>
            <%--CREATE FIELD PERSONNEL--%>
            <div class="app-modal transition-bottom" id="CreateFieldPersonnelModal" style="display:none">
                <span class="app-modal-closer" onclick="HideDiv('CreateFieldPersonnelModal')" ><strong>X</strong></span>
                 <span><h4>Create Field Personnel</h4></span>
                <div>
                    <input runat="server" id="CFPNameTextBox" type="text" class=" form-control" placeholder="Name..." /><br />
                    <input runat="server" id="CFPEmailTextBox" type="text" class="form-control" placeholder="Email..." /><br />
                    <input runat="server" id="IsFPEdit" type="hidden" />
                </div>
                <div>
                    <asp:Button OnClientClick="HideDiv('CreateFieldPersonnelModal')"  ID="CFPOkBtn" CssClass="btn"  ClientIDMode="Static" runat="server" Text="Ok" OnClick="CFPOkBtn_Click" />
                </div>        
            </div>
            <%--Create Static Email--%>
            <div class="app-modal transition-bottom" id="CreateStaticEmailModal" style="display:none">
                <span class="app-modal-closer" onclick="HideDiv('CreateStaticEmailModal')" ><strong>X</strong></span>
                 <span><h4>Create Static Email</h4></span>
                <div>
                    <input runat="server" id="CSENameTextBox" type="text" class=" form-control" placeholder="Name..." /><br />
                    <input runat="server" id="CSEEmailTextBox" type="text" class="form-control" placeholder="Email..." /><br />
                    <input runat="server" id="IsStaticEdit" type="hidden" />
                </div>
                <div>
                    <asp:Button OnClientClick="HideDiv('CreateStaticEmailModal')"  ID="CSEOkBtn" CssClass="btn"  ClientIDMode="Static" runat="server" Text="Ok" OnClick="CSEOkBtn_Click" />
                </div>        
            </div>
            <%--Create Customer--%>
            <div class="app-modal transition-bottom" id="CreateCustomerModal" style="display:none">
                <span class="app-modal-closer" onclick="HideDiv('CreateCustomerModal')" ><strong>X</strong></span>
                 <span><h4>Create Customer</h4></span>
                <div>
                    <input runat="server" id="CustCompany" type="text" class="form-control" placeholder="CompanyName">
                    <input runat="server" id="CustAddr" type="text"  class="form-control" placeholder="Address">
                    <input runat="server" id="CustAddr2" type="text" class="form-control" placeholder="Address Line #2">
                    <input runat="server" id="CustCty" type="text" class="form-control" placeholder="City">
                    <input runat="server" id="CustState" type="text"  class="form-control" placeholder="State">
                    <input runat="server" id="CustPostal" type="text"  class="form-control" placeholder="Postal">
                    <input runat="server" id="CustCountry" type="text"  value="US" class="form-control" placeholder="Country">
                    <input runat="server" id="CustName" type="text" class="form-control" placeholder="Attn">
                    <input runat="server" id="CustEmail" type="text" class="form-control" placeholder="Email">
                    <input runat="server" id="CustPhone" type="text" class="form-control" placeholder="Phone">  
                    <input runat="server" id="IsCustEdit" type="hidden" />
                </div>
                <div>
                    <asp:Button OnClientClick="HideDiv('CreateCustomerModal')"  ID="CCOkBtn" CssClass="btn"  ClientIDMode="Static" runat="server" Text="Ok" OnClick="CCOkBtn_Click" />
                </div>        
            </div>
            <%--Create Certificate--%>
            <div class="app-modal transition-bottom" id="CreateCertificateModal" style="display:none">
                <span class="app-modal-closer" onclick="HideDiv('CreateCertificateModal')" ><strong>X</strong></span>
                 <span><h4>Create Certificate</h4></span>
                <div>
                   
                </div>
                <div>
                    <asp:Button OnClientClick="HideDiv('CreateCertificateModal')"  ID="CCertOkBtn" CssClass="btn"  ClientIDMode="Static" runat="server" Text="Ok" OnClick="CCertOkBtn_Click" />
                </div>        
            </div>
           <%--Create Asset Email--%>
            <div class="app-modal transition-bottom" id="CreateAssetModal" style="display:none">
                <span class="app-modal-closer" onclick="HideDiv('CreateAssetModal')" ><strong>X</strong></span>
                 <span><h4>Create Asset</h4></span>
                <div class="row ShowScroll">
                    <input runat="server" id="IsAssetEdit" type="hidden" />
                    <div class="col-md-3">
                        <asp:UpdatePanel ID="AssetImageUploadUpdatePanel" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>   
                                <asp:Image BorderStyle="Solid" BorderColor="LightGray" BorderWidth="1" ImageUrl="/Images/transparent.png" runat="server" ID="AssetImgBox"  ImageAlign="Middle" Width="100%" />
                                <asp:FileUpload ID="AssetImageUpload" AllowMultiple="true" runat="server" CssClass="form-control"/>
                                <asp:Button OnClientClick="HideDiv('CreateAssetModal')"  ID="UploadImageBtn" CssClass="btn"  ClientIDMode="Static" runat="server" Text="Upload" OnClick="UploadImageBtn_Click" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="UploadImageBtn" EventName="click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col-md-9">
                        <asp:UpdatePanel ID="createIdUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true" >
                            <ContentTemplate>
                                <div class="bg-starrag rounded" style="width:90%"><asp:PlaceHolder ID="AssetModalErrorMessagePlaceHolder" runat="server" Visible="false"></asp:PlaceHolder></div>
                                <div class="row  fg-black ">
                                    <div class="col-md-10">
                                         <span style="font-weight:bold;" ><strong>Asset Name</strong></span>
                                         <asp:TextBox runat="server" ID="AssetName" CssClass="form-control" TextMode="SingleLine" />
                                    </div>
                                    <div class="col-md-2">
                                         <span style="font-weight:bold;" ><strong>Asset Number</strong></span>
                                         <asp:TextBox runat="server" ID="AssetNumber" CssClass="form-control" TextMode="SingleLine" />
                                    </div>
                                </div> 
                                <div class="row  fg-black ">
                                <div class="col-md-3">
                                    <span style="font-weight:bold;" >Weight (lbs)</span>
                                    <asp:TextBox runat="server" ID="Weight" CssClass="form-control" TextMode="Number" />
                               </div>
                                <div class="col-md-9">
                                    <div class="row">
                                        <div class="col-md-2" style="text-align:center">
                                        <span style="font-weight:bold;" >Calibrated</span>
                                        <div><asp:CheckBox ID="CalibratedCheckBox" runat="server" />
                                        </div>
                                        </div>
                                        <div class="col-md-5">
                                            <span style="font-weight:bold;" >Calibration Company</span>
                                            <asp:TextBox runat="server" ID="CalCompanyText" CssClass="form-control" TextMode="SingleLine" />
                                        </div>
                                        <div class="col-md-5">
                                            <span style="font-weight:bold;" >Calibration Period</span>
                                            <asp:TextBox runat="server" ID="CalPeriodText" CssClass="form-control" TextMode="Number" />
                                        </div>                        
                                    </div>
                                </div>
                                </div>
                                <div class="row  fg-black ">
                                    <div class="col-md-12">
                                         <span style="font-weight:bold;" ><strong>Description</strong></span>
                                         <asp:TextBox Height="150px" runat="server" ID="DescriptionText" CssClass="form-control" TextMode="MultiLine" />
                                    </div>
                                </div> 
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="CreateAssetModalOkBtn" EventName="click" />
                            </Triggers>
                        </asp:UpdatePanel>

                        <asp:Button OnClientClick="HideDiv('CreateAssetModal')"  ID="CreateAssetModalOkBtn" CssClass="btn"  ClientIDMode="Static" runat="server" Text="Ok" OnClick="CreateAssetModalOkBtn_Click" />
                    </div>
                </div>
                   
            </div>

            <%--Browser--%>
            <div class="wrap">
                <div class="resizable resizable1">
                    <div class="inner">     
                         <asp:UpdatePanel runat="server" ID="AppLeftPanelUpdatePanel" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:TreeView 
                                     EnableViewState="true"                                    
                                     ViewStateMode="Enabled"  
                                     ShowLines="false" 
                                     runat="server"
                                     ID ="DirTree" 
                                     ClientIDMode="Static" 
                                     NodeWrap="false"   
                                ></asp:TreeView>    
                            </ContentTemplate>
                            
                        </asp:UpdatePanel>
                        
                    </div>  
                </div>
                <div class="resizable resizable2">
                    <div class="inner">   
                        <asp:UpdatePanel runat="server" ID="AppRightPanelUpdatePanel" ChildrenAsTriggers="true" UpdateMode="Conditional" >
                            <ContentTemplate>
                                     <nav class="navbar navbar-inverse  bg-grayDark" style="border-bottom-left-radius:8px !important;border-bottom-right-radius:0px !important;border-top-left-radius:0px !important;border-top-right-radius:0px !important; position:fixed; padding-right:25px; width:auto; right:0px; text-align:left !important; margin-top:-23px !important; margin-left:7px!important; z-index:25000; vertical-align:top">
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
                                                    <a class="dropdown-toggle" data-toggle="dropdown" href="#"><i class="glyphicon glyphicon-cog"></i>
                                                    <span class="caret"></span></a>
                                                    <ul class="dropdown-menu starrag-menu">
                                                        <li><a href="#" onclick="ToggleList();" >Toggle List</a></li>                
                                                    </ul>
                                                  </li>
                                                  <li style="margin-top:10px; width:200px; height:34px; margin-top:20px; vertical-align:bottom !important">
                                                       <div class="text"  style="width:200px !important; ">
                                                           <asp:TextBox ID="avSearchString" Width="200px" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                                       </div>
                                                  </li>
                                                  <li>             
                                                       <asp:LinkButton ID ="AssetSearchBtn" ClientIDMode="Static" runat="server"  OnClick="SearchButton_Click"><span class="glyphicon glyphicon-search""></span></asp:LinkButton>
                                                  </li>                                                     
                                                  <li class="dropdown">
                                                    <a class="dropdown-toggle" data-toggle="dropdown" href="#"><i class="glyphicon glyphicon-plus av-hand-cursor"></i>
                                                    <span class="caret"></span></a>
                                                    <ul class="dropdown-menu pull-right starrag-menu">
                                                        <li><a href="#" onclick="NoEdit(); ShowDiv('CreateStaticEmailModal');"  >Static Email</a></li>   
                                                        <li><a href="#" onclick="NoEdit(); ShowDiv('CreateCustomerModal');"  >Customer</a></li>  
                                                        <li><a href="#" onclick="NoEdit(); ShowDiv('CreateOfficePersonnelModal'); "  >Office Personnel</a></li>  
                                                        <li><a href="#" onclick="NoEdit(); ShowDiv('CreateFieldPersonnelModal');" >Field Personnel</a></li>  
                                                        <li><a href="#" onclick="NoEdit(); ShowDiv('CreateAssetModal');" >Asset</a></li>  
                                                        <li><a href="#" onclick="NoEdit(); ShowDiv('CreateCertificateModal');" >Certificate</a></li>  
                                                    </ul>
                                                  </li>
                                                  <li id="ControlPanelSave" runat="server">
                                                      <asp:LinkButton ID ="ControlPanelSaveBtn" OnClientClick="ShowLoader();  setTimeout(HideLoader, 2000);" ToolTip="Save" ClientIDMode="Static" runat="server"  OnClick="ControlPanelSaveBtn_Click"><i class="glyphicon glyphicon-floppy-disk av-hand-cursor fg-starrag"></i></asp:LinkButton>
                                                  </li>                                                    
                                              </ul>   
                                            </div>
                                          </div>
                                        </nav>
                                <asp:MultiView ID ="AppRightPanelMultiView" EnableViewState="true"  ActiveViewIndex="0" runat="server">
                                    <%--Settings View--%>
                                    <asp:View ID="SettingsView" runat="server">      
                                        <asp:UpdatePanel runat="server" ID="SettingsViewUpdatePanel" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:MultiView ID="SettingsMultiview" ActiveViewIndex="0" runat="server">
                                                    <asp:View ID="GeneralSettingsView" runat="server">
                                                        <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                            <span class="shadow-metro-black"><strong><h3>Control Panel</h3></strong></span>
                                                        </div>
                                                        
                                                        <fieldset class="groupbox-border">
                                                            <legend class="groupbox-border fg-l0xx0r">Database Settings</legend>
                                                            <asp:CheckBox Text="Test Mode (Disable Emails)" ID="TESTMODE_CHECKBOX" runat="server" />
                                                        </fieldset>
                                                        <fieldset class="groupbox-border">
                                                            <legend class="groupbox-border fg-l0xx0r">Quick Admin</legend>
                                                                <asp:Button ToolTip="Sort Images" ID="SortAssetImages" OnClientClick="UpdateFooterStatus('Sort Running...');" CssClass="btn btn-sm" runat="server" Text="Sort Asset Images" Font-Bold="true" OnClick="SortAssetImages_Click"  />
                                                                <asp:Button ToolTip="Create Directories" ID="CreateDirectoriesBtn" CssClass="btn btn-sm" runat="server" Text="Developer Test Button" Font-Bold="true" OnClick="DeveloperAction_Click"  />
                                                        </fieldset>
                                                    </asp:View>
                                                    <asp:View ID="UserSettingsView" runat="server">
                                                        <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                            <span class="shadow-metro-black"><strong><h3>User Settings</h3></strong></span>
                                                        </div>
                                                        <hr />
                                                        <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                            <span class="shadow-metro-black"><strong><h4>Role Action</h4></strong></span>
                                                        </div>
                                                        <div class="border-bottom-blue" style="overflow:hidden; padding-left:15px">
                                                            
                                                            <span>User</span>
                                                            <asp:DropDownList ViewStateMode="Enabled" Visible="true"  ClientIDMode="Static" ID='UserDropDownList' AppendDataBoundItems="true" runat="server" DataSource='<%#GetUserNames() %>'  CssClass="dropdown-button" >
                                                                <asp:ListItem Text="--Select One--" Value="" /> 
                                                            </asp:DropDownList>
                                                            <span>Role</span>
                                                            <asp:DropDownList  ViewStateMode="Enabled" Visible="true"  ClientIDMode="Static" ID='RoleDropDown' AppendDataBoundItems="true" runat="server" DataSource='<%#GetRoleNames() %>'  CssClass="dropdown-button" >
                                                                <asp:ListItem Text="--Select One--" Value="" /> 
                                                            </asp:DropDownList>
                                                            <asp:Button ID="CopyUserToRoleBtn" runat="server" OnClick="CopyUserToRoleBtn_Click" CssClass="btn btn-sm" Text="Copy To" />
                                                            <asp:Button ID="ChangeUserToRoleBtn" runat="server" OnClick="ChangeUserToRoleBtn_Click" CssClass="btn btn-sm" Text="Change To" />
                                                        </div>
                                                        <hr />
                                                        <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                            <span class="shadow-metro-black"><strong><h4>Roles</h4></strong></span>
                                                        </div>
                                                        <asp:UpdatePanel ChildrenAsTriggers="true" ID="RolesUpdatePanel" EnableViewState="true" ViewStateMode="Enabled" ClientIDMode="Static" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <asp:Repeater ID="RolesAndUsersRepeater" ClientIDMode="Static" runat="server" ViewStateMode="Enabled" EnableViewState="true">
                                                                    <HeaderTemplate>
                                                                        <div class="panel-group" id="rolesAccordion">
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <div class="panel panel-default">
                                                                            <div class="panel-heading">
                                                                            <h3 class="panel-title text-left">
                                                                                <a data-toggle="collapse" data-parent="#rolesAccordion" href='#collapse<%# Container.ItemIndex + 1%>'>
                                                                                    <h5><%# Eval("RoleName")%></h5>
                                                                                </a>
                                                                            </h3>
                                                                            </div>
                                                                            <div id='collapse<%# Container.ItemIndex + 1%>' class="panel-collapse collapse">
                                                                            <div class="panel-body text-left">
                                                                                <!--Repeaters for users of this role-->
                                                                                <asp:Repeater DataSource='<%# Eval("RoleUsers")%>' ViewStateMode="Enabled" EnableViewState="true" runat="server" >
                                                                                    <ItemTemplate>
                                                                                        <div class="row bg-sg-box">                                           
                                                                                                <div class="col-sm-12  text-left bg-white" style="width:auto !important">
                                                                                                    <a href="#" class="btn btn-sm" title="Delete" onclick="Super('delete_role','<%#Eval("UserId")%>-dd-<%#Eval("RoleId") %>');" >X</a>
                                                                                                    
                                                                                                <strong><%# Eval("UserName")%></strong> 
                                                                                                </div>                      
                                                                                        </div>                                     
                                                                                    </ItemTemplate>
                                                                                </asp:Repeater>
                                                                            </div>
                                                                            </div>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                    <FooterTemplate>
                                                                        </div>
                                                                    </FooterTemplate>
                                                                </asp:Repeater>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>

                                                        <hr />

                                                        <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                            <span class="shadow-metro-black"><strong><h4>Users</h4></strong></span>
                                                        </div>
                                                        <asp:UpdatePanel ID="UserUpdatePanel" ChildrenAsTriggers="true" ClientIDMode="Static" UpdateMode="Conditional" runat="server">
                                                            <ContentTemplate>
                                                               <asp:Repeater ID ="UserRepeater" ClientIDMode="Static" runat="server">
                                                                   <ItemTemplate>
                                                                    
                                                                          <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                                <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                                    <a href="#" class="btn btn-sm" title="Delete User" onclick="Super('delete_user','<%#Eval("UserId")%>-dd-<%#Eval("RoleId") %>');" >X</a>
                                                                                    <a href="#" class="btn btn-sm" title="Approve User" onclick="Super('approve_user','<%#Eval("UserId")%>-dd-<%#Eval("RoleId") %>');" >Approve</a>
                                                                                    <span><%# Eval("UserName")%></span>
                                                                                </div>  
                                                                        </div>
                                                                   </ItemTemplate>
                                                               </asp:Repeater>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </asp:View>
                                                    <asp:View ID="DatabaseSettingsView" runat="server">
                                                        <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                            <span class="shadow-metro-black"><strong><h3>Database Settings</h3></strong></span>
                                                        </div>
                                                        <hr />
                                                        <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                            <span class="shadow-metro-black"><strong><h4>Sql</h4></strong></span>
                                                            <span>
                                                                <asp:Button OnClientClick="ShowLoader()" ID="SendSQL" CssClass="btn" ClientIDMode="Static" runat="server" Text="Asset Library to SQL" OnClick="SendSQL_Click" />
                                                                <asp:Button OnClientClick="ShowLoader()"  ID="SendSettingsSQL" CssClass="btn"  ClientIDMode="Static" runat="server" Text="Settings To SQL" OnClick="SendSettingsSQL_Click" />
                                                                <asp:Button OnClientClick="ShowLoader()"  ID="PullSettings" CssClass="btn"  ClientIDMode="Static" runat="server" Text="SQL To Settings" OnClick="PullSettings_Click" />
                                                                <asp:Button OnClientClick="ShowLoader()"  ID="PullSQL" CssClass="btn"  ClientIDMode="Static" runat="server" Text="SQL to Asset Library" OnClick="PullSQL_Click" />
                                                                <asp:Button OnClientClick="ShowLoader()"  ID="DeleteSQL" CssClass="btn"  ClientIDMode="Static" runat="server" Text="Erase SQL DataBase" OnClick="DeleteSQL_Click" />
                                                                <asp:Button OnClientClick="ShowLoader()"  ID="DeleteSettingsSQL" CssClass="btn"  ClientIDMode="Static" runat="server" Text="Erase Settings SQL" OnClick="DeleteSettingsSQL_Click" />
                                                            </span>
                                                        </div>
                                                        <hr />
                                                        <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                            <span class="shadow-metro-black"><strong><h4>Import / Export</h4></strong></span>
                                                            <asp:Button ID="ExportLibraryBtn" CssClass="btn" ClientIDMode="Static" runat="server" Text="Export Library" OnClick="ExportLibraryBtn_Click" />
                                                            <asp:Button  ID="ExportXmlBtn" CssClass="btn" ClientIDMode="Static" runat="server" Text="Export Xml" OnClick="ExportXmlBtn_Click" />
                                                            <asp:Button OnClientClick="ShowLoader()" ID="ImportLibraryBtn" CssClass="btn" ClientIDMode="Static" runat="server" Text="Import Library" OnClick="ImportLibraryBtn_Click" />
                                                            <fieldset class="groupbox-border">
                                                            <legend class="groupbox-border fg-l0xx0r">Legacy Windows App</legend>
                                                                <asp:FileUpload ID="HistoryUploader" runat="server" Width="250px" />
                                                                <asp:Button OnClientClick="ShowLoader();"  ID="UploadHistory" ClientIDMode="Static" CssClass=" btn" runat="server" Text="Upload History File" OnClick="UploadHistory_Click" />
                                                                <asp:FileUpload ID="NoticeUploader" runat="server" Width="250px" CssClass="text-center" />
                                                                <asp:Button OnClientClick="ShowLoader();"  ID="UploadNotices" ClientIDMode="Static" CssClass="btn" runat="server" Text="Upload Notices File" OnClick="UploadNotices_Click" />
                                                                <asp:FileUpload ID="LibraryUploader" runat="server" Width="250px" />
                                                                <asp:Button  OnClientClick="ShowLoader();"  ID="UploadLibrary" ClientIDMode="Static" CssClass="btn" runat="server" Text="Upload Asset File" OnClick="UploadLibrary_Click" />
                                                            </fieldset>
                                                        </div>
                                                        <hr />
                                                        <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                            <span class="shadow-metro-black"><strong><h4>Email / Messaging</h4></strong></span>
                                                            <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                                <span class="shadow-metro-black"><strong><h5>Checkout Email Body</h5></strong></span>
                                                                <div class=" textarea" style="min-height:75px !important; padding:3px"><textarea style="width:100%; height:75px"  class="form-control" id="checkoutmsgbox" runat="server" ></textarea></div>
                                                            </div>
                                                            <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                                <span class="shadow-metro-black"><strong><h5>Checkin Email Body</h5></strong></span>
                                                                <div class=" textarea " style="min-height:75px !important; padding:3px"><textarea style="width:100%; height:75px"  class="form-control textarea form-control" id="checkinmsgbox" runat="server" ></textarea></div>
                                                            </div>
                                                            <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                                <span class="shadow-metro-black"><strong><h5>Notification Email Body</h5></strong></span>
                                                                <div class=" textarea" style="min-height:75px !important; padding:3px"><textarea style="width:100%; height:75px"  class="form-control" id="notificationmsgbox" runat="server" ></textarea></div>
                                                            </div>
                                                            <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                                <span class="shadow-metro-black"><strong><h5>Office Personnel Email Body</h5></strong></span>
                                                                <div class=" textarea" style="min-height:75px !important; padding:3px"><textarea style="width:100%; height:75px" class="form-control" id="shipmsgbox" runat="server" ></textarea></div>
                                                            </div>
                                                        </div>
                                                        <hr />
                                                        <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                            <span class="shadow-metro-black"><strong><h4>Shipping Services</h4></strong></span>
                                                            <fieldset class="groupbox-border">                                                            
                                                            <legend class="groupbox-border fg-l0xx0r">UPS Settings</legend>
                                                            <input runat="server" id="ups_aln" type="text" class="form-control" placeholder="Access License #..." />
                                                            <input runat="server" id="ups_userid" type="text" class="form-control" placeholder="User ID..." />
                                                            <input runat="server" id="ups_pwd" type="text" class="form-control" placeholder="Password..." />
                                                            <input runat="server" id="ups_shippernumber" type="text" class="form-control" placeholder="Shipper #" />
                                                            </fieldset>
                                                            <fieldset class="groupbox-border">
                                                            <legend class="groupbox-border fg-l0xx0r">Fedex Settings</legend>
                                                    
                                                            </fieldset>
                                                        </div>
                                                    </asp:View>
                                                </asp:MultiView>                                                
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="AssetSearchBtn" EventName="click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </asp:View>
                                    <%--Transaction View--%>
                                    <asp:View ID="TransactionsView" runat="server">
                                        <asp:MultiView ID ="TransactionMultiView" ActiveViewIndex="0" runat="server">
                                            <asp:View ID="TransactionList" runat="server">
                                                <asp:UpdatePanel runat="server" ID="TransactionListUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                                    <ContentTemplate>
                                                        <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                            <span class="fg-black shadow-metro-black"><strong><h3>Transactions</h3></strong></span>
                                                        </div>
                                                        <asp:Repeater ID="TransactionRepeater" ClientIDMode="Static" runat="server" >
                                                                        <ItemTemplate>
                                                                            <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                                    <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                                        <a href="#" class="btn" title="Delete" onclick="Super('delete_transaction','<%#Eval("TransactionID")%>');" >X</a>
                                                                                        <a title="Complete Transaction" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" href='/Account/OutCart.aspx?pend=<%#Eval("TransactionID")%>'>></a>    
                                                                                        <a style="text-decoration:none" href='/Account/Transactions.aspx?tid=<%#Eval("TransactionID")%>'><span class="fg-black shadow-metro-black"><%# ((DateTime)Eval("Date")).ToShortDateString()%></span>&nbsp-&nbsp <span class="fg-black shadow-metro-black"><%# Eval("Name")%></span><span class="fg-black shadow-metro-black"> &nbsp-&nbsp  <span class="fg-black shadow-metro-black"><%# (Eval("Customer") as Helpers.Customer).CompanyName%></span> <asp:Literal Text="" runat="server" ID="TransactionLiteral"  /></a>
                                                                                    </div>  
                                                                            </div>
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </asp:View>
                                             <asp:View ID="IndividualTRansaction" runat="server">
                                                 <asp:UpdatePanel runat="server" ID="IndividualTransactionUpdatePanel" UpdateMode="Conditional">
                                                     <ContentTemplate>
                                                         <div class="awp_box_title">
                                                            Transaction
                                                        </div>
                                                                 <div class="col-md-12 fg-white shadow-metro-black" >
                                                   <div style="width:auto; opacity:0.75 !important; font-weight:bold" class="bg-sg-title rounded">
                                                                     <span class=" top-right-btn bg-green"><a style="text-decoration:none!important;" class="fg-white" title="Complete Transaction" href='/Account/OutCart.aspx?pend=<%= TID%>'>Complete</a></span>

                                                        <div>
                                                            <span>Confimation Number:  <%= TID %></span>
                                                        </div>
                                                        <div>
                                                        <span>Ship To:  <%= ShipToName %></span>
                                                        </div>
         
                                                        <div>
                                                            <span><a href='mailto:<%= Email %>'> Email:  <%= Email %></a></span>
                                                        </div>
                                                        <div>
                                                            <span>Comment:  <%= OrderNumber %></span>
                                                        </div>
                                                    </div>
        
                                                </div>
                                                <div class="col-md-12">
                                                    <div>
                                                        <asp:Repeater ID="TransactionItemRepeater"  runat="server" OnItemCommand="TransactionItemRepeater_ItemCommand">
                                                            <ItemTemplate>
                                                                    <div  class="file " title='' draggable="true"  onclick="BarcodeScanned('<%# Eval("AssetNumber")%>', '<%# Eval("IsHistoryItem")%>','<%# Eval("DateShippedTicks")%>');">
                                                                            <div id =" file-color " class="file-color <%# Eval("Color") %>" style="width:100%; height:100%;"></div>

                                                                    <div class="i-icon">   
                                                                        <img style="box-shadow: rgba(0, 0, 0, 0.70) 0px 0px 10px;" src='<%# Eval("FirstImage")+".r?w=133&h=100" %>' />
                                                                    </div>

                                                                    <div runat="server" class="i-title"><strong class="fg-white shadow-metro-black"><%# Eval("AssetName") %></strong> </div>
  
                                                                    <div class="i-check">
                                                                <span id="InOutIndicator"  runat="server" class="edge-badge fg-white shadow-metro-black icon-badge " style="z-index:3 !important;"><strong><%# Eval("AssetNumber")%></strong></span></div>
                                                                        <div runat="server" id="DownloadLink" class="i-download" style="margin-top:-4px;padding-left:2px" onclick="event.stopPropagation();">  <asp:LinkButton ID="RemoveFromCheckOutButton" CommandName="Delete" CommandArgument='<%# Eval("AssetNumber")%>' runat="server"  > <i title="Add To Task List" id="dl_link"  style="font-size:1.5em;" class="glyphicon glyphicon-remove c-red  av-hand-cursor"></i></asp:LinkButton>       

   
                                                                </div>

                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                </div>

                                                     </ContentTemplate>
                                                 </asp:UpdatePanel>
                                            </asp:View>
                                        </asp:MultiView>
                                    </asp:View>
                                    <%--Customers View--%>
                                    <asp:View ID="CustomersView" runat="server">
                                        <asp:UpdatePanel runat="server"  ID="CustomerUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" >
                                            <ContentTemplate>
                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                    <span class="fg-black shadow-metro-black"><strong><h3>Customers</h3></strong></span>
                                                </div>
                                                <asp:Repeater ID="CustomersRepeater" ClientIDMode="Static" runat="server">
                                                                <ItemTemplate>
                                                                    <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                            <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                                <a href="#" class="btn" title="Delete" onclick="Super('delete_customer','<%# Eval("CompanyName") %>-dd-<%# Eval("Postal") %>');" >X</a>
                                                                                <a title="Edit Customers" onclick="CP_EditCustomer('<%# Eval("CompanyName") %>-dd-<%# Eval("Postal") %>')" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" href="#">(Edit)</a>    
                                                                                <span ><%# Eval("CompanyName") %>&nbsp-&nbsp<%# Eval("Address") %> ,<%# Eval("Postal") %> </span>
                                                                                <a href='mailto:<%# Eval("Email") %>'>Email: <%# Eval("Email") %></a>
                                                                            </div>  
                                                                    </div>
                                                                </ItemTemplate>
                                                    
                                                            </asp:Repeater>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </asp:View>
                                    <%--Customer View--%>
                                    <asp:View ID="CustomerView" runat="server">
                                        <asp:UpdatePanel runat="server"  ID="CustomerViewUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" >
                                            <ContentTemplate>
                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                    <span class="fg-black shadow-metro-black"><strong><h3><asp:Label ID="CustomerViewDocumentsLabel" runat="server" /></h3></strong></span>
                                                </div>
                                                <div class="row" style="margin-left:15px">
                                                    <div class="col-md-3">
                                                        <address>                                                    
                                                            <strong><asp:Label ID="CustomerViewCompanyNameLabel" runat="server" /></strong><br>
                                                            <asp:Label ID="CustomerViewAddressLabel" runat="server" />&nbsp;<asp:Label ID="CustomerViewAddress2Label" runat="server" />&nbsp;<asp:Label ID="CustomerViewAddress3Label" runat="server" /><br>
                                                            <asp:Label ID="CustomerViewCityLabel" runat="server" />, <asp:Label ID="CustomerViewStateLabel" runat="server" />, <asp:Label ID="CustomerViewPostalLabel" runat="server" /><br>
                                                            <asp:Label ID="CustomerViewCountryLabel" runat="server" /><br>
                                                            <abbr title="Phone">Phone:</abbr> <asp:Label ID="CustomerViewPhoneLabel" runat="server" />
                                                        </address>
                                                        <address>
                                                            <strong><asp:Label ID="CustomerViewAttnLabel" runat="server" /></strong><br>
                                                            <a href='#'><asp:Label ID="CustomerViewEmailLabel" runat="server" /></a>
                                                        </address>
                                                    </div>
                                                </div>
                                                 
                                                <%--Documents--%>
                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                    <span style="display:inline-flex" class="fg-black shadow-metro-black">
                                                        <strong><h4>Documents &nbsp;&nbsp; </h4></strong>
                                                      <span style="display:inline-flex;"> <asp:FileUpload  ID="CustomerViewDocumentsFileUpload"  runat="server" CssClass="form-contol" /></span>
                                                        <span style="display:inline-flex;"> <asp:LinkButton Text="Upload" runat="server" CssClass="btn bg-green" ID="CustomerViewDocumentsBtn" OnClick="CustomerViewDocumentsBtn_Click" /></span>
                                                    </span>
                                                </div>
                                                <asp:Repeater ID="CustomerViewDocumentsRepeater" ClientIDMode="Static" runat="server">
                                                    <ItemTemplate>
                                                        <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                    <a href="#" class="btn" title="Delete" onclick="Super('deldoc_cust','<%# CustomerGuid %>-dd-<%#Eval("AssetNumber")%>-dd-<%#Eval("FileName")%>');" >X</a>
                                                                    <a href="#" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" title="Show" onclick="window.open('<%# Eval("FileName") %>', '_blank', 'fullscreen=no'); return false;" >[show]</a>
                                                                    <span ><%# Eval("FileName") %></span>
                                                                </div>  
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <%--Current Assets--%>
                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                    <span style="display:inline-flex" class="fg-black shadow-metro-black">
                                                        <strong><h4>Assigned Assets &nbsp;&nbsp; </h4></strong>
                                                    </span>
                                                </div>
                                                <asp:Repeater ID="CustomerViewAssignedAssetRepeater" ClientIDMode="Static" runat="server">
                                                    <ItemTemplate>
                                                        <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                            <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                <span ><%# Eval("AssetNumber") %>&nbsp-&nbsp<%# Eval("AssetName") %>&nbsp-&nbsp Is Out: <%# Eval("IsOut").ToString() %></span>
                                                            </div>  
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <%--Kits--%>
                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                    <span style="display:inline-flex" class="fg-black shadow-metro-black">
                                                        <strong><h4>Asset Kits &nbsp;&nbsp; </h4></strong>
                                                    </span>
                                                </div>
                                                <asp:Repeater ID="CustomerViewAssetKitsRepeater" ClientIDMode="Static" runat="server">
                                                    <ItemTemplate>
                                                        <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                            <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                <a href="#" class="btn" title="Delete" onclick="Super('delkit_cust','<%# CustomerGuid %>-dd-<%#Eval("Guid")%>');" >X</a>
                                                                <a href="#" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" title="Show" onclick=" Super('send_kit','<%# CustomerGuid %>-dd-<%#Eval("Guid")%>');" >[Send To Checkout]</a>
                                                                <span >Assets:&nbsp;<%# Eval("AssetsString") %></span>
                                                            </div>  
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <%--Order Numbers--%>
                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                    <span style="display:inline-flex" class="fg-black shadow-metro-black">
                                                        <strong><h4>Order Numbers &nbsp;&nbsp; </h4></strong>                                                      
                                                    </span>
                                                </div>
                                                <asp:Repeater ID="CustomerViewOrderNumbersRepeater" ClientIDMode="Static" runat="server">
                                                    <ItemTemplate>
                                                        <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                    <span ><%# Eval("FileName") %></span>
                                                                </div>  
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </asp:View>
                                    <%--Personnel View--%>
                                    <asp:View ID="PersonnelView" runat="server">
                                        <asp:UpdatePanel ID="PersonnelViewMasterUpdatePanel" ChildrenAsTriggers="false" UpdateMode="Conditional" runat="server">
                                            <ContentTemplate>
                                                <asp:MultiView ID="PersonnelMultiView" ActiveViewIndex="0" runat="server">
                                                    <asp:View ID="PersonnelMainView" runat="server">
                                                        <asp:UpdatePanel runat="server" ID="PersonnelMainViewUpdatePanel" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                                    <span class="fg-black shadow-metro-black"><strong><h3>Static Email List</h3></strong></span>
                                                                </div>
                                                                <asp:Repeater ID="StaticEmailRepeater" ClientIDMode="Static" runat="server">
                                                                    <ItemTemplate>
                                                                        <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                                <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                                    <a href="#" class="btn" title="Delete" onclick="Super('delete_static','<%#Eval("Email")%>');" >X</a>
                                                                                    <a title="Edit" onclick="CP_EditStatic('<%# Eval("Email") %>');" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" href="#">(Edit)</a>    
                                                                                    <span ><%# Eval("Name") %>&nbsp-&nbsp</span>
                                                                                    <a href='mailto:<%# Eval("Email") %>'>Email: <%# Eval("Email") %></a>
                                                                                </div>  
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                                <hr />
                                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                                    <span class="fg-black shadow-metro-black"><strong><h3>Office Personnel</h3></strong></span>
                                                                </div>
                                                                <asp:Repeater ID="PersonnelOfficeMainViewRepeater" ClientIDMode="Static" runat="server">
                                                                    <ItemTemplate>
                                                                        <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                                <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                                    <a href="#" class="btn" title="Delete" onclick="Super('delete_op','<%#Eval("Email")%>');" >X</a>
                                                                                    <a title="Edit" onclick="CP_EditOP('<%# Eval("Email") %>');" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" href="#">(Edit)</a>    
                                                                                    <span ><%# Eval("Name") %>&nbsp-&nbsp</span>
                                                                                    <a href='mailto:<%# Eval("Email") %>'>Email: <%# Eval("Email") %></a>
                                                                                </div>  
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                                <hr />
                                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                                    <span class="fg-black shadow-metro-black"><strong><h3>Field Personnel</h3></strong></span>
                                                                </div>
                                                                <asp:Repeater ID="PersonnelFieldMainViewRepeater" ClientIDMode="Static" runat="server">
                                                                    <ItemTemplate>
                                                                        <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                                <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                                    <a href="#" class="btn" title="Delete" onclick="Super('delete_fp','<%#Eval("Email")%>');" >X</a>
                                                                                    <a title="Edit" onclick="CP_EditFP('<%# Eval("Email") %>');" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" href="#">(Edit)</a>    
                                                                                    <span ><%# Eval("Name") %>&nbsp-&nbsp</span>
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

                                                    </asp:View>
                                                     <asp:View ID="PersonnelOfficeView" runat="server">
                                                        <asp:UpdatePanel runat="server" ID="PersonnelOfficeUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                                            <ContentTemplate>
                                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                                    <span class="fg-black shadow-metro-black"><strong><h3>Office Personnel</h3></strong></span>
                                                                </div>
                                                                <asp:Repeater ID="PersonnelOfficeRepeater" ClientIDMode="Static" runat="server">
                                                                                <ItemTemplate>
                                                                                    <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                                            <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                                                <a href="#" class="btn" title="Delete" onclick="Super('delete_op','<%#Eval("Email")%>');" >X</a>
                                                                                                <a title="Edit" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" href="#">(Edit)</a>    
                                                                                                <span ><%# Eval("Name") %>&nbsp-&nbsp</span>
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

                                                    </asp:View>
                                                     <asp:View ID="PersonnelFieldView" runat="server">
                                                        <asp:UpdatePanel runat="server" ID="PersonnelFieldUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                                            <ContentTemplate>
                                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                                    <span class="fg-black shadow-metro-black"><strong><h3>Field Personnel</h3></strong></span>
                                                                </div>
                                                                <asp:Repeater ID="PersonnelFieldRepeater" ClientIDMode="Static" runat="server">
                                                                                <ItemTemplate>
                                                                                    <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                                            <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                                                <a href="#" class="btn" title="Delete" onclick="Super('delete_fp','<%#Eval("Email")%>');" >X</a>
                                                                                                <a title="Edit" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" href="#">(Edit)</a>    
                                                                                                <span ><%# Eval("Name") %>&nbsp-&nbsp</span>
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

                                                    </asp:View>
                                                </asp:MultiView>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="AssetSearchBtn" EventName="click" />
                                            </Triggers>
                                        </asp:UpdatePanel>

                                    </asp:View>
                                    <%--Notification View--%>
                                    <asp:View ID="NotificationsView" runat="server">
                                        <asp:UpdatePanel runat="server" ChildrenAsTriggers="false" ID="NotificationsViewUpdatePanel" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                    <span class="fg-black shadow-metro-black"><strong><h3>Notifications</h3></strong></span>
                                                </div>
                                                <asp:Repeater ID="NotificationsRepeater" ClientIDMode="Static" runat="server">
                                                    <ItemTemplate>
                                                        <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                    <a href="#" class="btn" title="Delete" onclick="Super('delete_notice','<%# Eval("Guid").ToString()%>');" >X</a>
                                                                    <a href="#" class="btn" title="Send" onclick="Super('send_notice','<%# Eval("Guid").ToString()%>');" >[send]</a>
                                                                    <%--<a title="Edit Notification" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" href="#">(Edit)</a>  --%>  
                                                                    <span ><%# Eval("Name") %>&nbsp-&nbsp Scheduled in: &nbsp<%# Eval("DaysUntil").ToString()%>&nbsp Days</span>
                                                                </div>  
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="AssetSearchBtn" EventName="click" />
                                            </Triggers>
                                        </asp:UpdatePanel>

                                    </asp:View>
                                    <%--Assets View --%>
                                    <asp:View ID="AssetsView" runat="server">
                                        <asp:UpdatePanel runat="server" ID="AssetsUpdatePanel" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                    <span class="fg-black shadow-metro-black"><strong><h3>Assets</h3></strong></span>
                                                </div>
                                                <asp:Repeater ID="AssetsRepeater" ClientIDMode="Static" runat="server">
                                                    <ItemTemplate>
                                                        <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                    <a href="#" class="btn" title="Delete" onclick="Super('delete_asset','<%#Eval("AssetNumber")%>');" >X</a>
                                                                    <a title="Edit Asset" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" href="#"  onclick="Super('edit_asset','<%#Eval("AssetNumber")%>');">(Edit)</a>    
                                                                    <span ><%# Eval("AssetNumber") %>&nbsp-&nbsp<%# Eval("AssetName") %>&nbsp-&nbsp Is Out: <%# Eval("IsOut").ToString() %></span>
                                                                </div>  
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="AssetSearchBtn" EventName="click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </asp:View>
                                    <%--Assets View --%>
                                    <asp:View ID="SingleAssetView" runat="server">
                                        <asp:UpdatePanel runat="server" ID="SingleAssetViewUpdatePanel" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                    <span class="fg-black shadow-metro-black"><strong><h3><asp:Label ID="SingleAssetViewLabel" runat="server" Text="0000"></asp:Label></h3></strong></span>
                                                </div>
                                                <%--Description--%>
                                                <%--Images--%>
                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                    <span style="display:inline-flex" class="fg-black shadow-metro-black">
                                                        <strong><h4>Images &nbsp;&nbsp; </h4></strong>
                                                      <span style="display:inline-flex;"> <asp:FileUpload  ID="SingleAssetImageFileUpload"  runat="server" CssClass="form-contol" /></span>
                                                        <span style="display:inline-flex;"> <asp:LinkButton Text="Upload" runat="server" CssClass="btn bg-green" ID="SingleAssetImageUploadBtn" OnClick="SingleAssetImageUploadBtn_Click" /></span>

                                                    </span>
                                                </div>
                                                <asp:Repeater ID="SingleAssetViewImageRepeater" ClientIDMode="Static" runat="server">
                                                    <ItemTemplate>
                                                        <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                    <a href="#" class="btn" title="Delete" onclick="Super('delimg_asset','<%#Eval("AssetNumber")%>-dd-<%#Eval("FileName")%>');" >X</a>
                                                                    <a href="#" class="btn btn-sm fg-black shadow-metro-black" style="font-weight:bold" title="Show" onclick="window.open('/Account/Images/<%#Eval("AssetNumber")%>/<%# Eval("FileName") %>', '_blank', 'fullscreen=no'); return false;" >[show]</a>

                                                                    <span ><%# Eval("FileName").ToString().Replace("/Account/","") %></span>
                                                                </div>  
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <%--Documents--%>
                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                    <span style="display:inline-flex" class="fg-black shadow-metro-black">
                                                        <strong><h4>Documents &nbsp;&nbsp; </h4></strong>
                                                        <span style="display:inline-flex;"> <asp:FileUpload  ID="SingleAssetDocumentFileUpload" runat="server" CssClass="form-contol" /></span>
                                                        <span style="display:inline-flex;"> <asp:LinkButton Text="Upload" runat="server" CssClass="btn bg-green" ID="SingleAssetDocumentFileUploadBtn" OnClick="SingleAssetDocumentFileUploadBtn_Click" /></span>
                                                    </span>
                                                </div>
                                                <asp:Repeater ID="SingleAssetViewDocumentRepeater" ClientIDMode="Static" runat="server">
                                                    <ItemTemplate>
                                                        <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                    <a href="#" class="btn" title="Delete" onclick="Super('deldoc_asset','<%#Eval("AssetNumber")%>-dd-<%#Eval("FileName")%>');" >X</a>
                                                                    <a href="#" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" title="Show" onclick="window.open('<%# Eval("FileName") %>', '_blank', 'fullscreen=no'); return false;" >[show]</a>
                                                                    <span ><%# Eval("FileName").ToString().Replace("/Account/","") %></span>
                                                                </div>  
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                <%--History--%>
                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                    <span style="display:inline-flex" class="fg-black shadow-metro-black">
                                                        <strong><h4>History &nbsp;&nbsp; </h4></strong>
                                                    </span>
                                                </div>
                                                <asp:Repeater ID="SingleAssetViewHistoryRepeater" ClientIDMode="Static" runat="server">
                                                    <ItemTemplate>
                                                        <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                    <a href="#" class="btn" title="Delete" onclick="Super('delhist_asset','<%#Eval("AssetNumber")%>-dd-<%#Eval("DateShippedString")%>');" >X</a>
                                                                    <a title="Show" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" href="#"  onclick="Super('history_asset','<%#Eval("AssetNumber")%>-dd-<%#Eval("DateShippedString")%>');">[show]</a>    
                                                                    <span ><%# Eval("OrderNumber") %>&nbsp;-&nbsp;<%# Eval("ShipTo") %>&nbsp;-&nbsp;<%# Eval("DateRecievedString") %>&nbsp;-&nbsp;<%# Eval("DateShippedString") %></span>
                                                                </div>  
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                                
                                                
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="AssetSearchBtn" EventName="click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </asp:View>

                                    <%--Certificate View--%>
                                    <asp:View ID="CertificateView" runat="server">
                                        <asp:UpdatePanel runat="server" ID="CertificateUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="false">
                                            <ContentTemplate>
                                                <div class=" border-bottom-blue" style="margin:0px !important; padding-left:15px">
                                                    <span class="fg-black shadow-metro-black"><strong><h3>Certificates</h3></strong></span>
                                                </div>
                                                <asp:Repeater ID="CertificateRepeater" ClientIDMode="Static" runat="server">
                                                    <ItemTemplate>
                                                        <div class="border-bottom-blue" style="overflow:hidden">                                        
                                                                <div class="col-sm-12 fg-black" style="width:auto !important; padding-left:10px; text-align:left; font-weight:normal !important">
                                                                    <a href="#" class="btn" title="Delete" onclick="Super('delete_certificate','<%#Eval("Guid")%>');" >X</a>
                                                                    <a title="Edit Asset" style="font-weight:bold" class="btn btn-sm fg-black shadow-metro-black" href="#"  onclick="Super('edit_certificate','<%# Eval("Guid") %>'); ShowDiv('CreateCertificateModal');">(Edit)</a>    
                                                                    <span ><a href="#" onclick="window.open('/Account/Certificates/<%# Eval("FileName") %>', '_blank', 'fullscreen=no'); return false;"><%# Eval("FileName") %>&nbsp-&nbsp<%# Eval("AssetNumber") %></a></span>
                                                                </div>  
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="AssetSearchBtn" EventName="click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </asp:View>
                                </asp:MultiView>
                            </ContentTemplate>
                        </asp:UpdatePanel>                       
                    </div>  
                </div>
            </div>
         </div>
        <div id="footer" style="overflow:hidden !important;">
            <asp:UpdatePanel runat="server" ID="AppFooterUpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="false" >
                <ContentTemplate>
                    <span class="fg-white"><asp:Label ClientIDMode="Static" ID="FooterStatusLabel" Text="" runat="server" /></span>
                </ContentTemplate>
                
            </asp:UpdatePanel>
          
        </div>
    </div>
    <asp:TextBox runat="server" ID ="CurrentView" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:TextBox runat="server" ID ="AppArgument" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:TextBox runat="server" ID ="AppCommand" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:TextBox runat="server" ID ="SuperButtonArg" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:TextBox runat="server" ID ="SuperButtonCommand" ClientIDMode="Static" style="display:none"></asp:TextBox>
    <asp:Button ID="SuperButton" runat="server" Text="Super" OnClick="SuperButton_Click" style="display:none" ClientIDMode="Static"/>

 

</asp:Content>
