/* global $ */

let checkDateDay;
let checkDateMonth;

let calendarObj = {
    1: 31,
    2: 28,
    3: 31,
    4: 30,
    5: 31,
    6: 30,
    7: 31,
    8: 31,
    9: 30,
    10: 31,
    11: 30,
    12: 31
};
let leapYear = function (yearVal) {
    return yearVal % 4 === 0 && yearVal % 100 !== 0 || yearVal % 400 === 0;
};

// these two basically do the exact same thing but one validation for start date and one for end date
// when I have time or for the next person who is unfortunate enough to update this code
// extract out validation to functions and call them on change within the parent form rather than the fieldset
let todaysDate = new Date();
let pastDate = new Date();
pastDate.setDate(pastDate.getDate() - (365 * 3) - 1);
const $datePicker = $('#datePicker');

function tryParseDate(year,month,day) {
    var ListofDays = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    if (isNaN(year) || isNaN(month) || isNaN(day)) return false;
    if (day <= 0 || month < 0 || year < 1900) return false;
    if (month > 11 || month < 0) return false;
    var lyear = false;

    if (month == 1) {
            if ((!(year % 4) && year % 100) || !(year % 400)) {
                lyear = true;
            }
            if ((lyear == false) && (day >= 29)) {
                return false;
            }
            if ((lyear == true) && (day > 29)) {
                return false;
            }
    }
    if (day > ListofDays[month] && month!=1)  {
        return false;
    }
    return new Date(year, month, day);
}

function getFromDate() {
    return tryParseDate($('#start-date-year').val(), $('#start-date-month').val()-1, $('#start-date-day').val());
}

function getToDate() {
    return tryParseDate($('#end-date-year').val(), $('#end-date-month').val() -1, $('#end-date-day').val());
}

function disableApplyFilter() {
    $('#apply-payment-filters').prop("disabled", true);
}

function enableApplyFilter() {
    $('#apply-payment-filters').prop("disabled", false);
}

function isToDateLessThenFromDate() {
    const fromDateParsed = getFromDate();
    const toDateParsed = getToDate();
    if (fromDateParsed != false && toDateParsed != false) {
        return fromDateParsed.getTime() > toDateParsed.getTime();
    }
    return false;
}

function isToDateInFuture() {
    let toDateParsed = getToDate();
    let nextDayDate = new Date();
    nextDayDate.setUTCDate(nextDayDate.getUTCDate() + 1);
    nextDayDate.setMonth(nextDayDate.getMonth() + 0);
    nextDayDate.setFullYear(nextDayDate.getFullYear() + 0); 

    if (toDateParsed != false) {
        return toDateParsed.getTime() > nextDayDate.getTime();
    }
    return false;
}

function isFromDateIsMoreThenThreeYears() {
    let fromDateParsed = getFromDate();
    let threeYearOldDate = new Date();

    threeYearOldDate.setUTCDate(threeYearOldDate.getUTCDate() + 0);
    threeYearOldDate.setMonth(threeYearOldDate.getMonth() + 0);
    threeYearOldDate.setFullYear(threeYearOldDate.getFullYear() - 3);

    if (fromDateParsed != false) {
        return fromDateParsed.getTime() < threeYearOldDate.getTime();
    }
    return false;
}



function updateErrorMessageToggle() {
    let startDateParsed = getFromDate();
    let toDateParsed = getToDate();
    let isFutureDate = isToDateInFuture();
    let isFromDateGreaterThenToDate = isToDateLessThenFromDate();
    let isFromDateIsMoreThenThreeYearOld = isFromDateIsMoreThenThreeYears();
    let displayError = false;
    if (isFromDateIsMoreThenThreeYearOld === true || isFutureDate === true || startDateParsed === false || isFromDateGreaterThenToDate === true || toDateParsed === false) {
        displayError = true;
        disableApplyFilter();
    } else {
        displayError = false;
        enableApplyFilter();
    }
    const isInvalidDate = (startDateParsed === false || toDateParsed === false);
    $datePicker.toggleClass('invalid-date-warning', isInvalidDate);
    $datePicker.toggleClass("govuk-form-group--error", displayError );
    $datePicker.toggleClass('three-year-warning', isInvalidDate ===false && isFromDateIsMoreThenThreeYearOld === true);
    $datePicker.toggleClass('future-date-warning', isInvalidDate=== false && isFutureDate === true);
    $datePicker.toggleClass('incorrect-date-order-warning', isInvalidDate === false && isFromDateGreaterThenToDate === true);
}

$('#start-date').on('change', function () {
    const $startDay = $(this).find('.govuk-date-input__day'), $startMonth = $(this).find('.govuk-date-input__month'),
        $startYear = $(this).find('.govuk-date-input__year');
    let dayVal = $startDay.val(), monthVal = $startMonth.val(), yearVal = $startYear.val(),
        isLeapYear = leapYear(yearVal), monthRange = monthVal, startDate = new Date(yearVal, monthVal - 1, dayVal);
    if (monthVal > 0 && monthVal < 13) {
        monthRange = (isLeapYear && monthVal == 2) ? 29 : calendarObj[parseInt(monthVal)];
    }

    $startDay.toggleClass('govuk-input--error', !(dayVal > 0 && dayVal <= monthRange) || startDate <= pastDate || startDate > todaysDate);
    $startMonth.toggleClass('govuk-input--error', !(monthVal > 0 && monthVal < 13) || startDate <= pastDate || startDate > todaysDate);
    $startYear.toggleClass('govuk-input--error', startDate <= pastDate || startDate > todaysDate);

    updateErrorMessageToggle();
});

$('#end-date').on('change', function () {
    const $endDay = $(this).find('.govuk-date-input__day'), $endMonth = $(this).find('.govuk-date-input__month'),
        $endYear = $(this).find('.govuk-date-input__year');
    let dayVal = $endDay.val(), monthVal = $endMonth.val(), yearVal = $endYear.val(),
        isLeapYear = leapYear(yearVal), monthRange = monthVal, endDate = new Date(yearVal, monthVal - 1, dayVal);

    if (monthVal > 0 && monthVal < 13) {
        monthRange = (isLeapYear && monthVal == 2) ? 29 : calendarObj[parseInt(monthVal)];
    }

    $endDay.toggleClass('govuk-input--error', !(dayVal > 0 && dayVal <= monthRange) || endDate > todaysDate || endDate <= pastDate);
    $endMonth.toggleClass('govuk-input--error', !(monthVal > 0 && monthVal < 13) || endDate > todaysDate || endDate <= pastDate);
    $endYear.toggleClass('govuk-input--error', endDate > todaysDate || endDate <= pastDate);
    updateErrorMessageToggle();

});
