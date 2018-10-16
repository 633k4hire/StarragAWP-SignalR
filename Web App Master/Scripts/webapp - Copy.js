document.ASSET = null;
document.currentAsset = "0000";
document.currentBarcodeSequence = "";
document.lastKeypress = new Date();
var _TotalNotices = 0;
var _checkin_idx = 0;
var _checkout_idx = 0;
var _notice_idx = 0;var _transaction_idx = 0;
//document.IsAvUp = false;
var Monitor = function (e) {
    //technically a logger but we will not store all strokes...just capture superfast input and escape key

    //sequenceLimitMs should be set as low as possible to prevent capture of human keyed numbers.
    //200 allow testing without a barcode scanner, you could try a value of 50 with a scanner.
    var sequenceLimitMs = 10;
    var now = new Date();
    var elapsed = now - document.lastKeypress;
    document.lastKeypress = now;
    //capture escape
   
    lastkey = e.keyCode;
    var keysArray = getNumberArray(keys);
    //CTRL_ENTER IS HOOKED
    if (keysArray.toString() == "13,17") {
        try {
            var url = document.location.href;
            try {
                url = url.substr(url.lastIndexOf('/') + 1);
            } catch (er) {
                url = url.substr(url.lastIndexOf('/'));
            }
            if (url.startsWith("CheckIn")) {
                var hr = $("#Finalize").attr('href');
                window.location.href = hr;
            }
            if (url.startsWith("Login")) {
                $("LoginBtn").click();
            }
            if (url.startsWith("OutCart")) {
                var newpath = document.location.href;
                newpath = newpath.replace(url, "");
                newpath = newpath + "CheckOut.aspx";
                window.location.href = newpath;
            }
        } catch (err) { }
        keys = [];
    }

    setTimeout(TimedKeyUp, 1000);

    //ENTER IS HOOKED
    if (e.charCode == 13)
    {
        var barcodeHasFocus = $('#BarcodeSearchBox').is(':focus');
        if (barcodeHasFocus == true) {
            BarcodeScanned($('#BarcodeSearchBox').val());
            return false;
        } 
        var SearchHasFocus = $('#avSearchString').is(':focus');
        if (SearchHasFocus == true) {
            //Search
            var aaa = $('#AssetSearchBtn');
            __doPostBack('ctl00$MainContent$AssetSearchBtn', '');
            return false;
        } 

        //do same for search here

        var url = document.location.href;
        url = url.substr(url.lastIndexOf('/') + 1);
       
        
        if (url.startsWith("CheckOut")) {
            var hr = $("#Finalize").attr('href');
            window.location.href = hr;
        }

        if (url.startsWith("CheckIn")) {
            var hr = $("#Finalize").attr('href');
            window.location.href = hr;
        }
        if (url.startsWith("AssetView"))
        {
            
        }
        if (url.startsWith("Login"))
        {
            $("LoginBtn").click();
        }
        return false;
    }
    //EESCAPE IS HOOKED
    if (e.charCode == 27) {
        var url = document.location.href;
        url = url.substr(url.lastIndexOf('/') + 1);
        if (url.startsWith("Checkout")) {
           
            //window.location.href = "/Account/AssetView.aspx";
        }
        //hideMetroCharm('#assets');
        //hideMetroCharm('#settings-charm');
        //hideMetroCharm('#assetview-charm');
        //hideMetroCharm('#AlertCharm');
        $('#ErrorBox').removeClass('open-error');
        $('#asset-modal-new').hide();
        HideDiv('asset-modal-new');
        window.sessionStorage.setItem("IsAvUp", "false");
        document.IsAvUp = false;
        HideLoader();
       
    }
    //only 0-9 e.charCode >= 48 && e.charCode <= 57
    var charStr = String.fromCharCode(e.charCode);
    var tt = (e.charCode - 48);
    if (/[a-z0-9A-Z]/i.test(charStr)) {
        //pressed key is a number
        if (elapsed < sequenceLimitMs || document.currentBarcodeSequence === "") {
            //event is part of a barcode sequence
            var tmp = String.fromCharCode((e.charCode - 48));
            if (/[a-zA-Z]/i.test(charStr))
            {
                document.currentBarcodeSequence += charStr;
            } else {
                document.currentBarcodeSequence += charStr;

            }
            if (document.currentBarcodeSequence.length > 1) {
                clearTimeout(document.printBarcodeTimeout);
                document.printBarcodeTimeout = setTimeout("setBarcode()", sequenceLimitMs + 10);
            }
        } else {
            if (/[a-zA-Z]/i.test(charStr)) {
                document.currentBarcodeSequence = "" + charStr;
            } else {
                document.currentBarcodeSequence = "" + charStr;
            }
            clearTimeout(document.printBarcodeTimeout);
        }
    } else {
        document.currentBarcodeSequence = "";
        clearTimeout(document.printBarcodeTimeout);
    }
    var res1 = document.currentBarcodeSequence;
    var res2 = 0;
}
var setBarcode = function () {
    var res1 = document.currentBarcodeSequence;
    var barcodeInput = document.getElementById("BarcodeSearchBox");
    if (barcodeInput != null) //user not logged in yet
    {
        barcodeInput.value = document.currentBarcodeSequence;
        //change icon to searching id="barcodeIcon" class="glyphicon glyphicon-barcode"
        var NAME = document.getElementById("barcodeIcon");
        if (NAME != null) {
            NAME.className = "glyphicon glyphicon-refresh normal-right-spinner";   // Set other class name
        }
        //fire off ajax call for auto search here
        document.currentAsset = document.currentBarcodeSequence;
        BarcodeScanned(document.currentBarcodeSequence);
    } else {
        alert("Please login to use Asset Scanner");
    }

}

//Event Hookups
window.onkeypress = Monitor;
var keys = [];
var lastkey;
window.addEventListener("keydown",
    function (e) {
        keys[e.keyCode] = e.keyCode;
        lastkey = e.keyCode;
        var keysArray = getNumberArray(keys);
        
        setTimeout(TimedKeyUp, 1000);
    },
    false);
window.addEventListener('keyup',
    function (e) {
        keys[e.keyCode] = false;        
    },
    false);

//Document Ready
$(document).ready(function () {

    // define our variables
    var fullHeightMinusHeader = 0;
    // create function to calculate ideal height values
    function calcHeights() {
        try {    
            fullHeightMinusHeader = jQuery(window).height() - jQuery("#MainMenu").outerHeight();
            jQuery(".main-content").height(fullHeightMinusHeader);
        } catch (err) {
            return false
        }
        
    } 

    // run on page load    
    calcHeights();
   
    // run on window resize event
    $(window).resize(function () {
        try { calcHeights(); } catch (er) { } 
    });

    //var isAvUp = window.sessionStorage.getItem("IsAvUp");
    //if (isAvUp == "true")
    //{
    //   var temp = window.sessionStorage.getItem('Asset');
    //   var asset = $.parseJSON(temp);
       
    //    LoadAsset(asset);
    //} else
    //{
     
    //    HideAssetModal();
    //}

});
function openPage(pageName, elmnt, color) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablink");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].style.backgroundColor = "";
    }
    try {
        document.getElementById(pageName).style.display = "block";
        window.sessionStorage.setItem("currrentTab", pageName);
    } catch (er) { }
    //elmnt.style.backgroundColor = color;

}
function HideAssetModal() {
    window.sessionStorage.setItem("IsAvUp", "false");
    document.IsAvUp = false;
    //$('#asset-modal').removeClass('open-asset');
    $('#asset-modal-new').hide();
}
function ShowAssetModal() {   
    window.sessionStorage.setItem("IsAvUp", "true");
    $('#asset-modal-new').show();
    //$('#asset-modal-new').addClass('open-asset');
    var currentTab = window.sessionStorage.getItem("currrentTab");
    if (currentTab !== null)
    {
        openPage(currentTab, this, 'red');
        if (currentTab == "ReportTab")
        {
            ShowAssetFrames();
            ResizeAssetReport();
        }
    } else {
        $("#AssetTab-btn").click();
    }
    
    return false;
}
function ToggleError()
{
    $('#ErrorBox').toggleClass('open-error');
}
function TimedKeyUp() {
    keys[lastkey] = false;

}
function getNumberArray(arr) {
    var newArr = new Array();
    for (var i = 0; i < arr.length; i++) {
        if (typeof arr[i] == "number") {
            newArr[newArr.length] = arr[i];
        }
    }
    return newArr;
}
function csvToArray(text) {
    let p = '', row = [''], ret = [row], i = 0, r = 0, s = !0, l;
    for (l in text) {
        l = text[l];
        if ('"' === l) {
            if (s && l === p) row[i] += l;
            s = !s;
        } else if (',' === l && s) l = row[++i] = '';
        else if ('\n' === l && s) {
            if ('\r' === p) row[i] = row[i].slice(0, -1);
            row = ret[++r] = [l = '']; i = 0;
        } else row[i] += l;
        p = l;
    }
    return ret;
}
function mbox(msg)
{
    alert(msg);
}
function TestModeChanged()
{    
    try {
        if ($('#TestModeSwitch').prop('checked')) {
            $.ajax({
                type: 'POST',
                url: '/Account/AssetController.aspx/SetTestMode',
                data: "{ischecked:'True'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: mbox("Test Mode True"),
                error: mbox("Error Test Mode True")
            });
            return false;
        } else {
            $.ajax({
                type: 'POST',
                url: '/Account/AssetController.aspx/SetTestMode',
                data: "{ischecked:'False'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: mbox("Test Mode False"),
                error: mbox("Error Test Mode False")
            });
            return false;
        }
    } catch (err) { NotifyCustom('Changed Test Mode', 'Failure', 'alert');}
}
function BarcodeScanned(num, isHistory, date) {
    try {
        //disable scan if on iframe pages
        var url = document.location.href;
        url = url.substr(url.lastIndexOf('/') + 1);
        if (url.startsWith("Checkout") || url.startsWith("CheckIn") || url.startsWith("PdfViewer")) {
            var NAME = document.getElementById("barcodeIcon");
            if (NAME !== null) {
                NAME.className = "glyphicon glyphicon-barcode";   // Set other class name
            }
            return false;
        }

        if (isHistory == "True") {
            $.ajax({
                type: 'POST',
                url: '/Account/AssetController.aspx/GetHistory',
                data: "{'num':'" + num + "','date':'" + date + "'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: AssetSuccess,
                error: AssetFailure

            });
            JumpToTab('AssetTab');
            return false;
        } else {
            $.ajax({
                type: 'POST',
                url: '/Account/AssetController.aspx/GetAsset',
                data: "{'num':'" + num + "'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: AssetSuccess,
                error: AssetFailure

            });           
        }
    } catch (err) {
        var aa = err;
    }

    
   
};
function printdiv(printpage) {
    var headstr = "<html><head><title></title></head><body>";
    var footstr = "</body>";

    var nnewstr = $('#' + printpage).html();
    var oldstr = document.body.innerHTML;
    document.body.innerHTML = headstr + nnewstr + footstr;
    window.pri
    window.print();
    document.body.innerHTML = oldstr;
    return false;
}
function AssetSuccess(msg) {
    //alert(msg.d.AssetNumber);
    var NAME = document.getElementById("barcodeIcon");
    if (NAME !== null) {
        NAME.className = "glyphicon glyphicon-barcode";   // Set other class name
    }
    document.ASSET = msg.d;
    window.sessionStorage.setItem("Asset", JSON.stringify(msg.d));
    var tmp = $("#BarcodeCheckBox").prop('checked');
    if (tmp === true)
    {
        if (msg.d.IsOut)
        {
            AjaxAddCheckin(msg.d.AssetNumber, true);
        } else {           
            AjaxAddCheckout(msg.d.AssetNumber, true);
        }
       
    } else {
        SetAutoCheck('false');
        $("#AssetTab-btn").click();
         LoadAsset(msg.d);
    }
   
    //Go To Asset Tab
    
    return false;
};
function HideCalUploader()
{
    $("#UploadAssetCert").hide();
}
function SetAutoCheck(autocheck)
{
    $.ajax({
        type: 'POST',
        url: '/Account/AssetController.aspx/SetAutoCheck',
        data: "{'autocheck':'" + autocheck+"'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json'  
    });

}
function Shake(id)
{
    var iconspan = $("#" + id);    
    iconspan.addClass("mif-ani-flash");
    return false;
}
function Quiet(id)
{
    var iconspan = $("#" + id);    
    if (iconspan.hasClass("mif-ani-flash")) {
        iconspan.removeClass("mif-ani-flash");
        
    }
    return false;
}
function LoadAsset(asset) {
    try {
        
        try {
            BindAssetHistory();         
          
        } catch (erx1)
        {
            var exxx = erx1;
        }
        var a = $("#av_imgidx").val("0");
        var a2 = $("#av_imgs").val(asset.Images);
        var a3 = csvToArray(asset.Images);
        // a.forEach(function (entry) {  });
        //fill in charm
        document.AssetImageList = a;
        document.CurrentAssetImageIdx = 0;
        var imglink = "/Account/Images/" + a3[0][0];


        //var imgsrcctrl = $("#AssetImageHolder");
        //$("#AssetImageHolder").attr("src", imglink);
        


        $("#av_AssetName").val(asset.AssetName);
       $("#AssetViewHeaderLabel").html(asset.AssetName);
        $("#AssetViewWindowLabel").html(asset.AssetNumber);
         $("#av_AssetNumber").val(asset.AssetNumber);
         $("#av_ShipTo").val(asset.ShipTo);
        $("#av_ServiceOrder").val(asset.OrderNumber);
          $("#av_DateShipped").val(asset.DateShippedString);
         $("#av_ServiceEngineer").val(asset.ServiceEngineer);
         $("#av_PersonShipping").val(asset.PersonShipping);
         $("#av_DateRecieved").val(asset.DateRecievedString);
        $("#av_Weight").val(asset.weight);
         $("#av_Description").val(asset.Description);
         $("#CalCompany").val(asset.CalibrationCompany);
         $("#CalPeriod").val(asset.CalibrationPeriod);
        //try {
        //   $("#AssetReceivingReportFrame").attr("src", asset.ReturnReport);
        //   $("#AssetShippingReportFrame").attr("src", asset.UpsLabel);
        //   $("#AssetPackingReportFrame").attr("src", asset.PackingSlip);
        //    //$("#AssetReceivingReportFrame").attr("src", "/Account/Receiving/d4eb709d-beec-40f1-9634-07180121f2c8.pdf");
        //   // $("#AssetShippingReportFrame").attr("src", "/Account/Receiving/d4eb709d-beec-40f1-9634-07180121f2c8.pdf");
        //    //$("#AssetPackingReportFrame").attr("src", "/Account/Receiving/d4eb709d-beec-40f1-9634-07180121f2c8.pdf");
        //} catch (erx) { }


        if (asset.IsOut) {
            $("#InOutBtn").attr("onclick", "AjaxAddCheckin('" + asset.AssetNumber + "')");
            var header = $("#AssetHeader");
            if (header.hasClass("bg-sg-title")) {
                header.removeClass("bg-sg-title");
                header.addClass("bg-red");
            }
            if (header.hasClass("bg-gray")) {
                header.removeClass("bg-gray");
                header.addClass("bg-red");
            }
            if (header.hasClass("bg-red")) {
                header.removeClass("bg-red");
                header.addClass("bg-red");
            }
        }
        else {
            $("#InOutBtn").attr("onclick", "AjaxAddCheckout('" + asset.AssetNumber + "')");
            var header = $("#AssetHeader");
            if (header.hasClass("bg-red")) {
                header.removeClass("bg-red");
                header.addClass("bg-sg-title");
            } 
            if (header.hasClass("bg-gray")) {
                header.removeClass("bg-gray");
                header.addClass("bg-sg-title");
            }
            if (header.hasClass("bg-sg-title")) {
                header.removeClass("bg-metro-dark");
                header.addClass("bg-sg-title");
            }
            header.addClass("bg-sg-title");
        }  
        if (asset.IsHistoryItem) {            
            var header = $("#AssetHeader");
            if (header.hasClass("bg-sg-title")) {
                header.removeClass("bg-sg-title");
                header.addClass("bg-gray");
            } 
            if (header.hasClass("bg-red")) {
                header.removeClass("bg-red");
                header.addClass("bg-gray");
            } 
            if (header.hasClass("bg-gray")) {
                header.removeClass("bg-gray");
                header.addClass("bg-gray");
            } 


        }
        if (asset.IsDamaged)
        {            
            $("#av_Damaged").prop('checked', true); 

            header.removeClass("bg-sg-title");
            header.removeClass("bg-red");
            header.removeClass("bg-gray");
            header.addClass("bg-amber");

        } else {
            $("#av_Damaged").prop('checked', false);
        }
        $("#av_Damaged").attr("onclick", "AssetIsDamaged('" + asset.AssetNumber + "')");

        if (asset.OnHold) {
            $("#av_OnHold").prop('checked', true);

            header.removeClass("bg-sg-title");
            header.removeClass("bg-red");
            header.removeClass("bg-gray");
            header.removeClass("bg-amber");
            header.addClass("bg-violet");
        } else {
            $("#av_OnHold").prop('checked', false);
        }
        $("#av_OnHold").attr("onclick", "AssetOnHold('" + asset.AssetNumber + "')");

        if (asset.IsCalibrated) {
            $("#av_CalibratedTool").prop('checked', true);
        } else {
            $("#av_CalibratedTool").prop('checked', false);
        }
        $("#av_CalibratedTool").attr("onclick", "AssetIsCalibrated('" + asset.AssetNumber + "')");
        window.sessionStorage.setItem("IsAvUp", "true");
        window.sessionStorage.setItem("AssetNumber", asset.AssetNumber);
        document.IsAvUp = true;
        ShowAssetModal();
        $("#CurrentAssetNumber").val(asset.AssetNumber);
        var aaaa = $("#CurrentAssetNumber").val();
        HideLoader(); 
        
    } catch (err) { }
}
function LoadAssetView(asset)
{
    try {
        var a = $("#CurrentAssetNumber");
        a.val(asset);       
        $("#ViewChangeBtn").click();
    } catch (ex)
    {
    }
}
function ResizeAssetReport() {
    //try {

        //ShowDiv('AssetPackingReportDiv');
    //ShowDiv('AssetShippingReportDiv');
   // ShowDiv('AssetReceivingReportDiv');
    //$("#AssetPackingReportFrame").height($("#AssetPackingReportDiv").height());
    //$("#AssetPackingReportFrame").width($("#AssetPackingReportDiv").width());
    //$("#AssetShippingReportFrame").height("300px");
    //$("#AssetShippingReportFrame").width($("#AssetShippingReportDiv").width());
   // $("#AssetReceivingReportFrame").height("100px");
    //$("#AssetReceivingReportFrame").width($("#AssetReceivingReportDiv").width()); } catch (er) { }
    

    //return false;
}
function SetAssetViewHeight() {
    //try { var newHeight = 0;
    //var myViewHeight = jQuery(".main-content").height();
    //newHeight = myViewHeight - 180;
    ////var a = $("#AssetImageDiv").height();
    ////$("#AssetImageDiv").height(newHeight);
    //$("#AssetCalibrationDiv").height(newHeight-50);
    //$("#AssetPackingReportDiv").height(newHeight-50);
    //$("#AssetShippingReportDiv").height(newHeight-50);
    //$("#AssetReceivingReportDiv").height(newHeight-50);
    //$("#AssetHistoryDiv").height(newHeight); } catch (er) { }
   
    //return false;
}
function HideAllFrames()
{
    //try {
    //    HideDiv('AssetPackingReportDiv');
    //    HideDiv('AssetShippingReportDiv');
    //    HideDiv('AssetReceivingReportDiv');
    //} catch (er) { }
}
function ShowAssetFrames() {
    try {
        ShowDiv('AssetPackingReportDiv');
        ShowDiv('AssetShippingReportDiv');
        ShowDiv('AssetReceivingReportDiv');
    } catch (er) { }
}
function BindAssetHistory() {
    $("#HistoryBinderBtn").click();   
}
function BindAssetCalibration() {
    var btn = document.getElementById("CalibrationBinderBtn");
    btn.click();
}
function JumpToTab(dest)
{
    //$("#AssetTab").css("opacity", 0);
    //$("#ImageTab").css("opacity", 0)
    //$("#ReportTab").css("opacity", 0);
    //$("#CalibrationTab").css("opacity", 0);
    //$("#HistoryTab").css("opacity", 0);
   // $("#" + dest).css("opacity", 1);
}
function AjaxAddCheckout(num, autocheck) {
    if (autocheck == null)
    {
        autocheck = false;
    }
    $.ajax({
        type: 'POST',
        url: '/Account/AssetController.aspx/AddCheckoutItem',
        data: "{'num':'" + num + "','autocheck':'" + autocheck+"'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: AddCheckOutItem,
        error: AssetFailure

    });
    event.stopPropagation();
   
};
function AjaxAddCheckin(num, autocheck) {
    if (autocheck == null) {
        autocheck = false;
    }

    $.ajax({
        type: 'POST',
        url: '/Account/AssetController.aspx/AddCheckinItem',
        data: "{'num':'" + num + "','autocheck':'" + autocheck + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: AddCheckInItem,
        error: AssetFailure

    });
    event.stopPropagation();
};
function AddCheckInItem(msg) {
    try {
        if (msg.d == null) return;    
        if (msg.d.IsOut == false) return false;
        AddNotice("checkin_items", msg.d.AssetName, _checkin_idx);
        _checkin_idx++;
        var count = _checkin_idx;
        var badge = $("#checkin_badge")       
        badge.text(count);        
        Blink("checkin_badge", 250, 5);
        CheckOutPanelUpdate();
    } catch (err) { }
}
function AddCheckOutItem(msg) {
    try {
        if (msg.d == null) return false;
        if (msg.d.IsOut == true || msg.d.OnHold == true || msg.d.IsDamaged == true) { return false };
        AddNotice("checkout_items", msg.d.AssetName, _checkout_idx);
        _checkout_idx++;
        var count = _checkout_idx;
        var badge = $("#checkout_badge")        
        badge.text(count);
        Blink("checkout_badge", 250, 5);
             CheckOutPanelUpdate();
    } catch (err) { }
    return false;
}
function ClearCheckOut() {
    $.ajax({
        type: 'POST',
        url: '/Account/AssetController.aspx/ClearCheckOut',
        data: "{'num':'0000'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: UpdateMenuCounts,
        error: AssetFailure

    });
    event.stopPropagation();
    
}function ClearCheckIn() {
    $.ajax({
        type: 'POST',
        url: '/Account/AssetController.aspx/ClearCheckIn',
        data: "{'num':'0000'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: UpdateMenuCounts,
        error: AssetFailure

    });
    event.stopPropagation();
    
}
function UpdateMenuCounts() {
    UpdateAllPanels();
    UpdateMenuItems();
}
function Blink(target, interval, count)
{
    
    var badge = $("#" + target)
    badge.fadeOut(interval)
    setTimeout(function () {
        badge.fadeIn(interval);
        if ( count >0)
        {
            setTimeout(Blink(target, interval,count-1));
        }
        
    }, interval);
    CheckOutPanelUpdate();   
}
function CheckOutPanelUpdate() {

    var btn = document.getElementById("UpdateAllCarts");
    btn.click();

}
function UpdateAllPanels() {

    var btn = document.getElementById("UpdateAllCarts");
    btn.click();

}
function CheckInPanelUpdate() {

    var btn = document.getElementById("UpdateAllCarts");
    btn.click();

}
function ChangeAssetView(view) {
    $("#avSelectedView").val(view);
    var btn = document.getElementById("avChangeView");
    btn.click();
}
function RefreshAssetView(view)
{    
    var btn = document.getElementById("AjaxRefresherBtn");
    btn.click();
}
function SetSearchType(t)
{
    $("#avSelectedSearch").val(t);
}
function ChangeAssetViewListType(type)
{
    var lv = $("#lv1");
    if (lv != null)
    {
        try {
            lv.removeClass("default");
        } catch (er) { }
        try {
            lv.removeClass("list-type-icons");
        } catch (er) { }
        try {
            lv.removeClass("list-type-tiles");
        } catch (er) { }
        try {
            lv.removeClass("list-type-listing");
        } catch (er) { }
        try {
            lv.addClass(type);
        } catch (err) { }
    }
}
function hideCheckInOut()
{
    hideMetroCharm('#assets');
}
function NextAssetImg()
{
    try {
        var imgs = $("#av_imgs").val();
        var a = csvToArray(imgs);
        var idx = $("#av_imgidx").val();
        var newidx = parseInt(idx) + 1;
        if (newidx >= a.length)
        {
             return;
        }
       
        var lnk = a[0][newidx];
        var imglink = "/Account/Images/" + lnk ;
        $("#avSlideShow").attr("src", imglink);
        //set index
        $("#av_imgidx").val(newidx);

    } catch (err) { PrevAssetImg(); }
}
function PrevAssetImg() {
    try {
        var imgs = $("#av_imgs").val();
        var a = csvToArray(imgs);
        var idx = $("#av_imgidx").val();
        var newidx = parseInt(idx) - 1;
        if (newidx < 0) {
            return;
        }
        var lnk = a[0][newidx];
        var imglink = "/Account/Images/" + lnk;
        $("#avSlideShow").attr("src", imglink);
        //set index
        $("#av_imgidx").val(newidx);

    } catch (err) { NextAssetImg(); }
   
}
function AssetFailure(msg) {
    var t = "ff";
};
function DisplayAsset(msg)
{
    var NAME = document.getElementById("barcodeIcon");
    if (NAME !== null) {
        NAME = "glyphicon glyphicon-barcode";   // Set other class name
    }

}
function NotifyClick() {
    
    
};
function NotifyCustom(cap, cont, ico) {
  
};
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
            minwidth: 10});
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
        event.stopPropagation();
    } catch (err) { }
}
function ToggleDiv(divname) {
     try {
       
     } catch (err) { }
}
function UpdateAsset(asset) {
    $.ajax({
        type: 'POST',
        url: '/Account/AssetController.aspx/UpdateAsset',
        data:  JSON.stringify(asset),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (result) {
            alert("Success Update Asset");
        },
        error: function (result) {
            alert("Error Update Asset");
        }   
    });
}
function AssetIsDamaged(num) {
    $.ajax({
        type: 'POST',
        url: '/Account/AssetController.aspx/GetAsset',
        data: "{'num':'" + num + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: IsDamagedResponse
    });
}
function AssetOnHold(num)
{
    $.ajax({
        type: 'POST',
        url: '/Account/AssetController.aspx/GetAsset',
        data: "{'num':'" + num + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: OnHoldResponse
    });
}
function AssetIsCalibrated(num) {
    $.ajax({
        type: 'POST',
        url: '/Account/AssetController.aspx/GetAsset',
        data: "{num:" + num + "}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: IsCalibratedResponse
    });
}
function OnHoldResponse(msg)
{
    try {
        var tmp = $("#av_OnHold").prop('checked');
        msg.d.OnHold = $.parseJSON(tmp);
        $.ajax({
            type: 'POST',
            url: '/Account/AssetController.aspx/AssetOnHold',
            data: "{'num':'" + msg.d.AssetNumber + "','b':'"+msg.d.OnHold+"'}",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (result) {
                
            },
            error: function (result) {
               
            }
        });
    } catch (err) {
        alert(err);}
    return false;
}
function IsDamagedResponse(msg) {
    try {
        var tmp =$("#av_OnHold").prop('checked');
        msg.d.IsDamaged = $.parseJSON(tmp); 
        $.ajax({
            type: 'POST',
            url: '/Account/AssetController.aspx/AssetIsDamaged',
            data: "{'num':'" + msg.d.AssetNumber + "','b':'" + msg.d.IsDamaged + "'}",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (result) {
             
            },
            error: function (result) {
                alert("Error Update Asset");
            }
        });
    } catch (err) { }
    return false;
}
function IsCalibratedResponse(msg) {
    try {
        var tmp =$("#av_CalibratedTool").prop('checked');
        msg.d.IsCalibrated = $.parseJSON(tmp); 
        $.ajax({
            type: 'POST',
            url: '/Account/AssetController.aspx/AssetIsCalibrated',
            data: "{'num':'" + msg.d.AssetNumber + "','b':'" + msg.d.IsCalibrated + "'}",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (result) {
                
            },
            error: function (result) {
                alert("Error Update Asset");
            }
        });
    } catch (err) { }
    return false;
}
function SetAvView()
{
    var isUp = $("#IsAvUp").html();

}
function ChangeLabelDisplay()
{
    $("#hiddenModeSwitch").click();
}

//THEME
function ChangeTheme()
{
    var a1 = $('.bg-metro')
    $.each(a1, function (index, value) {
        $(value).removeClass("bg-metro")
        $(value).addClass("bg-red")
    });
    var a = $('.fg-white')
    $.each(a, function (index, value) {
        $(value).removeClass("fg-white")
        $(value).addClass("fg-red")
    });
    var b = $('.bg-metro-dark')
    $.each(b, function (index, value) {
        $(value).css("background-color", "red")
    });
    var c = $('.bg-metro-light')
    $.each(c, function (index, value) {
        $(value).css("background-color", "red")
    });
  
}
function PrevAsset()
{
    if (window.sessionStorage.getItem("IsAvUp") == "true") {
        var temp = window.sessionStorage.getItem('Asset');
        var asset = $.parseJSON(temp);

    }
}
function NextAsset()
{   
    if (window.sessionStorage.getItem("IsAvUp") == "true") {
        var temp = window.sessionStorage.getItem('Asset');
        var asset = $.parseJSON(temp);
       
    }
}



//MAIN LOADING
function UpdateMenuItems()
{
    try {

        $.ajax({
            type: 'POST',
            url: '/Account/AssetController.aspx/GetMenuItems',
            data: "{'num':'0'}",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: GetMenuItemsSuccess,
            error: GetMenuItemsError

        });
        //$.ajax({
        //    type: 'POST',
        //    url: '/Account/AssetController.aspx/GetNotifications',
        //    data: "{'num':'0'}",
        //    contentType: 'application/json; charset=utf-8',
        //    dataType: 'json',
        //    success: GetNoticeItemsSuccess,
        //    error: GetMenuItemsError

        //});
        event.stopPropagation();
    } catch (err) { }
   
}

var PollInterval = 500;
var RunNoticeTimer = true;
var RunTransactionTimer = true;
//POLLING
function PollNotices()
{
    if (RunNoticeTimer) {
        $.ajax({
            type: 'POST',
            url: '/Account/AssetController.aspx/GetNotifications',
            data: "{'num':'0'}",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: GetNoticeItemsPollSuccess,
            error: GetNoticeItemsPollError

        });
    }
}
function PollTransactions()
{
    if (RunTransactionTimer) {
        $.ajax({
            type: 'POST',
            url: '/Account/AssetController.aspx/GetAllTransactions',
            data: "{'AppName':'AWP_STARRAG_US'}",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: GettransactionItemsPollSuccess,
            error: GetTransItemsPollError

        });
    }
}
function GetTransItemsPollError(msg)
{
    setTimeout(PollTransactions, PollInterval);
}

function GettransactionItemsPollSuccess(msg) {
    try {
        var data = msg.d;
        _transaction_idx = 0;
        var area = $("#transaction_items");

        area.html("");
        var bb = $("#transaction_badge")
        bb.text("0");
        data.forEach(function (entry) {
            AddTransaction("transaction_items", entry.Name, entry.TransactionID);
            _transaction_idx++;
            var count = _transaction_idx;
            var badge = $("#transaction_badge")
            badge.text(count);
        });
        var a = PollInterval
        setTimeout(PollTransactions, PollInterval+100);
    } catch (e) {

    }
    

}

function GetNoticeItemsSuccess(msg) {
    var data = msg.d;
    data.forEach(function (entry) {
        AddNotice("notice_items", entry.Text, _notice_idx);
        _notice_idx++;
        var count = _notice_idx;
        var badge = $("#notice_badge")
        badge.text(count);
    });

}
function GetNoticeItemsPollSuccess(msg) {
    var data = msg.d;
    
    _notice_idx = 0;
    var area = $("#notice_items");

    area.html("");
    var bb = $("#notice_badge")
    bb.text("0");
    data.forEach(function (entry) {
        AddNotice("notice_items", entry.Text, _notice_idx);
        _notice_idx++;
        var count = _notice_idx;
        var badge = $("#notice_badge")
        badge.text(count);
    });
    var a = PollInterval
    if (_TotalNotices != _notice_idx && _notice_idx!=0) {
        Blink("notice_badge", 250, 5);        
    }
    _TotalNotices = _notice_idx;
    setTimeout(PollNotices, PollInterval);
}

function GetNoticeItemsPollError(msg)
{
    RunNoticeTimer = false;
}

function ResetNoticeBadge()
{
    var badge = $("#notice_badge")
    badge.text("0");
}

function DeleteNotice(target, idx)
{
    switch (target) {
        case "transaction_items":
            $.ajax({
                type: 'POST',
                url: '/Account/AssetController.aspx/RemoveTransaction',
                data: "{'transactionID':'" + idx + "'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                error: function (result)
                {
                    alert("Error Removed Item");
                }
            });
            break;
        case "notice_items":
            $.ajax({
                type: 'POST',
                url: '/Account/AssetController.aspx/RemoveNoticeItem',
                data: "{'num':'" + idx + "'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                error: function (result) {
                    alert("Error Removed Item");
                }
            });
            break;
        case "checkin_items":
            $.ajax({
                type: 'POST',
                url: '/Account/AssetController.aspx/RemoveCheckinItem',
                data: "{'num':'" + idx + "'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                error: function (result) {
                    alert("Error Removed Item");
                }
            });
            
            break;
        case "checkout_items":
            $.ajax({
                type: 'POST',
                url: '/Account/AssetController.aspx/RemoveCheckoutItem',
                data: "{'num':'"+idx+"'}",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                error: function (result) {
                    alert("Error Removed Item");
                }
            });
            
            break;


        default:
            break;
    }
    UpdateAllPanels();
    UpdateMenuItems();
}

function GetMenuItemsSuccess(msg)
{
    var area = $("#checkin_items");

    area.html("");
    var area2 = $("#checkout_items");
    _checkin_idx = 0;
    _checkout_idx = 0;
    area2.html("");
    var bb = $("#checkin_badge")
    bb.text("0");
    var b2 = $("#checkout_badge")
    b2.text("0");

    var data = msg.d;
    data.forEach(function (entry)
    {
        if (entry.IsOut)
        {
            AddNotice("checkin_items", entry.AssetName, _checkin_idx);
            _checkin_idx++;
            var count = _checkin_idx;
            var badge = $("#checkin_badge")
            badge.text(count);
        } else
        {
            AddNotice("checkout_items", entry.AssetName, _checkout_idx);
            _checkout_idx++;
            var count = _checkout_idx;
            var badge = $("#checkout_badge")
            badge.text(count);
        }
    });
}

function GetMenuItemsError(msg) {

}

function AddNotice(target, text, idx)
{

    var template = $('#notice-template').text();
    template = template.replace('%%TEXT%%', text);
    template = template.replace('%%Target%%', target); template = template.replace('%%Index%%', idx);
    template = $(template);
    //template.prop('id', asset.AssetNumber);
   // template.data('asset-id', asset.AssetNumber);

    var area = $("#" + target);
    
    area.prepend(template);
    
}
function AddTransaction(target, text, transactionID) {

    var template = $('#transaction-template').text();
    template = template.replace('%%TEXT%%', text);
    template = template.replace('%%Target%%', target);
    template = template.replace('%%Index%%', transactionID);
    template = template.replace('%%TID%%', transactionID);
    template = $(template);
    var area = $("#" + target);
    area.prepend(template);
}

function CustomCheckoutAddressChecked()
{
    var cb = $("#MainContent_cb_CustomAddress");
    var custom = $("#CustomAddressInput");
    var customer = $("#CustomerAddressInput");
    if (cb[0].checked)
    {
        customer.hide();
        custom.show();
    } else
    {
        customer.show();
        custom.hide();
    }
}

function ToggleDropDown(input)
{
    event.stopPropagation();
    var aa = $("#"+input);
    aa.dropdown('toggle');
}

function doSomething(arg) {
        $("#ASuperBtnArg").val(arg);
    var a = $("#ASuperBtn");
    $("#ASuperBtn").click();
}
  

