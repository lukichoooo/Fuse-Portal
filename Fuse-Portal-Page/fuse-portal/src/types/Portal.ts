// ---- Main DTOs ----
export interface ExamDto {
    id: number;
    questions: string;
    scoreFrom100?: number;
    answers: string;
    results?: string | null;
    grade?: number | null;
    subjectId: number;
    subjectName?: string | null;
}

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
    start: string;   // ISO string
    end: string;     // ISO string
    subjectId: number;
    metadata?: string | null;
}

export interface SubjectDto {
    id: number;
    name: string;
    schedules: ScheduleDto[];
    metadata?: string | null;
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
    start: string;   // ISO string
    end: string;     // ISO string
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
    start: string;   // ISO string
    end: string;     // ISO string
    location?: string;
    metadata?: string;
}

export interface SyllabusRequestDtoNoSubjectId {
    name: string;
    content: string;
    metadata?: string;
}

// ---- Full Subject Request (used in parser) ----
export interface SubjectFullRequestDto {
    name: string;
    grade?: number | null;
    metadata?: string;
    schedules: ScheduleRequestDtoNoSubjectId[];
    lecturers: LecturerRequestDtoNoSubjectId[];
    syllabuses: SyllabusRequestDtoNoSubjectId[];
}

// ---- Portal Parser Response ----
export interface PortalParserResponseDto {
    subjects: SubjectFullRequestDto[];
    metadata?: string;
}
