$(function () {
    $.contextMenu({
        selector: '.file',
        callback: function (key, options) {
            var m =  key+",";           
            var att = this[0].innerText;
            SuperButtonAsync(this,m+att);
        },
        items: {           
            "cut": { name: "Cut", icon: "cut" },
            "copy": { name: "Copy", icon: "copy" },
            "paste": { name: "Paste", icon: "paste" },
            "delete": { name: "Delete", icon: "delete" },
            "rename": { name: "Rename", icon: "edit" },

            "sep1": "---------",
        }        
    });
    $.contextMenu({
        selector: '.inner',
        callback: function (key, options) {
            var m = key + ",";
            var att = "";
            SuperButtonAsync(this, m + att);
        },
        items: {
            "New Folder": { name: "New Folder", icon: "glyphicon glyphicon-inbox" },            
        }
    });

    $('.file').on('click', function (e) {
        console.log('clicked', this);
    })
});

function SuperButtonAsync(source, msg)
{
    $("#SuperButtonArg").val(msg);
    $("#SuperButton").click();
}