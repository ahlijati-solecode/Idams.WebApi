import axios from "axios";
axios.interceptors.request.use(function (config) {
    // Do something before request is sent
    config.headers.common['x-api-key'] = window.localStorage.getItem('access-token');
    return config;
}, function (error) {
    // Do something with request error
    return Promise.reject(error);
});
axios.interceptors.response.use(function (config) {
    // Do something before request is sent
    return config;
}, function (error) {
    // Do something with request error
    const errorCode = error.response.status;
    if (error.response?.data?.errorCode === 1 && errorCode === 401) {
        // When Error Code 1 / 0 we need to challange to API, for get new Access token
    }
    return Promise.reject(error);
});

export default axios;