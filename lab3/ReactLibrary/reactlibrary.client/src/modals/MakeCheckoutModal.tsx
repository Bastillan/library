import { useState, useEffect } from 'react';
import api from '../services/api';

interface MakeCheckoutModalProps {
    modalId: string;
    reservationId: number | null;
    onCheckoutMade: () => void;
}

const MakeCheckoutModal = ({ modalId, reservationId, onCheckoutMade }: MakeCheckoutModalProps) => {
    const [message, setMessage] = useState<string | null>(null);
    const [contentDanger, setContentDanger] = useState<string | null>(null);

    const handleMakeCheckout = async () => {
        await api.post(`/Checkouts/${reservationId}`)
            .then(() => {
                onCheckoutMade();
                setMessage('Book was successfully checked out');
                setContentDanger(null);
            })
            .catch(error => {
                if (error.response.status === 400) {
                    setContentDanger('Can not check out this book');
                    setMessage(null);
                }
                if (error.response.status === 409) {
                    setContentDanger('Book you wanted to delete was just modified by another user. Try again');
                    setMessage(null);
                }
            })
    }

    useEffect(() => {
        if (reservationId) {
            setMessage(null);
            setContentDanger(`Are you sure you want to checkout this book?`);
        } else {
            setContentDanger("No reservation id");
        }
    }, [reservationId]);

    return (
        <div className="modal fade" id={`${modalId}`} aria-hidden="true" aria-labelledby={`modalTitle${modalId}`} tabIndex={-1}>
            <div className="modal-dialog">
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className="modal-title" id={`modalTitle${modalId}`}>Checkout book</h5>
                        <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div className="modal-body">
                        {contentDanger && <div className="alert alert-danger">{contentDanger}</div>}
                        {message && <div className="alert alert-success">{message}</div>}
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="button" className="btn btn-danger" onClick={handleMakeCheckout}>Check out</button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default MakeCheckoutModal;