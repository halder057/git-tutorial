$.notificationCategorySubscription = $.notificationCategorySubscription ||
    {
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

        userData: null,

        subscriptionCancellationConfirmationMessage: "You have unsaved subscription changes, which will be lost if you proceed!",

        initialSubscriptionState: null,

        init: function () {
            toastr.options.timeOut = 1000;
            toastr.options.preventDuplicates = true;
            $.material.init();
            $.notificationCategorySubscription.bindWindowUnloadEvent();
            $.notificationCategorySubscription.checkSaveStateOnInPageNavigation();
            $.notificationCategorySubscription.bindApplicationChangeEvent();
            $.notificationCategorySubscription.bindDependencyEvents();
            $.notificationCategorySubscription.bindNodeSelectedEvent();
            $.notificationCategorySubscription.bindSelectAllIndicationChange();
            $.notificationCategorySubscription.bindIndicationChange();

            $.notificationCategorySubscription.userData = userData;
            $('#userSelectionTreeView').treeview({
                data: $.notificationCategorySubscription.getUserSelectionTreeViewData($.notificationCategorySubscription.userData),
                showCheckbox: false,
                selectedBackColor: '#795548',
                highlightSelected: true,
                multiSelect: false
            });

            $('li.node-userSelectionTreeView').livequery(function () {
                if ($(this).children('span.expand-icon').length < 1) {
                    $(this).on("click", function () {
                        if ($.notificationCategorySubscription.hasUnsavedSubscriptionChanges() == false
                                || confirm($.notificationCategorySubscription.subscriptionCancellationConfirmationMessage) == true) {
                            return true;
                        }
                        else {
                            return false;
                        }
                    });
                }
            });

            var appPref = $("input[name='app-preference']").val();
            $('#applicationSelector option[value="' + appPref + '"]').prop('selected', 'selected');
            $.notificationCategorySubscription.processApplicationChange();
        },

        hasUnsavedSubscriptionChanges: function () {
            var currentSubscriptionState = $.notificationCategorySubscription.formSubscriptionData();
            $.notificationCategorySubscription.initialSubscriptionState.ApplicationID = -1;
            currentSubscriptionState.ApplicationID = -1;
            $.notificationCategorySubscription.initialSubscriptionState.UserIds = [];
            currentSubscriptionState.UserIds = [];

            if ($.notificationCategorySubscription.initialSubscriptionState != null
                && (currentSubscriptionState.CategoriesWithColumns.length > 0
                || currentSubscriptionState.DeliveryFrequency != null
                || currentSubscriptionState.DeliveryFrequency != undefined
                || currentSubscriptionState.DeliveryMethos.length > 0
                || currentSubscriptionState.IndicationSubscriptions.length > 0
                || currentSubscriptionState.NotificationCategorySubscriptions.length > 0)) {

                if (_.isEqual($.notificationCategorySubscription.initialSubscriptionState, currentSubscriptionState)) {
                    return false;
                }
                else {
                    return true;
                }
            }
            return false;
        },

        bindWindowUnloadEvent: function () {
            window.addEventListener("beforeunload", function (event) {
                if ($.notificationCategorySubscription.hasUnsavedSubscriptionChanges() == true) {
                    event.returnValue = $.notificationCategorySubscription.subscriptionCancellationConfirmationMessage;
                }
            });
        },

        checkSaveStateOnInPageNavigation: function () {
            $(document).on("click", "#applicationSelector", function (evt) {
                if ($.notificationCategorySubscription.hasUnsavedSubscriptionChanges() == false
                            || confirm($.notificationCategorySubscription.subscriptionCancellationConfirmationMessage) == true) {
                    return true;
                }
                else {
                    evt.prventDefault();
                    return false;
                }
            });
        },

        bindApplicationChangeEvent: function () {
            $(document).on("change", "#applicationSelector", function (evt) {
                $.notificationCategorySubscription.processApplicationChange();
            });
        },

        reloadContents: function () {
            if ($.notificationCategorySubscription.hasUnsavedSubscriptionChanges() == false
                            || confirm($.notificationCategorySubscription.subscriptionCancellationConfirmationMessage) == true) {
                $.notificationCategorySubscription.processApplicationChange();
            }
        },

        bindNodeSelectedEvent: function () {
            $(document).on("nodeSelected", "#userSelectionTreeView", function (event, node) {
                var isLeaf = (node.tags != null && node.tags.length > 0);

                if (isLeaf) {
                    var applicationID = parseInt($("#applicationSelector").val());
                    if (applicationID == -1) {
                        $('#userSelectionTreeView').treeview('unselectNode', [node.nodeId, { silent: true }]);
                        toastr.error('No application is selected!');
                    }
                    else {
                        var userID = parseInt(node.tags[0]);
                        $.notificationCategorySubscription.getSelectedSubscriptions(userID);
                    }
                }
            });
        },

        getUserSelectionTreeViewData: function (SubscriptionClients) {
            if (SubscriptionClients != null && SubscriptionClients.length > 0) {
                var userselectiontree = [];
                if (SubscriptionClients.length > 1) {
                    var rootnode = {};
                    rootnode.text = 'All Clients';
                    rootnode.nodes = [];
                    rootnode.selectable = false;
                    for (var i in SubscriptionClients) {
                        var client = SubscriptionClients[i];
                        var clientnode = {};
                        clientnode.text = client.ClientName;
                        clientnode.selectable = false;
                        if (client.SubscriptionUsers.length > 0) {
                            clientnode.nodes = [];
                            for (var j in client.SubscriptionUsers) {
                                var user = client.SubscriptionUsers[j];
                                var usernode = {};
                                usernode.text = user.UserName;
                                usernode.tags = [user.UserId.toString()];
                                clientnode.nodes.push(usernode);
                            }
                        }
                        rootnode.nodes.push(clientnode);
                    }
                    userselectiontree.push(rootnode);
                }
                else {
                    for (var i in SubscriptionClients) {
                        var client = SubscriptionClients[i];
                        var clientnode = {};
                        clientnode.text = client.ClientName;
                        clientnode.selectable = false;
                        if (client.SubscriptionUsers.length > 0) {
                            clientnode.nodes = [];
                            for (var j in client.SubscriptionUsers) {
                                var user = client.SubscriptionUsers[j];
                                var usernode = {};
                                usernode.text = user.UserName;
                                usernode.tags = [user.UserId.toString()];
                                clientnode.nodes.push(usernode);
                            }
                        }
                        userselectiontree.push(clientnode);
                    }
                }
                return userselectiontree;
            }
        },

        getSubscription: function (data) {

            var url = $("input[name='subscriptions']").val();

            var blockUIOption = $.notificationCategorySubscription.blockUIOption;
            blockUIOption.message = 'Getting subscriptions...';
            $.blockUI(blockUIOption);
            $.ajax({
                type: "POST",
                url: url,
                data: data,
                success: function (response) {
                    $.unblockUI();

                    $subscriptionContainer = $("#subscriptions-container");
                    $subscriptionContainer.html(response);

                    //set notification category treeview
                    var treedata = $.notificationCategorySubscription.jstreeData(categoryTreeviewData);
                    $('#categorySelectionTreeView').jstree('destory');
                    $('#categorySelectionTreeView').jstree({
                        'core': {
                            'data': treedata,
                            "themes": {
                                "name": "default",
                                "icons": false,
                                "responsive": false,
                            }
                        },
                        "checkbox": {
                            "keep_selected_style": false
                        },
                        "plugins": ["checkbox"]
                    }).bind('ready.jstree', function (e, data) {
                        $.notificationCategorySubscription.initialSubscriptionState = $.notificationCategorySubscription.formSubscriptionData();
                    });

                    //initialize libraries
                    $.material.init();
                    $('[data-toggle="tooltip"]').tooltip();

                    //Initialize delivery ui
                    $.notificationCategorySubscription.initDeliveryUI();

                    //update 'select all indication' checkbox initial state
                    $.notificationCategorySubscription.updateSelectAllCheckboxState(
                        "#select-all-indication",
                        "input[data-elementid='indication-list-checkbox']");
                },
                error: function (exception, settings, xmlhttp) {
                    $.unblockUI();

                    toastr.error($('#ajax-error-message').val());

                }
            });
        },

        getSelectedSubscriptions: function (userID) {

            var applicationID = $("#applicationSelector").val();
            var data = {
                forApplicationChange: false,
                applicationID: applicationID,
                userID: userID
            }
            $.notificationCategorySubscription.getSubscription(data);
        },

        processApplicationChange: function () {
            // on application change, all selected nodes will be unselected
            var selectedNodes = $('#userSelectionTreeView').treeview('getSelected');
            for (var i in selectedNodes) {
                $('#userSelectionTreeView').treeview('unselectNode', [selectedNodes[i].nodeId, { silent: true }]);
            }

            var applicationID = $("#applicationSelector").val();
            var data = {
                forApplicationChange: true,
                applicationID: applicationID
            }
            $.notificationCategorySubscription.getSubscription(data);
        },

        bindDependencyEvents: function () {

            //Bind Email checkbox change event
            var MethodEmailID = $('#method-email-id').val();
            var emailCheckBoxSelector = 'input[data-delivery-method-id="' + MethodEmailID + '"]';
            $(document).on("change", emailCheckBoxSelector, function (evt) {

                $emailCheckbox = $(this);
                if ($emailCheckbox.is(':checked')) {
                    $("div[email-dependent-view]").show(400);
                    $.notificationCategorySubscription.setDefaultIfRequired();
                }
                else {
                    $("div[email-dependent-view]").hide(400);
                }
            });

            //Bind Email checkbox change event
            var frequencyInstantID = $('#frequency-instant-id').val();
            var frequencyRadioGroupSelector = 'input[name="delivery-frequency"]:radio';
            var contentVolumeSingleId = $('#content-single-id').val();;
            var contentVolumeBundleId = $('#content-bundle-id').val();;
            $(document).on("change", frequencyRadioGroupSelector, function (evt) {

                var frequencyID = $(this).val();
                var isFrequencyInstantSelected = frequencyID == frequencyInstantID;
                var contentVolumeId = isFrequencyInstantSelected ? contentVolumeSingleId : contentVolumeBundleId;

                $("div[periodic-dependent-view] input[type=radio]").prop('disabled', isFrequencyInstantSelected);
                $("input[type=radio][name='content-volume'][value='" + contentVolumeId + "']").prop('checked', true);

            });
        },

        setDefaultIfRequired: function () {

            var selectedFreq = $('input:radio[name="delivery-frequency"]:checked').val();

            if (selectedFreq === undefined || selectedFreq === null) {
                //Set default frequency
                $('input[name="delivery-frequency"]:radio:first').prop('checked', true).trigger('change');

                //Set default content volume
                $('input[name="content-volume"]:radio:first').prop('checked', true);
            }
        },

        initDeliveryUI: function () {

            //Disable Push Method
            var MethodPushID = $('#method-push-id').val();
            $('input[data-delivery-method-id="' + MethodPushID + '"]').attr("disabled", true);

            //Disable Text Method
            var MethodTextID = $('#method-text-id').val();
            $('input[data-delivery-method-id="' + MethodTextID + '"]').attr("disabled", true);

            //Trigger Email Change event to show/hide dependent view
            var MethodEmailID = $('#method-email-id').val();
            var emailCheckBoxSelector = 'input[data-delivery-method-id="' + MethodEmailID + '"]';
            $(emailCheckBoxSelector).trigger("change");

            //disable/enable content section depending on frequency
            var selectedFreq = $('input:radio[name="delivery-frequency"]:checked').val();
            var frequencyInstantID = $('#frequency-instant-id').val();
            var isFrequencyInstantSelected = selectedFreq == frequencyInstantID;
            $("div[periodic-dependent-view] input[type=radio]").prop('disabled', isFrequencyInstantSelected);


            //Set default radio for content volume (if needed)
            var selectedContent = $('input:radio[name="content-volume"]:checked').val();
            var contentRadioGroupSelector = 'input[name="content-volume"]:radio';
            if (selectedContent === undefined || selectedContent === null) {
                $(contentRadioGroupSelector + ':first').prop('checked', true);
            }
        },

        cancelChanges: function () {
            window.location.reload();
        },

        unsubscribe: function () {
            //Clear all checkboxes
            $('[data-elementid="subscription-checkbox"]').prop("checked", false);
            $('[data-elementid="indication-list-checkbox"]').prop("checked", false);
            $('[data-delivery-method-checkboxes]').prop("checked", false).trigger('change');
            $('#select-all-category').prop("checked", false);
            $('#select-all-indication').prop("checked", false);

            $.notificationCategorySubscription.saveChanges(true);
            $('#subscription-confirmation-dialog').modal("hide");
        },

        validateSubscription: function () {
            var initialMistake = null;
            if ($("#applicationSelector").val() == "-1") {
                initialMistake = "No application is selected!";
            }
            else if ($('#userSelectionTreeView') != null
                 && $('#userSelectionTreeView') != undefined
                 && $('#userSelectionTreeView').length > 0
                 && $('#userSelectionTreeView').treeview('getSelected').length < 1) {
                initialMistake = "No user is selected!";
            }

            if (initialMistake != null) {
                toastr.error(initialMistake);
                return;
            }
            var mistake = null;

            if ($('[data-elementid="indication-list-checkbox"]:checked').length > 0) {
                //check if any category / column selected
                var categoryColumnList = $.notificationCategorySubscription.formSelectedColumnsData();
                var anyCategorySelected = false;
                for (var i in categoryColumnList) {
                    var item = categoryColumnList[i];
                    if (item.IsSubscribed == true) {
                        anyCategorySelected = true;
                        break;
                    }
                }

                if (anyCategorySelected == false) {
                    mistake = "notification category";
                }
            }
            else {
                mistake = (mistake != null) ? mistake + ", indication" : "indication";
            }
            if ($('[data-delivery-method-checkboxes]:checked').length < 1) {
                mistake = (mistake != null) ? mistake + ", delivery method" : "delivery method";
            }
            if (mistake != null) {
                $('#mistaken-subscription-field').text(mistake);
                $('#subscription-confirmation-dialog').modal("show");
            }
            else {
                $.notificationCategorySubscription.saveChanges();
            }
        },

        saveChanges: function (unSubscribe) {

            var jSettingsData = $.notificationCategorySubscription.formSubscriptionData();
            var jData = JSON.stringify(jSettingsData);
            var url = $("input[name='subscription-save-url']").val();

            var message = (unSubscribe == true) ? "Unsubscribing..." : 'Saving subscriptions...';
            var unsubscribedStatusMessage = "Successfully unsubscribed";
            var blockUIOption = $.notificationCategorySubscription.blockUIOption;
            blockUIOption.message = message;
            $.blockUI(blockUIOption);
            $.ajax({
                type: "POST",
                url: url,
                contentType: "application/json",
                data: jData,
                success: function (response) {
                    $.unblockUI();

                    $subscriptionContainer = $("#subscriptions-container");

                    $.notificationCategorySubscription.initialSubscriptionState = jSettingsData;

                    if (!response.error) {
                        if (unSubscribe == true)
                            toastr.success(unsubscribedStatusMessage);
                        else
                            toastr.success(response.message);
                    }
                    else {
                        toastr.error(response.message);
                    }
                },
                error: function (exception, settings, xmlhttp) {
                    $.unblockUI();
                    toastr.error($('#ajax-error-message').val());

                }
            });
        },

        formSubscriptionData: function () {

            var subscriptionSubmissionModel = {};
            var notificationCategorySubscriptions = [];
            //Collect delivery
            var $deliveryMethodsCheckboxes = $("input[data-delivery-method-checkboxes]");
            var deliveryMethods = [];
            for (var index = 0; index < $deliveryMethodsCheckboxes.length; index += 1) {
                var deliveryMethod = {};
                var $item = $($deliveryMethodsCheckboxes[index]);
                deliveryMethod['ID'] = parseInt($item.attr('data-delivery-method-id'));
                deliveryMethod['IsChecked'] = $item.is(":checked");
                deliveryMethods.push(deliveryMethod);
            }

            //Collect selected users
            var selectedUserIds = [];
            var selectedNodes = $('#userSelectionTreeView').treeview('getSelected');
            for (var i in selectedNodes) {
                if (selectedNodes[i].tags != null && selectedNodes[i].tags.length > 0) {
                    var currentUserNode = selectedNodes[i];
                    selectedUserIds.push(parseInt(currentUserNode.tags[0]));
                }
            }


            var $indicationCheckboxes = $("input[data-elementid='indication-list-checkbox']");
            var indicationSubscriptions = [];

            for (var index = 0; index < $indicationCheckboxes.length; index += 1) {
                var indications = {};
                indications['IndicationID'] = parseInt($($indicationCheckboxes[index]).attr('data-indication-id'));
                indications['IsSubscribed'] = $($indicationCheckboxes[index]).prop('checked');
                indications['IndicationAbbreviation'] = $($indicationCheckboxes[index]).attr('data-indication-abbr');
                indicationSubscriptions.push(indications)

            }

            //Set
            subscriptionSubmissionModel.NotificationCategorySubscriptions = notificationCategorySubscriptions;
            subscriptionSubmissionModel.DeliveryMethos = deliveryMethods;
            subscriptionSubmissionModel.DeliveryFrequency = $("input:radio[name ='delivery-frequency']:checked").val();
            subscriptionSubmissionModel.ContentVolume = $("input:radio[name ='content-volume']:checked").val();
            subscriptionSubmissionModel.ApplicationID = $("#applicationSelector").val();
            subscriptionSubmissionModel.UserIds = selectedUserIds;
            subscriptionSubmissionModel.IndicationSubscriptions = indicationSubscriptions;
            subscriptionSubmissionModel.CategoriesWithColumns = $.notificationCategorySubscription.formSelectedColumnsData();

            return subscriptionSubmissionModel;
        },

        bindSelectAllIndicationChange: function () {
            $(document).on("change", "#select-all-indication", function (evt) {
                var $AllCheckbox = $(this);
                if ($AllCheckbox.is(':checked')) {
                    $("input[data-elementid='indication-list-checkbox']").prop("checked", true);
                }
                else {
                    $("input[data-elementid='indication-list-checkbox']").prop("checked", false);
                }

                $.notificationCategorySubscription.updateCategoriesWithColumns();
            });
        },

        bindIndicationChange: function () {
            $(document).on("change", "input[data-elementid='indication-list-checkbox']", function (evt) {

                $.notificationCategorySubscription.updateSelectAllCheckboxState(
                    "#select-all-indication",
                    "input[data-elementid='indication-list-checkbox']");

                $.notificationCategorySubscription.updateCategoriesWithColumns();

            });
        },

        formSelectedColumnsData: function () {
            var tree = $('#categorySelectionTreeView').jstree(true);
            var selectedCategoryColumns = [];
            var rootNode = tree.get_node('ROOT');
            var categoryNodeIds = rootNode.children;

            console.log('formSelectedColumnsData');
            console.log(categoryNodeIds);

            for (var i in categoryNodeIds) {

                var categoryNodeId = categoryNodeIds[i];
                var categoryNode = tree.get_node(categoryNodeId);

                var columnNodeIds = categoryNode.children;

                var selectedColumns = [];

                for (var j in columnNodeIds) {

                    var columnNodeId = columnNodeIds[j];
                    var columnNode = tree.get_node(columnNodeId);

                    if (columnNode.state.selected == true) {

                        var columnIDs = columnNode.data.columnIDs;

                        for (var l in columnIDs) {
                            var column = {};
                            column.ColumnID = parseInt(columnIDs[l]);
                            column.IsSubscribed = true;
                            selectedColumns.push(column);
                        }
                    }
                }

                var CategoryWithColumns = {};
                CategoryWithColumns.NotificationCategoryID = parseInt(categoryNode.data.categoryID);
                CategoryWithColumns.Columns = selectedColumns;
                CategoryWithColumns.IsSubscribed = selectedColumns.length > 0;

                selectedCategoryColumns.push(CategoryWithColumns);
            }

            return selectedCategoryColumns;
        },

        updateCategoriesWithColumns: function () {

            //selected indications
            var $indicationCheckboxes = $("input[data-elementid='indication-list-checkbox']:checked");
            var subscribedIndications = [];
            for (var index = 0; index < $indicationCheckboxes.length; index += 1) {
                var indications = {};
                indications['IndicationID'] = parseInt($($indicationCheckboxes[index]).attr('data-indication-id'));
                indications['IsSubscribed'] = true;
                indications['IndicationAbbreviation'] = $($indicationCheckboxes[index]).attr('data-indication-abbr');
                subscribedIndications.push(indications);
            }
            //application id
            var appId = $("#applicationSelector").val();

            //current tree selections
            var selectedCategoryColumns = $.notificationCategorySubscription.formSelectedColumnsData();

            //generate parameters
            var data = {
                ApplicationId: parseInt(appId),
                UserId: $('#userSelectionTreeView').treeview('getSelected').length > 0 ? parseInt($('#userSelectionTreeView').treeview('getSelected')[0].tags[0]) : -1,
                SubscribedIndications: subscribedIndications,
                SelectedCategoryColumns: selectedCategoryColumns
            };

            var jData = JSON.stringify(data);

            var url = $("input[name='categories']").val();
            var blockUIOption = $.notificationCategorySubscription.blockUIOption;
            blockUIOption.message = 'Updating categories and columns';
            $.blockUI(blockUIOption);
            $.ajax({
                type: "POST",
                url: url,
                contentType: "application/json",
                data: jData,
                success: function (response) {
                    $.unblockUI();

                    $.notificationCategorySubscription.reloadCategoryTree(response);
                },
                error: function (exception, settings, xmlhttp) {
                    $.unblockUI();
                    toastr.error($('#ajax-error-message').val());
                }
            });
        },

        updateSelectAllCheckboxState: function (parentSelector, childSelector) {
            var childCount = $(childSelector).length;
            var checkedChildCount = $(childSelector + ':checked').length;
            var status = (childCount == checkedChildCount && childCount != 0);
            $(parentSelector).prop("checked", status);
        },

        reloadCategoryTree: function (data) {
            var newData = $.notificationCategorySubscription.jstreeData(data);
            $('#categorySelectionTreeView').jstree(true).settings.core.data = newData;
            $('#categorySelectionTreeView').jstree(true).refresh();
        },

        jstreeData: function (modeldata) {

            if (modeldata != null && modeldata.length > 0) {
                var treeData = [];
                var rootnode = {};
                rootnode.id = 'ROOT';
                rootnode.text = 'All Categories';
                rootnode.children = [];
                rootnode.data = {
                    "type": "root"
                };

                for (var i in modeldata) {

                    var category = modeldata[i];
                    var categoryNode = {};
                    categoryNode.text = category.NotificationCategoryName;
                    categoryNode.data = {
                        "type": "CATEGORY",
                        "categoryID": category.NotificationCategoryID
                    };

                    categoryNode.children = [];

                    if (category.Columns.length > 0) {
                        for (var k = 0; k < category.Columns.length ; k++) {
                            var column = category.Columns[k];
                            var colIds = [column.ColumnID];
                            for (var l = (parseInt(k) + 1) ; l < category.Columns.length ; l++) {
                                if (column.ColumnName == category.Columns[l].ColumnName) {
                                    colIds.push(category.Columns[l].ColumnID);
                                    category.Columns.splice(l, 1);
                                }
                            }

                            var columnNode = {};
                            columnNode.text = column.ColumnName;
                            columnNode.state = { selected: column.IsSubscribed };
                            columnNode.data = {
                                "type": "COLUMN",
                                "columnIDs": colIds
                            };

                            categoryNode.children.push(columnNode);
                        }
                    }
                    rootnode.children.push(categoryNode);
                }
                treeData.push(rootnode);

                return treeData;
            }

        }
    };
$(function () {

    $subscriptionPage = $('#notification-subscription-page');
    if ($subscriptionPage !== null && $subscriptionPage !== undefined && $subscriptionPage.length > 0) {
        // do processing on notification subscription management page
        $.notificationCategorySubscription.init();
    }
})
