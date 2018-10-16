<div class="app-bar" >
            <asp:LoginView ID="brandSpot" runat="server" ViewStateMode="Disabled">
                <AnonymousTemplate>
                    <a class="app-bar-element branding" style="text-decoration:none;"  ><strong><i class="glyphicon glyphicon-globe"></i>&nbspStarrag AWP</strong></a>
                </AnonymousTemplate>
                <LoggedInTemplate>
                    <a class="app-bar-element branding" onclick="toggleMetroCharm('#assets')"><i class="glyphicon glyphicon-tasks"></i><strong>&nbsp;Tasks</strong></a>
                </LoggedInTemplate>
            </asp:LoginView>
                    
                    <span class="app-bar-divider"></span>
                    <ul class="nav navbar-nav small-dropdown">
                        <asp:LoginView runat="server" ID="flexer" EnableViewState="false">
                            <AnonymousTemplate>
                                <li ><a runat="server" href="~/"><i class="glyphicon glyphicon-home"></i></a></li>
                             
                            </AnonymousTemplate>
                            <LoggedInTemplate>
                                <li><a runat="server" href="~/"><i class="glyphicon glyphicon-home"></i></a></li>
                                <li>
                                  <a  runat="server" href="Account/PrintBarcodes.aspx"><i class="mif-printer"></i>&nbsp;</a>
                                </li>  
                                <li>
                          <a onclick="ShowLoader()" runat="server" href="Account/AssetView.aspx"><i class="glyphicon glyphicon-dashboard"></i>&nbsp;<strong>Assets</strong></a>
                        </li>     
                                <li>
                                    <div class="input-control" data-role="input" style="vertical-align:top">
                                        <input id="BarcodeSearchBox" type="number" style="width:130px;"  />
                            <button class="button" id="BarcodeButton" type="button" onclick=" BarcodeScanned($('#BarcodeSearchBox').val());"><i id="barcodeIcon" style="font-size:1.7em;" class="glyphicon glyphicon-barcode "></i></button>
                                       
                        </div>
                                     <asp:CheckBox ClientIDMode="Static" id="BarcodeCheckBox"  runat="server"  style="vertical-align:middle" /><i class="glyphicon glyphicon-check" style="padding-left:5px"></i>
                        </li>     
                            </LoggedInTemplate>
                        </asp:LoginView>
                    </ul>

                    <!--Right Side Logged In Menu Items-->
            <ul class="nav navbar-nav small-dropdown place-right">
                <asp:PlaceHolder ID ="NotificationsHolder" runat="server" ClientIDMode="Static" Visible="false">
                <li>
                            <a class="dropdown-toggle"><i id="NotificationIcon" runat="server" class="mif-bell" onclick="Quiet('NotificationIcon')"></i></a>
                            <ul class="d-menu place-right ShowScroll" data-role="dropdown" style=" text-shadow:none!important;display: none; width:250px; height:auto; max-height:300px; z-index:90000; opacity: 0.95;filter: alpha(opacity=95);">
                                    <div id="ActiveNoticeContainer" style="padding:2px;"><!--Dynamic Placeholder-->   

                                         <!--Repeater for notifications NON OVERFLOW SCROLLABLE CONTAINER-->
                                        <asp:UpdatePanel runat="server" ID ="UserNoticeUpdatePanel" ClientIDMode="Static" ChildrenAsTriggers="true" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:Repeater ID="UserNoticerRepeater" ClientIDMode="Static" runat="server">
                                                    <HeaderTemplate>   
                                                        <div class="listview set-border padding5 default" data-role="listview" id="noticelv1" >
                                                    </HeaderTemplate>
                                                    <ItemTemplate>  
                                                        <div class="list">
                                                            <img src="/Images/transparent.png" class="list-icon"/>
                                                            <span class="list-title"><a href='<%# Eval("Href") %>'><strong><%# Eval("TimeString")%> - <%# Eval("Entry") %></strong></a></span>
                                                        </div>                                                      
                                                    </ItemTemplate>                                                    
                                                    <FooterTemplate> </div></FooterTemplate>
                                                </asp:Repeater>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="UpdateAllCarts" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>                                         
                                    </div>
                            </ul>
                </li>
                </asp:PlaceHolder>
            <asp:LoginView ID="notifyView" EnableViewState="false" runat="server">
                <RoleGroups>
                                        <asp:RoleGroup Roles="superadmin,Admins,Users">
                                            <ContentTemplate>
                      <li>
                                    <a href="/Map.aspx" ><i class="mif-map"></i></a>
                      </li>
                      <li>
                                    <a href="/Account/CheckIn.aspx" ><i class="glyphicon glyphicon-cloud-upload"></i></a>
                      </li>
                      <li >
                                    <a href="/Account/OutCart.aspx" ><i class="glyphicon glyphicon-cloud-download"></i></a>
                      </li>
                      <li>
                            <a class="dropdown-toggle"><i id="UserIcon" runat="server" class="mif-user" onclick="Quiet('UserIcon')"></i></a>
                            <ul class="d-menu place-right" data-role="dropdown" style=" text-shadow:none!important;display: none; width:auto; height:auto; max-height:300px; z-index:90000; opacity: 0.95;filter: alpha(opacity=95);">
                                 <li>
                                     <h4 class="text-light"><a runat="server" href="~/Account/Admin/AdminDashboard" title="Admin settings">Admin</a></h4>
                                 </li>  
                                <li>
                                     <h4 class="text-light"><a runat="server" href="~/Account/Manage" title="Manage your account">Manage Account</a></h4>
                                 </li> 
                                <li>
                                     <h4 class ="text-light"><asp:LoginStatus runat="server" LogoutAction="Redirect" LogoutText="Log off" LogoutPageUrl="~/" OnLoggingOut="Unnamed_LoggingOut" /></h4>
                                 </li> 
                            </ul>
                     </li>


                                            </ContentTemplate>
                                       </asp:RoleGroup>
                </RoleGroups>
                <LoggedInTemplate>                   
                     
                      <li>
                                    <a href="/Account/CheckIn.aspx" ><i class="glyphicon glyphicon-cloud-upload"></i></a>
                      </li>
                      <li >
                                    <a href="/Account/OutCart.aspx" ><i class="glyphicon glyphicon-cloud-download"></i></a>
                      </li>
                      <!--<li data-flexorderorigin="3" data-flexorder="4" class=""><a  onclick="toggleMetroCharm('#settings-charm')" ><i class="glyphicon glyphicon-cog"></i></a></li>-->
                      <li>
                            <a class="dropdown-toggle"><i id="UserIcon" runat="server" class="mif-user" onclick="Quiet('UserIcon')"></i></a>
                            <ul class="d-menu place-right" data-role="dropdown" style=" text-shadow:none!important;display: none; width:auto; height:auto; max-height:300px; z-index:90000; opacity: 0.95;filter: alpha(opacity=95);">
                                <li>
                                     <h4 class="text-light"><a runat="server" href="~/Account/Manage" title="Manage your account">Manage Account</a></h4>
                                 </li> 
                                <li>
                                     <h4 class ="text-light"><asp:LoginStatus runat="server" LogoutAction="Redirect" LogoutText="Log off" LogoutPageUrl="~/" OnLoggingOut="Unnamed_LoggingOut" /></h4>
                                 </li> 
                            </ul>
                     </li>                                    

             
                </LoggedInTemplate>
                 <AnonymousTemplate>
                     <li>
                            <a class="dropdown-toggle"><i id="UserIcon" runat="server" class="mif-user" onclick="Quiet('UserIcon')"></i></a>
                            <ul class="d-menu place-right" data-role="dropdown" style=" text-shadow:none!important;display: none; width:auto; height:auto; max-height:300px; z-index:90000; opacity: 0.95;filter: alpha(opacity=95);">
                                 <li>
                                     <h4 class="text-light"><a runat="server" href="~/Account/Register">Register</a></h4>
                                 </li>  
                                <li>
                                     <h4 class="text-light"><a runat="server" href="~/Account/Login">Log in</a></h4>
                                 </li> 
                            </ul>
                     </li>
                                   
                                         
                                    
                                    </AnonymousTemplate>
            </asp:LoginView>
             </ul>     
            <!--  End Right Items-->


                <div class="app-bar-pullbutton automatic" style="display: none;"></div><div class="clearfix" style="width: 0;"></div><nav class="app-bar-pullmenu hidden flexstyle-nav navbar-nav" style="display: none;"><ul class="app-bar-pullmenubar hidden nav navbar-nav"></ul></nav></div>

        <%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebUserControl1.ascx.cs" Inherits="Web_App_Master.WebUserControl1" %>
