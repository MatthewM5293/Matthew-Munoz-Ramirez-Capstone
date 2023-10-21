// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function openForm() {
    document.getElementById("myForm").style.display = "block";
}

function closeForm() {
    document.getElementById("myForm").style.display = "none";
}

window.setTimeout(function () {
    $(".alert").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 3000);

$(document).ready(function () {
    $('[data-toggle="popover"]').popover({
        html: true,
        content: function () {
            return $('#popover-content').html();
        }
    });

    $('.forLinks').on('shown.bs.popover', function () {
        var element = $('.popover'),
            style = window.getComputedStyle(element[0]),
            left = style.getPropertyValue('left');
        var leftValue = left.replace("px", "");
        $('.popover').css('left', parseInt(leftValue) - 5);
    });

    $('body').on('click', function (e) {
        $('[data-toggle=popover]').each(function () {
            // hide any open popovers when the anywhere else in the body is clicked 
            if (!$(this).is(e.target) && $(this).has(e.target).length === 0 && $('.popover').length != 0) {
                $('.forLinks').trigger('click');
            }
        });
    });
});


//Modals
var modal = document.getElementById("myModal");

// Get the button that opens the modal
var btn = document.getElementById("myBtn");

// Get the <span> element that closes the modal
var span = document.getElementsByClassName("close")[0];

// When the user clicks on the button, open the modal
btn.onclick = function () {
    modal.style.display = "block";
}

// When the user clicks on <span> (x), close the modal
span.onclick = function () {
    modal.style.display = "none";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}


//Popouts
$(function () {
    $("[data-toggle=popover]").popover({
        html: true,
        placement: "bottom",
        content: function () {
            var content = $(this).attr("data-popover-content");
            return $(content).children(".popover-body").html();
        },
        title: function () {
            var title = $(this).attr("data-popover-content");
            return $(title).children(".popover-heading").html();
        }
    });
});
