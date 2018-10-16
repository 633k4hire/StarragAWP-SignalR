<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebApp.aspx.cs" Inherits="Web_App_Master.Account.WebApp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Web App</title>
    	<meta name="viewport" content="width=device-width, initial-scale=1.0, minimum-scale=1.0"/>
	
	<link rel="stylesheet" media="screen, projection" href="~/Content/screen.css"/>
	<link href="../Content/metro.css" rel="stylesheet" />
    <script type="text/javascript" src="../Scripts/jquery-1.11.0.min.js"></script>
    <script src="../Scripts/metro.js"></script>
	
	
    <script type="text/javascript">
        $(document).ready(function () {

            // define our variables
            var fullHeightMinusHeader, sideScrollHeight = 0;

            // create function to calculate ideal height values
            function calcHeights() {

                // set height of main columns
                fullHeightMinusHeader = $(window).height() - $(".app-header").outerHeight();
                $(".main-content, .sidebar-two, .sidebar-one").height(fullHeightMinusHeader);

                // set height of sidebar scroll content
                sideScrollHeight = fullHeightMinusHeader - $(".nav-menu").outerHeight() - $(".buttons").outerHeight();
                $(".side-scroll").height(sideScrollHeight);

            } // end calcHeights function

            // run on page load    
            calcHeights();

            // run on window resize event
            $(window).resize(function () {
                calcHeights();
            });

        });

    </script>
</head>


    <body style="">
	    <form id="form1" runat="server" style="height:100%">
       
  
	<!-- app-header -->
	<div class="app-header">
		<div class="app-bar">
                    <a class="app-bar-element branding">Metro UI CSS</a>
                    <span class="app-bar-divider"></span>
                    <ul class="nav navbar-nav small-dropdown">
                        <li data-flexorderorigin="0" data-flexorder="1"><a href="">Home</a></li>
                        <li data-flexorderorigin="1" data-flexorder="2">
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
                        <li data-flexorderorigin="2" data-flexorder="3">
                            <a href="" class="dropdown-toggle">Support</a>
                            <ul class="d-menu" data-role="dropdown">
                                <li><a href="">About</a></li>
                                <li><a href="">Contacts</a></li>
                                <li><a href="">Community forum</a></li>
                                <li>
                                    <a href="" class="dropdown-toggle">Support</a>
                                    <ul class="d-menu" data-role="dropdown">
                                        <li><a href="" class="dropdown-toggle">About</a></li>
                                        <li><a href="">Contacts</a></li>
                                        <li><a href="">Community forum</a></li>
                                    </ul>
                                </li>
                            </ul>
                        </li>
                        <li data-flexorderorigin="3" data-flexorder="4"><a href="">Help</a></li>
                    </ul>
                    <div class="app-bar-element place-right active-container">
                        <a class="dropdown-toggle fg-white"><span class="mif-enter"></span> Enter</a>
                        <div class="app-bar-drop-container place-right" data-role="dropdown" data-no-close="true" style="display: none;">
                            <div class="padding20 fg-dark">
                                
                                    <h4 class="text-light">Login to service...</h4>
                                    <div class="input-control text">
                                        <span class="mif-user prepend-icon"></span>
                                        <input type="text">
                                    </div>
                                    <div class="input-control text">
                                        <span class="mif-lock prepend-icon"></span>
                                        <input type="password">
                                    </div>
                                    <label class="input-control checkbox small-check">
                                        <input type="checkbox">
                                        <span class="check"></span>
                                        <span class="caption">Remember me</span>
                                    </label>
                                    <div class="form-actions">
                                        <button class="button primary">Login</button>
                                        <button class="button link fg-grayLighter">Cancel</button>
                                    </div>
                                
                            </div>
                        </div>
                    </div>
                <div class="app-bar-pullbutton automatic" style="display: none;"></div><div class="clearfix" style="width: 0;"></div><nav class="app-bar-pullmenu hidden flexstyle-nav navbar-nav" style="display: none;"><ul class="app-bar-pullmenubar hidden nav navbar-nav"></ul></nav></div>

        
	</div><!-- /app-header -->
	
	<!-- sidebar-one -->
	<div class="sidebar-one" style="height: 618px;">
		
		<!-- nav-menu -->
		<div class="nav-menu">
			<ul>
				<li><a href="https://learnwebcode.com/tutorial-files/full-height-width/#">Menu Item 1</a></li>
				<li><a href="https://learnwebcode.com/tutorial-files/full-height-width/#">Menu Item 2</a></li>
				<li><a href="https://learnwebcode.com/tutorial-files/full-height-width/#">Menu Item 3</a></li>
				<li><a href="https://learnwebcode.com/tutorial-files/full-height-width/#">Menu Item 4</a></li>
				<li><a href="https://learnwebcode.com/tutorial-files/full-height-width/#">Menu Item 5</a></li>
			</ul>	
		</div><!-- /nav-menu -->

		<!-- side-scroll -->
		<div class="side-scroll" style="height: 348px;">

			<!-- side-scroll-inner -->
			<div class="side-scroll-inner">

				<h4>Scrollable Content</h4>

				<p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</p>

				<p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</p>

			</div><!-- /side-scroll-inner -->

		</div><!-- /side-scroll -->

		<!-- buttons -->
		<div class="buttons">
			<!-- buttons-inner -->
			<div class="buttons-inner">
				<a href="https://learnwebcode.com/tutorial-files/full-height-width/#">Button 1</a>
				<a href="https://learnwebcode.com/tutorial-files/full-height-width/#">Button 2</a>
			</div><!-- /buttons-inner -->
		</div><!-- /buttons -->
		
	</div><!-- /sidebar-one -->
	
	<!-- main-content -->
	<div class="main-content" style="height: 618px;">
		
		<!-- main-content-inner -->
		<div class="main-content-inner">
			
			
		</div><!-- /main-content-inner -->
		
	</div><!-- /main-content -->
	
	<!-- sidebar-two -->
	<div class="sidebar-two" style="height: 618px;">
		
		<!-- sidebar-two-inner -->
		<div class="sidebar-two-inner">
			
			<h3>Sidebar</h3>
			<p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum.</p>
			
			<p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum.</p>
			
			<p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum.</p>
			
			<p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum.</p>
			
		</div><!-- /sidebar-two-inner -->
		
	</div><!-- /sidebar-two -->
	
          </form>

</body>
</html>
