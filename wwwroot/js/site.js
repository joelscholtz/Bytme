// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function UpdateQtyOrder(ordline_id, new_qty) {

    $.ajax({
        type: "POST",
        data: {
            orderline_id: ordline_id,
            qty: new_qty
        },

        url: "/ShoppingCart/UpdateQuantityInShoppingCart",
        async: true,
        success: function (response) {
            document.body.innerHTML = response;
        },
        error: function () {
            return "error";
        }
    });
}