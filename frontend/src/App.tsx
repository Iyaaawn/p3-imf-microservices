import { Navigate, Route, Routes } from "react-router-dom";
import NavBar from "./components/NavBar";

import RequestsPage from "./pages/RequestsPage";
import NewRequestPage from "./pages/NewRequestPage";

// Client-side routing: the URL decides which page component renders, with no page reload
export default function App() {
  return (
    <>
      <NavBar />
      <main className="container">
        <Routes>
          <Route path="/" element={<RequestsPage />} />
          <Route path="/new" element={<NewRequestPage />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </main>
    </>
  );
}
