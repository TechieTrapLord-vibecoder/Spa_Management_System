// DataTables initialization for Blazor
window.DataTableInterop = {
  // Initialize a DataTable on an element
  init: function (tableId, options) {
    const defaultOptions = {
      pageLength: 10,
      lengthMenu: [
        [10, 25, 50, -1],
        [10, 25, 50, "All"],
      ],
      responsive: true,
      language: {
        search: "Search:",
        lengthMenu: "Show _MENU_ entries",
        info: "Showing _START_ to _END_ of _TOTAL_ entries",
        infoEmpty: "No entries available",
        infoFiltered: "(filtered from _MAX_ total entries)",
        paginate: {
          first: "«",
          last: "»",
          next: "›",
          previous: "‹",
        },
        emptyTable: "No data available",
      },
      dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>rtip',
      ordering: true,
      searching: true,
      paging: true,
      info: true,
    };

    // Merge custom options
    const mergedOptions = { ...defaultOptions, ...options };

    // Wait for the element to exist
    return new Promise((resolve) => {
      const checkElement = setInterval(() => {
        const table = document.getElementById(tableId);
        if (table) {
          clearInterval(checkElement);

          // Destroy existing instance if any
          if ($.fn.DataTable.isDataTable("#" + tableId)) {
            $("#" + tableId)
              .DataTable()
              .destroy();
          }

          // Initialize DataTable
          const dt = $("#" + tableId).DataTable(mergedOptions);
          resolve(true);
        }
      }, 100);

      // Timeout after 5 seconds
      setTimeout(() => {
        clearInterval(checkElement);
        resolve(false);
      }, 5000);
    });
  },

  // Destroy a DataTable
  destroy: function (tableId) {
    if ($.fn.DataTable.isDataTable("#" + tableId)) {
      $("#" + tableId)
        .DataTable()
        .destroy();
    }
  },

  // Refresh/reload DataTable data
  refresh: function (tableId) {
    if ($.fn.DataTable.isDataTable("#" + tableId)) {
      $("#" + tableId)
        .DataTable()
        .ajax.reload();
    }
  },

  // Redraw the table (use after data changes from Blazor)
  redraw: function (tableId) {
    return new Promise((resolve) => {
      setTimeout(() => {
        if ($.fn.DataTable.isDataTable("#" + tableId)) {
          $("#" + tableId)
            .DataTable()
            .destroy();
        }

        const table = document.getElementById(tableId);
        if (table) {
          $("#" + tableId).DataTable({
            pageLength: 10,
            responsive: true,
            language: {
              search: "Search:",
              emptyTable: "No data available",
            },
          });
        }
        resolve(true);
      }, 100);
    });
  },
};
