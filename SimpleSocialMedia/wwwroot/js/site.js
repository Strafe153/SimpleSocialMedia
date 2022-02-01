let postPicturesToDelete = [];

// Inputs a loaded picture as the new profile picture
function loadFile(event) {
    let output = document.querySelector("#profile-pic");
    output.src = URL.createObjectURL(event.target.files[0]);
    output.onload = () => URL.revokeObjectURL(output.src);
};

// shows individual post's comment section
function showComments(postId) {
    $(`.post-comments-${postId}`).css("display", "block");
    $(`#show-comments-btn-${postId}`).css("display", "none");
    $(`#hide-comments-btn-${postId}`).css("display", "block");
}

// hides individual post's comment section
function hideComments(postId) {
    $(`.post-comments-${postId}`).css("display", "none");
    $(`#show-comments-btn-${postId}`).css("display", "block");
    $(`#hide-comments-btn-${postId}`).css("display", "none");
}

// shows individual post's button to attach pictures
function showAddCommentPicturesButton(postId) {
    const currentPicturesButton = $(`#comment-pictures-btn-${postId}`);
    currentPicturesButton.css("display", "block");
    $(".comment-pictures-btn").not(currentPicturesButton).css("display", "none");
}

// Hides a post picture when pressed "Delete" button
$(".delete-button").on("click", function (event) {
    let postPictureId = $(event.target).attr("value");

    $(event.target).parent().css("display", "none");
    postPicturesToDelete.push(postPictureId);
});

// Marks the loaded profile picture as selected and outputs its name in the input type="file" field
$("#profile-pic-input").on("change", function () {
    let fileName = $(this).val().split("\\").pop();
    $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
});

// Marks all the loaded post pictures as selected and outputs their count in the input type="file" field
$("#post-picture").on("change", function () {
    $(".custom-file-label").text(`Pictures chosen: ${$(this)[0].files.length}`);
});