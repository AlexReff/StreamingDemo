import React, { PropsWithChildren } from "react";

interface LayoutProps {
    //
}

export const Layout: React.FC<PropsWithChildren<LayoutProps>> = ({ children }) => {
    return (
        <div>
            {children}
        </div>
    );
};
