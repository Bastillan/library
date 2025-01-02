import { useState } from 'react';
import api from '../services/api';

interface AddBookModalProps {
    modalId: string;
    onBookAdded: () => void;
}
interface PostBookDTO {
    Title: string;
    Author: string;
    Genre: string;
    Publisher: string;
    PublicationDate: string;
}

const AddBookModal = ({ modalId, onBookAdded }: AddBookModalProps) => {
    const [postBookDTO, setPostBookDTO] = useState<PostBookDTO>({
        Title: "",
        Author: "",
        Genre: "",
        Publisher: "",
        PublicationDate: "",
    });
    const [error, setError] = useState<string | null>(null);
    const [errors, setErrors] = useState<Record<string, string[]>>({});
    const [message, setMessage] = useState<string | null>(null);

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = event.target;
        setPostBookDTO((prevData) => ({
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
            const response = await api.post("/Books", postBookDTO);

            if (response.status === 201) {
                onBookAdded();
                setMessage("Successfully added new book")
            }
        } catch (error: any) {
            if (error.response) {
                if (error.response.status === 400) {
                    setError("Invalid input");
                    if (error.response.data.errors) {
                        setErrors(error.response.data.errors);
                    }
                }
            } else {
                setError("An error occured. Please try again.");
            }
        }
    };

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
                                <input className="form-control" placeholder="Title" aria-required="true" type="text" name="Title" onChange={handleInputChange} />
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
                                <input className="form-control" placeholder="Author" aria-required="true" type="text" name="Author" onChange={handleInputChange} />
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
                                <input className="form-control" placeholder="Genre" aria-required="true" type="text" name="Genre" onChange={handleInputChange} />
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
                                <input className="form-control" placeholder="Publisher" aria-required="true" type="text" name="Publisher" onChange={handleInputChange} />
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
                                <input className="form-control" placeholder="PublicationDate" aria-required="true" type="date" name="PublicationDate" onChange={handleInputChange} />
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
                        <button type="button" className="btn btn-primary" onClick={handleSubmit}>Add book</button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AddBookModal;