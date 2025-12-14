
export interface ChatDto {
    id: number;
    name: string;
}

export interface FileDto {
    name: string;
    text: string;
}

export interface FileUpload {
    name: string;
    stream: Blob | File;
}
export interface SendMessageResponseDto {
    response: MessageDto,
    userMessage: MessageDto
}

export interface MessageDto {
    id: number;
    text: string;
    fromUser: boolean;
    createdAt: string; // ISO string
    chatId: number;
    files: FileDto[];
}

export interface ClientMessage {
    text: string;
    chatId: number;
}

export interface MessageRequest {
    message: ClientMessage;
    fileIds: number[];
    stream: boolean;
}

export interface CreateChatRequest {
    chatName?: string; // default "New Chat"
}

export interface ChatFullDto {
    id: number;
    name: string;
    messages: MessageDto[];
}
