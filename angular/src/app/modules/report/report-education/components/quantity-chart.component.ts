import { Component, Input, OnInit, OnChanges, SimpleChanges, ViewChild, AfterViewInit } from '@angular/core';
import { ChartOptions } from 'chart.js';

@Component({
  selector: 'quantity-chart',
  template: `
    <div class="quantity-chart-section mt-4" *ngIf="chartData?.length > 0">
      <div class="section-header mb-4">
        <h3 class="section-title">
          <i class="pi pi-chart-bar mr-2"></i>
          {{ "The quantity of candidates from each university by recruitment status." | localize }}
        </h3>
        <p class="section-description text-muted">
          {{ "Analysis of the number of candidates from each university in each recruitment status." | localize }}
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
                [height]="400">
              </p-chart>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .quantity-chart-section {
      .section-header {
        .section-title {
          color: #495057;
          font-size: 1.5rem;
          font-weight: 600;
          margin-bottom: 8px;
          .pi { color: #007bff; }
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
      }
    }
  `]
})
export class QuantityChartComponent implements OnInit, OnChanges, AfterViewInit {
  @Input() chartData: any[] = [];
  @Input() filterEnabled: boolean = false;
  @ViewChild('chartRef') chartRef: any;

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
    // Ensure chart is ready before applying filter
    setTimeout(() => {
      if (this.chartRef?.chart) {
        this.chartInstance = this.chartRef.chart;
        if (this.filterEnabled) {
          this.applyCurrentFilterState();
        }
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['filterEnabled'] && !changes['filterEnabled'].firstChange) {
      if (this.chartRef?.chart) {
        this.chartInstance = this.chartRef.chart;
        this.applyCurrentFilterState();
      }
    }

    if (changes['chartData'] && this.chartData?.length > 0) {
      this.originalChartData = JSON.parse(JSON.stringify(this.chartData[0].dataChart));
      this.filteredChartData = JSON.parse(JSON.stringify(this.chartData[0].dataChart));
      this.setChartOptions();
    }
  }

  private applyCurrentFilterState(): void {
    if (!this.chartInstance || !this.originalChartData) return;

    const hiddenStates = this.chartInstance.data.datasets.map((_, i) => {
      return this.chartInstance.getDatasetMeta(i).hidden;
    });

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
      const newLabels = allLabels.filter((_, labelIdx) => {
        return activeDatasets.some(ds => ds.data[labelIdx] !== 0);
      });

      const finalLabels = newLabels.length > 0 ? newLabels : allLabels;

      this.chartInstance.data.labels = finalLabels;
      this.chartInstance.data.datasets.forEach((ds, i) => {
        const original = this.originalChartData.datasets[i];
        ds.data = finalLabels.map(label => {
          const index = allLabels.indexOf(label);
          return index >= 0 ? original.data[index] : 0;
        });
      });
    }

    this.chartInstance.update();
  }

  private setChartOptions(): void {
    this.chartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'top',
          labels: {
            usePointStyle: true,
            pointStyle: 'rect',
            font: { size: 12 }
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

            this.applyCurrentFilterState();
          }
        },
        tooltip: {
          mode: 'index',
          intersect: false,
          backgroundColor: 'rgba(0,0,0,0.8)',
          titleColor: '#fff',
          bodyColor: '#fff',
          borderColor: '#ddd',
          borderWidth: 1,
          callbacks: {
            label: (context) => `${context.dataset.label}: ${context.parsed.y}`,
            footer: (items) => {
              const sum = items.reduce((acc, item) => acc + item.parsed.y, 0);
              return `Total: ${sum}`;
            }
          }
        }
      },
      scales: {
        x: {
          stacked: true,
          title: {
            display: true,
            text: 'Education Institution',
            font: { size: 14, weight: 'bold' }
          },
          ticks: { font: { size: 11 } }
        },
        y: {
          stacked: true,
          beginAtZero: true,
          title: {
            display: true,
            text: 'Number of Candidates',
            font: { size: 14, weight: 'bold' }
          },
          ticks: { stepSize: 1 }
        }
      }
    };
  }
}
