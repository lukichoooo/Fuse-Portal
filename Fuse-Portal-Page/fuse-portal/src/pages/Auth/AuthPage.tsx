import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AuthService from '../../services/AuthService';
import type { LoginRequest, RegisterRequest } from '../../types/AuthTypes';
import { countryCodes, type AddressDto } from '../../types/Address';
import './AuthPage.css';
import ErrorPopup from '../../components/Errors/ErrorPopup';
import type { BackendError } from '../../types/Error';

export default function AuthPage() {

    const navigate = useNavigate();

    const [isRegister, setIsRegister] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Login state
    const [loginEmail, setLoginEmail] = useState('');
    const [loginPassword, setLoginPassword] = useState('');

    // Register state
    const [regName, setRegName] = useState('');
    const [regEmail, setRegEmail] = useState('');
    const [regPassword, setRegPassword] = useState('');
    const [regCity, setRegCity] = useState('');
    const [regCountry, setRegCountry] = useState('USA');

    const handleLogin = async () => {
        const dto: LoginRequest = { email: loginEmail, password: loginPassword };
        try {
            const res = await AuthService.login(dto);

            if (res?.accessToken) {
                navigate('/');
            } else {
                setError('Invalid login response');
            }
        } catch (err: BackendError | any) {
            setError(err?.error || 'Login failed');
        }
    };


    const handleRegister = async () => {
        const address: AddressDto = { city: regCity, countryA3: regCountry };
        const dto: RegisterRequest = { name: regName, email: regEmail, password: regPassword, address };

        try {
            const res = await AuthService.register(dto);

            if (res?.accessToken) {
                navigate('/');
            } else {
                setError('Invalid registration response');
            }
        } catch (err: any) {
            setError(err?.error || 'Registration failed');
        }
    };


    return (
        <div className="auth-page">
            {error && (
                <ErrorPopup
                    message={error}
                    onClose={() => setError(null)}
                />
            )}

            <div className="auth-card">
                <h2>{isRegister ? 'Register' : 'Login'}</h2>

                {isRegister ? (
                    <>
                        <input type="text" placeholder="Name" value={regName} onChange={e => setRegName(e.target.value)} />
                        <input type="email" placeholder="Email" value={regEmail} onChange={e => setRegEmail(e.target.value)} />
                        <input type="password" placeholder="Password" value={regPassword} onChange={e => setRegPassword(e.target.value)} />
                        <input type="text" placeholder="City" value={regCity} onChange={e => setRegCity(e.target.value)} />
                        <select value={regCountry} onChange={e => setRegCountry(e.target.value)}>
                            {countryCodes.map(c => <option key={c} value={c}>{c}</option>)}
                        </select>
                        <button onClick={handleRegister} className="btn btn-primary">Register</button>
                    </>
                ) : (
                    <>
                        <input type="email" placeholder="Email" value={loginEmail} onChange={e => setLoginEmail(e.target.value)} />
                        <input type="password" placeholder="Password" value={loginPassword} onChange={e => setLoginPassword(e.target.value)} />
                        <button onClick={handleLogin} className="btn btn-primary">Login</button>
                    </>
                )}

                <button className="toggle-btn" onClick={() => setIsRegister(!isRegister)}>
                    {isRegister ? 'Already have an account? Login' : "Don't have an account? Register"}
                </button>
            </div>
        </div>
    );
}
