<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="menu.ascx.cs" Inherits="Web_App_Master.Account.menu" %>
<div class="app-bar">

                    <a class="app-bar-element branding">Metro UI CSS</a>

                    <ul class="nav navbar-nav">
                        <li data-flexorderorigin="0" data-flexorder="1"><a href="">Home</a></li>
                        <li data-flexorder="2" data-flexorderorigin="1"><a href="">Unimportant</a></li>
                        <li data-flexorder="1" data-flexorderorigin="2"><a href="">Important</a></li>
                        <li data-flexorderorigin="3" data-flexorder="4">
                            <a href="" class="dropdown-toggle">Products</a>
                            <ul class="d-menu" data-role="dropdown">
                                <li><a href="">Windows 10</a></li>
                                <li><a href="">Spartan</a></li>
                                <li><a href="">Outlook</a></li>
                                <li><a href="">Office 2015</a></li>
                                <li class="divider"></li>
                                <li><a href="" class="dropdown-toggle">Other Products</a>
                                    <ul class="d-menu" data-role="dropdown">
                                        <li><a href="">Internet Explorer 10</a></li>
                                        <li><a href="">Skype</a></li>
                                        <li><a href="">Surface</a></li>
                                    </ul>
                                </li>
                            </ul>
                        </li>
                    </ul>
                    <div class="app-bar-pullbutton automatic" style="display: block;"></div>
                    <ul class="nav navbar-nav place-right hidden" data-flexdirection="reverse">
                        
                        
                        
                    </ul>
                <div class="clearfix" style="width: 0;"></div><nav class="app-bar-pullmenu hidden flexstyle-nav navbar-nav" style="display: none;"><ul class="app-bar-pullmenubar hidden nav navbar-nav"></ul><ul class="app-bar-pullmenubar nav navbar-nav" style="display: block;"><li data-flexorderorigin="0" data-flexorder="1" class="app-bar-pullmenu-entry">
                            <div class="input-control text">
                                <input type="text" placeholder="Search...">
                            </div>
                        </li><li data-flexorderorigin="1" data-flexorder="2" class="app-bar-pullmenu-entry"><a href=""><span class="mif-phone icon"></span>Call</a></li><li data-flexorderorigin="2" data-flexorder="3" class="app-bar-pullmenu-entry"><a href=""><span class="mif-cart icon"></span> Cart(0)</a></li></ul></nav></div>