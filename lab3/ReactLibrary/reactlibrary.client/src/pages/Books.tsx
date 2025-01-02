import { useEffect, useState } from "react";

import api from "../services/api";
import { useAuth } from '../services/useAuth';
import AddBookModal from '../modals/AddBookModal';

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

function Books() {
    const [books, setBooks] = useState<Book[]>([]);
    const [genres, setGenres] = useState<string[]>([]);
    const [genre, setGenre] = useState<string>("");
    const [title, setTitle] = useState<string>("");
    const [author, setAuthor] = useState<string>("");
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);
    const { user } = useAuth();
    const [showAddBookModal, setShowAddBookModal] = useState(false);

    const handleShowAddBookModal = () => setShowAddBookModal(true);
    const handleCloseAddBookModal = () => setShowAddBookModal(false);

    const handleChangeGenre = (event: React.ChangeEvent<HTMLSelectElement>) => {
        setGenre(event.currentTarget.value);
    }

    const handleChangeTitle = (event: React.ChangeEvent<HTMLInputElement>) => {
        setTitle(event.currentTarget.value);
    }

    const handleChangeAuthor = (event: React.ChangeEvent<HTMLInputElement>) => {
        setAuthor(event.currentTarget.value);
    }

    const handleSubmit = (event: React.FormEvent) => {
        event.preventDefault();
        fetchBooks();
        fetchGenres();
    }

    async function fetchGenres() {
        await api.get('/Books/genres')
            .then(response => {
                setGenres(response.data);
            })
            .catch(error => {
                setError(error);
            })
    }

    async function fetchBooks() {
        await api.get(`/Books?genre=${genre}&title=${title}&author=${author}`)
            .then(response => {
                setBooks(response.data);
            })
            .catch(error => {
                setError(error);
            })
            .finally(() => {
                setLoading(false);
            })
    }

    useEffect(() => {
        fetchBooks();
        fetchGenres();
    }, []);

    if (loading) return "Loading...";
    if (error) return "Error!";
    return (
        <>
            <h1>Books</h1>
            {user?.role == "Librarian" && (
                <>
                    <button type="button" className="btn btn-success btn-sm" data-bs-toggle="modal" data-bs-target="#AddModal">Add</button>
                </>
            )}
            <form className="form-inline" onSubmit={handleSubmit}>
                <label className="me-2">Genre: <select onChange={handleChangeGenre}>
                    <option value="">All</option>
                    {genres.map((g) => (
                        <option value={g}>{g}</option>
                    ))}
                </select></label>
                <label className="me-2">Title: <input type="text" onChange={handleChangeTitle} /></label>
                <label className="me-2">Author: <input type="text" onChange={handleChangeAuthor} /></label>
                <input type="submit" value="Filter" />
            </form>
            <table className="table">
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Author</th>
                        <th>Genre</th>
                        <th>Publisher</th>
                        <th>Publication date</th>
                        <th>Status</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {books.map(book => (
                        <tr>
                            <td>{book.title}</td>
                            <td>{book.author}</td>
                            <td>{book.genre}</td>
                            <td>{book.publisher}</td>
                            <td>{new Date(book.publicationDate).toLocaleDateString()}</td>
                            <td>{book.status}</td>
                            <td>
                                {user?.role == "Librarian" && (
                                    <>
                                        <button className="btn btn-primary btn-sm">Edit</button>
                                        <button className="btn btn-danger btn-sm">Delete</button>
                                    </>
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <AddBookModal modalId="AddModal" onBookAdded={fetchBooks} />
        </>
    );
}

export default Books;