import { AppRoutes } from "@shared/AppRoutes";

const SESSION_KEY = {
    REDIRECT_URL: 'redirectUrl'
};
export class UrlHelper {
    /**
     * The URL requested, before initial routing.
     */
    static readonly initialUrl: string = null;

    static getInitialUrl(): string {
        const sessionRedirectUrl = sessionStorage.getItem(SESSION_KEY.REDIRECT_URL);
        if (sessionRedirectUrl) {
            sessionStorage.removeItem(SESSION_KEY.REDIRECT_URL);
            return sessionRedirectUrl;
        }
        return this.initialUrl || AppRoutes.APP.HOME;
    }

    static setInitialUrl(url: string): void {
        (this as any).initialUrl = url;
    }

    static setRedirectUrl(url: string): void {
        sessionStorage.setItem(SESSION_KEY.REDIRECT_URL, url);
        this.setInitialUrl(url);
    }
    
    static getQueryParameters(): any {
        return document.location.search
            .replace(/(^\?)/, '')
            .split('&')
            .map(function (n) { return n = n.split('='), this[n[0]] = n[1], this; }.bind({}))[0];
    }
}
