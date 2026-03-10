window.sessionMonitor = (() => {
    let dotnetRef = null;
    let onVisibility = null;
    let onFocus = null;
    let onPageShow = null;

    function notify() {
        if (dotnetRef) {
            dotnetRef.invokeMethodAsync('OnAppResume');
        }
    }

    return {
        register: function (ref) {
            dotnetRef = ref;

            onVisibility = () => {
                if (document.visibilityState === 'visible') {
                    notify();
                }
            };
            onFocus = () => notify();
            onPageShow = () => notify();

            document.addEventListener('visibilitychange', onVisibility);
            window.addEventListener('focus', onFocus);
            window.addEventListener('pageshow', onPageShow);
        },
        unregister: function () {
            if (onVisibility) document.removeEventListener('visibilitychange', onVisibility);
            if (onFocus) window.removeEventListener('focus', onFocus);
            if (onPageShow) window.removeEventListener('pageshow', onPageShow);
            onVisibility = null;
            onFocus = null;
            onPageShow = null;
            dotnetRef = null;
        }
    };
})();
