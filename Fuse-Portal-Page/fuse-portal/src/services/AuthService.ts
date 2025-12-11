import api from '../api/api';
import type { AuthResponse, LoginRequest, RegisterRequest } from '../types/AuthTypes';

const ACCESS_TOKEN_KEY = 'accessToken';
const REFRESH_TOKEN_KEY = 'refreshToken';

export default class AuthService {
    private static storeTokens(response: AuthResponse) {
        localStorage.setItem(ACCESS_TOKEN_KEY, response.accessToken);
        localStorage.setItem(REFRESH_TOKEN_KEY, response.refreshToken);
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
}

