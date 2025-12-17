import { Link } from 'react-router-dom';
import {
    MessageCircle,
    BookOpen,
    FileText,
    Calendar,
    ClipboardCheck,
    Bell,
    Settings
} from 'lucide-react';
import './Home.css';

interface Box {
    name: string;
    route: string;
    icon: React.ElementType;
}

export default function Home() {
    const boxes: Box[] = [
        { name: 'Notifications', route: '/notifications', icon: Bell },
        { name: 'Settings', route: '/settings', icon: Settings },
    ];

    return (
        <div className="home">
            <header className="home-header">
                <h1>Home</h1>
                <p>Welcome to your learning dashboard</p>
            </header>

            {/* HERO */}
            <Link to="/chats" className="hero">
                <img src="/mascot/reading-book.png" alt="Talk to Ruby" />
                <div className="hero-text">
                    <MessageCircle size={36} />
                    <span>Talk to Ruby</span>
                </div>
            </Link>

            {/* FEATURES */}
            <section className="features">
                <Link to="/portal" className="feature-card">
                    <BookOpen size={28} />
                    <span>Portal</span>
                </Link>

                <Link to="/parser" className="feature-card">
                    <FileText size={28} />
                    <span>Portal Parser</span>
                </Link>

                <Link to="/calendar" className="feature-card">
                    <Calendar size={28} />
                    <span>Calendar</span>
                </Link>

                <Link to="/mockexams" className="feature-card">
                    <ClipboardCheck size={28} />
                    <span>Mock Exams</span>
                </Link>
            </section>

            {/* UTILITIES */}
            <section className="grid">
                {boxes.map(({ name, route, icon: Icon }) => (
                    <Link to={route} className="box" key={name}>
                        <Icon size={26} />
                        <span>{name}</span>
                    </Link>
                ))}
            </section>
        </div>
    );
}
