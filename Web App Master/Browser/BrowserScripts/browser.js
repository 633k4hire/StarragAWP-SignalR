var IsResizing = false;
var bShowList = true;
$(document).ready(function () {
    var left = 5;
    var right = viewWidth - 5;
    var viewWidth = jQuery(window).width();
    
    // run on page load  
    if (!bShowList) {
        
        left = 5;
        right = viewWidth - 5;
      
    } else {
        left = 328;
        right = viewWidth - 328;     
    }
    $(".resizable1").width(left);
    $("#DirTree").width(left - 19);
    $(".resizable2").width(right);
    $("#BackgoundDiv").hide();
   
    function calcFileAppHeight() {
        var footer = 50;
        var toolbar = 50;
        var newHeight = jQuery(window).height() - jQuery("#MainMenu").height() - jQuery("#footer").height() - jQuery("#toolbar").height()-4;
        $("#browserbox").height(newHeight);

        if (IsResizing == false) {
            var viewWidth = jQuery(window).width();
            var left = 5;
            var right = viewWidth - 5;
            // run on page load  
            if (!bShowList) {

                left = 5;
                right = viewWidth - 5;

            } else {
                left = 328;
                right = viewWidth - 328;
            }
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
        var viewWidth = jQuery(window).width();
        if (viewWidth < 600) {
            var left = 5;
            var right = viewWidth - 5;
            $(".resizable1").width(left);
            $(".resizable2").width(right);
        } else {
            var left = 328;
            var right = viewWidth - 328;
            $(".resizable1").width(left);
            $(".resizable2").width(right);
        }
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
function ResizeAppWidth(left) {
    if (left == null) {left= 5}
    var viewWidth = jQuery(window).width();   
    var right = viewWidth - left;
    $(".resizable1").width(left);
    $("#DirTree").width(left - 19); //19 = scrollbar width margin
    $(".resizable2").width(right);
}
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
var OtpMode = true;
function ToggleError() {
    $('#ErrorBox').toggleClass('open-error');
}
function ToggleCog() {
    $('#CogBox').toggleClass('open-cog');
    $('#cog-chevron').toggleClass('glyphicon-chevron-up');
    $('#cog-chevron').toggleClass('glyphicon-chevron-down');
}
function openModalDiv(divname) {
    try {
        $('#' + divname).dialog({
            draggable: true,
            resizable: true,
            show: 'Transfer',
            hide: 'Transfer',
            width: 320,
            autoOpen: false,
            minHeight: 10,
            minwidth: 10
        });
        $('#' + divname).dialog('open');
        $('#' + divname).parent().appendTo($("form:first"));
    } catch (err) { }
    return false;
}
function closeModalDiv(divname) {
    try {
        $('#' + divname).dialog('close');
    } catch (err) { }
    return false;
}
function ShowLoader() {
    $("#FullScreenLoader").show();
}
function HideLoader() {
    $("#FullScreenLoader").hide();
}
function ShowDiv(divname) {
    try {
        $('#' + divname).show();
    } catch (err) { }
}
function HideDiv(divname) {
    try {
        $('#' + divname).hide();
    } catch (err) { }
}
function ToggleOtpMode() {
    var checked = $("#OtpModeSwitch");
    OtpMode = checked[0].checked;
    if (OtpMode) {
        //upload
        $("#SuperButtonArg").val("Encrypt");
        $("#SuperButton").click();

    } else {
        //download
        $("#SuperButtonArg").val("Decrypt");
        $("#SuperButton").click();
    }
}
function updateKey() {
    $("#SuperButtonArg").val("updatekey");
    $("#SuperButton").click();
}
function DecryptMode() {


}

