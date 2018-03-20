﻿@{
    Layout = null;
    Response.ContentType = "application/javascript";
    Response.Cache.SetExpires(DateTime.Now.AddDays(30));
}

$(function () {

    $("BODY").on('click', '.nav-detail-next', function () {
        var sequence = $(this).data("relative-to");
        $(".loader").addClass("loading");
        $.ajax({
            cache: false,
            url: '@(Url.Action("NextItemDetails"))?sequence=' + sequence + ((window.location.search == '') ? '' : '&' + window.location.search.substr(1))
        })
        .always(function () {
            $(".loader").removeClass("loading");
        })
        .done(function (data, textStatus, jqXHR) {
            $("#details-content").html(data);
            $("#details-content").loaded();
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status == 404) {
                alert('No more items yet.');
            } else {
                alert('Failed to retrieve next item.\r\nPlease retry.');
            }
        });
    });

    $("BODY").on('click', '.nav-detail-prev', function () {
        var sequence = $(this).data("relative-to");
        $(".loader").addClass("loading");
        $.ajax({
            cache: false,
            url: '@(Url.Action("PreviousItemDetails"))?sequence=' + sequence + ((window.location.search == '') ? '' : '&' + window.location.search.substr(1))
        })
        .always(function () {
            $(".loader").removeClass("loading");
        })
        .done(function (data, textStatus, jqXHR) {
            $("#details-content").html(data);
            $("#details-content").loaded();
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status == 404) {
                alert('Reached first item. No previous item.');
            } else {
                alert('Failed to retrieve previous item.\r\nPlease retry.');
            }
        });
    });

    $("BODY").on("click", ".userAgentCmd", function() {
        value = $("#HTTP_USER_AGENT").text();
        $("#useragent-dlg .value-holder").html(value);
        $('#useragent-dlg .modal-body .loader').removeClass('hidden');
        $('#useragent-dlg modal-body data').addClass('hidden');
        $('#useragent-dlg .modal-body .error').addClass('hidden');
        $('#useragent-dlg').modal();
        $.ajax({
            cache: false,
            url: '@(Url.Action("UserAgentInfo"))',
            method: 'POST',
            data: { id: value }
        })
        .always(function () {
            $('#useragent-dlg .modal-body .loader').addClass('hidden');
        })
        .done(function (data, textStatus, jqXHR) {
            $("#useragent-dlg .modal-body .data TABLE").html(toTable(data));
            $('#useragent-dlg .modal-body .data').removeClass('hidden');
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            $('#useragent-dlg .modal-body .error').removeClass('hidden');
        });
    });

    $("BODY").on("click", ".remoteAddrCmd", function() {
        value = $("#REMOTE_ADDR").text();
        latProperty = $(this).data("latitude-property");
        lngProperty = $(this).data("longitude-property");
        $("#remoteaddr-dlg .value-holder").html(value);
        $('#remoteaddr-dlg .modal-body .loader').removeClass('hidden');
        $('#remoteaddr-dlg modal-body data').addClass('hidden');
        $('#remoteaddr-dlg .modal-body .error').addClass('hidden');
        $('#remoteaddr-dlg').modal();
        $.ajax({
            cache: false,
            url: '@(Url.Action("RemoteHostInfo"))',
            method: 'POST',
            data: { id: value }
        })
        .always(function () {
            $('#remoteaddr-dlg .modal-body .loader').addClass('hidden');
        })
        .done(function (data, textStatus, jqXHR) {
            $("#remoteaddr-dlg .modal-body .data TABLE").html(toTable(data));
            $('#remoteaddr-dlg .modal-body .data').removeClass('hidden');
            // https://developers.google.com/maps/documentation/javascript/examples/marker-simple
            var gloc;
            if (latProperty == lngProperty) {
                // There is a single property of lat and long, assume it is a string in "lat,long" format (as for ipinfo.io):
                gloc = new google.maps.LatLng(data[latProperty].split(',')[0], data[latProperty].split(',')[1]);
            } else {
                // There are two different properties for lat and long:
                gloc = new google.maps.LatLng(data[latProperty], data[lngProperty]);
            }
            var gmap = new google.maps.Map(document.getElementById('google-map'), { zoom: 6, center: gloc });
            var gmark = new google.maps.Marker({ position: gloc, map: gmap, title: value, animation: 'DROP' /* placing animation: 'DROP' solves the problem of big blue box behind marker :) */ });
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            $('#remoteaddr-dlg .modal-body .error').removeClass('hidden');
        });

    });

});

function toTable(object) {
    var s = '';
    for (var property in object) {
        s += '<tr><th>' + property + '</th><td>' + object[property] + '</td></tr>';
    }
    return s;
}

