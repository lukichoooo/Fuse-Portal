
// ErrorPopup.tsx
import React, { useEffect } from "react";

interface ErrorPopupProps {
    message: string;
    duration?: number;
    onClose?: () => void;
}

const ErrorPopup: React.FC<ErrorPopupProps> = ({ message, duration = 3000, onClose }) => {
    useEffect(() => {
        if (duration && onClose) {
            const timer = setTimeout(onClose, duration);
            return () => clearTimeout(timer);
        }
    }, [duration, onClose]);

    return (
        <div
            style={{
                position: "fixed",
                bottom: "5rem",
                right: "1rem",
                width: "300px",
                padding: "1rem",
                backgroundColor: "red",
                color: "white",
                fontWeight: "bold",
                borderRadius: "5px",
                zIndex: 1000,
            }}
        >
            {message}
        </div>
    );
};

export default ErrorPopup;
