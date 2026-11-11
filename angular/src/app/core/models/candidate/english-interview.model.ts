export interface EnglishInterviewResult {
  id: number;
  sessionId: string;
  scenarioTitle: string;
  coachName: string;
  interviewedAt: string | null;
  durationSeconds: number | null;
  overallScore: number | null;
  cefrLevel: string;
  pronunciationScore: number | null;
  fluencyScore: number | null;
  vocabularyScore: number | null;
  grammarScore: number | null;
  domainScore: number | null;
  ieltsBand: number | null;
  toeflScore: number | null;
  report: { [key: string]: unknown } | null;
}
