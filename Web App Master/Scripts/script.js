jQuery(document).ready(function() {
	
	// define our variables
	var fullHeightMinusHeader, sideScrollHeight = 0;
	
	// create function to calculate ideal height values
	function calcHeights() {
		
		// set height of main columns
		fullHeightMinusHeader = jQuery(window).height() - jQuery(".app-header").outerHeight();
		jQuery(".main-content, .sidebar-one").height(fullHeightMinusHeader);
		
		// set height of sidebar scroll content
        jQuery(".sidebar-one").height(fullHeightMinusHeader);
		sideScrollHeight = fullHeightMinusHeader / 2;
        jQuery(".side-scroll").height(sideScrollHeight);
        jQuery(".side-scroll2").height(sideScrollHeight);
					
	} // end calcHeights function
	
    // run on page load    
	calcHeights();
	
	// run on window resize event
	jQuery(window).resize(function() {
		calcHeights();
	});
	
});