import { ReactNode } from 'react';
import { useAuth } from './useAuth';
import { Navigate } from 'react-router-dom';

interface PrivateRouteProps {
    children: ReactNode;
}

const PrivateRoute = ({ children }: PrivateRouteProps) => {
    const { user } = useAuth();

    if (!user) {
        return <Navigate to='/account/login' />;
    }
    return (
        <>{children}</>
    );
}

export default PrivateRoute;