import React, { useEffect } from 'react';
import { useAuth } from '../hooks/useAuth';
const HelloWorldComponent = () => {
    const auth = useAuth();
    useEffect(() => {
        if (!auth.isAuthenticated)
            window.location = process.env.REACT_APP_API_URL + `/challange/?returnUrl=${encodeURI(window.location)}`;
    }, [auth])
    return (
        <div>
            {auth.user ? <React.Fragment>
                <h1>User Claims </h1>

                <div>
                    {JSON.stringify(auth.user)}
                </div>
            </React.Fragment> : (
                <h1>User logout</h1>

            )}

            <div>
                <span onClick={auth.signOut}>Logout</span>
            </div>
        </div>
    );
};

export default HelloWorldComponent;