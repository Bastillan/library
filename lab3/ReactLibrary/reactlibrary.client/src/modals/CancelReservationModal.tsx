import { useState, useEffect } from 'react';
import api from '../services/api';

interface CancelReservationModalProps {
    modalId: string;
    reservationId: number | null;
    onReservationCanceled: () => void;
}

const CancelReservationModal = ({ modalId, reservationId, onReservationCanceled }: CancelReservationModalProps) => {
    const [message, setMessage] = useState<string | null>(null);
    const [contentDanger, setContentDanger] = useState<string | null>(null);

    const handleDeleteBook = async () => {
        await api.delete(`/Reservations/${reservationId}`)
            .then(() => {
                onReservationCanceled();
                setMessage('Reservation was successfully canceled');
                setContentDanger(null);
            })
            .catch(error => {
                if (error.response.status === 404) {
                    setContentDanger('Reservation you wanted to cancel was not found');
                    setMessage(null);
                }
                if (error.response.status === 409) {
                    setContentDanger('Book you wanted to cancel reservation for was just modified by another user. Try again');
                    setMessage(null);
                }
                if (error.response.status === 400) {
                    setContentDanger('Can not cancel this reservation');
                    setMessage(null);
                }
            })
    }

    useEffect(() => {
        if (reservationId) {
            setMessage(null);
            setContentDanger("Are you sure you want to cancel this reservation ?");
        } else {
            setContentDanger("No reservation id.");
        }
    }, [reservationId]);

    return (
        <div className="modal fade" id={`${modalId}`} aria-hidden="true" aria-labelledby={`modalTitle${modalId}`} tabIndex={-1}>
            <div className="modal-dialog">
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className="modal-title" id={`modalTitle${modalId}`}>Cancel reservation</h5>
                        <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div className="modal-body">
                        {contentDanger && <div className="alert alert-danger">{contentDanger}</div>}
                        {message && <div className="alert alert-success">{message}</div>}
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="button" className="btn btn-danger" onClick={handleDeleteBook}>Cancel reservation</button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default CancelReservationModal;