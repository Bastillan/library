import { useState, useEffect } from 'react';
import api from '../services/api';

interface EditBookModalProps {
    modalId: string;
    bookId: number | null;
    onBookDeleted: () => void;
}

const DeleteBookModal = ({ modalId, bookId, onBookDeleted }: EditBookModalProps) => {
    const [message, setMessage] = useState<string | null>(null);
    const [contentDanger, setContentDanger] = useState<string | null>(null);

    const handleDeleteBook = async () => {
        await api.delete(`/Books/${bookId}`)
            .then(() => {
                onBookDeleted();
                setMessage('Book was successfully deleted');
                setContentDanger(null);
            })
            .catch(error => {
                if (error.response.status === 404) {
                    setContentDanger('Book you wanted to delete was not found');
                    setMessage(null);
                }
                if (error.response.status === 409) {
                    setContentDanger('Book you wanted to delete was just modified by another user. Try again');
                    setMessage(null);
                }
            })
    }

    useEffect(() => {
        if (bookId) {
            setMessage(null);
            setContentDanger("Are you sure you want to delete this book ?");
        } else {
            setContentDanger("No book id");
        }
    }, [bookId]);

    return (
        <div className="modal fade" id={`${modalId}`} aria-hidden="true" aria-labelledby={`modalTitle${modalId}`} tabIndex={-1}>
            <div className="modal-dialog">
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className="modal-title" id={`modalTitle${modalId}`}>Delete book</h5>
                        <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div className="modal-body">
                        {contentDanger && <div className="alert alert-danger">{contentDanger}</div>}
                        {message && <div className="alert alert-success">{message}</div>}
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="button" className="btn btn-danger" onClick={handleDeleteBook}>Delete</button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default DeleteBookModal;