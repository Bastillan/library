import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { useAuth } from '../services/useAuth';
import api from '../services/api';

interface RegistrationRequest {
    UserName: string;
    FirstName: string;
    LastName: string;
    Email: string;
    Password: string;
}

function Register() {
    const [formData, setFormData] = useState<RegistrationRequest>({
        UserName: "",
        FirstName: "",
        LastName: "",
        Email: "",
        Password: "",
    });
    const [confirmPassword, setConfirmPassword] = useState<string>("");
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
    };

    const handleSubmit = async (event: React.FormEvent) => {
        event.preventDefault();
        setError(null);
        setErrors({});

        if (formData.Password !== confirmPassword) {
            setErrors((prevErrors) => ({
                ...prevErrors,
                ConfirmPassword: ["Passwords do not match."],
            }));
            return;
        }

        try {
            const response = await api.post("Users/register", formData);

            if (response.status === 201) {
                const token = response.data.authToken;
                login(token);
                navigate('/account');
            }
        } catch (error: any) {
            if (error.response) {
                if (error.response.data.errors) {
                    setErrors(error.response.data.errors);
                } else {
                    const apiErrors = error.response.data;
                    const formattedErrors: Record<string, string[]> = {};
                    Object.keys(apiErrors).forEach((key) => {
                        if (key.startsWith("Password")) {
                            formattedErrors["Password"] = [
                                ...(formattedErrors["Password"] || []),
                                ...apiErrors[key],
                            ];
                        } else {
                            formattedErrors[key] = apiErrors[key];
                        }
                    });
                    setErrors(formattedErrors);
                }
            } else {
                setError("An error occured. Please try again.");
            }
        }
    };

    return (
        <div className="col-md-4">
            <h2>Create an Account</h2>
            <hr />
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
                    <input className="form-control" placeholder="firstname" aria-required="true" type="text" name="FirstName" onChange={handleInputChange} />
                    <label className="form-label">First name</label>
                    {errors.FirstName && (
                        <div className="text-danger">
                            {errors.FirstName.map((err, index) => (
                                <p key={index}>{err}</p>
                            ))}
                        </div>
                    )}
                </div>
                <div className="form-floating mb-3">
                    <input className="form-control" placeholder="lastname" aria-required="true" type="text" name="LastName" onChange={handleInputChange} />
                    <label className="form-label">Last name</label>
                    {errors.LastName && (
                        <div className="text-danger">
                            {errors.LastName.map((err, index) => (
                                <p key={index}>{err}</p>
                            ))}
                        </div>
                    )}
                </div>
                <div className="form-floating mb-3">
                    <input className="form-control" placeholder="email" aria-required="true" type="email" name="Email" onChange={handleInputChange} />
                    <label className="form-label">Email</label>
                    {errors.Email && (
                        <div className="text-danger">
                            {errors.Email.map((err, index) => (
                                <p key={index}>{err}</p>
                            ))}
                        </div>
                    )}
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
                <div className="form-floating mb-3">
                    <input className="form-control" placeholder="confirmpassword" aria-required="true" type="password" name="ConfirmPassword" onChange={(event) => { setConfirmPassword(event?.target.value) }} />
                    <label className="form-label">Confirm Password</label>
                    {errors.ConfirmPassword && (
                        <div className="text-danger">
                            {errors.ConfirmPassword.map((err, index) => (
                                <p key={index}>{err}</p>
                            ))}
                        </div>
                    )}
                </div>
                <button type="submit" className="w-100 btn btn-lg btn-primary">Register</button>
            </form>
            {error && <div className="text-danger">{error}</div> }
        </div>
    );
}

export default Register;