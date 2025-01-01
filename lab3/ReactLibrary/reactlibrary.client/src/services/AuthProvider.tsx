import { useState, useEffect, ReactNode } from 'react';
import { jwtDecode } from 'jwt-decode';

import { AuthContext, User } from './AuthContext';

interface Props {
    children: ReactNode;
}

export const AuthProvider = ({ children }: Props) => {
    const [user, setUser] = useState<User | null>(null);

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            try {
                const decoded: any = jwtDecode(token);
                if (decoded.exp * 1000 > Date.now()) {
                    setUser({
                        id: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'], // User ID
                        username: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'], // Username
                        role: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'], // Role
                        exp: decoded.exp, // Expiration time
                    });
                } else {
                    localStorage.removeItem('token');
                }
            } catch {
                localStorage.removeItem('token');
            }
        }
    }, []);

    const login = (token: string) => {
        localStorage.setItem('token', token);
        const decoded: User = jwtDecode(token);
        setUser(decoded);
    };

    const logout = () => {
        localStorage.removeItem('token');
        setUser(null);
    }

    return (
        <AuthContext.Provider value={{ user, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};