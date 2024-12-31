import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../services/useAuth';
import api from '../services/api';

function Login() {
    const [userName, setUserName] = useState('');
    const [password, setPassword] = useState('');
    const { login } = useAuth();
    const navigate = useNavigate();

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        try {
            const response = await api.post('/Users/login', { userName, password });
            const token = response.data.authToken;
            login(token);
            navigate(-1);
            //window.location.href = '/account';
        } catch (error) {
            console.error('Login failed', error);
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <input type="text" onChange={(event) => setUserName(event.target.value)} placeholder="Username" required />
            <input type="password" onChange={(event) => setPassword(event.target.value)} placeholder="Password" required />
            <button type="submit">Login</button>
        </form>
    );
}

export default Login;