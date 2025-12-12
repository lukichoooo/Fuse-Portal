import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './Header.css';
import AuthService from '../../services/AuthService';

export default function Header() {

    const navigate = useNavigate();

    const [loggedIn, setLoggedIn] = useState(AuthService.isLoggedIn());

    function handleLogoutClick() {
        AuthService.logout();
        navigate('/');
        setLoggedIn(false);
    }

    useEffect(() => {
        const interval = setInterval(() => {
            setLoggedIn(AuthService.isLoggedIn());
        }, 1000);
        return () => clearInterval(interval);
    }, []);

    return (
        <header className="header">
            <Link to="/" className="logo">
                <img src="/logos/ruby.png" alt="Logo" className="logo-box" />
                <span className="logo-text">Ruby</span>
            </Link>

            <div className="header-actions">
                {!loggedIn ? (
                    <>
                        <Link to="/auth" className="btn">Not Logged in?</Link>
                    </>
                ) : (
                    <>
                        <Link to="/profile" className="btn">Profile</Link>
                        <button onClick={handleLogoutClick} className="btn btn-secondary">Logout</button>
                    </>
                )}
            </div>
        </header>
    );
}

