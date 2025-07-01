// percentage-chart.component.ts
import { Component, Input, OnInit } from '@angular/core';
import { ChartOptions } from 'chart.js';

@Component({
  selector: 'percentage-chart',
  template: `
    <div class="percentage-chart-section mt-4" *ngIf="chartData.length > 0">
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
        <div class="col-12" *ngFor="let item of chartData">
          <div class="chart-container mb-4">
            <div class="chart-wrapper">
              <p-chart
                type="bar"
                [data]="item.dataChart"
                [options]="chartOptions"
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
          
          .pi {
            color: #6f42c1;
          }
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
        margin-bottom: 20px;
        
        .chart-header {
          padding: 15px 20px;
          border-bottom: 1px solid #dee2e6;
          
          .chart-title {
            font-size: 1.1rem;
            font-weight: 600;
            margin: 0;
          }
        }
        
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
export class PercentageChartComponent implements OnInit {
  @Input() chartData: any[] = [];
  
  chartOptions: ChartOptions = {};
  
  ngOnInit(): void {
    this.setChartOptions();
  }
  
  private setChartOptions(): void {
    this.chartOptions = {
      indexAxis: 'y',
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: true,
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
        },
        tooltip: {
          callbacks: {
            label: (context) => {
              return `${context.dataset.label}: ${context.parsed.x.toFixed(1)}%`;
            }
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
            callback: function(value) {
              return value + '%';
            }
          }
        },
        y: {
          stacked: true,
          title: {
            display: true,
            text: 'Recruitment status',
            font: { size: 14, weight: 'bold' }
          }
        }
      }
    };
  }
}