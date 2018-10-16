$(document).ready(function () {

    function calcHeight() {
        var fullHeightMinusHeader = jQuery(window).height() - 450;
        if (fullHeightMinusHeader > 100) {
           // $("#L0XBOX").height(fullHeightMinusHeader);
            $("#FILEBOX").height(fullHeightMinusHeader);
            $("#TEST").height(fullHeightMinusHeader);
        }
        //$("#DEBUGBOX").height(newHeight);
    }

    // run on page load    
    calcHeight();

    // run on window resize event
    $(window).resize(function () {
        calcHeight();
    });

});
