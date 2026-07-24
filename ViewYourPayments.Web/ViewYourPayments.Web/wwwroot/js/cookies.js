$(document).ready(function () {

    // One year in seconds.
    const COOKIE_MAX_AGE_SECONDS = 60 * 60 * 24 * 365;
    const PREFERENCES_SET_COOKIE_VALUE = "cookies_preferences_set=true";
    const PREFERENCES_NOT_SET_COOKIE_VALUE = "cookies_preferences_set=false";

    var cookiesPrompt = document.getElementById("cookies");

    if (cookiesPrompt) {
        document.cookie = PREFERENCES_NOT_SET_COOKIE_VALUE + ";max-age=" + COOKIE_MAX_AGE_SECONDS + ";secure;path=/";
        var preferencesSetCookie = PREFERENCES_SET_COOKIE_VALUE + ";max-age=" + COOKIE_MAX_AGE_SECONDS + ";secure;path=/";

        cookiesPrompt.classList.remove('govuk-visually-hidden');

        var yescookies = document.getElementById('cookiesallowed');
        var nocookies = document.getElementById('cookiesnotallowed');

        var acceptButton = document.getElementById("accept-button");
        acceptButton.addEventListener("click", function () {

            let cookiesPolicyValue = createCookiesPolicyValue(true, true, true);
            let policyAcceptedCookie = "cookies_policy=" + cookiesPolicyValue + ";max-age=" + COOKIE_MAX_AGE_SECONDS + ";secure;path=/";

            document.cookie = policyAcceptedCookie;
            document.cookie = preferencesSetCookie;

            // Google Anaytics opt-in
            gaOptIn();

            // MS Clarity
            msClarityOptIn();

            yescookies.classList.remove('govuk-visually-hidden');
            cookiesPrompt.classList.add("govuk-visually-hidden");

        }, false);

        var rejectButton = document.getElementById("reject-button");
        rejectButton.addEventListener("click", function () {

            let cookiesPolicyValue = createCookiesPolicyValue(false, false, false);
            let policyRejectedCookie = "cookies_policy=" + cookiesPolicyValue + ";max-age=" + COOKIE_MAX_AGE_SECONDS + ";secure;path=/";

            document.cookie = policyRejectedCookie;
            document.cookie = preferencesSetCookie;

            // Google Anaytics opt-out
            gaOptOut();

            nocookies.classList.remove('govuk-visually-hidden');
            cookiesPrompt.classList.add("govuk-visually-hidden");

        }, false);

        var hideMessageButtonAccept = document.getElementById("hide-message-button-accept");
        hideMessageButtonAccept.addEventListener("click", function () {

            yescookies.classList.add('govuk-visually-hidden');

        }, false);

        var hideMessageButtonReject = document.getElementById("hide-message-button-reject");
        hideMessageButtonReject.addEventListener("click", function () {

            nocookies.classList.add('govuk-visually-hidden');

        }, false);

        function createCookiesPolicyValue(settings, usage, campaigns) {
            return "{\"essential\":true,\"settings\":" + settings + ",\"usage\":" + usage + ",\"campaigns\":" + campaigns + "}";
        }
    }
});