import axios from "../utils/axiosUtils";
import React, { useState, useEffect, useContext, createContext } from "react";

const authContext = createContext();
// Provider component that wraps your app and makes auth object ...
// ... available to any child component that calls useAuth().
export function ProvideAuth({ children }) {
    const auth = useProvideAuth();
    return <authContext.Provider value={auth}>{children}</authContext.Provider>;
}
// Hook for child components to get the auth object ...
// ... and re-render when it changes.
export const useAuth = () => {
    return useContext(authContext);
};
// Provider hook that creates auth object and handles state
function useProvideAuth() {
    const [isAuthenticated, setAuthenticated] = useState(window.localStorage.getItem('access-token') ? true : false);
    const [isReceiveData, setReceiveData] = useState(null);
    const [user, setUser] = useState(false);
    const onMessage = (event) => {
        console.log(event)
        if (event.origin !== process.env.REACT_APP_API_URL)
            return;
        const item = JSON.parse(event.data);
        switch (item.type) {
            case "token":
                window.localStorage.setItem('access-token', item.value)
                setAuthenticated(true)
                setReceiveData(true)
                break;
            case "redirect_uri":
                window.location = item.value;
                break;
            default:
                break;
        }

        console.log("Received ", event.data)
    }
    useEffect(() => {
        const token = window.localStorage.getItem('access-token');
        if (token) {
            setAuthenticated(true);
        } else {
            setAuthenticated(false);
        }
        window.addEventListener("message", onMessage);
        return () => {
            window.removeEventListener("message", onMessage);
        }
    }, []);
    useEffect(() => {
        if (isAuthenticated) {
            axios.get(`${process.env.REACT_APP_API_URL}/Account/CurrentUser`)
                .then(b => {
                    setUser(b.data);
                })
        }
    }, [isAuthenticated]);
    const signOut = () => {
        window.localStorage.removeItem('access-token')
        window.location = process.env.REACT_APP_API_URL + '/Account/Signout'
    }
    // Return the user object and auth methods
    return {
        isAuthenticated,
        signOut,
        user,
        isReceiveData
    };
}