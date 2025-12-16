// PortalService.ts
import api from '../api/api';
import type {
    SubjectDto,
    SubjectFullDto,
    ScheduleDto,
    LecturerDto,
    SyllabusDto,
    SyllabusFullDto as SyllabusFullDto,
    SubjectRequestDto,
    ScheduleRequestDto,
    LecturerRequestDto,
    SyllabusRequestDto,
    ExamDto
} from '../types/Portal';

export default class PortalService {

    // Exams
    static async generateMockExam(syllabusId: number): Promise<ExamDto> {
        const res = await api.get<ExamDto>(`/portal/exam/${syllabusId}`);
        return res.data;
    }

    static async checkExamAnswers(examDto: ExamDto): Promise<ExamDto> {
        const res = await api.post<ExamDto>('/portal/exam', examDto);
        return res.data;
    }


    // Subjects
    static async getSubjects(lastSubjectId?: number, pageSize?: number): Promise<SubjectDto[]> {
        const params: Record<string, any> = {};
        if (lastSubjectId) params.lastSubjectId = lastSubjectId;
        if (pageSize) params.pageSize = pageSize;

        const res = await api.get<SubjectDto[]>('/portal/subjects', { params });
        return res.data;
    }


    static async getFullSubject(subjectId: number): Promise<SubjectFullDto> {
        const res = await api.get<SubjectFullDto>(`/portal/subject/${subjectId}`);
        return res.data;
    }

    static async addSubject(request: SubjectRequestDto): Promise<SubjectFullDto> {
        const res = await api.post<SubjectFullDto>('/portal/subject', request);
        return res.data;
    }

    static async removeSubject(subjectId: number): Promise<SubjectDto> {
        const res = await api.delete<SubjectDto>(`/portal/subject/${subjectId}`);
        return res.data;
    }

    // Schedules
    static async addSchedule(request: ScheduleRequestDto): Promise<ScheduleDto> {
        const res = await api.post<ScheduleDto>('/portal/schedule', request);
        return res.data;
    }

    static async removeSchedule(scheduleId: number): Promise<ScheduleDto> {
        const res = await api.delete<ScheduleDto>(`/portal/schedule/${scheduleId}`);
        return res.data;
    }

    // Lecturers
    static async addLecturer(request: LecturerRequestDto): Promise<LecturerDto> {
        const res = await api.post<LecturerDto>('/portal/lecturer', request);
        return res.data;
    }

    static async removeLecturer(lecturerId: number): Promise<LecturerDto> {
        const res = await api.delete<LecturerDto>(`/portal/lecturer/${lecturerId}`);
        return res.data;
    }

    // syllabuses
    static async addSyllabus(request: SyllabusRequestDto): Promise<SyllabusDto> {
        const res = await api.post<SyllabusDto>('/portal/syllabus', request);
        return res.data;
    }

    static async removeSyllabus(syllabusId: number): Promise<SyllabusDto> {
        const res = await api.delete<SyllabusDto>(`/portal/syllabus/${syllabusId}`);
        return res.data;
    }

    static async getSyllabus(syllabusId: number): Promise<SyllabusFullDto> {
        const res = await api.get<SyllabusFullDto>(`/portal/syllabus/${syllabusId}`);
        return res.data;
    }

    // Upload raw HTML or page Text
    static async uploadHtml(html: string): Promise<void> {
        await api.post('/portal/upload-page', html, {
            headers: { 'Content-Type': 'text/plain' }
        });
    }
}
