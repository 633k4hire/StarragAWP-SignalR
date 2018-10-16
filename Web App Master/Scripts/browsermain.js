var IsResizing = false;
var bShowList = false;
$(document).ready(function () {

    // run on page load  
    var viewWidth = jQuery(window).width();
    var left = 5;
    var right = viewWidth - 5;
    $(".resizable1").width(left);
    $("#DirTree").width(left-19);
    $(".resizable2").width(right);

    function calcFileAppHeight() {
        var footer = 50;
        var toolbar = 50;
        var newHeight = jQuery(window).height() - jQuery("#MainMenu").height() - jQuery("#footer").height() - jQuery("#toolbar").height()-4;
        $("#browserbox").height(newHeight);

        if (IsResizing == false) {
            var viewWidth = jQuery(window).width();
            var left = 5;
            var right = viewWidth - 5;
            $(".resizable1").width(left);
            $(".resizable2").width(right);
        }

        var aa = $(".resizable1").width();
        $("#DirTree").width(aa - 19);
        
       
    }

    calcFileAppHeight();
    // run on window resize event
    $(window).resize(function () {
        calcFileAppHeight();
    });



});
$(function () {
    $(".resizable1").resizable(
        {
            autoHide: true,
            handles: 'e',
            start: function (e, ui) {
                IsResizing = true;
            },
            resize: function (e, ui) {
                var parent = ui.element.parent();
                //alert(parent.attr('class'));
                var remainingSpace = parent.width() - ui.element.outerWidth(),
                    divTwo = ui.element.next(),
                    divTwoWidth = (remainingSpace - (divTwo.outerWidth() - divTwo.width())) / parent.width() * 100 + "%";
                divTwo.width(divTwoWidth);
                var aa = $(".resizable1").width();
                $("#DirTree").width(aa - 19);
            },
            stop: function (e, ui) {
                var parent = ui.element.parent();
                ui.element.css(
                    {
                        width: ui.element.width() / parent.width() * 100 + "%",
                    });
                IsResizing = false;
            }
        });
});
function treeClick(evt) {
    // this gets the element clicked so you can do what you like with it
    var src = window.event != window.undefined ? window.event.srcElement : evt.target;
}
function fileNodeClick(id) {
    $("#FileID").val(id);
    $("#FileClick").click();
}
function dirNodeClick(id) {
    $("#DirID").val(id);
    $("#DirClick").click();

}
function ToggleList()
{
    bShowList = !bShowList;
    var viewWidth = jQuery(window).width();
    if ($(".resizable1").width() > 5)
    {
        var left = 5;
        var right = viewWidth - 5;
        $(".resizable1").width(left);
        $(".resizable2").width(right);
        return;
    }
    if (bShowList)
    {       
        var left = 328;
        var right = viewWidth - 328;
        $(".resizable1").width(left);
        $(".resizable2").width(right);
    }
    else
    {
        var left = 5;
        var right = viewWidth - 5;
        $(".resizable1").width(left);
        $(".resizable2").width(right);
    }
}
