import { Navigate } from 'react-router-dom';
import { ReactNode } from 'react';
import { useAuth } from './useAuth';

interface PrivateRouteProps {
    children: ReactNode;
}

const PrivateRoute = ({ children }: PrivateRouteProps) => {
    const { user } = useAuth();

    if (!user) {
        return <Navigate to="/account/login" />;
    }
    return (
        <>{children}</>
    );
}

export default PrivateRoute;