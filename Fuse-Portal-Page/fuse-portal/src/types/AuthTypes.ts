import type { AddressDto } from "./Address";

export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    name: string;
    email: string;
    password: string;
    address: AddressDto;
}

export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
}
