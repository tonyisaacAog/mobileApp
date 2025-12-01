// Site-wide JavaScript functionality

$(document).ready(function() {
    // Auto-hide alerts after 5 seconds
    $('.alert').delay(5000).fadeOut();

    // Confirm delete actions
    $('a[href*="Delete"]').on('click', function(e) {
        var linkText = $(this).text();
        if (!confirm('Are you sure you want to ' + linkText.toLowerCase() + ' this item?')) {
            e.preventDefault();
        }
    });

    // Form validation feedback
    $('form').on('submit', function() {
        var form = $(this);
        var submitBtn = form.find('button[type="submit"]');
        var originalText = submitBtn.text();

        submitBtn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');

        // Re-enable after 3 seconds (in case of error)
        setTimeout(function() {
            submitBtn.prop('disabled', false).text(originalText);
        }, 3000);
    });

    // Table row hover effects
    $('table tbody tr').hover(
        function() {
            $(this).addClass('table-active');
        },
        function() {
            $(this).removeClass('table-active');
        }
    );

    // Search functionality (if search inputs exist)
    $('input[type="search"], input[data-search]').on('keyup', function() {
        var searchTerm = $(this).val().toLowerCase();
        var table = $(this).closest('.card').find('table');

        if (table.length) {
            table.find('tbody tr').each(function() {
                var row = $(this);
                var text = row.text().toLowerCase();

                if (text.indexOf(searchTerm) === -1) {
                    row.hide();
                } else {
                    row.show();
                }
            });
        }
    });

    // Responsive table handling
    function checkTableOverflow() {
        $('.table-responsive').each(function() {
            var table = $(this).find('table');
            if (table.width() > $(this).width()) {
                $(this).addClass('table-overflow');
            } else {
                $(this).removeClass('table-overflow');
            }
        });
    }

    checkTableOverflow();
    $(window).resize(checkTableOverflow);
});

// Utility functions
function showSuccessMessage(message) {
    var alertHtml = '<div class="alert alert-success alert-dismissible fade show" role="alert">' +
        message +
        '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>' +
        '</div>';

    $('.container main').prepend(alertHtml);
    $('.alert').delay(5000).fadeOut();
}

function showErrorMessage(message) {
    var alertHtml = '<div class="alert alert-danger alert-dismissible fade show" role="alert">' +
        message +
        '<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>' +
        '</div>';

    $('.container main').prepend(alertHtml);
    $('.alert').delay(5000).fadeOut();
}
