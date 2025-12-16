import { Routes, Route } from 'react-router-dom'
import './App.css'
import Header from './components/Header/Header'
import Home from './pages/Home/Home'
import AuthPage from './pages/Auth/AuthPage'
import ChatDashboard from './pages/Chat/ChatDashboard'
import ProtectedRoute from './pages/Auth/ProtectedRoute'
import CalendarPage from './pages/Portal/CalendarPage'
import StudentPortal from './pages/Portal/StudentPortal'
import MockExamPage from './pages/Portal/MockExamPage'

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
                        path="/calendar"
                        element={
                            <ProtectedRoute>
                                <CalendarPage />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/portal"
                        element={
                            <ProtectedRoute>
                                <StudentPortal />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/mockexams"
                        element={
                            <ProtectedRoute>
                                <MockExamPage />
                            </ProtectedRoute>
                        }
                    />

                </Routes>
            </main>
        </>
    )
}

export default App

