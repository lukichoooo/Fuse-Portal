
// ---- Main DTOs ----
export interface SyllabusDto {
    id: number;
    name: string;
}

export interface LecturerDto {
    id: number;
    name: string;
}

export interface SyllabusFullDto {
    id: number;
    name: string;
    content: string;
    metadata?: string | null;
}

export interface ScheduleDto {
    id: number;
    start: string;             // ISO string
    end: string;               // ISO string
    subjectId: number;
    metadata?: string | null;
}

export interface SubjectDto {
    id: number;
    name: string;
    metadata?: string | null;
    schedules: ScheduleDto[];
}

export interface SubjectFullDto {
    id: number;
    name: string;
    grade?: number | null;
    schedules: ScheduleDto[];
    lecturers: LecturerDto[];
    syllabuses: SyllabusDto[];
    metadata?: string | null;
}


// ---- Request DTOs (with SubjectId) ----
export interface LecturerRequestDto {
    name: string;
    subjectId: number;
}

export interface ScheduleRequestDto {
    start: string;             // ISO string
    end: string;               // ISO string
    subjectId: number;
    location?: string;
    metadata?: string;
}

export interface SyllabusRequestDto {
    name: string;
    content: string;
    subjectId: number;
    metadata?: string;
}

export interface SubjectRequestDto {
    name: string;
    grade?: number | null;
    metadata?: string;
}

// ---- Request DTOs (No SubjectId) ----
export interface LecturerRequestDtoNoSubjectId {
    name: string;
}

export interface ScheduleRequestDtoNoSubjectId {
    start: string;             // ISO string
    end: string;               // ISO string
    location?: string;
    metadata?: string;
}

export interface SyllabusRequestDtoNoSubjectId {
    name: string;
    content: string;
    metadata?: string;
}
