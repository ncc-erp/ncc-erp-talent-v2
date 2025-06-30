import { ChartOptions } from "chart.js";

export function setOptionsStageChart(): ChartOptions {
  return {
    indexAxis: "y",
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: "bottom",
        labels: {
          usePointStyle: true,
          pointStyle: "rect",
          padding: 10,
          font: { size: 11 },
        },
      },
      tooltip: {
        callbacks: {
          label: (context) => {
            return `${context.dataset.label}: ${context.parsed.x.toFixed(1)}%`;
          },
        },
      },
    },
    scales: {
      x: {
        stacked: true,
        beginAtZero: true,
        max: 100,
        title: {
          display: true,
          text: "Tỷ trọng (%)",
        },
        ticks: {
          callback: function (value) {
            return value + "%";
          },
        },
      },
      y: {
        stacked: true,
        title: {
          display: true,
          text: "Giai đoạn tuyển dụng",
        },
      },
    },
  };
}

export function setOptionsCombinedChart(): ChartOptions {
  return {
    responsive: true,
    maintainAspectRatio: false,
    interaction: {
      mode: "index",
      intersect: false,
    },
    plugins: {
      legend: {
        display: true,
        position: "top",
        labels: {
          usePointStyle: true,
          pointStyle: "rect",
          padding: 15,
          font: {
            size: 12,
          },
        },
      },
      tooltip: {
        mode: "index",
        intersect: false,
        backgroundColor: "rgba(0,0,0,0.8)",
        titleColor: "#fff",
        bodyColor: "#fff",
        borderColor: "#ddd",
        borderWidth: 1,
        callbacks: {
          label: (context) => {
            const label = context.dataset.label || "";
            const value = context.parsed.y;
            return `${label}: ${value}`;
          },
          footer: (tooltipItems) => {
            let sum = 0;
            tooltipItems.forEach(function (tooltipItem) {
              sum += tooltipItem.parsed.y;
            });
            return `Total: ${sum}`;
          },
        },
      },
    },
    scales: {
      x: {
        display: true,
        title: {
          display: true,
          text: "Education Institution",
          font: {
            size: 14,
            weight: "bold",
          },
        },
        ticks: {
          maxRotation: 45,
          font: {
            size: 11,
          },
        },
        stacked: true,
      },
      y: {
        type: "linear",
        display: true,
        title: {
          display: true,
          text: "Number of Candidates",
          font: {
            size: 14,
            weight: "bold",
          },
          color: "#333",
        },
        beginAtZero: true,
        stacked: true,
        grid: {
          color: "rgba(0,0,0,0.1)",
        },
        ticks: {
          font: {
            size: 11,
          },
          stepSize: 1,
        },
      },
    },
    elements: {
      bar: {
        borderRadius: 2,
        borderSkipped: false,
      },
    },
  };
}
