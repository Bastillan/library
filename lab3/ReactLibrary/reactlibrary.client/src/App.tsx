import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./pages/Layout";
import Home from "./pages/Home";
import Books from "./pages/Books";
import Reservations from "./pages/Reservations";
import Checkouts from "./pages/Checkouts";
import Account from "./pages/Account";
import Register from "./pages/Register";
import Login from "./pages/Login";
import NoPage from "./pages/NoPage";
import { AuthProvider } from './services/AuthProvider';

function App() {
    return (
        <AuthProvider>
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<Layout />}>
                        <Route index element={<Home />} />
                        <Route path="books" element={<Books />} />
                        <Route path="reservations" element={<Reservations />} />
                        <Route path="checkouts" element={<Checkouts />} />
                        <Route path="account">
                            <Route index element={<Account /> }/>
                            <Route path="register" element={<Register />} />
                            <Route path="login" element={<Login />} />
                        </Route>    
                        <Route path="*" element={<NoPage /> } />
                    </Route>
                </Routes>
            </BrowserRouter>
        </AuthProvider>
    );
}

export default App;