

var g_Admin_ID = jQuery('#g_HiddenAdmin_ID').val();
var g_ServiceUrl = jQuery('#g_HiddenServieUrl').val();
var g_LocalUrl = jQuery('#g_HiddenLocalUrl').val();
var g_AuthorizedToken = jQuery('#g_HiddenAuthorizedToken').val();

GetAllRidesByPage();

function GetAllRidesByPage() {

    var pageIndex = 1;
    var pageSize = 10;
    var params = "?admin_ID=" + g_Admin_ID + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize;

    

    jQuery.support.cors = true;
    $.ajax({
        type: "GET",
        url: g_ServiceUrl + "api/Admin/GetAllRidesByPage" + params,
        dataType: 'json',
        cache: false,
        headers={ "Authorized": "Bearer: " + g_AuthorizedToken },
        success: function (data) {
            if (data !== null && data !== undefined && data !== "undefined" && data !== "") {

                window.location = "http://localhost:50922/" + data;
            } else {

                alertify.error("Giriş işleminiz sırasında bir hata oluştu!");
            }
        },
        error: function (xhr, txtStatus, errorThrown) {
            alertify.error("Hata Kodu:" + xhr.status + " " + txtStatus + "\n" + errorThrown);
        }
    });

}