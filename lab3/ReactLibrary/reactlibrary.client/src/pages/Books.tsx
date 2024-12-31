import { useEffect, useState } from "react";

import api from "../services/api";
import { Book } from "../types/Books";

function Books() {
    const [books, setBooks] = useState<Book[]>([]);
    const [genres, setGenres] = useState<string[]>([]);
    const [genre, setGenre] = useState<string>("");
    const [title, setTitle] = useState<string>("");
    const [author, setAuthor] = useState<string>("");
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

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
                        </tr>
                    ))}
                </tbody>
            </table>
        </>
    );
}

export default Books;