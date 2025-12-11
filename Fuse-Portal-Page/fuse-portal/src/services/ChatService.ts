// ChatService.ts
import api from '../api/api';
import type {
    ChatDto,
    ChatFullDto,
    CreateChatRequest,
    MessageDto,
    MessageRequest,
    SendMessageResponseDto,
} from '../types/Chat';

export default class ChatService {
    static async getAllChats(lastId?: number, pageSize?: number): Promise<ChatDto[]> {
        const params: Record<string, any> = {};
        if (lastId) params.lastId = lastId;
        if (pageSize) params.pageSize = pageSize;

        const res = await api.get<ChatDto[]>('/chat', { params: params });
        return res.data;
    }

    static async getFullChat(chatId: number, lastId?: number, pageSize?: number): Promise<ChatFullDto> {
        const params: Record<string, any> = {};
        if (lastId) params.lastId = lastId;
        if (pageSize) params.pageSize = pageSize;

        const res = await api.get<ChatFullDto>(`/chat/${chatId}`, { params: params });
        return res.data;
    }

    static async createChat(request: CreateChatRequest): Promise<ChatDto> {
        const res = await api.post<ChatDto>('/chat', request);
        return res.data;
    }

    static async deleteMessage(msgId: number): Promise<MessageDto> {
        const res = await api.delete<MessageDto>(`/chat/messages/${msgId}`);
        return res.data;
    }

    static async sendMessage(request: MessageRequest): Promise<SendMessageResponseDto> {
        const res = await api.post<SendMessageResponseDto>('/chat/messages/text', request);
        return res.data;
    }

    static async uploadFiles(files: File[]): Promise<number[]> {
        const formData = new FormData();
        files.forEach(f => formData.append('Files', f));

        const res = await api.post<number[]>('/chat/messages/file', formData, {
            headers: {
                // explicitly allow Axios to detect FormData
                'Content-Type': 'multipart/form-data'
            }
        });

        return res.data;
    }




}

