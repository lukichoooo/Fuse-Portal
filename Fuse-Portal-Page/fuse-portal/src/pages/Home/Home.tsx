import { Link } from 'react-router-dom';
import './Home.css';

interface Box {
    name: string;
    route: string;
    logo: string;
}

export default function Home() {
    const boxes: Box[] = [
        { name: 'Notifications', route: '/notifications', logo: '/logos/notifications.png' },
        { name: 'Settings', route: '/settings', logo: '/logos/settings.png' },
    ];

    return (
        <div className="home">
            <header>
                <h1>Home</h1>
                <p>Welcome to your learning dashboard</p>
            </header>

            <Link to="/chats" className="big-box">
                <img src="/mascot/reading-book.png" alt="Featured" />
                <div className="big-box-text">
                    <span>Talk to Ruby</span>
                </div>
            </Link>

            <Link to="/portal" className="big-box">
                <img src="/mascot/reading-book.png" alt="Featured" />
                <div className="big-box-text">
                    <span>Portal</span>
                </div>
            </Link>

            <Link to="/parser" className="big-box">
                <img src="/mascot/reading-book.png" alt="Featured" />
                <div className="big-box-text">
                    <span>Portal Parser</span>
                </div>
            </Link>

            <Link to="/calendar" className="big-box">
                <img src="/mascot/reading-book.png" alt="Featured" />
                <div className="big-box-text">
                    <span>Calendar</span>
                </div>
            </Link>

            <Link to="/mockexams" className="big-box">
                <img src="/mascot/reading-book.png" alt="Featured" />
                <div className="big-box-text">
                    <span>Mock Exams</span>
                </div>
            </Link>

            <div className="grid">
                {boxes.map((box) => (
                    <Link to={box.route} className="box" key={box.name}>
                        <img src={box.logo} alt={box.name} />
                        <span>{box.name}</span>
                    </Link>
                ))}
            </div>
        </div>
    );
}

