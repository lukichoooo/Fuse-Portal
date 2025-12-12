import { Navigate } from "react-router-dom";
import AuthService from "../../services/AuthService";
import type { JSX } from "react";

export default function ProtectedRoute({ children }: { children: JSX.Element }) {
    if (!AuthService.isLoggedIn()) return <Navigate to="/auth" replace />;

    return children;
}

