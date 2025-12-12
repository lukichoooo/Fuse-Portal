import api from '../api/api';
import type { AuthResponse, LoginRequest, RegisterRequest } from '../types/AuthTypes';

const ACCESS_TOKEN_KEY = 'accessToken';
const REFRESH_TOKEN_KEY = 'refreshToken';
const LOGIN_TIME = 'loginTime';
const SESSION_TIME = 5 * 60 * 1000;

export default class AuthService {
    private static storeTokens(response: AuthResponse) {
        localStorage.setItem(ACCESS_TOKEN_KEY, response.accessToken);
        localStorage.setItem(REFRESH_TOKEN_KEY, response.refreshToken);
        localStorage.setItem(LOGIN_TIME, String(Date.now()));
    }

    static async login(userLogin: LoginRequest): Promise<AuthResponse> {
        const response = await api.post<AuthResponse>('/auth/login', userLogin);
        this.storeTokens(response.data);
        return response.data;
    }

    static async register(userRegister: RegisterRequest): Promise<AuthResponse> {
        const response = await api.post<AuthResponse>('/auth/register', userRegister);
        this.storeTokens(response.data);
        return response.data;
    }

    static isLoggedIn(): boolean {
        if (this.isSessionExpired()) {
            this.logout();
            return false;
        }
        return Boolean(localStorage.getItem(ACCESS_TOKEN_KEY));
    }

    static getAccessToken(): string | null {
        return localStorage.getItem(ACCESS_TOKEN_KEY);
    }

    static getRefreshToken(): string | null {
        return localStorage.getItem(REFRESH_TOKEN_KEY);
    }

    static logout(): void {
        localStorage.removeItem(ACCESS_TOKEN_KEY);
        localStorage.removeItem(REFRESH_TOKEN_KEY);
    }

    static isSessionExpired(): boolean {
        const loginTime = Number(localStorage.getItem("loginTime"));
        if (!loginTime) return true;

        return Date.now() - loginTime > SESSION_TIME;
    }
}

