import { useState, useEffect } from 'react';
import api from '../services/api';

interface returnBookModalProps {
    modalId: string;
    checkoutId: number | null;
    onBookReturned: () => void;
}

const ReturnBookModal = ({ modalId, checkoutId, onBookReturned }: returnBookModalProps) => {
    const [message, setMessage] = useState<string | null>(null);
    const [contentDanger, setContentDanger] = useState<string | null>(null);

    const handleReturnBook = async () => {
        await api.delete(`/Checkouts/${checkoutId}`)
            .then(() => {
                onBookReturned();
                setMessage('Book was successfully returned');
                setContentDanger(null);
            })
            .catch(error => {
                if (error.response.status === 409) {
                    setContentDanger('Book you wanted to return for was just modified by another user. Try again');
                    setMessage(null);
                }
                if (error.response.status === 400) {
                    setContentDanger('Can not return this book');
                    setMessage(null);
                }
            })
    }

    useEffect(() => {
        if (checkoutId) {
            setMessage(null);
            setContentDanger("Are you sure you want to return this book?");
        } else {
            setContentDanger("No checkout id.");
        }
    }, [checkoutId]);

    return (
        <div className="modal fade" id={`${modalId}`} aria-hidden="true" aria-labelledby={`modalTitle${modalId}`} tabIndex={-1}>
            <div className="modal-dialog">
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className="modal-title" id={`modalTitle${modalId}`}>Return book</h5>
                        <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div className="modal-body">
                        {contentDanger && <div className="alert alert-danger">{contentDanger}</div>}
                        {message && <div className="alert alert-success">{message}</div>}
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="button" className="btn btn-danger" onClick={handleReturnBook}>Return book</button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ReturnBookModal;