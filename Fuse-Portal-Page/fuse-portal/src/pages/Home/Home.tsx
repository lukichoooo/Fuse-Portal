
import './Home.css'
import { Link } from 'react-router-dom'
import userService from '../../services/user/UserService'

export default function Home() {
    const boxes = [
        { name: 'Fuse-Portal', route: '/portal', logo: '/logos/fuse-portal.png' },
        { name: 'University Portal', route: '/uni-portal', logo: '/logos/university.png' },
        { name: 'Career', route: '/career', logo: '/logos/career.png' },
        { name: 'Notifications', route: '/notifications', logo: '/logos/notifications.png' },
        { name: 'Progress', route: '/progress', logo: '/logos/progress.png' },
        { name: 'Settings', route: '/settings', logo: '/logos/settings.png' },
    ]

    return (
        <div className="home">
            <h1>Home</h1>
            <p>Welcome to the home page</p>

            <div className="grid">
                {boxes.map((box) => {
                    const enabled = userService.isEnabled(box.route)

                    return enabled ? (
                        <Link key={box.name} to={box.route} className="grid-box clickable">
                            <img src={box.logo} alt={box.name} />
                            <span>{box.name}</span>
                        </Link>
                    ) : (
                        <div key={box.name} className="grid-box disabled">
                            <img src={box.logo} alt={box.name} />
                            <span>{box.name}</span>
                        </div>
                    )
                })}
            </div>
        </div>
    )
}

