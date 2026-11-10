import { Component, Input, OnChanges, OnDestroy } from '@angular/core';
import { EnglishInterviewResult } from '@app/core/models/candidate/english-interview.model';
import { EnglishInterviewService } from '@app/core/services/candidate/english-interview.service';
import { Subscription } from 'rxjs';

interface ReportEntry {
  label: string;
  value: string;
}

interface ReportSection {
  title: string;
  entries: ReportEntry[];
}

interface InterviewView extends EnglishInterviewResult {
  scores: { label: string; value: number | null }[];
  speakingMetrics: { label: string; value: string }[];
  sections: ReportSection[];
}

@Component({
  selector: 'talent-english-interview',
  templateUrl: './english-interview.component.html',
  styleUrls: ['./english-interview.component.scss']
})
export class EnglishInterviewComponent implements OnChanges, OnDestroy {
  @Input() candidateId: number;

  result: InterviewView = null;
  isLoading = false;
  loadFailed = false;
  private request: Subscription;

  constructor(private englishInterviewService: EnglishInterviewService) { }

  ngOnChanges(): void {
    this.loadResults();
  }

  ngOnDestroy(): void {
    this.request?.unsubscribe();
  }

  loadResults(): void {
    this.request?.unsubscribe();
    this.result = null;
    this.loadFailed = false;
    if (!this.candidateId) {
      return;
    }

    this.request = this.englishInterviewService.getByCVId(this.candidateId).subscribe(response => {
      this.isLoading = response.loading;
      if (response.loading) {
        return;
      }
      this.loadFailed = !response.success;
      const result = response.result?.[0];
      this.result = result ? {
        ...result,
        scores: [
          { label: 'Pronunciation', value: result.pronunciationScore },
          { label: 'Fluency', value: result.fluencyScore },
          { label: 'Vocabulary', value: result.vocabularyScore },
          { label: 'Grammar', value: result.grammarScore },
          { label: 'Domain', value: result.domainScore }
        ],
        speakingMetrics: this.speakingMetrics(result.report),
        sections: this.reportSections(result.report)
      } : null;
    });
  }

  private reportSections(report: EnglishInterviewResult['report']): InterviewView['sections'] {
    return ['key_strengths', 'areas_to_improve']
      .map(key => ({ title: this.label(key), entries: this.reportEntries(report?.[key]) }))
      .filter(section => section.entries.length > 0);
  }

  private speakingMetrics(report: EnglishInterviewResult['report']): InterviewView['speakingMetrics'] {
    return [
      { key: 'average_wpm', label: 'Speaking rate', suffix: ' words/min' },
      { key: 'total_fillers', label: 'Filler words', suffix: '' }
    ].filter(metric => report?.[metric.key] != null && report[metric.key] !== '')
      .map(metric => ({ label: metric.label, value: String(report[metric.key]) + metric.suffix }));
  }

  private isInternalField(key: string): boolean {
    const normalized = key.replace(/([a-z0-9])([A-Z])/g, '$1_$2').toLowerCase();
    return /(^|_)ids?$/.test(normalized) || normalized === 'words_breakdown';
  }

  private reportEntries(value: unknown, label = ''): ReportEntry[] {
    if (value == null || value === '') {
      return [];
    }
    if (Array.isArray(value)) {
      return value.reduce((entries, item) => entries.concat(this.reportEntries(item, label)), [] as ReportEntry[]);
    }
    if (typeof value === 'object') {
      return Object.keys(value).filter(key => !this.isInternalField(key)).reduce((entries, key) => entries.concat(
        this.reportEntries(value[key], [label, this.label(key)].filter(Boolean).join(' / '))
      ), [] as ReportEntry[]);
    }
    return [{ label, value: String(value) }];
  }

  private label(key: string): string {
    const labels = {
      key_strengths: 'Key strengths',
      areas_to_improve: 'Areas to improve'
    };
    const text = key.replace(/_/g, ' ');
    return labels[key] || text.charAt(0).toUpperCase() + text.slice(1);
  }
}
