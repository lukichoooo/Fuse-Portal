import { Routes, Route } from 'react-router-dom'
import './App.css'
import Header from './components/Header/Header'
import Home from './pages/Home/Home'
import AuthPage from './pages/Auth/AuthPage'
import ChatDashboard from './pages/Chat/ChatDashboard'
import ProtectedRoute from './pages/Auth/ProtectedRoute'
import PortalPage from './pages/Portal/PortalPage'

function App() {
    return (
        <>
            <Header />
            <main>
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/auth" element={<AuthPage />} />

                    {/* Auth required */}
                    <Route
                        path="/chats"
                        element={
                            <ProtectedRoute>
                                <ChatDashboard />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/portal"
                        element={
                            <ProtectedRoute>
                                <PortalPage />
                            </ProtectedRoute>
                        }
                    />
                </Routes>
            </main>
        </>
    )
}

export default App

