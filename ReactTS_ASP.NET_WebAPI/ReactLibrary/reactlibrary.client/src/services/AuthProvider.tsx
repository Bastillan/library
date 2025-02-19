import { useState, useEffect, ReactNode } from 'react';
import { jwtDecode } from 'jwt-decode';

import { AuthContext, User } from './AuthContext';
import api from './api';

interface Props {
    children: ReactNode;
}

export const AuthProvider = ({ children }: Props) => {
    const [user, setUser] = useState<User | null>(null);

    useEffect(() => {
        const token = localStorage.getItem('token');
        const refreshToken = localStorage.getItem('refreshToken');

        if (token) {
            try {
                const decoded: any = jwtDecode(token);
                const isExpired = new Date(decoded.exp) > new Date();
                if (isExpired && refreshToken) {
                    refreshAccessToken();
                } else if (!isExpired) {
                    setUserFromToken(decoded);
                } else {
                    logout();
                }
            } catch {
                logout();
            }
        }
    }, []);

    const setUserFromToken = (decoded: any) => {
        setUser({
            id: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'], // User ID
            username: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'], // Username
            role: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'], // Role
            exp: decoded.exp, // Expiration time
        });
    };

    const refreshAccessToken = async () => {
        try {
            const response = await api.post('/Users/refresh-token', {
                accessToken: localStorage.getItem('token'),
                refreshToken: localStorage.getItem('refreshToken'),
            });

            const { authToken, refreshToken } = response.data;

            localStorage.setItem('token', authToken);
            localStorage.setItem('refreshToken', refreshToken);

            const decoded: any = jwtDecode(authToken);
            setUserFromToken(decoded);
        } catch (error) {
            logout();
        }
    }

    const login = (token: string, refreshToken: string) => {
        localStorage.setItem('token', token);
        localStorage.setItem('refreshToken', refreshToken)
        const decoded: any = jwtDecode(token);
        setUserFromToken(decoded);
    };

    const logout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        setUser(null);
    }

    return (
        <AuthContext.Provider value={{ user, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};