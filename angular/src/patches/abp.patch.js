function isFromMezon() {
    if (window.Mezon && window.Mezon.WebView) {
        return true;
    }

    return false;
}

const originalSetCookieValue = abp.utils.setCookieValue;

abp.utils.setCookieValue = function (key, value, expireDate, path, domain, attributes) {
    var isMezonEnv = isFromMezon();

    attributes = attributes || {};

    if (isMezonEnv) {
        attributes['SameSite'] = 'None';
        attributes['Secure'] = true;
    }

    originalSetCookieValue.call(this, key, value, expireDate, path, domain, attributes);
};
