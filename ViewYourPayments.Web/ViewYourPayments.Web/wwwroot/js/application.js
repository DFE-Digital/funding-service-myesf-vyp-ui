/* global $ */


$(document).ready(function () {
//    $('.govuk-back-link').on('click', function (e) {
//    e.preventDefault();
//    window.history.back();
//    });

    $('#reset-date-range').on('click', function (e) {
        var budgetGroupHeaderControl = $('#transaction-summary-accordion-heading-1');
        var budgetGroupSection = $('#budget-group-summary-section');
        if (budgetGroupHeaderControl && budgetGroupHeaderControl.length > 0 && budgetGroupSection.hasClass('govuk-accordion__section--expanded')) {
            $('#transaction-summary-accordion-heading-1').click();
        }
        return true;
    });
})

$(document).ready(function () {
    var hidSection = document.getElementById('hidActiveSection');
    if (hidSection !== null) {
        var selectedSection = document.getElementById(hidSection.value)
        if (selectedSection !== null) {
            selectedSection.scrollIntoView(true);
        }       
    }
});
