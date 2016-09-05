$.viewLogs = $.viewLogs ||
    {
        table: null,

        blockUIOption: {
            message: 'Processing',
            css: {
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: 0.5,
                color: '#fff'
            }
        },

        initialize: function () {
            $('#eventSelectorDiv :input').prop('disabled', true);
            $.viewLogs.Events.bindEvents();
        },

        Events: {
            bindEvents: function () {
                $.viewLogs.Events.onModuleChange();
                $.viewLogs.Events.onEventChange();
                $.viewLogs.Events.onStackTraceButtonClick();
            },

            onModuleChange: function () {
                $(document).off('change').on('change', '#moduleSelector', function (evt) {
                    var selectedModuleId = $('option:selected', this).attr('value');
                    // Now search which module matches
                    var currentModule = null;
                    var len = modules.length;
                    for (var i = 0; i < len; i++) {
                        if (selectedModuleId == modules[i].moduleId) {
                            currentModule = modules[i];
                            break;
                        }
                    }
                    var eventList = currentModule.moduleEvents;
                    len = eventList.length;
                    var eventDropDown = $('#eventSelector');
                    eventDropDown.empty();
                    for (var i = 0; i < len; i++) {
                        eventDropDown.append(
                            $('<option />').val(eventList[i].eventId).text(eventList[i].eventName)
                        );
                    }
                    $('#eventSelectorDiv :input').prop('disabled', false);
                    // Render Error Table with moduleId and eventId
                    $.viewLogs.renderLogTable($('#moduleSelector').val(), $('#eventSelector').val());
                });
            },

            onEventChange: function () {
                $(document).ready(function () {

                    $(document).on('change', '#eventSelector', function (evt) {
                        console.log('Document eventselector');
                        var selectedModuleId = $('#moduleSelector').find(':selected').attr('value');
                        var selectedEventId = $('#eventSelector').find(':selected').attr('value');

                        console.log(selectedModuleId + " $$ " + selectedEventId);
                    });
                });
            },

            onStackTraceButtonClick: function () {
                //console.log('Came to  onStackTraceButtonClick');
                $(document).on("click", "button[notification-error-id]", function (evt) {

                    var $button = $(this);
                    var notificationErrorId = $button.attr('notification-error-id');
                    console.log('Came to  Buttons click Event  ' + notificationErrorId);
                    var len = errorModel.length;
                    for (var i = 0; i < len; i++) {
                        if (notificationErrorId == errorModel[i].ID) {
                            var stackTrace = errorModel[i].StackTrace;
                            break;
                        }
                    }
                    console.log(stackTrace);
                    $.viewLogs.renderStackTraceModal(stackTrace);
                });
            },
        },
        // Render Modal when StackTrace button is clicked
        renderStackTraceModal: function (stackTrace) {
            $('#stack-trace-holder').text(stackTrace);
            $('#stack-trace-modal').modal('toggle');
        },
        renderLogTable: function (moduleId, eventId) {
            var url = $("input[name='get-logged-data-url']").val();

            var blockUIOption = $.viewLogs.blockUIOption;
            blockUIOption.message = 'Getting Log Data...';
            $.blockUI(blockUIOption);

            var data = {
                moduleId: moduleId,
                eventId: eventId
            };
            $.ajax({
                type: "POST",
                url: url,
                data: data,
                success: function (response) {
                    $.unblockUI();
                    $.viewLogs.populateTableWithData(response);
                },
                error: function (exception, settings, xmlhttp) {
                    $.unblockUI();
                    toastr.error($('#ajax-error-message').val());

                }
            });
        },
        populateTableWithData: function (htmlResponse) {
            //console.log('came to populate Table With Data');
            var $logTableContainer = $("#log-table-container");
            $logTableContainer.html(htmlResponse);
            var $table = $('#error-table');
            //console.log('table  ' + $table);
            $table.DataTable({
                "iDisplayLength": 50,
                "bLengthChange": false,
                columnDefs: [{ targets: 'no-sort', orderable: false }],
                "aoColumns": [{ "bSortable": true }, { "bSortable": false }, { "bSortable": false }],
                "dom": '<"top"i>rt<"bottom"p><"clear">'
            });
        },
        Utilities: {

        },

    };

$(function () {
    console.log('Came to function');
    $changesPage = $('#view-log-page');
    if ($changesPage !== null && $changesPage !== undefined && $changesPage.length > 0) {
        $.viewLogs.initialize();
    }
});
