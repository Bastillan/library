import { Outlet, Link } from "react-router-dom";

import { useAuth } from '../services/useAuth';

function Layout() {
    const { user, logout } = useAuth();
    return (
        <>
            <header>
                <nav className="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
                    <div className="container-fluid">
                        <Link className="navbar-brand" to="/">ReactLibrary</Link>
                        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                            aria-expanded="false" aria-label="Toggle navigation">
                            <span className="navbar-toggler-icon"></span>
                        </button>
                        <div className="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                            <ul className="navbar-nav flex-grow-1">
                                <li className="nav-item">
                                    <Link className="nav-link text-dark" to="/">Home</Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link text-dark" to="/books">Books</Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link text-dark" to="/reservations">Reservations</Link>
                                </li>
                                <li className="nav-item">
                                    <Link className="nav-link text-dark" to="/checkouts">Checkouts</Link>
                                </li>
                            </ul>
                            <ul className="navbar-nav">
                                {user ? (
                                    <>
                                        <li className="nav-item">
                                            <Link className="nav-link text-dark" to="/account/">Hello {user?.username}!</Link>
                                        </li>
                                        <li className="nav-item">
                                            <button className="nav-link text-dark" onClick={logout}>Logout</button>
                                        </li>
                                    </>
                                ) : (
                                    <>
                                        <li className="nav-item">
                                            <Link className="nav-link text-dark" to="/account/register">Register</Link>
                                        </li>
                                        <li className="nav-item">
                                            <Link className="nav-link text-dark" to="/account/login">Login</Link>
                                        </li>
                                    </>
                                )}
                            </ul>
                        </div>
                    </div>
                </nav>
            </header>
            <div className="container">
                <main role="main" className="pb-3">
                    <Outlet />
                </main>
            </div>
        </>
    );
}

export default Layout;