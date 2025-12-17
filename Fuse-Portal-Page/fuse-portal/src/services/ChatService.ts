//
import * as signalR from "@microsoft/signalr";
// ChatService.ts
import api from '../api/api';
import type {
    ChatDto,
    ChatFullDto,
    CreateChatRequest,
    FileDto,
    MessageDto,
    MessageRequest,
    SendMessageResponseDto,
} from '../types/Chat';

export default class ChatService {

    private static connection: signalR.HubConnection | null = null;

    static async getAllChats(lastId?: number, pageSize?: number): Promise<ChatDto[]> {
        const params: Record<string, any> = {};
        if (lastId) params.lastId = lastId;
        if (pageSize) params.pageSize = pageSize;

        const res = await api.get<ChatDto[]>('/chat', { params: params });
        return res.data;
    }

    static async getFullChat(chatId: number, firstMsgId?: number, pageSize?: number): Promise<ChatFullDto> {
        const params: Record<string, any> = {};
        if (firstMsgId) params.lastId = firstMsgId;
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


    private static async getConnection(): Promise<signalR.HubConnection> {
        if (!this.connection) {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("http://localhost:5016" + "/hub/chat")
                .withAutomaticReconnect()
                .build();

            await this.connection.start();
        }
        return this.connection;
    }

    static async getFile(fileId: number): Promise<FileDto> {
        const { data } = await api.get<FileDto>(`/chat/messages/file/${fileId}`);
        return data;
    }

    static async sendMessageWithStream(
        request: MessageRequest,
        onReceived: (chunk: string) => void
    ): Promise<SendMessageResponseDto> {
        const streamedConnection = await this.getConnection();
        const chatId = request.message.chatId.toString();

        // Subscribe for this call only
        const handler = (receivedChatId: string, chunk: string) => {
            if (receivedChatId === chatId) onReceived(chunk);
        };
        streamedConnection.on("messageReceived", handler);

        await streamedConnection.invoke("JoinChat", chatId);
        const res = await api.post<SendMessageResponseDto>('/chat/ws/messages/text', request);

        // Remove the handler but stay in the group for streaming
        streamedConnection.off("messageReceived", handler);

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

