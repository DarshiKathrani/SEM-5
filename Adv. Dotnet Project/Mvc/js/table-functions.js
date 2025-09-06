// ========================================
// Reusable Table Functions for Grocery Store Management
// ========================================

/**
 * Initialize table functionality (search, sort, alerts)
 * @param {string} tableId - ID of the table element
 * @param {string} searchId - ID of the search input element
 */
function initializeTable(tableId, searchId) {
    // Initialize search functionality
    initializeSearch(tableId, searchId);

    // Initialize sort functionality
    initializeSort(tableId);

    // Initialize auto-dismiss alerts
    initializeAlerts();
}

/**
 * Initialize search functionality
 * @param {string} tableId - ID of the table element
 * @param {string} searchId - ID of the search input element
 */
function initializeSearch(tableId, searchId) {
    const searchInput = document.getElementById(searchId);
    if (!searchInput) return;

    searchInput.addEventListener('keyup', function () {
        const searchTerm = this.value.toLowerCase();
        const table = document.getElementById(tableId);
        if (!table) return;

        const rows = table.querySelectorAll('tbody tr');
        let visibleCount = 0;

        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            const isVisible = text.includes(searchTerm);
            row.style.display = isVisible ? '' : 'none';
            if (isVisible) visibleCount++;
        });

        // Update table info if it exists
        updateTableInfo(visibleCount, rows.length);
    });
}

/**
 * Initialize sort functionality
 * @param {string} tableId - ID of the table element
 */
function initializeSort(tableId) {
    const table = document.getElementById(tableId);
    if (!table) return;

    const sortableHeaders = table.querySelectorAll('.sortable');

    sortableHeaders.forEach(header => {
        header.addEventListener('click', function () {
            const tbody = table.querySelector('tbody');
            const rows = Array.from(tbody.querySelectorAll('tr'));
            const column = Array.from(this.parentNode.children).indexOf(this);
            const sortIcon = this.querySelector('.sort-icon');

            // Reset all sort icons
            sortableHeaders.forEach(h => {
                const icon = h.querySelector('.sort-icon');
                if (icon && icon !== sortIcon) {
                    icon.className = 'fas fa-sort sort-icon';
                }
            });

            // Determine sort direction
            const isAscending = sortIcon.classList.contains('fa-sort-up');

            // Sort rows
            rows.sort((a, b) => {
                const aText = a.children[column]?.textContent.trim() || '';
                const bText = b.children[column]?.textContent.trim() || '';

                // Try to parse as numbers first
                const aNum = parseFloat(aText.replace(/[^\d.-]/g, ''));
                const bNum = parseFloat(bText.replace(/[^\d.-]/g, ''));

                let comparison = 0;
                if (!isNaN(aNum) && !isNaN(bNum)) {
                    comparison = aNum - bNum;
                } else {
                    comparison = aText.localeCompare(bText);
                }

                return isAscending ? -comparison : comparison;
            });

            // Update sort icon
            if (isAscending) {
                sortIcon.className = 'fas fa-sort-down sort-icon';
            } else {
                sortIcon.className = 'fas fa-sort-up sort-icon';
            }

            // Re-append sorted rows
            rows.forEach(row => tbody.appendChild(row));
        });
    });
}

/**
 * Initialize auto-dismiss alerts
 */
function initializeAlerts() {
    setTimeout(function () {
        const alerts = document.querySelectorAll('.alert');
        alerts.forEach(alert => {
            if (window.bootstrap && window.bootstrap.Alert) {
                const bsAlert = new bootstrap.Alert(alert);
                bsAlert.close();
            }
        });
    }, 5000);
}

/**
 * Show delete confirmation modal
 * @param {string} itemName - Name of the item to delete
 * @param {string} deleteUrl - URL for delete action
 */
function showDeleteModal(itemName, deleteUrl) {
    const modal = document.getElementById('deleteModal');
    if (!modal) return;

    const nameElement = document.getElementById('customerNameToDelete') ||
        document.getElementById('itemNameToDelete');
    const confirmBtn = document.getElementById('confirmDeleteBtn');

    if (nameElement) nameElement.textContent = itemName;
    if (confirmBtn) confirmBtn.href = deleteUrl;

    if (window.bootstrap && window.bootstrap.Modal) {
        const bsModal = new bootstrap.Modal(modal);
        bsModal.show();
    }
}

/**
 * Update table info (showing X of Y items)
 * @param {number} visible - Number of visible items
 * @param {number} total - Total number of items
 */
function updateTableInfo(visible, total) {
    const tableInfo = document.querySelector('.table-info span');
    if (tableInfo) {
        tableInfo.innerHTML = `Showing <strong>1-${visible}</strong> of <strong>${total}</strong> items`;
    }
}

/**
 * Format currency for display
 * @param {number} amount - Amount to format
 * @param {string} currency - Currency symbol (default: $)
 * @returns {string} Formatted currency string
 */
function formatCurrency(amount, currency = '$') {
    if (isNaN(amount)) return currency + '0.00';
    return currency + parseFloat(amount).toFixed(2);
}

/**
 * Format date for display
 * @param {string|Date} date - Date to format
 * @returns {string} Formatted date string
 */
function formatDate(date) {
    if (!date) return '-';
    const d = new Date(date);
    if (isNaN(d.getTime())) return '-';
    return d.toLocaleDateString();
}

/**
 * Generate status badge HTML
 * @param {string} status - Status text
 * @param {string} type - Badge type (active, pending, inactive)
 * @returns {string} HTML for status badge
 */
function generateStatusBadge(status, type = 'active') {
    const badgeClass = `status-badge status-${type}`;
    return `<span class="${badgeClass}">${status}</span>`;
}

/**
 * Initialize filters (if filter elements exist)
 * @param {string} tableId - ID of the table element
 */
function initializeFilters(tableId) {
    const filterElements = document.querySelectorAll('[data-filter]');
    if (filterElements.length === 0) return;

    filterElements.forEach(filter => {
        filter.addEventListener('change', function () {
            applyFilters(tableId);
        });
    });
}

/**
 * Apply all active filters to the table
 * @param {string} tableId - ID of the table element
 */
function applyFilters(tableId) {
    const table = document.getElementById(tableId);
    if (!table) return;

    const rows = table.querySelectorAll('tbody tr');
    const filters = {};

    // Collect all filter values
    document.querySelectorAll('[data-filter]').forEach(filter => {
        const filterType = filter.getAttribute('data-filter');
        const filterValue = filter.value.toLowerCase();
        if (filterValue && filterValue !== 'all') {
            filters[filterType] = filterValue;
        }
    });

    // Apply filters to rows
    let visibleCount = 0;
    rows.forEach(row => {
        let isVisible = true;

        Object.keys(filters).forEach(filterType => {
            const cell = row.querySelector(`[data-${filterType}]`);
            if (cell) {
                const cellValue = cell.getAttribute(`data-${filterType}`).toLowerCase();
                if (!cellValue.includes(filters[filterType])) {
                    isVisible = false;
                }
            }
        });

        row.style.display = isVisible ? '' : 'none';
        if (isVisible) visibleCount++;
    });

    updateTableInfo(visibleCount, rows.length);
}

/**
 * Export table data to CSV
 * @param {string} tableId - ID of the table element
 * @param {string} filename - Name of the CSV file
 */
function exportToCSV(tableId, filename = 'data.csv') {
    const table = document.getElementById(tableId);
    if (!table) return;

    const rows = [];
    const headers = [];

    // Get headers
    table.querySelectorAll('thead th').forEach(th => {
        headers.push(th.textContent.trim());
    });
    rows.push(headers);

    // Get visible rows
    table.querySelectorAll('tbody tr').forEach(tr => {
        if (tr.style.display !== 'none') {
            const row = [];
            tr.querySelectorAll('td').forEach(td => {
                row.push(td.textContent.trim());
            });
            rows.push(row);
        }
    });

    // Create CSV content
    const csvContent = rows.map(row =>
        row.map(cell => `"${cell}"`).join(',')
    ).join('\n');

    // Download CSV
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    window.URL.revokeObjectURL(url);
}

// Auto-initialize on DOM ready
document.addEventListener('DOMContentLoaded', function () {
    // Auto-dismiss alerts
    initializeAlerts();

    // You can add more auto-initialization here if needed
    console.log('Table functions loaded successfully');
});