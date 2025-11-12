
import { useState } from 'react'
import { Link } from 'react-router-dom'
import './Header.css'

export default function Header() {
    const [open, setOpen] = useState(false)

    return (
        <header className="header">
            <Link to="/" className="logo">
                <img src="/logos/fuse-portal.png" alt="Logo" className="logo-box" />
                <span className="logo-text">Portal</span>
            </Link>

            <div className="header-actions">
                <Link to="/chat" className="chat-btn">
                    <img src="/logos/ai-chat.png" alt="Chat" className="chat-icon" />
                    <span>Chat with AI Mentor</span>
                </Link>

                <div className="dropdown">
                    <button className="dropdown-toggle" onClick={() => setOpen(!open)}>
                        Menu ▾
                    </button>
                    {open && (
                        <ul className="dropdown-menu">
                            <li>Profile</li>
                            <li>Settings</li>
                            <li>Logout</li>
                        </ul>
                    )}
                </div>
            </div>
        </header>
    )
}

