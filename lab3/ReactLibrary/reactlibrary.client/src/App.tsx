import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./pages/Layout";
import Home from "./pages/Home";
import Books from "./pages/Books";
import Reservations from "./pages/Reservations";
import Checkouts from "./pages/Checkouts";
import NoPage from "./pages/NoPage";

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Layout />}>
                    <Route index element={<Home />} />
                    <Route path="books" element={<Books />} />
                    <Route path="reservations" element={<Reservations />} />
                    <Route path="checkouts" element={<Checkouts />} />
                    <Route path="*" element={<NoPage /> } />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}

export default App;