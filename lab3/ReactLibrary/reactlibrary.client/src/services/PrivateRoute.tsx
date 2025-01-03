import { ReactNode } from 'react';
import { useAuth } from './useAuth';
import { useNavigate } from 'react-router-dom';

interface PrivateRouteProps {
    children: ReactNode;
}

const PrivateRoute = ({ children }: PrivateRouteProps) => {
    const { user } = useAuth();
    const navigate = useNavigate();

    if (!user) {
        navigate('/account/login');
    }
    return (
        <>{children}</>
    );
}

export default PrivateRoute;