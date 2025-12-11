
import { Routes, Route } from 'react-router-dom'
import './App.css'
import Header from './components/Header/Header'
import Home from './pages/Home/Home'
import AuthPage from './pages/Auth/AuthPage'
import ChatDashboard from './pages/Chat/ChatDashboard'

function App() {
    return (
        <>
            <Header />
            <main>
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/auth" element={<AuthPage />} />

                    {/* Auth required */}
                    <Route path="/chats" element={<ChatDashboard />} />
                </Routes>
            </main>
        </>
    )
}

export default App

