<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" ValidateRequest="false" EnableEventValidation="false" CodeBehind="AdminDashboard.aspx.cs" Inherits="Web_App_Master.Account.Admin.AdminDashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
            

    <nav class="navbar navbar-inverse bg-grayDark" style="margin-top:0px !important; height:auto; width:100%; left:0px!important; border-radius:0px!important; position:fixed !important; z-index:25000;">
         
        <ul class="nav navbar-nav starrag-menu">
            <li class="fg-white"><h2>Administration</h2> </li>
            <li>
                <asp:LinkButton ID ="UsersAndRolesBtn" OnClick="UsersAndRolesBtn_Click" ToolTip="User Administration" ClientIDMode="Static" runat="server"><span >Users</span></asp:LinkButton>
            </li>
            <li>
                <asp:LinkButton ID ="AssetAdminBtn" OnClick="AssetAdminBtn_Click" ToolTip="Library Options" ClientIDMode="Static" runat="server"><span class="mif-books mif-3x"></span>Settings</asp:LinkButton>
            </li>
            <li>
                <asp:LinkButton ID ="AssetPeopleManager" OnClick="AssetPeopleManager_Click" ToolTip="Personnel" ClientIDMode="Static" runat="server"><span class="mif-users mif-3x"></span>Personnel</asp:LinkButton>
            </li>
            <li>
                <asp:LinkButton ID ="CustomerManager" OnClick="CustomerManager_Click" ToolTip="Customers" ClientIDMode="Static" runat="server"><span class="mif-contacts-mail mif-3x"></span>Customers</asp:LinkButton>
            </li>
           <li>
                <asp:LinkButton ID ="NotificationManagerBtn" OnClick="NotificationManagerBtn_Click" ToolTip="Notification Manager" ClientIDMode="Static" runat="server"><span class="mif-bell mif-3x">Notices</span></asp:LinkButton>
            </li>
            <li>
                <asp:LinkButton ID ="AssetManagerBtn" OnClick="AssetManagerBtn_Click" ToolTip="Asset Manager" ClientIDMode="Static" runat="server"><span class=" mif-bt-settings mif-3x">Assets</span></asp:LinkButton>
            </li>   
        </ul>
           <asp:PlaceHolder ID ="ApplyChangesButton" runat="server" Visible="true">
               <ul class="nav navbar-nav navbar-right">
                  <li title="Test Mode" style="padding-top:15px;">
                        <span style="display:inline-block; padding-right:10px;" class="fg-white shadow-metro-black">  
                            TestMode:<asp:Label Text="True" runat="server" ID="TestModeLabel" />                         
                        </span>
                       <span style="display:inline-block; padding-right:10px;">  
                            <asp:Button OnClick="ChangeTestModeBtn_Click" CssClass="btn btn-default" runat="server" Text="Change" ID="ChangeTestModeBtn" />
                        </span>
                  </li>
               </ul>

            </asp:PlaceHolder>
     
    </nav>
<%--  <label class="awp-switch">
                                <input title="Test Mode" id="TestModeSwitch"  type="checkbox" checked="checked" onchange="TestModeChanged()">
                                <span class="check"></span>
                            </label>        --%>           
                           





    
    <div style="padding-top:60px"></div>
    <!--Multiview-->
     <asp:PlaceHolder ID ="MessagePlaceHolder" ClientIDMode="Static" runat="server" Visible="false">
        <div class="row">
                <div class="col-md-12">
                    <div class="awp_box rounded bg-sg-title shadow">
                        <div class="awp_box_title bg-sg-title">
                           <span class="fg-white shadow-metro-black"><span class="mif-warning mif-ani-flash mif-ani-slow"></span></span>
                        </div>
                        <div class="awp_box_content bg-red fg-white shadow-metro-black">
                            <asp:Literal ID="ErrorMsg" runat="server"></asp:Literal>
                       </div>
                    </div>
                </div>
            </div>

    </asp:PlaceHolder>
   <asp:MultiView ID="AdminMultiView" ActiveViewIndex="0" runat="server" EnableViewState="true" ViewStateMode="Enabled">
        <asp:View ID="RolesAndUsersAdminView"  runat="server">

             <div class="row">
         <div class="col-md-12">
       
         </div>
    </div>
            <!--row end-->
            <div class="row">
                 <div class="col-md-8">
                    <div class="awp_box rounded bg-sg-title shadow">
           
                    <div class="awp_box_title bg-sg-title">
                       <span class="fg-white shadow-metro-black">Roles</span>
                    </div>
                    <div class="awp_box_content bg-sg-box">
                        <asp:UpdatePanel ChildrenAsTriggers="true" ID="RolesUpdatePanel" ClientIDMode="Static" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                            <asp:Repeater ID="RolesAndUsersRepeater" OnItemDataBound="RolesAndUsersRepeater_ItemDataBound" ClientIDMode="Static" runat="server" ViewStateMode="Enabled" EnableViewState="true">
                            <HeaderTemplate>
                               <div class="panel-group" id="rolesAccordion">

                            </HeaderTemplate>
                            <ItemTemplate>
                              <div class="panel panel-default">
                                  <div class="panel-heading">
                                    <h4 class="panel-title text-left">
                                      <a data-toggle="collapse" data-parent="#rolesAccordion" href='#collapse<%# Container.ItemIndex + 1%>'>
                                          <strong><%# Eval("RoleName")%></strong>
                                      </a>
                                    </h4>
                                  </div>
                                  <div id='collapse<%# Container.ItemIndex + 1%>' class="panel-collapse collapse">
                                    <div class="panel-body text-left">
                                        <!--Repeaters for users of this role-->
                                        <asp:Repeater DataSource='<%# Eval("RoleUsers")%>' runat="server">
                                            <ItemTemplate>
                                                <div class="row bg-sg-box">                                           
                                                        <div class="col-sm-12  text-left bg-white" style="width:auto !important">
                                                            <asp:Button ToolTip="Delete User From Role" ID="DeleteFromRole" CssClass="btn btn-sm" runat="server" Text="X" Font-Bold="true" CommandName='<%#Eval("UserId")%>' CommandArgument='<%#Eval("RoleId") %>' OnCommand="DeleteFromRole_Command" />
                                                    
                                                            <asp:DropDownList ClientIDMode="Static" ID='RoleDropDown' AppendDataBoundItems="true" runat="server" DataSource='<%#GetRoleNames() %>'  CssClass="dropdown-button">
                                                                <asp:ListItem Text="--Select One--" Value="" /> 
                                                            </asp:DropDownList>
                                                            <asp:Button ToolTip="Copy User To ROle" ID="CopyRole" CssClass="btn btn-sm" runat="server" Text="Copy" Font-Bold="true" CommandName='<%#Eval("UserId")%>' CommandArgument='<%#Eval("RoleId") %>' OnCommand="CopyRole_Command" />
                                                            <asp:Button ToolTip="Change User Role" ID="ChangeRole" CssClass="btn btn-sm" runat="server" Text="Change" Font-Bold="true" CommandName='<%#Eval("UserId")%>' CommandArgument='<%#Eval("RoleId") %>' OnCommand="ChangeRole_Command" />
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
                    </div>
                </div>
                 </div>
                   <div class="col-md-4">
                <div class="awp_box rounded bg-sg-title shadow">
           
                    <div class="awp_box_title bg-sg-title">
                       <span class="fg-white shadow-metro-black">Users</span>
                    </div>
                    <div class="awp_box_content bg-sg-box">
                        <asp:UpdatePanel ID="UserUpdatePanel" ChildrenAsTriggers="true" ClientIDMode="Static" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                               <asp:Repeater ID ="UserRepeater" ClientIDMode="Static" runat="server">
                                   <ItemTemplate>
                                       <div class="row  bg-sg-box-dark " style="text-align:left">
                                           
                                           <div class="col-sm-12 bg-sg-box fg-white shadow-metro-black" style="text-align:left">
                                              <asp:Button ToolTip="Delete User" ID="DeleteUser" CssClass="btn btn-sm" runat="server" Text="X" Font-Bold="true" CommandName='<%#Eval("UserId")%>' CommandArgument='<%#Eval("RoleId") %>' OnCommand="DeleteUser_Command" />
                                               <strong><%# Eval("UserName")%></strong>
                                           </div>                                                
                           
                                                 
                                            
                                      </div>
                                   </ItemTemplate>
                               </asp:Repeater>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            </div>
                        
            <!--row end-->   
        </asp:View>
        <asp:View ID="AssetAdminView" runat="server">
      
            <div id="LibraryOptionsBox" class="row">
                <div class =" col-md-12">
                    <div class="awp_box rounded bg-sg-title shadow">
                        <div class="awp_box_title bg-sg-title">
                             <span class="fg-white shadow-metro-black">Library Options</span>
                        </div>
                        <div class="awp_box_content bg-sg-box">
                             <div class="row">
                                       <asp:PlaceHolder runat="server" ID="MessageHolder" Visible="false">
                                           <div class="col-md-12"><p class="fg-red text-bold" >
                                                <asp:Literal runat="server" ID="Notification" />
                                            </p></div>                        
                                        </asp:PlaceHolder>
                                        </div>
                             <div class="row">
                                        <div class="col-md-6">
                                            <div class="awp_box rounded bg-sg-title shadow">
           
                                                <div class="awp_box_title bg-sg-title">
                                                   <span class="fg-white shadow-metro-black">Upload Library</span>
                                                </div>
                                                <div class="awp_box_content bg-sg-box text-center">
                                                          <asp:FileUpload ID="LibraryUploader" runat="server" Width="250px" />
                                                         <asp:Button  OnClientClick=""  ID="UploadLibrary" ClientIDMode="Static" CssClass="btn" runat="server" Text="Upload Asset File" OnClick="UploadLibrary_Click" />
                                                </div>
                                            </div>
                                       </div>
                                       <div class="col-md-6">
                                            <div class="awp_box rounded bg-sg-title shadow">
           
                                                <div class="awp_box_title bg-sg-title">
                                                   <span class="fg-white shadow-metro-black">Upload Notices</span>
                                                </div>
                                                <div class="awp_box_content bg-sg-box text-center">
                                                    <asp:FileUpload ID="NoticeUploader" runat="server" Width="250px" CssClass="text-center" />
                                                    <asp:Button OnClientClick=""  ID="UploadNotices" ClientIDMode="Static" CssClass="btn" runat="server" Text="Upload Notices File" OnClick="UploadNotices_Click" />
                                                </div>
                                            </div>
                                       </div>
                                       <div class="col-md-6">
                                            <div class="awp_box rounded bg-sg-title shadow">
           
                                                <div class="awp_box_title bg-sg-title">
                                                   <span class="fg-white shadow-metro-black">Upload History</span>
                                                </div>
                                                <div class="awp_box_content bg-sg-box text-center">
                                                    <asp:FileUpload ID="HistoryUploader" runat="server" Width="250px" />
                                                    <asp:Button OnClientClick=""  ID="UploadHistory" ClientIDMode="Static" CssClass=" btn" runat="server" Text="Upload History File" OnClick="UploadHistory_Click" />
                                                </div>
                                            </div>
                                       </div>
                                       <div class="col-md-6">
                                            <div class="awp_box rounded bg-sg-title shadow">
           
                                                <div class="awp_box_title bg-sg-title">
                                                   <span class="fg-white shadow-metro-black">Export</span>
                                                </div>
                                                <div class="awp_box_content bg-sg-box">
                                                    <asp:Button OnClientClick=""  ID="ExportLibrary" ClientIDMode="Static" CssClass=" btn" runat="server" Text="Export Library" OnClick="ExportLibrary_Click" />


                                                </div>
                                            </div>
                                       </div>
                                           </div>
                             <div class="row">
                                        <div class="col-md-12">
                                            <div class="awp_box rounded bg-sg-title shadow">
           
                                                <div class="awp_box_title bg-sg-title">
                                                   <span class="fg-white shadow-metro-black">SQL Options</span>
                                                </div>
                                                <div class="awp_box_content bg-sg-box">
                                                    <asp:Button OnClientClick="ShowLoader()" ID="SendSQL" CssClass="btn" ClientIDMode="Static" runat="server" Text="Asset Library to SQL" OnClick="SendSQL_Click" />
                                                    <asp:Button OnClientClick="ShowLoader()"  ID="SendSettingsSQL" CssClass="btn"  ClientIDMode="Static" runat="server" Text="Settings To SQL" OnClick="SendSettingsSQL_Click" />
                                                    <asp:Button OnClientClick="ShowLoader()"  ID="PullSettings" CssClass="btn"  ClientIDMode="Static" runat="server" Text="SQL To Settings" OnClick="PullSettings_Click" />
                                                    <asp:Button OnClientClick="ShowLoader()"  ID="PullSQL" CssClass="btn"  ClientIDMode="Static" runat="server" Text="SQL to Asset Library" OnClick="PullSQL_Click" />
                                                    <asp:Button OnClientClick="ShowLoader()"  ID="DeleteSQL" CssClass="btn"  ClientIDMode="Static" runat="server" Text="Erase SQL DataBase" OnClick="DeleteSQL_Click" />
                                                    <asp:Button OnClientClick="ShowLoader()"  ID="DeleteSettingsSQL" CssClass="btn"  ClientIDMode="Static" runat="server" Text="Erase Settings SQL" OnClick="DeleteSettingsSQL_Click" />
                                                </div>
                                            </div>
                                       </div>
                                       </div>
                                          
                        </div>
                    </div>
                </div>
            </div>
            <div id="StaticEmailsBox" class="row">
                 <div class =" col-md-12">
                    <span class="awp-save-btn bg-red fg-white shadow">
                        <asp:LinkButton runat="server" ID="AddStatic" CssClass="fg-white" ClientIDMode="Static" OnClick="AddStatic_Click">
                        <i title="Add"  style="font-size:1em;" class="glyphicon glyphicon-plus av-hand-cursor"></i></asp:LinkButton>
                    </span>
                    <div class="awp_box rounded bg-sg-title shadow">
                        <div class="awp_box_title bg-sg-title fg-white shadow-metro-black">
                             Static Emails
                        </div>
                        <div class="awp_box_content bg-sg-box">
                            <asp:UpdatePanel ID="staticupdatepanel" ChildrenAsTriggers="true"  UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <asp:PlaceHolder runat="server" id="AddStaticPlaceHolder" Visible="false">
                                    <div class="form-group">
                                        <div>
                                            <div><input class="form-control" type="text" name="AddStaticName" placeholder="Name" value="" runat="server" /></div>
                                            <div><input class="form-control" type="text" name="AddStaticEmail" placeholder="Email" value="" runat="server" /></div>
                                            <div><asp:Button Text="Add" CssClass="btn btn-default" runat="server" ID="AddStaticSubmit" OnClick="AddStaticSubmit_Click" /></div>
                                        </div>
                                    </div>
                                    </asp:PlaceHolder>
                                    <asp:Repeater ID="StaticEmailRepeater" runat="server">
                                        <ItemTemplate>
                                            <div class="row  bg-sg-box-dark " style="text-align:left">
                                           
                                                   <div class="col-sm-12 bg-sg-box fg-white shadow-metro-black" style="text-align:left">
                                                      <asp:Button ToolTip="Delete Email" ID="DeleteStatic" CssClass="btn btn-sm" runat="server" Text="X" Font-Bold="true" CommandName='<%#Eval("Name")%>' CommandArgument='<%#Eval("Email") %>' OnCommand="DeleteStatic_Command" />
                                                       <strong><%# Eval("Name")%> : <%# Eval("Email")%></strong>
                                                   </div>  
                                              </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="AddStatic" EventName="click" />
                                    <asp:AsyncPostBackTrigger ControlID="AddStaticSubmit" EventName="click" />
                                </Triggers>
                            </asp:UpdatePanel>
                            
                        </div>
                    </div>
                 </div>
            </div>
                <div id="StandardMsgBox"  class="row">
                 <div class="col-md-12">
                    <div class="awp_box rounded bg-sg-title shadow">
           
                    <div class="awp_box_title bg-sg-title">
                       <span class="fg-white shadow-metro-black">Standard Messages</span>
                    </div>
                    <div class="awp_box_content bg-sg-box">
                       <div class="col-md-12">
                            <span class="awp-save-btn bg-red fg-white shadow">
                                <asp:LinkButton runat="server" ID="SaveCheckOutMsgBtn" CssClass="fg-white" ClientIDMode="Static" OnClick="SaveCheckOutMsgBtn_Click">
                                <i title="Save"  style="font-size:1em;" class="glyphicon glyphicon-floppy-disk av-hand-cursor"></i></asp:LinkButton>
                            </span>
                        <div class="awp_box rounded bg-sg-title shadow">           
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black">Check Out Message</span>
                            </div>
                            <div class="awp_box_content bg-sg-box">
                                <div class=" textarea av-input" style="min-height:75px !important;"><textarea class="form-control" id="checkoutmsgbox" runat="server" ></textarea></div>
                            </div>
                        </div>
                    </div>
                       <div class="col-md-12">
                            <span class="awp-save-btn bg-red fg-white shadow">
                                <asp:LinkButton runat="server" ID="SaveCheckInMsgBtn" CssClass="fg-white" ClientIDMode="Static" OnClick="SaveCheckInMsgBtn_Click">
                                <i title="Save"  style="font-size:1em;" class="glyphicon glyphicon-floppy-disk av-hand-cursor"></i></asp:LinkButton>
                            </span>
                        <div class="awp_box rounded bg-sg-title shadow">           
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black">Check In Message</span>
                            </div>
                            <div class="awp_box_content bg-sg-box">
                                <div class=" textarea av-input" style="min-height:75px !important;"><textarea class="form-control textarea av-input" id="checkinmsgbox" runat="server" ></textarea></div>
                            </div>
                        </div>
                    </div>
                       <div class="col-md-12">
                            <span class="awp-save-btn bg-red fg-white shadow">
                                <asp:LinkButton runat="server" ID="SaveNoticMsgBtn" CssClass="fg-white" ClientIDMode="Static" OnClick="SaveNoticMsgBtn_Click">
                                <i title="Save"  style="font-size:1em;" class="glyphicon glyphicon-floppy-disk av-hand-cursor"></i></asp:LinkButton>
                            </span>
                        <div class="awp_box rounded bg-sg-title shadow">           
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black">Notification Message</span>
                            </div>
                            <div class="awp_box_content bg-sg-box">
                                <div class=" textarea av-input" style="min-height:75px !important;"><textarea class="form-control" id="notificationmsgbox" runat="server" ></textarea></div>
                            </div>
                        </div>
                    </div>                        
                       <div class="col-md-12">
                            <span class="awp-save-btn bg-red fg-white shadow">
                                <asp:LinkButton runat="server" ID="SaveShipperMsgBtn" CssClass="fg-white" ClientIDMode="Static" OnClick="SaveShipperMsgBtn_Click">
                                <i title="Save"  style="font-size:1em;" class="glyphicon glyphicon-floppy-disk av-hand-cursor"></i></asp:LinkButton>
                            </span>
                        <div class="awp_box rounded bg-sg-title shadow">           
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black">Shipper Message</span>
                            </div>
                            <div class="awp_box_content bg-sg-box">
                                <div class=" textarea av-input" style="min-height:75px !important;"><textarea class="form-control" id="shipmsgbox" runat="server" ></textarea></div>
                            </div>
                        </div>
                    </div>
                 </div>
                </div>

                 </div>

            </div>
                <div id="UPSbox"  class="row">
                 <div class="col-md-12">
                    <!--BUTTONS-->
                    <span class="awp-save-btn bg-red fg-white shadow">
                        <asp:LinkButton runat="server" ID="SaveUpsAcctBtn" CssClass="fg-white" ClientIDMode="Static" OnClick="SaveUpsAcctBtn_Click">
                        <i title="Save "  style="font-size:1em;" class="glyphicon glyphicon-floppy-disk av-hand-cursor"></i></asp:LinkButton>
                    </span>

                    <!--BOX-->
                     <div class="awp_box rounded bg-sg-title shadow">           
                    <div class="awp_box_title bg-sg-title">
                       <span class="fg-white shadow-metro-black">UPS Account</span>
                    </div>
                    <div class="awp_box_content bg-sg-box">
                        <div class="row">
                            <div class="col-md-3">
                                <div class="awp_box rounded bg-sg-title shadow">           
                                    <div class="awp_box_title bg-sg-title">
                                       <span class="fg-white shadow-metro-black">Access License Number</span>
                                    </div>
                                    <div class="awp_box_content bg-sg-box">
                                        <div class=" input-control text ">                                    
                                             <input runat="server" id="ups_aln" type="text" class="av-input" placeholder="Access License #..." />
                                        </div>
                                    </div>
                               </div>
                           </div>
                            <div class="col-md-3">
                                <div class="awp_box rounded bg-sg-title shadow">           
                                    <div class="awp_box_title bg-sg-title">
                                       <span class="fg-white shadow-metro-black">User ID</span>
                                    </div>
                                    <div class="awp_box_content bg-sg-box">
                                        <div class=" input-control text ">                                    
                                             <input runat="server" id="ups_userid" type="text" class="av-input" placeholder="User ID..." />
                                        </div>
                                    </div>
                               </div>
                           </div>
                            <div class="col-md-3">
                                <div class="awp_box rounded bg-sg-title shadow">           
                                    <div class="awp_box_title bg-sg-title">
                                       <span class="fg-white shadow-metro-black">Password</span>
                                    </div>
                                    <div class="awp_box_content bg-sg-box">
                                        <div class=" input-control text ">                                    
                                             <input runat="server" id="ups_pwd" type="password" class="av-input" placeholder="Password..." />
                                        </div>
                                    </div>
                               </div>
                           </div>
                            <div class="col-md-3">
                                <div class="awp_box rounded bg-sg-title shadow">           
                                    <div class="awp_box_title bg-sg-title">
                                       <span class="fg-white shadow-metro-black">Shipper Number</span>
                                    </div>
                                    <div class="awp_box_content bg-sg-box">
                                        <div class=" input-control text ">                                    
                                             <input runat="server" id="ups_shippernumber" type="text" class="av-input" placeholder="Shipper #" />
                                        </div>
                                    </div>
                               </div>
                           </div>


                        </div>
                    </div>
                </div>
                 </div>

            </div>


        

                

        </asp:View>
        <asp:View ID ="PersonnelView" runat="server">             
            <div id="PersonnelBox"  class="row">           
                     <div class =" col-md-6">
                        <div class="awp_box rounded bg-sg-title shadow">       
                            <span class="awp-bar " style="padding-bottom:-10px!important;">
                                 <span class="awp-bar-btn bg-green fg-white shadow"><i onclick="ShowDiv('CreateEngineerModal')" title="Add To Task List" class="mif-plus av-hand-cursor"></i></span>
                            </span>  
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black">Service Engineers</span>
                            </div>
                            <div class="awp_box_content bg-sg-box">
                                <asp:Repeater ID="EngineerRepeater" runat="server">
                                            <ItemTemplate>
                                                <div class="row bg-sg-box" style="margin:10px;">                                           
                                                        <div class="col-sm-12 bg-sg-box" style="width:auto !important; text-align:left !important">
                                                            <asp:Button ToolTip="Delete" CommandName="DeleteEngineer" ID="DeleteEngineer" CssClass="btn btn-sm" runat="server" Text="X" Font-Bold="true" CommandArgument='<%# Eval("Name") %>' OnCommand="DeleteEngineer_Command" />
                                                            <strong class="fg-white shadow-metro-black"><%# Eval("Name")%>-<%# Eval("Email")%></strong></div>  
                                               </div>
                                     
                                            </ItemTemplate>
                                        </asp:Repeater>
                            </div>
                        </div>

                    </div>
                    <div class =" col-md-6">
                        <div class="awp_box rounded bg-sg-title shadow">    
                            <span class="awp-bar " style="padding-bottom:-10px!important;">
                                 <span class="awp-bar-btn bg-green fg-white shadow"><i onclick="ShowDiv('CreateShipperModal')" title="Add To Task List" class="mif-plus av-hand-cursor"></i></span>
                            </span>  
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black">Shipping Personel</span>
                            </div>
                            <div class="awp_box_content bg-sg-box" style="padding:15px;">                                
                                        <asp:Repeater ID="ShippingPersonRepeater" ClientIDMode="Static" runat="server">
                                            <ItemTemplate>
                                                <div class="row bg-sg-box"  style="margin:10px;">                                           
                                                        <div class="col-sm-12 bg-sg-box" style="width:auto !important; text-align:left !important">
                                                            <asp:Button ToolTip="Delete" ID="DeleteShippingPersonBtn" CssClass="btn btn-sm" runat="server" Text="X" Font-Bold="true" CommandName='<%#Eval("Email")%>'  OnCommand="DeleteShippingPersonBtn_Command" />
                                                            <strong class="fg-white shadow-metro-black"><%# Eval("Name")%>-<%# Eval("Email")%></strong></div>  
                                               </div>
                                     
                                            </ItemTemplate>
                                        </asp:Repeater>
                         
                            </div>
                        </div>

                    </div>

                </div>
                <div id="CreateEngineerModal" class="awp-outer-dialog" style="display:none; z-index:99998" >
        <div class=" awp-inner-dialog" style="opacity:initial !important; top:115px !important;">
                    <span class="awp-dialog-close-btn bg-red fg-white shadow " onclick="HideDiv('CreateEngineerModal')"><i title="Close"  style="vertical-align:top" class="mif-cross av-hand-cursor fg-white shadow-metro-black"></i></span>
                    <div class="awp_box rounded bg-sg-box-dark shadow" style="left:50% !important; top:30%">
                        <div class="awp_box_title bg-sg-title">
                           <span class="fg-white shadow-metro-black"><span class="mif-file-upload mif-2x"></span>Create Personnel</span>
                        </div>
                        <div class="awp_box_content bg-sg-title fg-white" style="text-align:left">  
                            <div>
                                 <span style="font-weight:bold;" ><strong>Name</strong></span>
                                 <asp:TextBox runat="server" ID="EngineerNameInput" CssClass="form-control" TextMode="SingleLine" />
                            </div>
                            <div>
                                 <span style="font-weight:bold;" ><strong>Email</strong></span>
                                 <asp:TextBox runat="server" ID="EngineerEmailInput" CssClass="form-control" TextMode="Email" />
                            </div>
                            <asp:Button CausesValidation="false"  runat="server" ID="CreateEngineerBtn" ClientIDMode="Static" OnClick="CreateEngineerBtn_Click" Text="Create" />
                       </div>
                    </div>      
        </div>
    </div>
                    <div id="CreateShipperModal" class="awp-outer-dialog" style="display:none; z-index:99998" >
        <div class=" awp-inner-dialog" style="opacity:initial !important; top:115px !important;">
                    <span class="awp-dialog-close-btn bg-red fg-white shadow " onclick="HideDiv('CreateShipperModal')"><i title="Close"  style="vertical-align:top" class="mif-cross av-hand-cursor fg-white shadow-metro-black"></i></span>
                    <div class="awp_box rounded bg-sg-box-dark shadow" style="left:50% !important; top:30%">
                        <div class="awp_box_title bg-sg-title">
                           <span class="fg-white shadow-metro-black"><span class="mif-file-upload mif-2x"></span>Create Personnel</span>
                        </div>
                        <div class="awp_box_content bg-sg-title fg-white " style="text-align:left">  
                            <div>
                                 <span style="font-weight:bold;" ><strong>Name</strong></span>
                                 <asp:TextBox runat="server" ID="ShipperNameInput" CssClass="form-control" TextMode="SingleLine" />
                            </div>
                            <div>
                                 <span style="font-weight:bold;" ><strong>Email</strong></span>
                                 <asp:TextBox runat="server" ID="ShipperEmialInput" CssClass="form-control" TextMode="Email" />
                            </div>
                            <asp:Button CausesValidation="false"  runat="server" ID="CreateShipperBtn" ClientIDMode="Static" OnClick="CreateShipper_Click" Text="Create" />
                       </div>
                    </div>      
        </div>
    </div>

       </asp:View>
        <asp:View ID ="CustomerView" runat="server">          
                <div id="CustomersBox"  class="row">
                        <div class =" col-md-12">
                        <div class="awp_box rounded bg-sg-title shadow"> 
                            <span class="awp-bar " style="padding-bottom:-10px!important;">
                                 <span class="awp-bar-btn bg-green fg-white shadow"><i onclick="ShowDiv('CreateCustomerModal')" title="Add To Task List" class="mif-plus av-hand-cursor"></i></span>
                            </span>                          
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black">Customers</span>
                            </div>
                            <div class="awp_box_content bg-sg-box">
                                        <asp:Repeater ID="CustomerRepeater" ClientIDMode="Static" runat="server" OnItemCommand="CustomerRepeater_ItemCommand">
                                            <ItemTemplate>
                                                <div class="panel-group">
                                                  <div class="panel panel-default">
                                                    <div class="panel-heading">
                                                      <h4 class="panel-title">
                                                        <a data-toggle="collapse" href='#collapse<%# Container.ItemIndex + 1%>'><%# Eval("CompanyName")%></a>
                                                      </h4>
                                                    </div>
                                                    <div id='collapse<%# Container.ItemIndex + 1%>' class="panel-collapse collapse">
                                                      <div class="panel-body">
                                                          <div class="row">
                                                            <div class="col-md-4">
                                                            <address>
                                                              <strong><%# Eval("CompanyName") %></strong><br>
                                                              <%# Eval("Address") %><br>
                                                                <%# Eval("Address2") %><br>
                                                                <%# Eval("Address3") %><br>
                                                              <%# Eval("City") %>, <%# Eval("State") %> <%# Eval("Postal") %><br>
                                                                <%# Eval("Country") %><br>
                                                              <abbr title="Phone">P:</abbr> <%# Eval("Phone") %>
                                                            </address>
                                                            <address>
                                                              <strong><%# Eval("Attn") %></strong><br>
                                                              <a href='mailto:<%# Eval("Email") %>'><%# Eval("Email") %></a>
                                                            </address>
                                                            </div>
                                                          </div>
                                                      </div>
                                                      <div class="panel-footer">
                                                            <asp:Button CommandName="DeleteCustomer" ToolTip="Delete" ID="DeleteEngineer" CssClass="btn btn-sm" runat="server" Text="X" Font-Bold="true" CommandArgument='<%# Eval("CompanyName") +"|"+ Eval("Address") %>'/>                                                         
                                                      </div>
                                                    </div>
                                                  </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                            </div>
                        </div>

                    </div>
            </div>
        <div id="CreateCustomerModal" class="awp-outer-dialog" style="display:none; z-index:99998" >
        <div class=" awp-inner-dialog" style="opacity:initial !important; top:10% !important;">
                    <span class="awp-dialog-close-btn bg-red fg-white shadow " onclick="HideDiv('CreateCustomerModal')"><i title="Close"  style="vertical-align:top" class="mif-cross av-hand-cursor fg-white shadow-metro-black"></i></span>
                    <div class="awp_box rounded bg-sg-box-dark shadow" style="left:50% !important; top:30%">
                        <div class="awp_box_title bg-sg-title">
                           <span class="fg-white shadow-metro-black"><span class="mif-file-upload mif-2x"></span>Create Customer</span>
                        </div>
                        <div class="awp_box_content bg-sg-title fg-white">  
                            <div class="row">
                                <div class ="col-md-6" style="text-align:left">
                                    <div>
                                         <span style="font-weight:bold;" ><strong>Company Name</strong></span>
                                         <asp:TextBox runat="server" ID="SprCompany" CssClass="form-control" TextMode="SingleLine" />
                                    </div>
                                    <div>
                                         <span style="font-weight:bold;" ><strong>Address</strong></span>
                                         <asp:TextBox runat="server" ID="SprAddr" CssClass="form-control" TextMode="SingleLine" />
                                    </div>                            
                                    <div>
                                         <span style="font-weight:bold;" ><strong>Address Line 2</strong></span>
                                         <asp:TextBox runat="server" ID="SprAddr2" CssClass="form-control" TextMode="SingleLine" />

                                    </div>
                                    <div>
                                         <span style="font-weight:bold;" ><strong>City</strong></span>
                                         <asp:TextBox runat="server" ID="SprCty" CssClass="form-control" TextMode="SingleLine" />
                                    </div>
                                    <div>
                                         <span style="font-weight:bold;" ><strong>State</strong></span>
                                         <asp:TextBox runat="server" ID="SprState" CssClass="form-control" TextMode="SingleLine" />
                                    </div>
                                </div>
                                <div class ="col-md-6" style="text-align:left">
                                     <div>
                                         <span style="font-weight:bold;" ><strong>Postal</strong></span>
                                         <asp:TextBox runat="server" ID="SprPostal" CssClass="form-control" TextMode="SingleLine" />
                                    </div>
                                    <div>
                                         <span style="font-weight:bold;" ><strong>Country</strong></span>
                                         <asp:TextBox runat="server" ID="SprCountry" CssClass="form-control" TextMode="SingleLine" />
                                    </div>
                                    <div>
                                         <span style="font-weight:bold;" ><strong>Attn:</strong></span>
                                         <asp:TextBox runat="server" ID="SprName" CssClass="form-control" TextMode="SingleLine" />
                                    </div>
                                    <div>
                                         <span style="font-weight:bold;" ><strong>Email</strong></span>
                                         <asp:TextBox runat="server" ID="SprEmail" CssClass="form-control" TextMode="Email" />
                                    </div>
                                    <div>
                                         <span style="font-weight:bold;" ><strong>Phone</strong></span>
                                         <asp:TextBox runat="server" ID="SprPhone" CssClass="form-control" TextMode="SingleLine" />
                                    </div>
                                </div>
                            </div>
                           
                       
                            <asp:Button CausesValidation="false"  runat="server" ID="CreateCustomerBtn" ClientIDMode="Static" OnClick="CreateCustomerBtn_Click" Text="Create" />
                       </div>
                    </div>      
        </div>
    </div>

       </asp:View>
        <asp:View ID ="NotificationsView" runat="server">
                <div id="NotificationsBox"  class="row">
                    <div class="col-md-12">
                         <div class="awp_box rounded bg-sg-title shadow">           
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black">Pending Notifications</span>
                            </div>
                            <div class="awp_box_content bg-sg-box">
                                <!--Repeaters for 30 Day Notices-->
                                        <asp:Repeater ID="Notice30DayRepeater" ClientIDMode="Static" runat="server">
                                            <ItemTemplate>
                                                <div class="row bg-sg-box" style="margin:10px;">                                           
                                                        <div class="col-sm-12 bg-sg-box" style="width:auto !important; padding-left:10px; text-align:left">
                                                            <asp:Button ToolTip="Delete Notice" ID="DeleteNotice30DayBtn" CssClass="btn btn-sm" runat="server" Text="X" Font-Bold="true" CommandName='<%#Eval("NoticeControlNumber")%>' CommandArgument='<%#Eval("GUID")%>'  OnCommand="DeleteNotice30DayBtn_Command" />
                                                            <asp:Button ToolTip="Send Notice" ID="SendNotice30DayBtn" CssClass="btn btn-sm" runat="server" Text="Send" Font-Bold="true" CommandName='<%#Eval("NoticeControlNumber")%>' CommandArgument='<%#Eval("GUID")%>' OnCommand="SendNotice30DayBtn_Command" />
                                                        <strong class="fg-white shadow-metro-black"><%# Eval("NoticeControlNumber")%></strong><strong class="fg-white shadow-metro-black"> &nbsp-&nbsp Scheduled in&nbsp<%# Eval("DaysUntil").ToString()%>&nbsp Days</strong> &nbsp-&nbsp 
                                                            <strong class="fg-white shadow-metro-black"><%# Eval("Name")%></strong>
                                                        </div>  
                                                        
                                               </div>
                                     <hr />
                                            </ItemTemplate>
                                        </asp:Repeater>
                            </div>
                         </div>
                       </div>       
                     
                </div>

       </asp:View>
               <asp:View ID ="AssetView" runat="server">
                <div id="AssetBox"  class="row">
                    <div class="col-md-12">
                         <div class="awp_box rounded bg-sg-title shadow">           
                            <div class="awp_box_title bg-sg-title">
                               <span class="fg-white shadow-metro-black">Assets</span>
                            </div>
                            <div class="awp_box_content bg-sg-box">
                                <!--Repeaters for 30 Day Notices-->
                                        <asp:Repeater ID="AssetRepeater" ClientIDMode="Static" runat="server">
                                            <ItemTemplate>
                                                <div class="row bg-sg-box" style="margin:10px;">                                           
                                                        <div class="col-sm-12 bg-sg-box" style="width:auto !important; padding-left:10px; text-align:left">
                                                            <asp:Button ToolTip="Delete Asset" ID="DeleteAssetBtn" CssClass="btn btn-sm" runat="server" Text="X" Font-Bold="true" CommandName='<%#Eval("AssetNumber")%>' CommandArgument='<%#Eval("AssetName")%>'  OnCommand="DeleteAssetBtn_Command"/>
                                                            <asp:Button ToolTip="Add Calibration" ID="AddCalibBtn" CssClass="btn btn-sm" runat="server" Text="Calib." Font-Bold="true" CommandName='<%#Eval("AssetNumber")%>' CommandArgument='<%#Eval("AssetName")%>' OnCommand="AddCalibBtn_Command" />
                                                        <strong class="fg-white shadow-metro-black"><%# Eval("AssetNumber")%></strong><strong class="fg-white shadow-metro-black"> &nbsp-&nbsp  <strong class="fg-white shadow-metro-black"><%# Eval("AssetName")%></strong>
                                                        </div>  
                                                        
                                               </div>
                                     <hr />
                                            </ItemTemplate>
                                        </asp:Repeater>
                            </div>
                         </div>
                       </div>       
                     
                </div>

       </asp:View>

    </asp:MultiView>

</asp:Content>
