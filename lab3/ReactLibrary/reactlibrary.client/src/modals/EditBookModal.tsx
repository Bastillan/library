import { useState, useEffect } from 'react';
import api from '../services/api';

interface EditBookModalProps {
    modalId: string;
    bookId: number | null;
    onBookEdited: () => void;
}

interface Book {
    id: number;
    title: string;
    author: string;
    genre: string;
    publisher: string;
    publicationDate: string;
    status: string;
    rowVersion: string;
}

interface PutBookDTO {
    Id: number;
    Title: string;
    Author: string;
    Genre: string;
    Publisher: string;
    PublicationDate: string;
    RowVersion: string;
}

const EditBookModal = ({ modalId, bookId, onBookEdited }: EditBookModalProps) => {
    const [error, setError] = useState<string | null>(null);
    const [errors, setErrors] = useState<Record<string, string[]>>({});
    const [message, setMessage] = useState<string | null>(null);
    const [putBookDTO, setPutBookDTO] = useState<PutBookDTO>({
        Id: -1,
        Title: "",
        Author: "",
        Genre: "",
        Publisher: "",
        PublicationDate: "",
        RowVersion: "",
    })

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = event.target;
        setPutBookDTO((prevData) => ({
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
        setMessage(null);

        try {
            const response = await api.put(`Books/${bookId}`, putBookDTO);

            if (response.status === 204) {
                onBookEdited();
                setMessage('Successfully edited book.');
            }
        } catch (error: any) {
            if (error.response) {
                if (error.response.status === 404) {
                    setError('Book you wanted to delete was not found.');
                }
                if (error.response.status === 409) {
                    setError('Book you wanted to delete was just modified by another user. Try again.');
                }
                if (error.response.status === 400) {
                    setError("Invalid input");
                    if (error.response.data.errors) {
                        setErrors(error.response.data.errors);
                    }
                }
            } else {
                setError('An error occured. Please try again.')
            }
        }
    }

    async function fetchBook() {
        await api.get(`/Books/${bookId}`)
            .then(response => {
                const data: Book = response.data;
                setPutBookDTO({
                    Id: data.id,
                    Title: data.title,
                    Author: data.author,
                    Genre: data.genre,
                    Publisher: data.publisher,
                    PublicationDate: new Date(data.publicationDate).toLocaleDateString(),
                    RowVersion: data.rowVersion,
                });
            })
            .catch(() => {
                setError('Could not fetch data.');
            })
    }

    useEffect(() => {
        if (bookId) {
            fetchBook();
        }
    }, [bookId]);

    return (
        <div className="modal fade" id={`${modalId}`} aria-hidden="true" aria-labelledby={`modalTitle${modalId}`} tabIndex={-1}>
            <div className="modal-dialog">
                <div className="modal-content">
                    <div className="modal-header">
                        <h5 className="modal-title" id={`modalTitle${modalId}`}>Add new book</h5>
                        <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div className="modal-body">
                        {error && <div className="alert alert-danger">{error}</div>}
                        {message && <div className="alert alert-success">{message}</div>}
                        <form>
                            <div className="form-floating mb-3">
                                <input className="form-control" value={putBookDTO.Title} aria-required="true" type="text" name="Title" onChange={handleInputChange} />
                                <label className="form-label">Title</label>
                                {errors.Title && (
                                    <div className="text-danger">
                                        {errors.Title.map((err, index) => (
                                            <p key={index}>{err}</p>
                                        ))}
                                    </div>
                                )}
                            </div>
                            <div className="form-floating mb-3">
                                <input className="form-control" value={putBookDTO.Author} aria-required="true" type="text" name="Author" onChange={handleInputChange} />
                                <label className="form-label">Author</label>
                                {errors.Author && (
                                    <div className="text-danger">
                                        {errors.Author.map((err, index) => (
                                            <p key={index}>{err}</p>
                                        ))}
                                    </div>
                                )}
                            </div>
                            <div className="form-floating mb-3">
                                <input className="form-control" value={putBookDTO.Genre} aria-required="true" type="text" name="Genre" onChange={handleInputChange} />
                                <label className="form-label">Genre</label>
                                {errors.Genre && (
                                    <div className="text-danger">
                                        {errors.Genre.map((err, index) => (
                                            <p key={index}>{err}</p>
                                        ))}
                                    </div>
                                )}
                            </div>
                            <div className="form-floating mb-3">
                                <input className="form-control" value={putBookDTO.Publisher} aria-required="true" type="text" name="Publisher" onChange={handleInputChange} />
                                <label className="form-label">Publisher</label>
                                {errors.Publisher && (
                                    <div className="text-danger">
                                        {errors.Publisher.map((err, index) => (
                                            <p key={index}>{err}</p>
                                        ))}
                                    </div>
                                )}
                            </div>
                            <div className="form-floating mb-3">
                                <input className="form-control" value={putBookDTO.PublicationDate} aria-required="true" type="date" name="PublicationDate" onChange={handleInputChange} />
                                <label className="form-label">Publication Date</label>
                                {errors.PublicationDate && (
                                    <div className="text-danger">
                                        {errors.PublicationDate.map((err, index) => (
                                            <p key={index}>{err}</p>
                                        ))}
                                    </div>
                                )}
                            </div>
                        </form>
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="button" className="btn btn-primary" onClick={handleSubmit}>Edit book</button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default EditBookModal;