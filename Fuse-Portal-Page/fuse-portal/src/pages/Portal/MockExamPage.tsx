import { useState, useEffect, useRef } from 'react';
import PortalService from '../../services/PortalService';
import ChatService from '../../services/ChatService';
import type {
    SubjectDto,
    SubjectFullDto,
    SyllabusDto,
    ExamDto,
    SyllabusRequestDto
} from '../../types/Portal';
import './MockExamPage.css'

// ==========================================
//  MAIN COMPONENT
// ==========================================

export default function MockExamPage() {
    // --- State ---
    const [step, setStep] = useState<1 | 2 | 3 | 4>(1); // 1: Select Subject, 2: Select Syllabus, 3: Exam, 4: Results
    const [isLoading, setIsLoading] = useState(false);

    // Data
    const [subjects, setSubjects] = useState<SubjectDto[]>([]);
    const [selectedSubject, setSelectedSubject] = useState<SubjectFullDto | null>(null);
    const [selectedSyllabus, setSelectedSyllabus] = useState<SyllabusDto | null>(null);

    // File Upload
    const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
    const [isUploading, setIsUploading] = useState(false);
    const fileInputRef = useRef<HTMLInputElement>(null);

    // Exam Session
    const [examData, setExamData] = useState<ExamDto | null>(null);
    const [userAnswer, setUserAnswer] = useState("");

    // --- Effects ---
    useEffect(() => {
        loadSubjects();
    }, []);

    const loadSubjects = async () => {
        setIsLoading(true);
        try {
            const data = await PortalService.getSubjects(undefined, 100);
            setSubjects(data);
        } catch (e) {
            console.error("Failed to load subjects", e);
        } finally {
            setIsLoading(false);
        }
    };

    // --- Handlers ---

    const handleSubjectSelect = async (subjectId: number) => {
        setIsLoading(true);
        try {
            const fullData = await PortalService.getFullSubject(subjectId);
            setSelectedSubject(fullData);
            setStep(2);
        } catch (e) {
            alert("Could not load subject details.");
        } finally {
            setIsLoading(false);
        }
    };

    const handleSyllabusSelect = async (syllabus: SyllabusDto) => {
        setSelectedSyllabus(syllabus);
    };

    const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files) {
            const filesArray = Array.from(e.target.files);
            setSelectedFiles(prev => [...prev, ...filesArray]);
        }
    };

    const handleRemoveFile = (index: number) => {
        setSelectedFiles(prev => prev.filter((_, i) => i !== index));
    };

    const handleUploadFiles = async () => {
        if (selectedFiles.length === 0 || !selectedSubject) return;

        setIsUploading(true);
        try {
            // Upload files and get file IDs
            const fileIds = await ChatService.uploadFiles(selectedFiles);

            // For each file, fetch its content and create a syllabus
            for (let i = 0; i < fileIds.length; i++) {
                const fileId = fileIds[i];
                const fileName = selectedFiles[i].name;

                try {
                    // Get file content
                    const fileDto = await ChatService.getFile(fileId);

                    // Create syllabus from file
                    const syllabusRequest: SyllabusRequestDto = {
                        name: fileName.replace(/\.[^/.]+$/, ''), // Remove file extension
                        content: fileDto.text,
                        subjectId: selectedSubject.id
                    };

                    const newSyllabus = await PortalService.addSyllabus(syllabusRequest);

                    // Update selected subject with new syllabus
                    setSelectedSubject(prev => {
                        if (!prev) return prev;
                        return {
                            ...prev,
                            syllabuses: [...prev.syllabuses, newSyllabus]
                        };
                    });
                } catch (err) {
                    console.error(`Failed to process file ${fileName}:`, err);
                    alert(`Failed to upload syllabus from ${fileName}`);
                }
            }

            // Clear selected files
            setSelectedFiles([]);
            if (fileInputRef.current) {
                fileInputRef.current.value = '';
            }

            alert('Syllabus files uploaded successfully!');
        } catch (err) {
            console.error('Failed to upload files:', err);
            alert('Failed to upload files. Please try again.');
        } finally {
            setIsUploading(false);
        }
    };

    const startExam = async () => {
        if (!selectedSyllabus) return;
        setIsLoading(true);
        try {
            const exam = await PortalService.generateMockExam(selectedSyllabus.id);
            setExamData(exam);
            setUserAnswer(""); // Reset answer
            setStep(3);
        } catch (e) {
            alert("Failed to generate mock exam. Please try again.");
        } finally {
            setIsLoading(false);
        }
    };

    const submitExam = async () => {
        if (!examData) return;

        // Confirm submission
        if (!window.confirm("Are you sure you want to submit your answers?")) return;

        setIsLoading(true);
        try {
            const payload: ExamDto = {
                ...examData,
                answers: userAnswer
            };

            const result = await PortalService.checkExamAnswers(payload);
            setExamData(result);
            setStep(4);
        } catch (e) {
            alert("Failed to submit exam.");
        } finally {
            setIsLoading(false);
        }
    };

    const resetFlow = () => {
        setStep(1);
        setSelectedSubject(null);
        setSelectedSyllabus(null);
        setExamData(null);
        setUserAnswer("");
        setSelectedFiles([]);
        if (fileInputRef.current) {
            fileInputRef.current.value = '';
        }
    };

    // --- Render Helpers ---

    const renderStep1 = () => (
        <div className="animate-fade">
            <div className="step-indicator">Step 1: Choose a Subject</div>
            {isLoading ? <div className="loader" /> : (
                <div className="selection-grid">
                    {subjects.map(sub => (
                        <div key={sub.id} className="selection-card" onClick={() => handleSubjectSelect(sub.id)}>
                            <div>
                                <h3>{sub.name}</h3>
                                <p>{sub.schedules?.length || 0} Schedules Active</p>
                            </div>
                            <span className="selection-badge">ID: #{sub.id}</span>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );

    const renderStep2 = () => (
        <div className="animate-fade">
            <div className="exam-header-no-border">
                <div className="step-indicator-inline">Step 2: Choose Syllabus</div>
                <button className="btn btn-text" onClick={() => setStep(1)}>Change Subject</button>
            </div>

            <h2 className="subject-title">Subject: {selectedSubject?.name}</h2>

            {/* File Upload Section */}
            <div className="file-upload-section">
                <h3 className="upload-heading">Upload New Syllabus Files</h3>
                <p className="upload-description">
                    Upload PDF, Word documents, or text files containing syllabus content
                </p>

                <div className="file-upload-controls">
                    <input
                        ref={fileInputRef}
                        type="file"
                        multiple
                        accept=".pdf,.doc,.docx,.txt"
                        onChange={handleFileSelect}
                        className="file-input-hidden"
                        id="file-upload"
                    />
                    <label htmlFor="file-upload" className="btn btn-secondary">
                        📎 Choose Files
                    </label>

                    {selectedFiles.length > 0 && (
                        <button
                            className={`btn btn-primary ${isUploading ? 'btn-disabled' : ''}`}
                            onClick={handleUploadFiles}
                            disabled={isUploading}
                        >
                            {isUploading ? 'Uploading...' : `⬆️ Upload ${selectedFiles.length} File${selectedFiles.length > 1 ? 's' : ''}`}
                        </button>
                    )}
                </div>

                {selectedFiles.length > 0 && (
                    <div className="selected-files-list">
                        {selectedFiles.map((file, index) => (
                            <div key={index} className="file-item">
                                <span className="file-name">📄 {file.name}</span>
                                <button
                                    className="btn-remove-file"
                                    onClick={() => handleRemoveFile(index)}
                                    title="Remove file"
                                >
                                    ✕
                                </button>
                            </div>
                        ))}
                    </div>
                )}
            </div>

            {/* Existing Syllabuses */}
            <h3 className="existing-syllabuses-heading">Select Existing Syllabus</h3>

            <div className="selection-grid">
                {selectedSubject?.syllabuses.length === 0 && <p>No syllabus found for this subject.</p>}

                {selectedSubject?.syllabuses.map(syl => (
                    <div
                        key={syl.id}
                        className={`selection-card ${selectedSyllabus?.id === syl.id ? 'active' : ''}`}
                        onClick={() => handleSyllabusSelect(syl)}
                    >
                        <h3>{syl.name}</h3>
                        <p className="syllabus-card-description">Click to select this topic for your exam.</p>
                    </div>
                ))}
            </div>

            <div className="start-exam-container">
                <button
                    className={`btn btn-primary ${!selectedSyllabus || isLoading ? 'btn-disabled' : ''}`}
                    onClick={startExam}
                    disabled={!selectedSyllabus || isLoading}
                >
                    {isLoading ? 'Generating...' : '🚀 Generate Mock Exam'}
                </button>
            </div>
        </div>
    );

    const renderStep3 = () => (
        <div className="animate-fade">
            <div className="exam-header-no-border">
                <div className="step-indicator-inline">Step 3: Examination</div>
                <span className="exam-time-label">Time: Unlimited</span>
            </div>

            <div className="paper">
                <h2 className="paper-title">Mock Exam: {selectedSyllabus?.name}</h2>
                <p className="paper-description">Read the questions carefully and type your answers below.</p>

                {/* Question Display */}
                <div className="question-block">
                    {examData?.questions}
                </div>

                {/* Answer Input */}
                <h4 className="answer-label">Your Answer:</h4>
                <textarea
                    className="answer-area"
                    value={userAnswer}
                    onChange={(e) => setUserAnswer(e.target.value)}
                    placeholder="Type your answer here..."
                    spellCheck={false}
                />

                <div className="submit-button-container">
                    <button
                        className={`btn btn-primary ${isLoading ? 'btn-disabled' : ''}`}
                        onClick={submitExam}
                        disabled={isLoading}
                    >
                        {isLoading ? 'Checking...' : '✅ Submit & Check Results'}
                    </button>
                </div>
            </div>
        </div>
    );

    const renderStep4 = () => (
        <div className="animate-fade">
            <div className="step-indicator">Step 4: Results</div>

            <div className="result-banner">
                <div className="grade-circle" style={{
                    backgroundColor: (examData?.grade || 0) >= 50 ? 'var(--success-color)' : '#d32f2f'
                }}>
                    {examData?.grade ?? '?'}
                </div>
                <h2>{(examData?.grade || 0) >= 50 ? 'Well Done!' : 'Needs Improvement'}</h2>
                <p>You scored {examData?.grade} out of {examData?.scoreFrom100 || 100}</p>
            </div>

            <div className="paper">
                <h3>Detailed Feedback</h3>
                <div className="question-block question-block-gray">
                    <strong>Original Questions:</strong><br />
                    {examData?.questions}
                </div>

                <div className="question-block">
                    <strong>Your Answer:</strong><br />
                    {examData?.answers}
                </div>

                <div className="feedback-text">
                    <strong>AI Evaluation:</strong><br />
                    {examData?.results}
                </div>

                <div className="reset-button-container">
                    <button className="btn btn-primary" onClick={resetFlow}>
                        🔄 Take Another Exam
                    </button>
                </div>
            </div>
        </div>
    );

    // --- Main Render ---
    return (
        <div className="exam-container">

            {/* Top Navigation */}
            <div className="exam-header">
                <div className="brand">
                    <span>📝</span> MockExam
                </div>
                <a href="/" className="back-link">← Back to Dashboard</a>
            </div>

            {/* Content Switcher */}
            {step === 1 && renderStep1()}
            {step === 2 && renderStep2()}
            {step === 3 && renderStep3()}
            {step === 4 && renderStep4()}

        </div>
    );
}
