// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/*$(".delete-button").on("click", function (event) {
    let postId = "@Model.Id";
    let postPictureId = $(event.target).attr("value");

    $(event.target).parent().css("display", "none");

    $.ajax({
        type: "POST",
        url: "@Url.Action('Delete', 'PostPicture')",
        data: { postPictureId: postPictureId, postId: postId },
        dataType: "text"
    });
});*/

$(".custom-file-input").on("change", function () {
    $(".custom-file-label").text(
        `Pictures chosen: ${$(this)[0].files.length}`);
});