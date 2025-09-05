window.analyticsTracker = {
    getClientInfo: () => ({
        userAgent: navigator.userAgent,
        language: navigator.language,
        platform: navigator.platform,
        screenWidth: window.screen?.width ?? null,
        screenHeight: window.screen?.height ?? null,
        devicePixelRatio: window.devicePixelRatio ?? null,
        timezone: Intl.DateTimeFormat().resolvedOptions().timeZone,
        referrer: document.referrer || null,
        url: location.href,
        title: document.title || null,
    }),
    getTitle: () => document.title || null,
};