$.spiderDashboard = $.spiderDashboard || {


    blockUIOption: {
        message: 'Processing',
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    },


    onAnalystChange: function() {
        var analyst = $('#analystSelector').val();
        var url = $.spiderDashboard.getHiddenFieldValue('#get-drugs-filewatchids-url');
        url = url + "?analyst=" + analyst;
        $.spiderDashboard.doGetAjaxCall(url, function (data) {
            $('#drugSelector').empty();
            $("#drugSelector").append('<option value="">All</option>');
            $.each(data.Drugs, function (index, value) {
                $("#drugSelector").append('<option value="' + value + '">' + value + '</option>');
            });
            $('#fileWatchIdSelector').empty();
            $.each(data.FileWatchIds, function (index, value) {
                $("#fileWatchIdSelector").append('<option value="' + value + '">' + value + '</option>');
            });
        }, function () {

        });
    },

    onDrugChange: function() {
        var analyst = $('#analystSelector').val();
        var drug = $('#drugSelector').val();
        var url = $.spiderDashboard.getHiddenFieldValue('#get-filewatchids-url');
        url = url + "?analyst=" + analyst + "&drug=" + drug;
        $.spiderDashboard.doGetAjaxCall(url, function (data) {
            $('#fileWatchIdSelector').empty();
            $.each(data.FileWatchIds, function (index, value) {
                $("#fileWatchIdSelector").append('<option value="' + value + '">' + value + '</option>');
            });

        }, function () {

        });
    },

    onSearchButtonClick: function () {
        var analyst = $('#analystSelector').val();
        var drug = $('#drugSelector').val();
        var filewatchid = $('#fileWatchIdSelector').val();
        var lastdate = $('#dateSelector').val();
        var url = $.spiderDashboard.getHiddenFieldValue('#get-historyandsubscriptiondata-url');
        url = url + "?analyst=" + analyst + "&drug=" + drug + "&filewatchid=" + filewatchid + "&date=" + lastdate;

        var blockUIOption = $.spiderDashboard.blockUIOption;
        blockUIOption.message = 'Loading Data...';
        $.blockUI(blockUIOption);

        $.spiderDashboard.doGetAjaxCall(url, function (data) {
            $('#historyData-table tbody').empty();
            $('#subscriptionData-table tbody').empty();
            console.log(data.HistoryAndSubscriptionData);
            $.each(data.HistoryAndSubscriptionData.FileHistoryDetails, function (index, value) {
                var tableHtml = '<tr>';
                tableHtml += '<td><a target="_blank" href="' + value.Url + '">' + value.Url + '</a></td>';
                tableHtml += '<td>' + value.DiscoveryDate + '</td>';
                tableHtml += '<td><a target="_blank" href="' + value.FileLocation + '">Open PDF</a></td>';
                tableHtml = tableHtml + '</tr>';
                $('#historyData-table tbody').append(tableHtml);
            });

            $.each(data.HistoryAndSubscriptionData.FileSubscriptionDetails, function (index, value) {
                var tableHtml = '<tr>';
                tableHtml += '<td>' + value.Mco + '</td>';
                tableHtml += '<td>' + value.DocAttributeName + '</td>';
                tableHtml += '<td>' + value.DrugName + '</td>';
                tableHtml = tableHtml + '</tr>';
                $('#subscriptionData-table tbody').append(tableHtml);
            });

           $.unblockUI();
        }, function () {

        });
    },

    registerEventHandlers: function() {
        $(document).on('change', '#analystSelector', function () {
            $.spiderDashboard.onAnalystChange();
        });

        $(document).on('change', '#drugSelector', function () {
            $.spiderDashboard.onDrugChange();
        });

        $(document).on('click', '#searchButton', function () {
            $.spiderDashboard.onSearchButtonClick();
        });
    },

    doGetAjaxCall: function (url, successCallback, errorCallback) {
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json",
            success: function (result) {
                successCallback(result);
            },
            error: function (exception, settings, xmlhttp) {
                console.log("error: " + JSON.stringify(exception));
                errorCallback();
            }
        });
    },

    doPostAjaxCall: function (url, data, successCallback, errorCallback) {
        $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (result) {
                successCallback(result);
            },
            error: function (exception, settings, xmlhttp) {
                console.log("error: " + JSON.stringify(exception));
                errorCallback();
            }
        });
    },

    getHiddenFieldValue: function (selector) {
        if ($(selector) !== null && $(selector) !== undefined)
            return $(selector).val();
        return null;
    },

    init: function () {
        $.spiderDashboard.registerEventHandlers();
    }
}

$(function() {
    $.spiderDashboard.init();
})

