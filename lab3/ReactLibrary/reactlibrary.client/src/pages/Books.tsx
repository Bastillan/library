import { useEffect, useState } from "react";
import axios from "axios";

import { Book } from '../types/Books';

function Books() {
    const [books, setBooks] = useState<Book[]>([]);

    async function fetchBooks() {
        const response = await axios.get('/api/Books');
        setBooks(response.data);
    }

    useEffect(() => {
        fetchBooks();
    }, []);

    return (
        <>
            <h1>Books</h1>
            <form className="form-inline">
                <label className="me-2">Genre: <select>
                    <option value="">All</option>
                </select></label>
                <label className="me-2">Title: <input type="text" /></label>
                <label className="me-2">Author: <input type="text" /></label>
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