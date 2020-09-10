var RichFields = RichFields || {
    // Mark any edited field with this class
    DirtyCls: 'rf-dirty',

    // Old version of the dirty class -- kept here so
    // old libraries will continue to function
    DeprecatedDirtyCls: 'wss-rf-dirty'
};

/**
 * Compares the original field value with the users' updated
 * (scrubbed) value.  If a change has been made, mark the
 * user visible field (the 'markThisId' field) as dirty.
 * 
 * @param {string} markThisId
 * ID of the user-visible input for editing.  This input will
 * be marked with DirtyCls if the user edits.
 * 
 * @param {string} origId
 * ID of the hidden input that holds the original, un-edited value
 * 
 * @param {string} scrubId
 * ID of the hidden input that holds the scrubbed value.
 * Each time the user edits, the scrubbed version will be
 * recomputed.
 */
RichFields.MarkIfDirty = function (markThisId, origId, scrubId) {
    var markThis = $('#' + markThisId);
    var orig = $('#' + origId);
    var scrub = $('#' + scrubId);

    var isClean = orig.val() === scrub.val();

    if (isClean) {
        markThis.removeClass(RichFields.DirtyCls);
    } else {
        markThis.addClass(RichFields.DirtyCls);
    }
};

/**
 * Called whenever the user changes the field value.
 * Takes the nicely formatted user-visible value and puts it
 * in a sanitized format appropriate for comparing with the original
 * value and for saving back to the server.
 * @param {string} scrubType
 * How to convert the value in to the scrubbed value
 * Blank means to do no conversion.
 * @param {string} inputId
 * ID of the input where the user enters values
 * @param {any} scrubId
 * ID of the hidden input to store the scrubbed result
 */
RichFields.Scrub = function (scrubType, inputId, scrubId) {
    var input = $('#' + inputId);
    var scrub = $('#' + scrubId);

    var v = input.val();

    switch (scrubType) {
        case 'number':
            v = RichFields.ScrubNumber(v);
            break;
        case 'monthyear-month':
            v = RichFields.ScrubMonthYearMonth(inputId, scrubId);
            break;
        case 'monthyear-year':
            v = RichFields.ScrubMonthYearYear(inputId, scrubId);
            break;
        default:
            // If no scrub type is specified, return the un-altered
            // value as the scrubbed value
            break;
    }

    scrub.val(v);
};



/**
 * 
 * @param {string} parentId
 * ID of an HTML element (often a form) that conains inputs
 * to be checked.
 * @returns {boolean}
 * true if any child has been edited
 */
RichFields.AnyChildrenAreDirty = function (parentId) {
    var parent = $('#' + parentId);

    return RichFields.AnyChildrenAreDirtyObj(parent);
};

/**
 * @param {any} parent
 * Find if any element under this parent element has the
 * dirty class (or the deprecated version of the class).
 * 
 * @returns {boolean}
 * True if any children have modified values
 */
RichFields.AnyChildrenAreDirtyObj = function (parent) {
    // parent is a jQuery object
    if (parent.find('.' + RichFields.DirtyCls).length !== 0 ||
        parent.find('.' + RichFields.DeprecatedDirtyCls).length !== 0) {
        return true;
    } else {
        return false;
    }
};

/**
 * If any child inputs are changed, enable the save button
 * for the form
 * @param {string} parentId
 * Id of the parent (usually a form) that contains all the fields.
 * 
 * @param {string} saveButtonId
 * Id of the button to enable
 */
RichFields.DoEnableSaveButtonIfAnyChildrenAreDirty = function
    (parentId, saveButtonId) {
    var childInputs = $('#' + parentId + ' :input');
    var childButtons = $('#' + parentId + ' :button');
    var saveButton = $('#' + saveButtonId);

    if (RichFields.AnyChildrenAreDirty(parentId)) {
        saveButton.prop('disabled', false);
    } else {
        saveButton.prop('disabled', true);
    }
};

/**
 * Attach the "DoEnableSaveButtonIfAnyChildrenAreDirty" function to
 * keyup, change, and click events for inputs and buttons under the
 * parent. This way any change will cause re-evaluation of dirty
 * state for all the child objects.
 * 
 * @param {string} parentId
 * Id of the parent (usually a form) that contains all the fields.
 *
 * @param {string} saveButtonId
 * Id of the button to enable
 */
RichFields.EnableSaveButtonIfAnyChildrenAreDirty = function (parentId, saveButtonId) {
    var childInputs = $('#' + parentId + ' :input');
    var childButtons = $('#' + parentId + ' :button');
    var saveButton = $('#' + saveButtonId);

    var f = function () {
        RichFields.DoEnableSaveButtonIfAnyChildrenAreDirty(parentId, saveButtonId);
    };

    childInputs.keyup(function () {
        f();
    });

    childInputs.change(function () {
        f();
    });

    childButtons.click(function () {
        f();
    });
};

/* Scrubbers --------------------------------------------------------------- */

/**
 * Keep only numbers, decimal points, and dash to indicate
 * negative.
 * 
 * If user supplies a negative number using parenthesis like this
 * (12.34) change to
 * -12.34 notation
 * 
 * @param {string} v
 * The value to be scrubbed
 * 
 * @returns {string}
 * The scrubbed value
 */
RichFields.ScrubNumber = function (v) {
    var negative = false;

    // if the number contains a '(' assume the user
    // is indicating a negative number
    if (v.indexOf('(') > -1) negative = true;

    // Strip out everything but digits 0-9, decimal point,
    // and hyphen
    v = v.replace(/[^0-9.\-]/g, '');

    // If negative and a negative sign isn't already present
    // add it
    if (negative && v > 0) v = -v;

    return v;
};

/**
 * Used for the MonthYear RichField.
 * Using the month chosen in select inputId, set a new
 * value for the input at scrubId
 * @param {string} inputId
 * ID of the select where user chooses a month
 * 
 * @param {string} scrubId
 * ID of the hidden input where scrubbed MonthYear date is
 * stored.
 * 
 * @returns {string}
 * The scrubbed value
 */
RichFields.ScrubMonthYearMonth = function (inputId, scrubId) {
    var input = $('#' + inputId);
    var scrub = $('#' + scrubId);

    var value = input.val();

    var dateStr = '';

    if (input.val() && input.val() !== '' && input.val() !== ' ') {
        // If the scrub input already has a valid date in it,
        // preserve the year from it and change only the month
        // If it doesn't yet have a vaule, use instead the year
        // of today's date.
        var date = new Date();

        if (scrub.val()) {
            date = new Date(scrub.val());
        }

        // MonthYear rich field doesn't allow selection of a day
        // so always set it to first day of the month
        dateStr = input.val().toString() + '/1/' +
            date.getFullYear().toString();
    }

    return dateStr;
};

/**
 * Used for the MonthYear RichField.
 * Using the year chosen in select inputId, set a new
 * value for the input at scrubId
 * @param {string} inputId
 * ID of the input where user enters a year
 *
 * @param {string} scrubId
 * ID of the hidden input where scrubbed MonthYear date is
 * stored.
 * 
 * @returns {string}
 * The scrubbed value
 */
RichFields.ScrubMonthYearYear = function (inputId, scrubId) {
    var input = $('#' + inputId);
    var scrub = $('#' + scrubId);

    var dateStr = '';
    
    if (input.val() && input.val() !== '' && input.val() !== ' ') {
        // If the scrub input already has a valid date in it,
        // preserve the month from it and change only the year.
        // If it doesn't yet have a vaule, use instead the month
        // of today's date.
        var date = new Date();
        if (scrub.val()) {
            date = new Date(scrub.val());
        }

        // MonthYear rich field doesn't allow selection of a day
        // so always set it to first day of the month
        dateStr = (date.getMonth() + 1).toString() + '/1/' +
            input.val().toString();

    }

    return dateStr;
};