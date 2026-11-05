import { Link, useNavigate } from "react-router-dom";

export default function NavBar() {
  const navigate = useNavigate();

  return (
    <header className="navbar">
      <Link to="/" className="brand">IMF Funding Management</Link>
      
        <nav>
          <Link to="/">Requests</Link>
          <Link to="/new">New request</Link>
          <span className="who">
          </span>
          <button
            className="link-button"
            onClick={() => {
              navigate("/login");
            }}
          >
            Log out
          </button>
        </nav>
      
    </header>
  );
}
