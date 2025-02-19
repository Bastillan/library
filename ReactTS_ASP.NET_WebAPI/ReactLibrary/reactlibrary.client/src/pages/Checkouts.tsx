import { useEffect, useState } from 'react';

import api from '../services/api';
import { useAuth } from '../services/useAuth';
import ReturnBookModal from '../modals/ReturnBookModal';

interface CheckoutDTO {
    id: number;
    userName: string;
    bookId: number;
    title: string;
    author: string;
    startTime: string;
    endTime: string;
}

function Checkouts() {
    const [checkouts, setCheckouts] = useState<CheckoutDTO[]>([]);
    const [title, setTitle] = useState<string>("");
    const [author, setAuthor] = useState<string>("")
    const [userName, setUserName] = useState<string>("")
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const { user } = useAuth();
    const [selectedCheckoutId, setSelectedCheckoutId] = useState<number | null>(null);

    const handleChangeTitle = (event: React.ChangeEvent<HTMLInputElement>) => {
        setTitle(event.currentTarget.value);
    }

    const handleChangeAuthor = (event: React.ChangeEvent<HTMLInputElement>) => {
        setAuthor(event.currentTarget.value);
    }

    const handleChangeUserName = (event: React.ChangeEvent<HTMLInputElement>) => {
        setUserName(event.currentTarget.value);
    }

    const handleSubmit = (event: React.FormEvent) => {
        event.preventDefault();
        fetchCheckouts();
    }

    async function fetchCheckouts() {
        await api.get(`/Checkouts?title=${title}&author=${author}&userName=${userName}`)
            .then(response => {
                setCheckouts(response.data);
            })
            .catch(() => {
                setError('Could not fetch data.');
            })
            .finally(() => {
                setLoading(false);
            })
    }

    useEffect(() => {
        fetchCheckouts();
    }, []);

    if (loading) return "Loading...";
    return (
        <>
            <h1>Checkouts</h1>
            {error && <div className="alert alert-danger">{error}</div>}
            <form className="form-inline" onSubmit={handleSubmit}>
                <label className="me-2">Title: <input type="text" onChange={handleChangeTitle} /></label>
                <label className="me-2">Author: <input type="text" onChange={handleChangeAuthor} /></label>
                {user?.role == "Librarian" && (
                    <label className="me-2">Username: <input type="text" onChange={handleChangeUserName} /></label>
                )}
                <input type="submit" value="Filter" />
            </form>
            <table className="table">
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Author</th>
                        <th>Start time</th>
                        <th>End time</th>
                        {user?.role == "Librarian" && (
                            <th>Username</th>
                        )}
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {checkouts.map(checkout => (
                        <tr>
                            <td>{checkout.title}</td>
                            <td>{checkout.author}</td>
                            <td>{new Date(checkout.startTime).toLocaleDateString()}</td>
                            <td>{checkout.endTime && new Date(checkout.endTime).toLocaleDateString()}</td>
                            {user?.role == "Librarian" && (
                                <td>{checkout.userName}</td>
                            )}
                            <td>
                                {user?.role == "Librarian" && !checkout.endTime && (
                                    <>
                                        <button className="btn btn-danger btn-sm" data-bs-toggle="modal" data-bs-target="#ReturnBookModal" onClick={() => setSelectedCheckoutId(checkout.id)}>Return book</button>
                                    </>
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <ReturnBookModal modalId="ReturnBookModal" checkoutId={selectedCheckoutId} onBookReturned={fetchCheckouts} />
        </>
    );
}

export default Checkouts;