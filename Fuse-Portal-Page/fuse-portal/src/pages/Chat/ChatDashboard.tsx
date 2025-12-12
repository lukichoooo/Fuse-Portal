import { useState, useRef, useEffect } from 'react';
import './ChatDashboard.css';
import type { ChatDto, MessageDto } from '../../types/Chat';
import ChatService from '../../services/ChatService';
import ErrorPopup from '../../components/Errors/ErrorPopup';
import type { BackendError } from '../../types/Error';


export default function ChatDashboard() {
    const [error, setError] = useState<string | null>(null);
    const [chats, setChats] = useState<ChatDto[]>([]);
    const [currentChatId, setCurrentChatId] = useState<number>(1);
    const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
    const [currChatMessages, setCurrChatMessages] = useState<MessageDto[]>([]);
    const [input, setInput] = useState('');
    const scrollRef = useRef<HTMLDivElement>(null);
    const pendingMessage: MessageDto = {
        text: "",
        id: 0,
        createdAt: "sending...",
        chatId: currentChatId,
        fromUser: true,
        files: []
    }

    useEffect(() => {
        ChatService.getAllChats()
            .then(setChats)
            .catch(err => setError(err.error || "Unknown error"));
    }, []);

    useEffect(() => {
        scrollRef.current?.scrollTo({
            top: scrollRef.current.scrollHeight,
            behavior: 'smooth',
        });
    }, [currChatMessages]);



    const sendMessage = async () => {
        const trimmed = input.trim();
        if (!trimmed && selectedFiles.length === 0) return;

        setInput('');
        const currPendingMessage: MessageDto = {
            ...pendingMessage,
            text: trimmed,
            chatId: currentChatId
        };
        setCurrChatMessages(prev => [...prev, currPendingMessage]);

        try {
            let fileIds: number[] = [];
            if (selectedFiles.length > 0) {
                fileIds = await ChatService.uploadFiles(selectedFiles);
                setSelectedFiles([]);
            }

            const response = await ChatService.sendMessage({
                message: { text: trimmed, chatId: currentChatId },
                fileIds
            });

            setCurrChatMessages(prev => [
                ...prev.slice(0, -1),
                response.userMessage,
                response.response
            ]);
        } catch (err: BackendError | any) {
            setError(err.error || "Failed to send message");
        }
    };


    const removeFileFromMessage = (index: number) => {
        setSelectedFiles(prev => prev.filter((_, i) => i !== index));
    };

    const startNewChat = async () => {
        try {
            const newChat = await ChatService.createChat({ chatName: 'New Chat' });
            setChats(prev => [...prev, newChat]);
            selectChat(newChat.id);
        } catch (err: any) {
            setError(err.error || "Unknown error");
        }
    };

    const selectChat = async (chatId: number) => {
        setCurrentChatId(chatId);
        const fullChat = await ChatService.getFullChat(chatId);
        setCurrChatMessages(fullChat.messages);
    };



    return (
        <div className="chat-dashboard">
            {error && (
                <ErrorPopup
                    message={error}
                    onClose={() => setError(null)}
                />
            )}

            <div className="chat-sidebar">
                <h3>Chats</h3>
                <ul>
                    {chats.map(chat => (
                        <li
                            key={chat.id}
                            className={chat.id === currentChatId ? 'active' : ''}
                            onClick={() => selectChat(chat.id)}
                        >
                            {chat.name}
                        </li>
                    ))}
                </ul>
                <button onClick={startNewChat}>+ New Chat</button>
            </div>

            <div className="chat-main">
                <div className="chat-header">
                    {chats.find(c => c.id === currentChatId)?.name || 'Select a chat'}
                </div>

                <div className="chat-messages" ref={scrollRef}>
                    {currChatMessages.map((msg, idx) => (
                        <>
                            <div
                                key={idx}
                                className={`chat-message ${msg.fromUser ? 'user' : 'bot'}`}
                                title={new Date(msg.createdAt).toLocaleString()}
                            >
                                {!msg.fromUser && (
                                    <img
                                        src='/logos/ruby.png'
                                        alt="Ruby"
                                        className="chat-avatar"
                                    />
                                )}
                                <span className="chat-text">{msg.text}</span>
                            </div>

                            <div className="chat-files">
                                {msg.files.map((f, i) => (
                                    <div key={i} className="file-icon-wrapper">
                                        <span className="file-icon">📎</span>
                                        <div className="file-tooltip">{f.text}</div>
                                    </div>
                                ))}
                            </div>

                        </>
                    ))}
                </div>


                <div className="chat-input">
                    <div className="file-upload-container">
                        <div className="file-upload">
                            <label htmlFor="fileInput" className="file-upload-button">
                                📎 Attach
                            </label>
                            <input
                                id="fileInput"
                                type="file"
                                multiple
                                onChange={(e) => {
                                    if (!e.target.files) return;
                                    const filesArray = Array.from(e.target.files);
                                    setSelectedFiles(filesArray);
                                }}
                            />
                        </div>

                        {/* Display selected file names */}
                        {selectedFiles.length > 0 && (
                            <div className="selected-files">
                                {selectedFiles.map((file, idx) => (
                                    <div key={idx} className="selected-file">
                                        {file.name}
                                        <span
                                            className="remove-file"
                                            onClick={() => removeFileFromMessage(idx)}
                                        >
                                            ✖
                                        </span>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>

                    <input
                        type="text"
                        placeholder="Type a message..."
                        value={input}
                        onChange={e => setInput(e.target.value)}
                        onKeyDown={e => e.key === 'Enter' && sendMessage()}
                    />
                    <button onClick={sendMessage}>Send</button>
                </div>
            </div>
        </div >
    );
}
