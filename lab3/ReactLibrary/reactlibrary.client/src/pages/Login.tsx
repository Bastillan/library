import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../services/useAuth';
import api from '../services/api';

interface AuthRequest {
    UserName: string;
    Password: string;
}

function Login() {
    const [formData, setFormData] = useState<AuthRequest>({
        UserName: "",
        Password: "",
    });
    const [error, setError] = useState<string | null>(null);
    const [errors, setErrors] = useState<Record<string, string[]>>({});
    const { login } = useAuth();
    const navigate = useNavigate();

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = event.target;
        setFormData((prevData) => ({
            ...prevData,
            [name]: value,
        }));

        setErrors((prevErrors) => ({
            ...prevErrors,
            [name]: [],
        }));
    }

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        setError(null);
        setErrors({});

        try {
            const response = await api.post('/Users/login', formData);

            if (response.status === 200) {
                const token = response.data.authToken;
                const refreshToken = response.data.refreshToken;
                login(token, refreshToken);
                navigate(-1);
                //window.location.href = '/account';
            }
        } catch (error: any) {
            if (error.response) {
                if (error.response.data.errors) {
                    setErrors(error.response.data.errors);
                } else {
                    setError(error.response.data);
                }
            } else {
                setError("An error occured. Please try again.");
            }
        }
    };

    return (
        <div className="col-md-4">
            <h2>Log in</h2>
            <hr />
            {error && <div className="text-danger">{error}</div> }
            <form onSubmit={handleSubmit}>
                <div className="form-floating mb-3">
                    <input className="form-control" placeholder="username" aria-required="true" type="text" name="UserName" onChange={handleInputChange} />
                    <label className="form-label">Username</label>
                    {errors.UserName && (
                        <div className="text-danger">
                            {errors.UserName.map((err, index) => (
                                <p key={index}>{err}</p>
                            )) }
                        </div>
                    ) }
                </div>
                <div className="form-floating mb-3">
                    <input className="form-control" placeholder="password" aria-required="true" type="password" name="Password" onChange={handleInputChange} />
                    <label className="form-label">Password</label>
                    {errors.Password && (
                        <div className="text-danger">
                            {errors.Password.map((err, index) => (
                                <p key={index}>{err}</p>
                            ))}
                        </div>
                    )}
                </div>
                <button type="submit" className="w-100 btn btn-lg btn-primary">Login</button>
            </form>
        </div>
    );
}

export default Login;