import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

import api from '../services/api';
import { useAuth } from '../services/useAuth';

interface UserData {
    id: string;
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
}
function Account() {
    const { user, logout } = useAuth();
    const [userData, setUserData] = useState<UserData | null>(null);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleDelete = async () => {
        const id = user?.id;
        await api.delete(`/Users/${id}`)
            .then(() => {
                logout();
                navigate('/account/register')
            })
            .catch(error => {
                if (error.response) {
                    setError(error.response.data);
                } else {
                    setError("An error occured while deleting user data. Pleas try again.");
                }
            })
    }

    async function fetchUserData() {
        const id = user?.id;
        await api.get(`/Users/${id}`)
            .then(response => {
                setUserData(response.data);
            })
            .catch(error => {
                if (error.response) {
                    setError(error.response.data);
                } else {
                    setError("An error occured. Pleas try again.");
                }
            })
    }

    useEffect(() => {
        fetchUserData();
    }, []);

    return (
        <div className="col-md-6">
            <h2>Account details</h2>
            {error && <div className="text-danger">{error}</div> }
            {userData && (
                <>
                    <div className="form-floating mb-3">
                        <input className="form-control" placeholder="" disabled/>
                        <label className="form-label">Username: {userData.userName}</label>
                    </div>
                    <div className="form-floating mb-3">
                        <input className="form-control" placeholder="" disabled/>
                        <label className="form-label">First name: {userData.firstName}</label>
                    </div>
                    <div className="form-floating mb-3">
                        <input className="form-control" placeholder="" disabled/>
                        <label className="form-label">Last name: {userData.lastName}</label>
                    </div>
                    <div className="form-floating mb-3">
                        <input className="form-control" placeholder="" disabled/>
                        <label className="form-label">Email: {userData.email}</label>
                    </div>
                    {user?.role !== "Librarian" && <button className="btn btn-danger" onClick={handleDelete}>Delete account</button>}
                </>
            )}
        </div>
    );
}

export default Account;