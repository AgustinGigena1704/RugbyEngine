window.storageInterop = {
    getSession: function (key) {
        return sessionStorage.getItem(key);
    },
    setSession: function (key, value) {
        sessionStorage.setItem(key, value);
    },
    removeSession: function (key) {
        sessionStorage.removeItem(key);
    },
    getLocal: function (key) {
        return localStorage.getItem(key);
    },
    setLocal: function (key, value) {
        localStorage.setItem(key, value);
    },
    removeLocal: function (key) {
        localStorage.removeItem(key);
    }
};
