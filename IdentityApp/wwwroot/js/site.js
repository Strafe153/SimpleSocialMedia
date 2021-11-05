let postPicturesToDelete = [];

// Inputs a loaded picture as the new profile picture
function loadFile(event) {
    let output = document.querySelector("#profile-pic");
    output.src = URL.createObjectURL(event.target.files[0]);
    output.onload = () => URL.revokeObjectURL(output.src);
};

// Hides a post picture when pressed "Delete" button
$(".delete-button").on("click", function(event) {
    let postPictureId = $(event.target).attr("value");

    $(event.target).parent().css("display", "none");
    postPicturesToDelete.push(postPictureId);
});

// Marks the loaded profile picture as selected and outputs its name in the input type="file" field
$("#profile-pic-input").on("change", function() {
    let fileName = $(this).val().split("\\").pop();
    $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
});

// Marks all the loaded post pictures as selected and outputs their count in the input type="file" field
$("#post-picture").on("change", function() {
    $(".custom-file-label").text(`Pictures chosen: ${$(this)[0].files.length}`);
});

$("#show-comments-btn").on("click", function() {
    $(".post-comments").css("display", "block");
    $("#hide-comments-btn").css("display", "block");
    $(this).css("display", "none");
});

$("#hide-comments-btn").on("click", function() {
    $(".post-comments").css("display", "none");
    $("#show-comments-btn").css("display", "block");
    $(this).css("display", "none");
});