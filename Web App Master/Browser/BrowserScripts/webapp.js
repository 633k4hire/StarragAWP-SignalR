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
function ToggleOtpMode()
{
    var checked = $("#OtpModeSwitch");
    OtpMode = checked[0].checked;
    if (OtpMode)
    {
        //upload
        $("#SuperButtonArg").val("Encrypt");
        $("#SuperButton").click();

    } else
    {
        //download
        $("#SuperButtonArg").val("Decrypt");
        $("#SuperButton").click();
    }
}
function updateKey()
{
    $("#SuperButtonArg").val("updatekey");
    $("#SuperButton").click();
}
function DecryptMode()
{


}
$(document).ready(function ()
{

   // $("#OtpModeSwitch")[0].checked = OtpMode;
});