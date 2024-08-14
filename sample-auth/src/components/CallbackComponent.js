import React, { useEffect } from 'react';

const CallbackComponent = () => {
    useEffect(() => {
        if (document.getElementById('iframeCallback')) return;
        var iframe_tag = document.createElement("iframe");
        iframe_tag.id = "iframeCallback";
        iframe_tag.style.display = "none"
        iframe_tag.setAttribute("src", `${process.env.REACT_APP_API_URL}/account/callback`);
        document.body.appendChild(iframe_tag);
    }, [])
    return (
        <div>
            Please Wait..
        </div>
    );
};

export default CallbackComponent;