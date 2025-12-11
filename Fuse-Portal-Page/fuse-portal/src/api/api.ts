import axios, { type AxiosRequestConfig } from 'axios';
import AuthService from '../services/AuthService';

const api = axios.create({
    baseURL: 'http://localhost:5016/api',
    headers: {
        'Content-Type': 'application/json',
    },
});

// Request interceptor for bearer token
api.interceptors.request.use(
    (config) => {
        const token = AuthService.getAccessToken();
        if (token) {
            config.headers['Authorization'] = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// Response interceptor to handle backend errors
api.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response && error.response.data) {
            return Promise.reject(error.response.data);
        }
        return Promise.reject(error);
    }
);

// Helper wrapper to allow overriding headers per request
export const requestWithOverride = <T = any>(
    method: 'get' | 'post' | 'put' | 'delete',
    url: string,
    data?: any,
    configOverride?: AxiosRequestConfig
) => {
    return api.request<T>({
        method,
        url,
        data,
        ...configOverride, // merge overrides
    });
};

export default api;

