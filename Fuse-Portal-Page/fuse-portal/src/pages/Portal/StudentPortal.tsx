import React, { useState, useEffect } from 'react';
import PortalService from '../../services/PortalService';
import type {
    SubjectDto,
    SubjectFullDto,
    SubjectRequestDto,
    // ScheduleRequestDto,
    // LecturerRequestDto,
    // SyllabusRequestDto
} from '../../types/Portal';

import './StudentPortal.css';

// ==========================================
// 2. HELPER COMPONENTS
// ==========================================

const Modal = ({ title, isOpen, onClose, children }: { title: string, isOpen: boolean, onClose: () => void, children: React.ReactNode }) => {
    if (!isOpen) return null;
    return (
        <div className="modal-overlay" onClick={(e) => e.target === e.currentTarget && onClose()}>
            <div className="modal">
                <h2 style={{ marginTop: 0, color: 'var(--primary-color)' }}>{title}</h2>
                {children}
            </div>
        </div>
    );
};

// ==========================================
// 3. MAIN DASHBOARD COMPONENT
// ==========================================

export default function UniversityDashboard() {
    // --- State Management ---
    const [subjects, setSubjects] = useState<SubjectDto[]>([]);
    const [selectedId, setSelectedId] = useState<number | null>(null);
    const [fullSubject, setFullSubject] = useState<SubjectFullDto | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    // Modal States
    const [modals, setModals] = useState({
        subject: false,
        lecturer: false,
        schedule: false,
        syllabus: false,
        upload: false
    });

    // Forms State
    const [formSubject, setFormSubject] = useState<SubjectRequestDto>({ name: '', grade: null, metadata: '' });
    const [formLecturer, setFormLecturer] = useState({ name: '' });
    const [formSchedule, setFormSchedule] = useState({ start: '', end: '', location: '', metadata: '' });
    const [formSyllabus, setFormSyllabus] = useState({ name: '', content: '', metadata: '' });
    const [htmlContent, setHtmlContent] = useState('');

    // --- Data Fetching ---
    const loadSubjects = async () => {
        try {
            const data = await PortalService.getSubjects(undefined, 50);
            setSubjects(data);
        } catch (e) { console.error("Error loading subjects", e); }
    };

    const loadFullSubject = async (id: number) => {
        setIsLoading(true);
        try {
            const data = await PortalService.getFullSubject(id);
            setFullSubject(data);
        } catch (e) { console.error("Error loading full subject", e); }
        setIsLoading(false);
    };

    useEffect(() => { loadSubjects(); }, []);

    useEffect(() => {
        if (selectedId) loadFullSubject(selectedId);
        else setFullSubject(null);
    }, [selectedId]);

    // --- Helper to toggle modals ---
    const toggleModal = (key: keyof typeof modals, state: boolean) => {
        setModals(prev => ({ ...prev, [key]: state }));
    };

    // --- CRUD Handlers ---

    // 1. SUBJECTS
    const handleAddSubject = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const res = await PortalService.addSubject(formSubject);
            setFormSubject({ name: '', grade: null, metadata: '' });
            toggleModal('subject', false);
            loadSubjects();
            setSelectedId(res.id); // Auto-select new subject
        } catch (error) { alert("Failed to add subject"); }
    };

    const handleRemoveSubject = async (e: React.MouseEvent, id: number) => {
        e.stopPropagation();
        if (!window.confirm("Delete this subject?")) return;
        try {
            await PortalService.removeSubject(id);
            if (selectedId === id) setSelectedId(null);
            loadSubjects();
        } catch (error) { alert("Failed to delete subject"); }
    };

    // 2. LECTURERS
    const handleAddLecturer = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedId) return;
        try {
            await PortalService.addLecturer({ name: formLecturer.name, subjectId: selectedId });
            setFormLecturer({ name: '' });
            toggleModal('lecturer', false);
            loadFullSubject(selectedId);
        } catch (error) { alert("Failed to add lecturer"); }
    };

    const handleRemoveLecturer = async (id: number) => {
        try {
            await PortalService.removeLecturer(id);
            if (selectedId) loadFullSubject(selectedId);
        } catch (error) { alert("Failed to remove lecturer"); }
    };

    // 3. SCHEDULES
    const handleAddSchedule = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedId) return;
        try {
            await PortalService.addSchedule({
                subjectId: selectedId,
                start: new Date(formSchedule.start).toISOString(),
                end: new Date(formSchedule.end).toISOString(),
                location: formSchedule.location,
                metadata: formSchedule.metadata
            });
            setFormSchedule({ start: '', end: '', location: '', metadata: '' });
            toggleModal('schedule', false);
            loadFullSubject(selectedId);
        } catch (error) { alert("Failed to add schedule"); }
    };

    const handleRemoveSchedule = async (id: number) => {
        try {
            await PortalService.removeSchedule(id);
            if (selectedId) loadFullSubject(selectedId);
        } catch (error) { alert("Failed to remove schedule"); }
    };

    // 4. SYLLABUS
    const handleAddSyllabus = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!selectedId) return;
        try {
            await PortalService.addSyllabus({
                subjectId: selectedId,
                name: formSyllabus.name,
                content: formSyllabus.content,
                metadata: formSyllabus.metadata
            });
            setFormSyllabus({ name: '', content: '', metadata: '' });
            toggleModal('syllabus', false);
            loadFullSubject(selectedId);
        } catch (error) { alert("Failed to add syllabus"); }
    };

    const handleRemoveSyllabus = async (id: number) => {
        try {
            await PortalService.removeSyllabus(id);
            if (selectedId) loadFullSubject(selectedId);
        } catch (error) { alert("Failed to remove syllabus"); }
    };

    // 5. HTML UPLOAD
    const handleHtmlUpload = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await PortalService.uploadHtml(htmlContent);
            setHtmlContent('');
            toggleModal('upload', false);
            alert("HTML Processed Successfully");
            loadSubjects();
        } catch (error) { alert("Upload failed"); }
    };

    // --- Render ---
    return (
        <>
            <div className="dashboard-layout">
                {/* --- SIDEBAR --- */}
                <aside className="sidebar">
                    <div className="brand">
                        <span>🎓</span> UniPortal
                    </div>

                    <div className="sidebar-header">
                        <span>Subjects</span>
                        <button className="add-btn-small" onClick={() => toggleModal('subject', true)} title="Add Subject">+</button>
                    </div>

                    <div className="subject-list">
                        {subjects.map(sub => (
                            <div
                                key={sub.id}
                                className={`subject-item ${selectedId === sub.id ? 'active' : ''}`}
                                onClick={() => setSelectedId(sub.id)}
                            >
                                <span>{sub.name}</span>
                                <button className="delete-btn" onClick={(e) => handleRemoveSubject(e, sub.id)}>×</button>
                            </div>
                        ))}
                    </div>

                    <div style={{ marginTop: '2rem', borderTop: '1px solid var(--border-color)', paddingTop: '1rem' }}>
                        <button className="btn btn-outline" style={{ width: '100%', fontSize: '0.85rem' }} onClick={() => toggleModal('upload', true)}>
                            📤 Import Raw HTML
                        </button>
                    </div>
                </aside>

                {/* --- MAIN CONTENT --- */}
                <main className="main-content">
                    {!selectedId ? (
                        <div className="empty-state">
                            <h2>Select a Subject</h2>
                            <p>Choose a subject from the sidebar to manage schedules, lecturers, and syllabus content.</p>
                        </div>
                    ) : isLoading || !fullSubject ? (
                        <div className="empty-state">Loading Data...</div>
                    ) : (
                        <>
                            {/* Header */}
                            <div className="header-section">
                                <div className="subject-title">
                                    <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
                                        <h1>{fullSubject.name}</h1>
                                        {fullSubject.grade && <span className="badge">Grade: {fullSubject.grade}</span>}
                                    </div>
                                    <div className="subject-meta">
                                        <span>ID: #{fullSubject.id}</span>
                                        {fullSubject.metadata && <span>• {fullSubject.metadata}</span>}
                                    </div>
                                </div>
                                <div style={{ display: 'flex', gap: '1rem' }}>
                                    {/* Exam Link (Requirement) */}
                                    <a
                                        href={`/mockexams`}
                                        className="btn btn-primary"
                                        target="_blank"
                                        rel="noreferrer"
                                    >
                                        📝 Generate Mock Exam
                                    </a>
                                </div>
                            </div>

                            {/* Schedules Grid */}
                            <div className="grid-section">
                                <div className="section-title">
                                    <h3>📅 Schedules</h3>
                                    <button className="btn btn-outline" onClick={() => toggleModal('schedule', true)}>+ Add</button>
                                </div>
                                <div className="cards-container">
                                    {fullSubject.schedules.length === 0 && <p style={{ color: '#999' }}>No schedules found.</p>}
                                    {fullSubject.schedules.map(sch => (
                                        <div key={sch.id} className="card">
                                            <div className="card-actions">
                                                <button className="delete-btn" onClick={() => handleRemoveSchedule(sch.id)}>×</button>
                                            </div>
                                            <h4>{new Date(sch.start).toLocaleDateString()}</h4>
                                            <p><strong>Time:</strong> {new Date(sch.start).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })} - {new Date(sch.end).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</p>
                                            <p><strong>Location:</strong> {sch.metadata || "Online"}</p>
                                        </div>
                                    ))}
                                </div>
                            </div>

                            {/* Lecturers Grid */}
                            <div className="grid-section">
                                <div className="section-title">
                                    <h3>👨‍🏫 Lecturers</h3>
                                    <button className="btn btn-outline" onClick={() => toggleModal('lecturer', true)}>+ Add</button>
                                </div>
                                <div className="cards-container">
                                    {fullSubject.lecturers.length === 0 && <p style={{ color: '#999' }}>No lecturers assigned.</p>}
                                    {fullSubject.lecturers.map(lec => (
                                        <div key={lec.id} className="card">
                                            <div className="card-actions">
                                                <button className="delete-btn" onClick={() => handleRemoveLecturer(lec.id)}>×</button>
                                            </div>
                                            <h4>{lec.name}</h4>
                                            <p>Faculty Member</p>
                                        </div>
                                    ))}
                                </div>
                            </div>

                            {/* Syllabus Grid */}
                            <div className="grid-section">
                                <div className="section-title">
                                    <h3>📚 Syllabus</h3>
                                    <button className="btn btn-outline" onClick={() => toggleModal('syllabus', true)}>+ Add</button>
                                </div>
                                <div className="cards-container">
                                    {fullSubject.syllabuses.length === 0 && <p style={{ color: '#999' }}>No syllabus uploaded.</p>}
                                    {fullSubject.syllabuses.map(syl => (
                                        <div key={syl.id} className="card" style={{ gridColumn: 'span 2' }}>
                                            <div className="card-actions">
                                                <button className="delete-btn" onClick={() => handleRemoveSyllabus(syl.id)}>×</button>
                                            </div>
                                            <h4>{syl.name}</h4>
                                            <p style={{ display: '-webkit-box', WebkitLineClamp: 3, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>
                                                {/* In a real app we might fetch content on demand if it's large, but Service returns it here */}
                                                {(fullSubject as any).syllabuses.find((s: any) => s.id === syl.id)?.content || "No preview available."}
                                            </p>
                                            {/* {syl.metadata && <p style={{ fontSize: '0.8rem', color: '#999', marginTop: '0.5rem' }}>Meta: {syl.metadata}</p>} */}
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </>
                    )}
                </main>
            </div>

            {/* ================= MODALS ================= */}

            {/* Subject Modal */}
            <Modal title="New Subject" isOpen={modals.subject} onClose={() => toggleModal('subject', false)}>
                <form onSubmit={handleAddSubject}>
                    <div className="form-group">
                        <label>Subject Name</label>
                        <input className="form-input" value={formSubject.name} onChange={e => setFormSubject({ ...formSubject, name: e.target.value })} required />
                    </div>
                    <div className="form-group">
                        <label>Grade (0-100)</label>
                        <input className="form-input" type="number" value={formSubject.grade || ''} onChange={e => setFormSubject({ ...formSubject, grade: parseInt(e.target.value) })} />
                    </div>
                    <div className="form-group">
                        <label>Metadata</label>
                        <input className="form-input" value={formSubject.metadata || ''} onChange={e => setFormSubject({ ...formSubject, metadata: e.target.value })} />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => toggleModal('subject', false)}>Cancel</button>
                        <button type="submit" className="btn btn-primary">Save Subject</button>
                    </div>
                </form>
            </Modal>

            {/* Lecturer Modal */}
            <Modal title="New Lecturer" isOpen={modals.lecturer} onClose={() => toggleModal('lecturer', false)}>
                <form onSubmit={handleAddLecturer}>
                    <div className="form-group">
                        <label>Name</label>
                        <input className="form-input" value={formLecturer.name} onChange={e => setFormLecturer({ name: e.target.value })} required />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => toggleModal('lecturer', false)}>Cancel</button>
                        <button type="submit" className="btn btn-primary">Add Lecturer</button>
                    </div>
                </form>
            </Modal>

            {/* Schedule Modal */}
            <Modal title="New Schedule" isOpen={modals.schedule} onClose={() => toggleModal('schedule', false)}>
                <form onSubmit={handleAddSchedule}>
                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                        <div className="form-group">
                            <label>Start</label>
                            <input type="datetime-local" className="form-input" value={formSchedule.start} onChange={e => setFormSchedule({ ...formSchedule, start: e.target.value })} required />
                        </div>
                        <div className="form-group">
                            <label>End</label>
                            <input type="datetime-local" className="form-input" value={formSchedule.end} onChange={e => setFormSchedule({ ...formSchedule, end: e.target.value })} required />
                        </div>
                    </div>
                    <div className="form-group">
                        <label>Location</label>
                        <input className="form-input" value={formSchedule.location} onChange={e => setFormSchedule({ ...formSchedule, location: e.target.value })} placeholder="Room 101 / Zoom Link" />
                    </div>
                    <div className="form-group">
                        <label>Metadata</label>
                        <input className="form-input" value={formSchedule.metadata} onChange={e => setFormSchedule({ ...formSchedule, metadata: e.target.value })} />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => toggleModal('schedule', false)}>Cancel</button>
                        <button type="submit" className="btn btn-primary">Save Schedule</button>
                    </div>
                </form>
            </Modal>

            {/* Syllabus Modal */}
            <Modal title="New Syllabus" isOpen={modals.syllabus} onClose={() => toggleModal('syllabus', false)}>
                <form onSubmit={handleAddSyllabus}>
                    <div className="form-group">
                        <label>Topic Name</label>
                        <input className="form-input" value={formSyllabus.name} onChange={e => setFormSyllabus({ ...formSyllabus, name: e.target.value })} required />
                    </div>
                    <div className="form-group">
                        <label>Content</label>
                        <textarea rows={5} className="form-input" value={formSyllabus.content} onChange={e => setFormSyllabus({ ...formSyllabus, content: e.target.value })} required />
                    </div>
                    <div className="form-group">
                        <label>Metadata</label>
                        <input className="form-input" value={formSyllabus.metadata || ''} onChange={e => setFormSyllabus({ ...formSyllabus, metadata: e.target.value })} />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => toggleModal('syllabus', false)}>Cancel</button>
                        <button type="submit" className="btn btn-primary">Save Syllabus</button>
                    </div>
                </form>
            </Modal>

            {/* Upload Modal */}
            <Modal title="Upload HTML" isOpen={modals.upload} onClose={() => toggleModal('upload', false)}>
                <form onSubmit={handleHtmlUpload}>
                    <div className="form-group">
                        <label>Raw HTML</label>
                        <textarea rows={8} className="form-input" value={htmlContent} onChange={e => setHtmlContent(e.target.value)} placeholder="Paste portal HTML here..." required />
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-outline" onClick={() => toggleModal('upload', false)}>Cancel</button>
                        <button type="submit" className="btn btn-primary">Upload & Parse</button>
                    </div>
                </form>
            </Modal>
        </>
    );
}
