$.manageChanges = $.manageChanges ||
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
            toastr.options.timeOut = 2000;
            toastr.options.preventDuplicates = true;
            $.material.init();
            $.manageChanges.Events.bindEvents();
            $.manageChanges.AssociatedUserManeger.Events.bindEvents();

            var appPref = $('input[name="app-preference"]').val();
            $('#applicationSelector option[value="' + appPref + '"]').prop('selected', 'selected');
            $.manageChanges.processApplicationChange();
        },

        processSelectedChanges: function (url, blockMessage, successMessage) {

            var $checkboxes = $('tbody tr input[data-elementid="changes-checkbox"]:checked');

            if ($checkboxes.length > 0) {

                var jData = JSON.stringify($.manageChanges.makeChangesData($checkboxes));

                var blockUIOption = $.manageChanges.blockUIOption;
                blockUIOption.message = blockMessage;
                $.blockUI(blockUIOption);

                $.ajax({
                    type: 'POST',
                    url: url,
                    contentType: 'application/json',
                    data: jData,
                    success: function (response) {
                        $.unblockUI();
                        console.log(response);
                        if (response.error) {
                            toastr.error(response.message);
                        }
                        else {
                            toastr.success(successMessage);
                            $.manageChanges.loadPendingChanges(response);
                        }

                    },
                    error: function (exception, settings, xmlhttp) {
                        $.unblockUI();
                        toastr.error($('#ajax-error-message').val());
                        console.log("error: " + JSON.stringify(exception));
                    }
                });
            } else {
                toastr.error('No changes selected');
            }

        },

        loadPendingChanges: function (htmlResponse) {
            //Add table in pending changes panel
            var $changesContainer = $("#changes-table-container");
            $changesContainer.html(htmlResponse);
            $.material.init();
            //Data table plugin
            var $table = $('#changes-table');
            $table.DataTable({
                "iDisplayLength": 50,
                "bLengthChange": false,
                columnDefs: [{ targets: 'no-sort', orderable: false }],
                "aoColumns": [{ "bSortable": false }, { "bSortable": false },
                    { "bSortable": false }, { "bSortable": false }, { "bSortable": false }],
                "dom": '<"top"i>rt<"bottom"p><"clear">'
            });
            $('#changes-table_filter').hide();
            $.manageChanges.filterChangeTableByStatus();
        },

        makeChangesData: function ($checkboxes) {
            var changeModel = {};
            changeModel.ChangeIds = [];
            var length = $checkboxes.length;
            for (var index = 0; index < length ; index += 1) {
                var cid = $($checkboxes[index]).attr('data-cid');
                changeModel.ChangeIds.push(cid);
            }

            changeModel.ExcludedUsers = [];
            var excludedList = $.manageChanges.AssociatedUserManeger.excludedUsersListForNotification;
            for (var changeId in excludedList) {
                var obj = {};
                obj.ChangeId = parseInt(changeId);
                obj.UserIds = excludedList[changeId];

                changeModel.ExcludedUsers.push(obj);
            }
            changeModel.ApplicationID = $("#applicationSelector").val();
            return changeModel;
        },

        processApplicationChange: function () {

            var applicationID = $("#applicationSelector").val();
            var url = $("input[name='get-changes-url']").val();

            var blockUIOption = $.manageChanges.blockUIOption;
            blockUIOption.message = 'Getting changes...';
            $.blockUI(blockUIOption);

            var data = {
                applicationID: applicationID
            };
            $.ajax({
                type: "POST",
                url: url,
                data: data,
                success: function (response) {
                    $.unblockUI();
                    $.manageChanges.loadPendingChanges(response);
                },
                error: function (exception, settings, xmlhttp) {
                    $.unblockUI();
                    toastr.error($('#ajax-error-message').val());

                }
            });

        },

        filterChangeTableByStatus: function () {

            var statusColumnIndex = $('#status-column-index').val();
            var showApproved = $("#show-approved").is(":checked");
            var showDiscarded = $("#show-discarded").is(":checked");
            var filterText = '';//All
            if (!showApproved & !showDiscarded) {
                filterText = '^Pending$';
            }
            else if (showApproved & !showDiscarded) {
                filterText = '^Pending$|^Approved';
            }
            else if (!showApproved & showDiscarded) {
                filterText = '^Pending$|^Discarded';
            }
            var table = $('#changes-table').DataTable();
            table.columns(statusColumnIndex).search(filterText, true, false).draw();

            // Change SelectAll CheckBox accordingly
            $.manageChanges.updateSelectAllCheckBoxState(
                "#select-all",
                "input[data-elementid='changes-checkbox']",
                '#changes-table');
        },

        updateSelectAllCheckBoxState: function (selectAllSelector, childCheckBoxSelector, dataTableSelector) {
            var dataTable = $(dataTableSelector).DataTable();
            var rows = dataTable.rows({ 'search': 'applied' }).nodes();
            var allCheckboxLength = $(childCheckBoxSelector, rows).length;
            var allCheckedCheckBoxLength = $(childCheckBoxSelector + ':checked', rows).length;
            var allCheckBoxStatus = (allCheckboxLength === allCheckedCheckBoxLength && allCheckboxLength !== 0);
            $(selectAllSelector).prop("checked", allCheckBoxStatus);
        },

        Events: {
            bindEvents: function () {
                $.manageChanges.Events.cancelButtonClick();
                $.manageChanges.Events.saveButtonClick();
                $.manageChanges.Events.discardButtonClick();
                $.manageChanges.Events.applicationChange();
                $.manageChanges.Events.selectAllChange();
                $.manageChanges.Events.categoryChange();
                $.manageChanges.Events.showDiscardCheckboxChange();
                $.manageChanges.Events.showAssociatedUsersEvent();
                $.manageChanges.Events.showApprovedCheckboxChange();
            },

            cancelButtonClick: function () {
                $(document).on("click", "#clear-button", function (evt) {
                    window.location.reload();
                });
            },

            discardButtonClick: function () {
                $(document).on("click", "#discard-button", function (evt) {
                    var url = $("input[name='discard-changes-url']").val();
                    var blockMessage = 'Discarding changes...';
                    var successMessage = $('#changes-successfully-discarded').val();
                    $.manageChanges.processSelectedChanges(url, blockMessage, successMessage);
                });
            },

            saveButtonClick: function () {
                $(document).on("click", "#save-button", function (evt) {
                    var url = $("input[name='changes-approval-url']").val();
                    var blockMessage = 'Approving changes...';
                    var successMessage = $('#changes-successfully-approved').val();
                    $.manageChanges.processSelectedChanges(url, blockMessage, successMessage);
                });
            },

            showDiscardCheckboxChange: function () {
                $(document).on("change", "#show-discarded", function (evt) {
                    $.manageChanges.filterChangeTableByStatus();
                });
            },

            showApprovedCheckboxChange: function () {
                $(document).on("change", "#show-approved", function (evt) {
                    $.manageChanges.filterChangeTableByStatus();
                });
            },

            applicationChange: function () {
                $(document).on("change", "#applicationSelector", function (evt) {
                    $.manageChanges.processApplicationChange();

                    // Change SelectAll CheckBox accordingly
                    $.manageChanges.updateSelectAllCheckBoxState(
                        "#select-all",
                        "input[data-elementid='changes-checkbox']",
                        '#changes-table');
                });
            },

            categoryChange: function () {
                var categoryColumnIndex = $('#category-column-index').val();
                $(document).on("change", "#notificationCategorySelector", function (evt) {
                    var filterText = $("#notificationCategorySelector option:selected").val();
                    filterText = filterText ? "^" + filterText + "$" : "";
                    var table = $('#changes-table').DataTable();
                    table.columns(categoryColumnIndex).search(filterText, true, false).draw();

                    // Change SelectAll CheckBox accordingly
                    $.manageChanges.updateSelectAllCheckBoxState(
                        "#select-all",
                        "input[data-elementid='changes-checkbox']",
                        '#changes-table');
                });
            },

            selectAllChange: function () {
                $(document).on("change", "#select-all", function (evt) {
                    var $AllCheckbox = $(this);
                    if ($AllCheckbox.is(':checked')) {
                        var table = $('#changes-table').DataTable();
                        var tableRows = table.rows({ 'search': 'applied' }).nodes();
                        $('input[data-elementid="changes-checkbox"]', tableRows).prop('checked', true);
                    }
                    else {
                        $('input[data-elementid="changes-checkbox"]', tableRows).prop('checked', false);
                    }
                });

                // Change SelectAll CheckBox Accordingly
                $(document).on("change", "input[data-elementid='changes-checkbox']", function (evt) {
                    $.manageChanges.updateSelectAllCheckBoxState(
                        "#select-all",
                        "input[data-elementid='changes-checkbox']",
                        '#changes-table');
                });
            },

            showAssociatedUsersEvent: function () {
                $(document).on("click", "button[data-change-id]", function (evt) {
                    var $button = $(this);
                    var categoryID = $button.attr('data-cat-id');
                    var changeID = $button.attr('data-change-id');

                    var url = $("input[name='subscribed-users-url']").val();
                    var blockUIOption = $.manageChanges.blockUIOption;
                    blockUIOption.message = 'Getting users...';
                    $.blockUI(blockUIOption);

                    var data = {
                        notificationCategoryID: categoryID,
                        changeId: Number(changeID)
                    };
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: data,
                        success: function (response) {
                            $.unblockUI();
                            $.manageChanges.AssociatedUserManeger.showAssociatedUsersModal(response, changeID);
                        },
                        error: function (exception, settings, xmlhttp) {
                            $.unblockUI();
                            toastr.error($('#ajax-error-message').val());
                        }
                    });
                });
            }
        },

        AssociatedUserManeger: {

            excludedUsersListForNotification: [],

            showAssociatedUsersModal: function (htmlData, changeID) {
                if ($.manageChanges.AssociatedUserManeger.excludedUsersListForNotification[changeID] === undefined) {
                    $.manageChanges.AssociatedUserManeger.excludedUsersListForNotification[changeID] = [];
                }
                var $modalDiv = $("#subscribed-users-list-dialog");
                $modalDiv.html(htmlData);
                var $table = $('#subscribed-users-list-dialog .table');
                $.material.init();
                var dataTable = $table.DataTable({
                    "iDisplayLength": 10,
                    "bLengthChange": false,
                    "sScrolly": "200",
                    columnDefs: [{ targets: 'no-sort', orderable: false }],
                    "aoColumns": [{ "bSortable": false } , { "bSortable": false }, { "bSortable": false }],
                    "dom": '<"top"i>rt<"bottom"p>'
                });
                var $ChangeIdInput = $("#subscribed-users-list-dialog input[name='change-id']");
                $ChangeIdInput.val(changeID);

                //uncheck previously excluded list
                var excludedUsers = $.manageChanges.AssociatedUserManeger.excludedUsersListForNotification[changeID];
                if (excludedUsers) {
                    for (var i = 0; i < excludedUsers.length; i++) {
                        $("input[data-elementid='exclude-list-checkbox'][data-userid='" + excludedUsers[i] + "']").prop("checked", false);
                    }
                }

                //set select all checkbox initial state
                $.manageChanges.updateSelectAllCheckBoxState(
                    "#select-all-subscriber",
                    "input[data-elementid='exclude-list-checkbox']",
                    "#subscribed-users-list-dialog .table");

                $modalDiv.modal('show');
            },

            updateExcludedUserList: function () {
                //Get Change ID
                var $ChangeIdInput = $("#subscribed-users-list-dialog input[name='change-id']");
                var changeID = $ChangeIdInput.val();
                //Get Unchecked Checkboxes
                var $checkboxes = $('#subscribed-users-list-dialog input[data-elementid="exclude-list-checkbox"]:not(:checked)');
                //Update Excluded user list for this change
                $.manageChanges.AssociatedUserManeger.excludedUsersListForNotification[changeID] = [];
                var length = $checkboxes.length;
                var tempArray = [];
                for (var index = 0; index < length ; index += 1) {
                    var cid = $($checkboxes[index]).attr('data-userid');
                    tempArray.push(parseInt(cid));
                }
                if (tempArray.length > 0) {
                    $.manageChanges.AssociatedUserManeger.excludedUsersListForNotification[changeID] = tempArray;
                }
                //Hide modal
                var $modalDiv = $("#subscribed-users-list-dialog");
                $modalDiv.modal('hide');
            },

            updateActionButtonText: function () {

                var changeID = $("#subscribed-users-list-dialog input[name='change-id']").val();
                var initialExcludeList = $.manageChanges.AssociatedUserManeger.excludedUsersListForNotification[changeID];
                var isDirty = false;

                //Get Unchecked Checkboxes
                var $checkboxes = $('#subscribed-users-list-dialog input[data-elementid="exclude-list-checkbox"]:not(:checked)');


                if (initialExcludeList.length == $checkboxes.length) {
                    var length = $checkboxes.length;
                    for (var index = 0; index < length ; index += 1) {
                        var cid = parseInt($($checkboxes[index]).attr('data-userid'));
                        var saveId = initialExcludeList[index];

                        if (cid != saveId) {
                            isDirty = true;
                            break;
                        }
                    }
                }
                else {
                    isDirty = true;
                }

                var btnText = isDirty ? "Update" : "Continue";
                $("#update-excluded-list-btn").html(btnText);
            },

            Events: {
                bindEvents: function () {
                    $.manageChanges.AssociatedUserManeger.Events.actionButtonClick();
                    $.manageChanges.AssociatedUserManeger.Events.selectAllCheckBoxChange();
                    $.manageChanges.AssociatedUserManeger.Events.checkboxChange();
                },

                actionButtonClick: function () {

                    $(document).on("click", "#update-excluded-list-btn", function (evt) {
                        $.manageChanges.AssociatedUserManeger.updateExcludedUserList();
                    });
                },

                selectAllCheckBoxChange: function () {
                    $(document).on("change", "#select-all-subscriber", function (evt) {
                        var dataTable = $('#subscribed-users-list-dialog .table').DataTable();
                        var tableRows = dataTable.rows({ 'search': 'applied' }).nodes();

                        var $AllCheckbox = $(this);
                        if ($AllCheckbox.is(':checked')) {
                            $("input[data-elementid='exclude-list-checkbox']", tableRows).prop("checked", true);
                        }
                        else {
                            $("input[data-elementid='exclude-list-checkbox']", tableRows).prop("checked", false);
                        }
                        $.manageChanges.AssociatedUserManeger.updateActionButtonText();
                    });
                },

                checkboxChange: function () {
                    $(document).on("change", "input[data-elementid='exclude-list-checkbox']", function (evt) {

                        $.manageChanges.AssociatedUserManeger.updateActionButtonText();

                        $.manageChanges.updateSelectAllCheckBoxState(
                            "#select-all-subscriber",
                            "input[data-elementid='exclude-list-checkbox']",
                            "#subscribed-users-list-dialog .table");
                    });
                }
            }
        }
    };

$(function () {
    $changesPage = $('#changes-management-page');
    if ($changesPage !== null && $changesPage !== undefined && $changesPage.length > 0) {
        $.manageChanges.initialize();
    }
});
