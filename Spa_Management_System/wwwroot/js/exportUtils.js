// =====================================================
// SPA MANAGEMENT SYSTEM - EXPORT UTILITIES
// Print, PDF, Excel export functionality
// =====================================================

// Global function for downloading files (used by PDF export)
window.downloadFile = function (filename, contentType, base64Content) {
  const link = document.createElement("a");
  link.download = filename;
  link.href = `data:${contentType};base64,${base64Content}`;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

window.exportUtils = {
  // Download file from base64 content (for PDF exports from C#)
  downloadFile: function (base64Content, filename, contentType) {
    const link = document.createElement("a");
    link.download = filename;
    link.href = `data:${contentType};base64,${base64Content}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  },

  // Print current view using iframe (works in MAUI WebView2)
  printElement: function (elementId, title) {
    var element = document.getElementById(elementId);
    if (!element) {
      console.error("Element not found:", elementId);
      alert("Content not found for printing");
      return;
    }

    // Create a hidden iframe for printing
    var iframe = document.createElement("iframe");
    iframe.style.position = "absolute";
    iframe.style.width = "0";
    iframe.style.height = "0";
    iframe.style.border = "none";
    iframe.style.left = "-9999px";
    document.body.appendChild(iframe);

    var doc = iframe.contentWindow.document;
    doc.open();
    doc.write(`
            <!DOCTYPE html>
            <html>
            <head>
                <title>${title || "Kaye Spa - Report"}</title>
                <style>
                    * { margin: 0; padding: 0; box-sizing: border-box; }
                    body { 
                        font-family: 'Segoe UI', Arial, sans-serif; 
                        padding: 20px; 
                        color: #333;
                    }
                    .print-header {
                        text-align: center;
                        margin-bottom: 20px;
                        padding-bottom: 10px;
                        border-bottom: 2px solid #454F4A;
                    }
                    .print-header h1 { color: #454F4A; font-size: 24px; margin-bottom: 5px; }
                    .print-header p { color: #666; font-size: 12px; }
                    table { 
                        width: 100%; 
                        border-collapse: collapse; 
                        margin-top: 10px;
                    }
                    th, td { 
                        border: 1px solid #ddd; 
                        padding: 8px; 
                        text-align: left; 
                        font-size: 12px;
                    }
                    th { 
                        background-color: #454F4A; 
                        color: white; 
                        font-weight: 600;
                    }
                    tr:nth-child(even) { background-color: #f9f9f9; }
                    .summary-card { 
                        display: inline-block; 
                        padding: 10px 15px; 
                        margin: 5px; 
                        border: 1px solid #ddd; 
                        border-radius: 8px;
                    }
                    .print-footer {
                        margin-top: 20px;
                        padding-top: 10px;
                        border-top: 1px solid #ddd;
                        text-align: center;
                        font-size: 10px;
                        color: #999;
                    }
                    @media print {
                        .no-print { display: none !important; }
                    }
                </style>
            </head>
            <body>
                <div class="print-header">
                    <h1>Kaye Spa</h1>
                    <p>${
                      title || "Report"
                    } | Generated: ${new Date().toLocaleString()}</p>
                </div>
                ${element.innerHTML}
                <div class="print-footer">
                    <p>Kaye Spa Management System | Confidential</p>
                </div>
            </body>
            </html>
        `);
    doc.close();

    // Wait for content to load then print
    iframe.contentWindow.focus();
    setTimeout(function () {
      iframe.contentWindow.print();
      // Clean up iframe after printing
      setTimeout(function () {
        document.body.removeChild(iframe);
      }, 1000);
    }, 250);
  },

  // Export to PDF by printing using iframe (works in MAUI WebView2)
  exportToPdf: function (selector, filename) {
    var element = document.querySelector(selector);
    if (!element) {
      element = document.getElementById(selector);
    }
    if (!element) {
      console.error("Element not found:", selector);
      alert("No content found to export");
      return;
    }

    // Create a hidden iframe for printing
    var iframe = document.createElement("iframe");
    iframe.style.position = "absolute";
    iframe.style.width = "0";
    iframe.style.height = "0";
    iframe.style.border = "none";
    iframe.style.left = "-9999px";
    document.body.appendChild(iframe);

    var doc = iframe.contentWindow.document;
    doc.open();
    doc.write(`
            <!DOCTYPE html>
            <html>
            <head>
                <title>${filename || "Kaye Spa Report"}</title>
                <style>
                    * { margin: 0; padding: 0; box-sizing: border-box; }
                    body { 
                        font-family: 'Segoe UI', Arial, sans-serif; 
                        padding: 30px; 
                        color: #454F4A;
                        background: white;
                    }
                    .print-header {
                        text-align: center;
                        margin-bottom: 25px;
                        padding-bottom: 15px;
                        border-bottom: 3px solid #454F4A;
                    }
                    .print-header h1 { 
                        color: #454F4A; 
                        font-size: 28px; 
                        margin-bottom: 5px;
                        letter-spacing: 2px;
                    }
                    .print-header p { color: #AA9478; font-size: 11px; }
                    table { 
                        width: 100%; 
                        border-collapse: collapse; 
                        margin-top: 15px;
                    }
                    th, td { 
                        border: 1px solid #DCD8CE; 
                        padding: 10px 12px; 
                        text-align: left; 
                        font-size: 11px;
                    }
                    th { 
                        background-color: #454F4A; 
                        color: white; 
                        font-weight: 600;
                        text-transform: uppercase;
                        font-size: 10px;
                    }
                    tr:nth-child(even) { background-color: #f8f7f5; }
                    .card { 
                        border: none !important;
                        box-shadow: none !important;
                        padding: 0 !important;
                    }
                    .btn, button { display: none !important; }
                    input, select { 
                        border: 1px solid #DCD8CE !important;
                        background: #f8f7f5 !important;
                    }
                    .print-footer {
                        margin-top: 30px;
                        padding-top: 15px;
                        border-top: 2px solid #DCD8CE;
                        text-align: center;
                        font-size: 9px;
                        color: #AA9478;
                    }
                    @media print {
                        .no-print { display: none !important; }
                        body { -webkit-print-color-adjust: exact; print-color-adjust: exact; }
                    }
                </style>
            </head>
            <body>
                <div class="print-header">
                    <h1>KAYE SPA</h1>
                    <p>Generated: ${new Date().toLocaleString()}</p>
                </div>
                ${element.innerHTML}
                <div class="print-footer">
                    <p>Kaye Spa Management System | Confidential Business Document</p>
                </div>
            </body>
            </html>
        `);
    doc.close();

    // Wait for content to load then print
    iframe.contentWindow.focus();
    setTimeout(function () {
      iframe.contentWindow.print();
      // Clean up iframe after printing
      setTimeout(function () {
        document.body.removeChild(iframe);
      }, 1000);
    }, 300);
  },

  // Export table to CSV
  exportTableToCSV: function (tableId, filename) {
    var table = document.getElementById(tableId);
    if (!table) {
      // Try to find table inside element
      var container = document.getElementById(tableId);
      if (container) {
        table = container.querySelector("table");
      }
    }

    if (!table) {
      console.error("Table not found:", tableId);
      alert("No table found to export");
      return;
    }

    var csv = [];
    var rows = table.querySelectorAll("tr");

    for (var i = 0; i < rows.length; i++) {
      var row = [];
      var cols = rows[i].querySelectorAll("td, th");

      for (var j = 0; j < cols.length; j++) {
        // Get text content and clean it
        var text = cols[j].textContent.replace(/"/g, '""').trim();
        row.push('"' + text + '"');
      }

      csv.push(row.join(","));
    }

    // Download CSV
    var csvContent = csv.join("\n");
    var blob = new Blob(["\ufeff" + csvContent], {
      type: "text/csv;charset=utf-8;",
    });
    var link = document.createElement("a");
    var url = URL.createObjectURL(blob);

    link.setAttribute("href", url);
    link.setAttribute("download", (filename || "export") + ".csv");
    link.style.visibility = "hidden";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  },

  // Export to Excel (XLSX format simulation using CSV with Excel mime type)
  exportTableToExcel: function (tableId, filename, sheetName) {
    var table = document.getElementById(tableId);
    if (!table) {
      var container = document.getElementById(tableId);
      if (container) {
        table = container.querySelector("table");
      }
    }

    if (!table) {
      console.error("Table not found:", tableId);
      alert("No table found to export");
      return;
    }

    var html = `
            <html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel">
            <head>
                <meta charset="UTF-8">
                <!--[if gte mso 9]>
                <xml>
                    <x:ExcelWorkbook>
                        <x:ExcelWorksheets>
                            <x:ExcelWorksheet>
                                <x:Name>${sheetName || "Report"}</x:Name>
                                <x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions>
                            </x:ExcelWorksheet>
                        </x:ExcelWorksheets>
                    </x:ExcelWorkbook>
                </xml>
                <![endif]-->
                <style>
                    td, th { border: 1px solid #ccc; padding: 5px; }
                    th { background-color: #454F4A; color: white; font-weight: bold; }
                </style>
            </head>
            <body>
                <table>${table.innerHTML}</table>
            </body>
            </html>
        `;

    var blob = new Blob([html], { type: "application/vnd.ms-excel" });
    var link = document.createElement("a");
    var url = URL.createObjectURL(blob);

    link.setAttribute("href", url);
    link.setAttribute("download", (filename || "export") + ".xls");
    link.style.visibility = "hidden";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  },

  // Export data array to CSV
  exportDataToCSV: function (data, headers, filename) {
    if (!data || data.length === 0) {
      alert("No data to export");
      return;
    }

    var csv = [];

    // Add headers
    if (headers && headers.length > 0) {
      csv.push(headers.map((h) => '"' + h.replace(/"/g, '""') + '"').join(","));
    }

    // Add data rows
    for (var i = 0; i < data.length; i++) {
      var row = data[i];
      if (Array.isArray(row)) {
        csv.push(
          row
            .map((cell) => '"' + String(cell || "").replace(/"/g, '""') + '"')
            .join(",")
        );
      } else if (typeof row === "object") {
        csv.push(
          Object.values(row)
            .map((cell) => '"' + String(cell || "").replace(/"/g, '""') + '"')
            .join(",")
        );
      }
    }

    var csvContent = csv.join("\n");
    var blob = new Blob(["\ufeff" + csvContent], {
      type: "text/csv;charset=utf-8;",
    });
    var link = document.createElement("a");
    var url = URL.createObjectURL(blob);

    link.setAttribute("href", url);
    link.setAttribute("download", (filename || "export") + ".csv");
    link.style.visibility = "hidden";
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  },
};

// =====================================================
// CHART.JS UTILITIES FOR DASHBOARD
// =====================================================

window.chartUtils = {
  charts: {},

  // Create a line chart
  createLineChart: function (canvasId, labels, datasets, options) {
    var ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    // Destroy existing chart
    if (this.charts[canvasId]) {
      this.charts[canvasId].destroy();
    }

    this.charts[canvasId] = new Chart(ctx, {
      type: "line",
      data: {
        labels: labels,
        datasets: datasets.map((ds, idx) => ({
          label: ds.label,
          data: ds.data,
          borderColor: ds.color || this.getColor(idx),
          backgroundColor: (ds.color || this.getColor(idx)) + "20",
          fill: ds.fill !== false,
          tension: 0.3,
          borderWidth: 2,
        })),
      },
      options: Object.assign(
        {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: { position: "top" },
          },
          scales: {
            y: { beginAtZero: true },
          },
        },
        options || {}
      ),
    });

    return this.charts[canvasId];
  },

  // Create a bar chart
  createBarChart: function (canvasId, labels, datasets, options) {
    var ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    if (this.charts[canvasId]) {
      this.charts[canvasId].destroy();
    }

    this.charts[canvasId] = new Chart(ctx, {
      type: "bar",
      data: {
        labels: labels,
        datasets: datasets.map((ds, idx) => ({
          label: ds.label,
          data: ds.data,
          backgroundColor: ds.color || this.getColor(idx),
          borderColor: ds.borderColor || this.getColor(idx),
          borderWidth: 1,
        })),
      },
      options: Object.assign(
        {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: { position: "top" },
          },
          scales: {
            y: { beginAtZero: true },
          },
        },
        options || {}
      ),
    });

    return this.charts[canvasId];
  },

  // Create a doughnut chart
  createDoughnutChart: function (canvasId, labels, data, options) {
    var ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    if (this.charts[canvasId]) {
      this.charts[canvasId].destroy();
    }

    this.charts[canvasId] = new Chart(ctx, {
      type: "doughnut",
      data: {
        labels: labels,
        datasets: [
          {
            data: data,
            backgroundColor: labels.map((_, idx) => this.getColor(idx)),
            borderWidth: 2,
            borderColor: "#fff",
          },
        ],
      },
      options: Object.assign(
        {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: { position: "right" },
          },
        },
        options || {}
      ),
    });

    return this.charts[canvasId];
  },

  // Create a pie chart
  createPieChart: function (canvasId, labels, data, options) {
    var ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    if (this.charts[canvasId]) {
      this.charts[canvasId].destroy();
    }

    this.charts[canvasId] = new Chart(ctx, {
      type: "pie",
      data: {
        labels: labels,
        datasets: [
          {
            data: data,
            backgroundColor: labels.map((_, idx) => this.getColor(idx)),
            borderWidth: 2,
            borderColor: "#fff",
          },
        ],
      },
      options: Object.assign(
        {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: { position: "right" },
          },
        },
        options || {}
      ),
    });

    return this.charts[canvasId];
  },

  // Create an area chart (line chart with fill for revenue vs expenses comparison)
  createAreaChart: function (canvasId, labels, datasets, options) {
    var ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    if (this.charts[canvasId]) {
      this.charts[canvasId].destroy();
    }

    this.charts[canvasId] = new Chart(ctx, {
      type: "line",
      data: {
        labels: labels,
        datasets: datasets.map((ds, idx) => ({
          label: ds.label,
          data: ds.data,
          borderColor: ds.color || this.getColor(idx),
          backgroundColor: (ds.color || this.getColor(idx)) + "40",
          fill: true,
          tension: 0.4,
          borderWidth: 2,
          pointRadius: 3,
          pointHoverRadius: 5,
        })),
      },
      options: Object.assign(
        {
          responsive: true,
          maintainAspectRatio: false,
          interaction: {
            mode: "index",
            intersect: false,
          },
          plugins: {
            legend: { position: "top" },
            tooltip: {
              callbacks: {
                label: function (context) {
                  return (
                    context.dataset.label +
                    ": ₱" +
                    context.parsed.y.toLocaleString()
                  );
                },
              },
            },
          },
          scales: {
            y: {
              beginAtZero: true,
              ticks: {
                callback: function (value) {
                  return "₱" + value.toLocaleString();
                },
              },
            },
          },
        },
        options || {}
      ),
    });

    return this.charts[canvasId];
  },

  // Create a stacked bar chart (for comparing revenue vs commission)
  createStackedBarChart: function (canvasId, labels, datasets, options) {
    var ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    if (this.charts[canvasId]) {
      this.charts[canvasId].destroy();
    }

    this.charts[canvasId] = new Chart(ctx, {
      type: "bar",
      data: {
        labels: labels,
        datasets: datasets.map((ds, idx) => ({
          label: ds.label,
          data: ds.data,
          backgroundColor: ds.color || this.getColor(idx),
          borderColor: ds.borderColor || ds.color || this.getColor(idx),
          borderWidth: 1,
        })),
      },
      options: Object.assign(
        {
          responsive: true,
          maintainAspectRatio: false,
          interaction: {
            mode: "index",
            intersect: false,
          },
          plugins: {
            legend: { position: "top" },
            tooltip: {
              callbacks: {
                label: function (context) {
                  return (
                    context.dataset.label +
                    ": ₱" +
                    context.parsed.y.toLocaleString()
                  );
                },
              },
            },
          },
          scales: {
            x: { stacked: false },
            y: {
              stacked: false,
              beginAtZero: true,
              ticks: {
                callback: function (value) {
                  return "₱" + value.toLocaleString();
                },
              },
            },
          },
        },
        options || {}
      ),
    });

    return this.charts[canvasId];
  },

  // Create horizontal bar chart (for therapist rankings)
  createHorizontalBarChart: function (canvasId, labels, datasets, options) {
    var ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    if (this.charts[canvasId]) {
      this.charts[canvasId].destroy();
    }

    this.charts[canvasId] = new Chart(ctx, {
      type: "bar",
      data: {
        labels: labels,
        datasets: datasets.map((ds, idx) => ({
          label: ds.label,
          data: ds.data,
          backgroundColor: ds.color || this.getColor(idx),
          borderColor: ds.borderColor || ds.color || this.getColor(idx),
          borderWidth: 1,
        })),
      },
      options: Object.assign(
        {
          indexAxis: "y",
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: { display: false },
            tooltip: {
              callbacks: {
                label: function (context) {
                  return "₱" + context.parsed.x.toLocaleString();
                },
              },
            },
          },
          scales: {
            x: {
              beginAtZero: true,
              ticks: {
                callback: function (value) {
                  return "₱" + value.toLocaleString();
                },
              },
            },
          },
        },
        options || {}
      ),
    });

    return this.charts[canvasId];
  },

  // Destroy chart
  destroyChart: function (canvasId) {
    if (this.charts[canvasId]) {
      this.charts[canvasId].destroy();
      delete this.charts[canvasId];
    }
  },

  // Simple bar chart - takes labels array and data array directly
  createSimpleBarChart: function (canvasId, labels, data, colors) {
    var ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    if (this.charts[canvasId]) {
      this.charts[canvasId].destroy();
    }

    var bgColors = Array.isArray(colors)
      ? colors
      : labels.map((_, idx) => this.getColor(idx));

    this.charts[canvasId] = new Chart(ctx, {
      type: "bar",
      data: {
        labels: labels,
        datasets: [
          {
            data: data,
            backgroundColor: bgColors,
            borderColor: bgColors,
            borderWidth: 1,
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false },
          tooltip: {
            callbacks: {
              label: function (context) {
                return "₱" + context.parsed.y.toLocaleString();
              },
            },
          },
        },
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              callback: function (value) {
                return "₱" + value.toLocaleString();
              },
            },
          },
        },
      },
    });

    return this.charts[canvasId];
  },

  // Simple horizontal bar chart - takes labels array and data array directly
  createSimpleHorizontalBarChart: function (canvasId, labels, data, color) {
    var ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    if (this.charts[canvasId]) {
      this.charts[canvasId].destroy();
    }

    var bgColor = color || this.getColor(0);

    this.charts[canvasId] = new Chart(ctx, {
      type: "bar",
      data: {
        labels: labels,
        datasets: [
          {
            data: data,
            backgroundColor: bgColor,
            borderColor: bgColor,
            borderWidth: 1,
          },
        ],
      },
      options: {
        indexAxis: "y",
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false },
          tooltip: {
            callbacks: {
              label: function (context) {
                return "₱" + context.parsed.x.toLocaleString();
              },
            },
          },
        },
        scales: {
          x: {
            beginAtZero: true,
            ticks: {
              callback: function (value) {
                return "₱" + value.toLocaleString();
              },
            },
          },
        },
      },
    });

    return this.charts[canvasId];
  },

  // Simple area chart with multiple datasets - takes labels, datasetLabels, dataArrays, colors
  createSimpleAreaChart: function (
    canvasId,
    labels,
    datasetLabels,
    dataArrays,
    colors
  ) {
    var ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    if (this.charts[canvasId]) {
      this.charts[canvasId].destroy();
    }

    var self = this;
    var datasets = datasetLabels.map((label, idx) => ({
      label: label,
      data: dataArrays[idx],
      borderColor: (colors && colors[idx]) || self.getColor(idx),
      backgroundColor: ((colors && colors[idx]) || self.getColor(idx)) + "40",
      fill: true,
      tension: 0.4,
      borderWidth: 2,
      pointRadius: 2,
      pointHoverRadius: 5,
    }));

    this.charts[canvasId] = new Chart(ctx, {
      type: "line",
      data: {
        labels: labels,
        datasets: datasets,
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        interaction: {
          mode: "index",
          intersect: false,
        },
        plugins: {
          legend: { position: "top" },
          tooltip: {
            callbacks: {
              label: function (context) {
                return (
                  context.dataset.label +
                  ": ₱" +
                  context.parsed.y.toLocaleString()
                );
              },
            },
          },
        },
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              callback: function (value) {
                return "₱" + value.toLocaleString();
              },
            },
          },
        },
      },
    });

    return this.charts[canvasId];
  },

  // Simple doughnut with custom colors
  createSimpleDoughnutChart: function (canvasId, labels, data, colors) {
    var ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    if (this.charts[canvasId]) {
      this.charts[canvasId].destroy();
    }

    var bgColors = Array.isArray(colors)
      ? colors
      : labels.map((_, idx) => this.getColor(idx));

    this.charts[canvasId] = new Chart(ctx, {
      type: "doughnut",
      data: {
        labels: labels,
        datasets: [
          {
            data: data,
            backgroundColor: bgColors,
            borderWidth: 2,
            borderColor: "#fff",
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { position: "right" },
          tooltip: {
            callbacks: {
              label: function (context) {
                return context.label + ": ₱" + context.parsed.toLocaleString();
              },
            },
          },
        },
      },
    });

    return this.charts[canvasId];
  },

  // Color palette
  getColor: function (index) {
    var colors = [
      "#454F4A", // Primary
      "#AA9478", // Accent
      "#6B9B7E", // Success
      "#7A9CAA", // Info
      "#D4A574", // Warning
      "#C97064", // Danger
      "#9B7AA8", // Purple
      "#5D8AA8", // Steel blue
      "#E8927C", // Salmon
      "#7EBDC2", // Teal
    ];
    return colors[index % colors.length];
  },
};

// Sidebar scroll position persistence
window.sidebarScroll = {
  save: function () {
    var nav = document.querySelector(".nav-scrollable");
    if (nav) {
      localStorage.setItem("sidebarScrollTop", nav.scrollTop);
    }
  },
  restore: function () {
    var nav = document.querySelector(".nav-scrollable");
    if (nav) {
      var scrollTop = localStorage.getItem("sidebarScrollTop");
      if (scrollTop) {
        nav.scrollTop = parseInt(scrollTop, 10);
      }
    }
  },
  init: function () {
    var nav = document.querySelector(".nav-scrollable");
    if (nav) {
      // Restore scroll position
      this.restore();

      // Save scroll position on scroll
      nav.addEventListener("scroll", function () {
        localStorage.setItem("sidebarScrollTop", nav.scrollTop);
      });
    }
  },
};

// Initialize sidebar scroll on page load
document.addEventListener("DOMContentLoaded", function () {
  window.sidebarScroll.init();
});

// Also handle Blazor navigation
if (typeof Blazor !== "undefined") {
  Blazor.addEventListener("enhancedload", function () {
    setTimeout(function () {
      window.sidebarScroll.init();
    }, 50);
  });
}
