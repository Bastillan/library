import { useEffect, useState } from 'react';

import api from '../services/api';
import { useAuth } from '../services/useAuth';
import CancelReservationModal from '../modals/CancelReservationModal';
import MakeCheckoutModal from '../modals/MakeCheckoutModal';

interface ReservationDTO {
    id: number;
    userName: string;
    bookId: number;
    title: string;
    author: string;
    reservationDate: string;
    validDate: string;
}

function Reservations() {
    const [reservations, setReservations] = useState<ReservationDTO[]>([]);
    const [title, setTitle] = useState<string>("");
    const [author, setAuthor] = useState<string>("")
    const [userName, setUserName] = useState<string>("")
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const { user } = useAuth();
    const [selectedReservationId, setSelectedReservationId] = useState<number | null>(null);

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
        fetchReservations();
    }

    async function fetchReservations() {
        await api.get(`/Reservations?title=${title}&author=${author}&userName=${userName}`)
            .then(response => {
                setReservations(response.data);
            })
            .catch(() => {
                setError('Could not fetch data.');
            })
            .finally(() => {
                setLoading(false);
            })
    }

    useEffect(() => {
        fetchReservations();
    }, []);

    if (loading) return "Loading...";
    return (
        <>
            <h1>Reservations</h1>
            {error && <div className="alert alert-danger">{error}</div>}
            <form className="form-inline" onSubmit={handleSubmit}>
                <label className="me-2">Title: <input type="text" onChange={handleChangeTitle} /></label>
                <label className="me-2">Author: <input type="text" onChange={handleChangeAuthor} /></label>
                {user?.role == "Librarian" && (
                    <label className="me-2">Username: <input type="text" onChange={handleChangeUserName} /></label>
                ) }
                <input type="submit" value="Filter" />
            </form>
            <table className="table">
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Author</th>
                        <th>Reservation date</th>
                        <th>Valid date</th>
                        {user?.role == "Librarian" && (
                            <th>Username</th>
                        ) }
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {reservations.map(reservation => (
                        <tr>
                            <td>{reservation.title}</td>
                            <td>{reservation.author}</td>
                            <td>{new Date(reservation.reservationDate).toLocaleDateString()}</td>
                            <td>{new Date(reservation.validDate).toLocaleDateString()}</td>
                            {user?.role == "Librarian" && (
                                <td>{reservation.userName}</td>
                            ) }
                            <td>
                                {user?.role == "Librarian" && (
                                    <>
                                        <button className="btn btn-danger btn-sm" data-bs-toggle="modal" data-bs-target="#CancelReservationModal" onClick={() => setSelectedReservationId(reservation.id)}>Cancel reservation </button>
                                        <button className="btn btn-success btn-sm" data-bs-toggle="modal" data-bs-target="#MakeCheckoutModal" onClick={() => setSelectedReservationId(reservation.id)}>Make checkout</button>
                                    </>
                                )}
                                {user?.role == "Reader" && (
                                    <>
                                        <button className="btn btn-danger btn-sm" data-bs-toggle="modal" data-bs-target="#CancelReservationModal" onClick={() => setSelectedReservationId(reservation.id)}>Cancel reservation</button>
                                    </>
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <CancelReservationModal modalId="CancelReservationModal" reservationId={selectedReservationId} onReservationCanceled={fetchReservations} />
            <MakeCheckoutModal modalId="MakeCheckoutModal" reservationId={selectedReservationId} onCheckoutMade={fetchReservations} />
        </>
    );
}

export default Reservations;