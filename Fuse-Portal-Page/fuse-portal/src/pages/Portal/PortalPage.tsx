import { useEffect, useState, useRef } from 'react';
import PortalService from '../../services/PortalService';
import type { ScheduleDto, SubjectDto, SubjectFullDto } from '../../types/Portal';
import './PortalPage.css';
import { Link } from 'react-router-dom';

interface DayCell {
    date: Date;
    schedules: ScheduleDto[];
}

const WEEK_DAYS = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];
const COLORS = [
    '#FFB6C1', '#87CEFA', '#90EE90', '#FFA500',
    '#DA70D6', '#F08080', '#20B2AA', '#B0C4DE'
];

export default function PortalPage() {
    const [subjects, setSubjects] = useState<SubjectDto[]>([]);
    const [selectedSubject, setSelectedSubject] = useState<SubjectFullDto | null>(null);

    const containerRef = useRef<HTMLDivElement>(null);
    const detailsRef = useRef<HTMLDivElement>(null);

    const currentDate = new Date();
    const currentMonth = currentDate.getMonth();
    const currentYear = currentDate.getFullYear();

    // -------------------------
    // Load subjects
    // -------------------------
    useEffect(() => {
        const fetch = async () => {
            const list = await PortalService.getSubjects();
            setSubjects(list);
        };
        fetch();
        goToToday();
    }, []);

    // -------------------------
    // Month window (-6 to +5)
    // -------------------------
    const getVisibleMonths = () => {
        const months = [];
        for (let offset = -6; offset <= 5; offset++) {
            let m = currentMonth + offset;
            let y = currentYear;
            if (m < 0) { m += 12; y--; }
            if (m > 11) { m -= 12; y++; }
            months.push({ month: m, year: y });
        }
        return months;
    };

    // -------------------------
    // Check schedule within day
    // -------------------------
    const isSameDayOrInRange = (d: Date, sc: ScheduleDto) => {
        const start = new Date(sc.start);
        const end = new Date(sc.end);

        const dMid = new Date(d.getFullYear(), d.getMonth(), d.getDate());
        const sMid = new Date(start.getFullYear(), start.getMonth(), start.getDate());
        const eMid = new Date(end.getFullYear(), end.getMonth(), end.getDate());

        return dMid >= sMid && dMid <= eMid;
    };

    // -------------------------
    // Build calendar days
    // -------------------------
    const getDaysOfMonth = (month: number, year: number): DayCell[] => {
        const days: DayCell[] = [];
        const start = new Date(year, month, 1);
        const end = new Date(year, month + 1, 1);

        for (let d = new Date(start); d < end; d.setDate(d.getDate() + 1)) {
            days.push({
                date: new Date(d),
                schedules: subjects.flatMap(s =>
                    s.schedules.filter(sc => isSameDayOrInRange(d, sc))
                )
            });
        }
        return days;
    };

    const isToday = (d: Date) =>
        d.getDate() === currentDate.getDate() &&
        d.getMonth() === currentDate.getMonth() &&
        d.getFullYear() === currentDate.getFullYear();

    const getSubjectColor = (subjectId: number) =>
        COLORS[subjectId % COLORS.length];

    // -------------------------
    // Scroll controls
    // -------------------------
    const goToToday = () => {
        if (!containerRef.current) return;
        const monthWidth = 900;     // matches CSS
        const todayIndex = 6;       // center month
        containerRef.current.scrollLeft = todayIndex * monthWidth;
    };

    // -------------------------
    // Details section
    // -------------------------
    const onScheduleClick = async (subjectId: number) => {
        try {
            const full = await PortalService.getFullSubject(subjectId);
            setSelectedSubject(full);

            if (detailsRef.current) {
                detailsRef.current.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        } catch (err) {
            console.error('Failed to fetch full subject', err);
        }
    };

    const visibleMonths = getVisibleMonths();

    return (
        <div className="portal-page">

            {/* Header */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <h2>Calendar</h2>
                <button onClick={goToToday} className="goto-today-button">
                    Go to Today
                </button>
            </div>

            {/* Calendar */}
            <div className="calendar-container" ref={containerRef}>
                {visibleMonths.map(({ month, year }) => {
                    const days = getDaysOfMonth(month, year);

                    return (
                        <div key={`${year}-${month}`} className="calendar-month">
                            <h3>
                                {new Date(year, month).toLocaleString('default', {
                                    month: 'long',
                                    year: 'numeric'
                                })}
                            </h3>

                            <div className="calendar-header">
                                {WEEK_DAYS.map(d => (
                                    <div key={d} className="calendar-header-cell">{d}</div>
                                ))}
                            </div>

                            <div className="calendar-grid">
                                {days.map((day, idx) => (
                                    <div
                                        key={idx}
                                        className={`calendar-cell ${isToday(day.date) ? 'calendar-today' : ''}`}
                                    >
                                        <span className="calendar-day">{day.date.getDate()}</span>

                                        {day.schedules.map(sc => {
                                            const subj = subjects.find(s => s.id === sc.subjectId);
                                            if (!subj) return null;

                                            return (
                                                <div
                                                    key={sc.id}
                                                    className="calendar-schedule"
                                                    style={{ backgroundColor: getSubjectColor(subj.id) }}
                                                    title={subj.name}
                                                    onClick={() => onScheduleClick(subj.id)}
                                                >
                                                    {subj.name}
                                                </div>
                                            );
                                        })}
                                    </div>
                                ))}
                            </div>
                        </div>
                    );
                })}
            </div>

            {/* Details */}
            {selectedSubject && (
                <div ref={detailsRef} className="subject-details">
                    <h3>{selectedSubject.name}</h3>

                    {selectedSubject.grade !== undefined && (
                        <p><strong>Grade:</strong> {selectedSubject.grade}</p>
                    )}

                    <p><strong>Description:</strong> {selectedSubject.metadata || 'No description'}</p>
                    <p><strong>Lecturers:</strong> {selectedSubject.lecturers.map(l => l.name).join(', ')}</p>

                    <p><strong>Schedules:</strong></p>
                    <ul>
                        {selectedSubject.schedules.map(sc => (
                            <li key={sc.id}>
                                {new Date(sc.start).toLocaleString()} — {new Date(sc.end).toLocaleString()}
                            </li>
                        ))}
                    </ul>

                    <p><strong>Syllabuses:</strong></p>
                    <ul>
                        {selectedSubject.syllabuses.map(t => (
                            <li key={t.id}>
                                {t.name}
                            </li>
                        ))}
                    </ul>

                    {/* Button to open mock exam generator */}
                    <Link
                        to={`/tests/${selectedSubject.id}`}
                        className="test-yourself-button"
                    >
                        Test Yourself
                    </Link>

                </div>
            )}
        </div>
    );
}
