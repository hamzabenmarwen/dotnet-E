$(document).ready(function () {
    // Initialize wishlist buttons on page load
    initializeWishlistButtons();

    $('.wishlist-toggle').click(function (e) {
        e.preventDefault();
        var button = $(this);
        var productId = button.data('product-id');

        toggleWishlist(productId, button);
    });

    function initializeWishlistButtons() {
        // Load saved wishlist state from localStorage
        $('.wishlist-toggle').each(function () {
            var button = $(this);
            var productId = button.data('product-id');
            var savedState = localStorage.getItem('wishlist_' + productId);

            if (savedState === 'true') {
                button.addClass('active');
                button.find('.far').hide();
                button.find('.fas').show();
            }
        });
    }

    function toggleWishlist(productId, button) {
        var isActive = button.hasClass('active');

        // Animation
        button.css('transform', 'scale(1.2)');

        setTimeout(function () {
            if (isActive) {
                // Remove from wishlist
                button.removeClass('active');
                button.find('.far').show();
                button.find('.fas').hide();
                localStorage.setItem('wishlist_' + productId, 'false');
                showNotification('💔 Produit retiré de la wishlist', 'info');
            } else {
                // Add to wishlist
                button.addClass('active');
                button.find('.far').hide();
                button.find('.fas').show();
                localStorage.setItem('wishlist_' + productId, 'true');
                showNotification('💖 Produit ajouté à la wishlist!', 'success');

                // Heart pulse animation
                button.addClass('pulse');
                setTimeout(function () {
                    button.removeClass('pulse');
                }, 600);
            }

            button.css('transform', 'scale(1)');
        }, 150);
    }

    function showNotification(message, type) {
        // Remove any existing notifications
        $('.custom-notification').remove();

        var alertClass = type === 'success' ? 'alert-success' : 'alert-info';
        var icon = type === 'success' ? 'fa-check-circle' : 'fa-info-circle';

        var notification = $(
            '<div class="alert ' + alertClass + ' alert-dismissible fade show custom-notification position-fixed">' +
            '<i class="fas ' + icon + ' me-2"></i>' +
            message +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
            '</div>'
        );

        $('body').append(notification);

        // Auto remove after 3 seconds
        setTimeout(function () {
            notification.alert('close');
        }, 3000);
    }
});