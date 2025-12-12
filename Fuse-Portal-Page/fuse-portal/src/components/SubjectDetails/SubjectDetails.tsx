
import { useEffect, useState } from 'react';
import PortalService from '../../services/PortalService';
import type { SubjectFullDto } from '../../types/Portal';

interface Props {
    subjectId: number;
    onLoaded?: () => void;
}

export default function SubjectDetails({ subjectId, onLoaded }: Props) {
    const [subject, setSubject] = useState<SubjectFullDto | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const run = async () => {
            try {
                const full = await PortalService.getFullSubject(subjectId);
                setSubject(full);
                setLoading(false);

                if (onLoaded) onLoaded();
            } catch (err) {
                console.error("Failed to load subject", err);
                setLoading(false);
            }
        };
        run();
    }, [subjectId]);

    if (loading) return <div className="subject-details">Loading...</div>;
    if (!subject) return <div className="subject-details">Failed to load subject.</div>;

    return (
        <div className="subject-details">
            <h3>{subject.name}</h3>
            {subject.grade !== undefined && (
                <p><strong>Grade:</strong> {subject.grade}</p>
            )}
            <p><strong>Description:</strong> {subject.metadata || 'No description'}</p>
            <p><strong>Lecturers:</strong> {subject.lecturers.map(l => l.name).join(', ')}</p>

            <p><strong>Schedules:</strong></p>
            <ul>
                {subject.schedules.map(sc => (
                    <li key={sc.id}>
                        {new Date(sc.start).toLocaleString()} — {new Date(sc.end).toLocaleString()}
                    </li>
                ))}
            </ul>
        </div>
    );
}
