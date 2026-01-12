import { AfterViewInit, Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { ChartOptions } from 'chart.js';
import * as pluginDataLabels from 'chartjs-plugin-datalabels';

@Component({
  selector: 'percentage-chart',
  template: `
    <div class="percentage-chart-section mt-4" *ngIf="chartData?.length > 0">
      <div class="section-header mb-4">
        <h3 class="section-title">
          <i class="pi pi-chart-line mr-2"></i>
          {{ "The proportion of candidates from each university by recruitment status." | localize }}
        </h3>
        <p class="section-description text-muted">
          {{ "The percentage of each university in the total number of CVs (100%)." | localize }}
        </p>
      </div>

      <div class="row">
        <div class="col-12">
          <div class="chart-container mb-4">
            <div class="chart-wrapper">
              <p-chart
                #chartRef
                type="bar"
                [data]="filteredChartData"
                [options]="chartOptions"
                [plugins]="chartPlugins"
                [height]="400">
              </p-chart>
            </div>
            <div class="chart-note mt-2">
              <small class="text-muted">
                <i class="pi pi-info-circle mr-1"></i>
                {{ "Each bar represents 100%, showing the proportion of each university." | localize }}
              </small>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .percentage-chart-section {
      .section-header {
        .section-title {
          color: #495057;
          font-size: 1.5rem;
          font-weight: 600;
          margin-bottom: 8px;
          .pi { color: #6f42c1; }
        }
        .section-description {
          font-size: 0.9rem;
          margin-bottom: 0;
        }
      }
      .chart-container {
        background: #fff;
        border-radius: 8px;
        border: 1px solid #dee2e6;
        .chart-wrapper {
          padding: 20px;
        }
        .chart-note {
          padding: 10px 20px;
          background: #f8f9fa;
          border-top: 1px solid #dee2e6;
          
          small {
            display: flex;
            align-items: center;
          }
        }
      }
    }
  `]
})
export class PercentageChartComponent implements OnInit, OnChanges, AfterViewInit {
  @Input() chartData: any[] = [];
  @Input() filterEnabled: boolean = false;
  @ViewChild('chartRef') chartRef: any;

  public chartPlugins = [pluginDataLabels.default];
  chartOptions: ChartOptions = {};
  filteredChartData: any;
  originalChartData: any;
  chartInstance: any;

  ngOnInit(): void {
    if (this.chartData?.length > 0) {
      this.originalChartData = JSON.parse(JSON.stringify(this.chartData[0].dataChart));
      this.filteredChartData = JSON.parse(JSON.stringify(this.chartData[0].dataChart));
      this.setChartOptions();
    }
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      if (this.chartRef?.chart) {
        this.chartInstance = this.chartRef.chart;
        if (this.filterEnabled) {
          this.applyFilter();
        }
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['filterEnabled'] && !changes['filterEnabled'].firstChange) {
      if (this.chartRef?.chart) {
        this.chartInstance = this.chartRef.chart;
        this.applyFilter();
      }
    }

    if (changes['chartData'] && this.chartData?.length > 0) {
      this.originalChartData = JSON.parse(JSON.stringify(this.chartData[0].dataChart));
      this.filteredChartData = JSON.parse(JSON.stringify(this.chartData[0].dataChart));
      this.setChartOptions();
    }
  }

  private applyFilter(): void {
    if (!this.chartInstance || !this.originalChartData) return;

    const hiddenStates = this.chartInstance.data.datasets.map((_, i) =>
      this.chartInstance.getDatasetMeta(i).hidden
    );

    const visibleDatasetIndexes = hiddenStates
      .map((hidden, i) => ({ i, hidden }))
      .filter(ds => !ds.hidden)
      .map(ds => ds.i);

    const allLabels = this.originalChartData.labels;

    if (!this.filterEnabled || visibleDatasetIndexes.length === this.originalChartData.datasets.length) {
      this.chartInstance.data.labels = [...allLabels];
      this.chartInstance.data.datasets.forEach((ds, i) => {
        ds.data = [...this.originalChartData.datasets[i].data];
      });
    } else if (visibleDatasetIndexes.length === 0) {
      this.chartInstance.data.labels = [...allLabels];
      this.chartInstance.data.datasets.forEach(ds => {
        ds.data = new Array(allLabels.length).fill(0);
      });
    } else {
      const activeDatasets = visibleDatasetIndexes.map(i => this.originalChartData.datasets[i]);
      const newLabels = allLabels.filter((_, labelIdx) =>
        activeDatasets.some(ds => ds.data[labelIdx] !== 0)
      );

      const finalLabels = newLabels.length > 0 ? newLabels : allLabels;

      this.chartInstance.data.labels = finalLabels;
      this.chartInstance.data.datasets.forEach((ds, i) => {
        const original = this.originalChartData.datasets[i];
        ds.data = finalLabels.map(label => {
          const idx = allLabels.indexOf(label);
          return idx >= 0 ? original.data[idx] : 0;
        });
      });
    }

    this.chartInstance.update();
  }

  private setChartOptions(): void {
    this.chartOptions = {
      indexAxis: 'y',
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom',
          labels: {
            usePointStyle: true,
            pointStyle: 'rect',
            padding: 10,
            font: { size: 11 }
          },
          onHover: (event, legendItem, legend) => {
            legend.chart.canvas.style.cursor = 'pointer';
          },
          onLeave: (event, legendItem, legend) => {
            legend.chart.canvas.style.cursor = 'default';
          },
          onClick: (e, legendItem, legend) => {
            const chart = legend.chart;
            const index = legendItem.datasetIndex;
            const meta = chart.getDatasetMeta(index);
            meta.hidden = meta.hidden === null ? !chart.data.datasets[index].hidden : null;

            if (!this.filterEnabled) {
              chart.update();
              return;
            }

            this.applyFilter();
          }
        },
        tooltip: {
          callbacks: {
            label: (context) => `${context.dataset.label}: ${context.parsed.x.toFixed(1)}%`
          }
        },
        datalabels: {
          formatter: (value, ctx) => {
            if (value <= 0) return "";
            // Access counts from the dataset (this property was added in mapToPercentageChart)
            // Note: ctx.dataset is typed as ChartDataSets which might not have 'counts' property in standard types
            const dataset: any = ctx.dataset;
            const count = dataset.counts ? dataset.counts[ctx.dataIndex] : 0;
            return value.toFixed(1) + '% (' + count + ')';
          },
          color: '#fff',
          font: { 
            size: 11 
          }
        }
      },
      scales: {
        x: {
          stacked: true,
          beginAtZero: true,
          max: 100,
          title: {
            display: true,
            text: 'Proportion (%)',
            font: { size: 14, weight: 'bold' }
          },
          ticks: {
            callback: (value) => value + '%'
          }
        },
        y: {
          stacked: true,
          title: {
            display: true,
            text: 'Recruitment Status',
            font: { size: 14, weight: 'bold' }
          }
        }
      }
    };
  }
}
