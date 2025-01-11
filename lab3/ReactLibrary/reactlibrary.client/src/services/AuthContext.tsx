import { createContext} from 'react';

export interface User {
    id: string;
    username: string;
    role: string;
    exp: number;
}

interface AuthContextType {
    user: User | null;
    login: (token: string, refreshToken: string) => void;
    logout: () => void;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);