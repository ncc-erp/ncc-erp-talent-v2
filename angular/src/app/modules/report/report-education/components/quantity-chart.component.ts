import { Component, Input, OnInit } from '@angular/core';
import { ChartOptions } from 'chart.js';

@Component({
  selector: 'quantity-chart',
  template: `
    <div class="quantity-chart-section mt-4" *ngIf="chartData.length > 0">
      <div class="section-header mb-4">
        <h3 class="section-title">
          <i class="pi pi-chart-bar mr-2"></i>
          {{ "Recruitment status analysis" | localize }}
        </h3>
        <p class="section-description text-muted">
          {{ "Analysis of the number of candidates from each university in each recruitment status." | localize }}
        </p>
      </div>
      
      <div class="row">
        <div class="col-12" *ngFor="let item of chartData">
          <div class="chart-container mb-4">
            <div class="chart-header">
              <h4 class="chart-title">
                {{ item.branchName }}
              </h4>
            </div>
            
            <div class="chart-wrapper">
              <p-chart
                type="bar"
                [data]="item.dataChart"
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
          
          .pi {
            color: #007bff;
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
      }
    }
  `]
})
export class QuantityChartComponent implements OnInit {
  @Input() chartData: any[] = [];
  
  chartOptions: ChartOptions = {};
  
  ngOnInit(): void {
    this.setChartOptions();
  }
  
  private setChartOptions(): void {
    this.chartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      interaction: {
        mode: 'index',
        intersect: false,
      },
      plugins: {
        legend: {
          display: true,
          position: 'top',
          labels: {
            usePointStyle: true,
            pointStyle: 'rect',
            padding: 15,
            font: { size: 12 }
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
            label: (context) => {
              const label = context.dataset.label || '';
              const value = context.parsed.y;
              return `${label}: ${value}`;
            },
            footer: (tooltipItems) => {
              let sum = 0;
              tooltipItems.forEach(function(tooltipItem) {
                sum += tooltipItem.parsed.y;
              });
              return `Total: ${sum}`;
            }
          }
        }
      },
      scales: {
        x: {
          display: true,
          title: {
            display: true,
            text: 'Education Institution',
            font: { size: 14, weight: 'bold' }
          },
          ticks: {
            maxRotation: 45,
            font: { size: 11 }
          },
          stacked: true
        },
        y: {
          type: 'linear',
          display: true,
          title: {
            display: true,
            text: 'Number of Candidates',
            font: { size: 14, weight: 'bold' },
            color: '#333'
          },
          beginAtZero: true,
          stacked: true,
          grid: { color: 'rgba(0,0,0,0.1)' },
          ticks: {
            font: { size: 11 },
            stepSize: 1
          }
        }
      },
      elements: {
        bar: {
          borderRadius: 2,
          borderSkipped: false
        }
      }
    };
  }
}