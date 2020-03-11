$(document).ready(function () {
    "use strict"; $(document).on("click", ".sub-menu", function () {
        $(this).children(".sub").toggle(); $(this).children(".sub").toggleClass("forScroll"); if ($(this).children(".sub").hasClass("forScroll")) { $("#sidebar").css({ "overflow-y": "scroll" }) }
        else { $("#sidebar").css({ "overflow-y": "auto" }) }
    })
    $("footer .fa-angle-up").click(function () { $("html").animate({ scrollTop: 0 }, 500) })

    //Like button
    $(".fas.fa-thumbs-up").hide();
    $(document).on("click", ".far.fa-thumbs-up", function () {
        $(this).hide();
        $(this).next().show().addClass("like-comment-btn-active");
    })
    $(document).on("click", ".fas.fa-thumbs-up", function () {
        $(this).hide();
        $(this).prev().show();
    })

    //Comment button
    $(".all-comments-container").hide();
    $(document).on("click", ".fa-comment-alt", function () {
        $(this).parent().parent().next().slideToggle("slow");
        $(this).parent().parent().next().css({ "display": "flex", "flex-direction": "column" });
        $(this).toggleClass("like-comment-btn-active");
    })

    //X button on post
    $(document).on("click", ".fa-times", function () {
        $(this).parent().parent().hide("slow").removeClass("d-flex");
        var postId = $(this).next().val();
        $.ajax(
            {
                url: "/Profile/DeletePost/" + postId,
                type: "POST",
        });
    })

    //Users un-follow
    var pairedUserId = $(".paired-user-id");
    for (var i = 0; i < pairedUserId.length; i++) {
        var pairedElement = $(`.un-follow-btn[data-id="${pairedUserId.eq(i).val()}"]`);
        pairedElement.removeClass("btn-secondary");
        pairedElement.addClass("btn-primary");
        pairedElement.children().eq(0).addClass("un-follow-none");
        pairedElement.children().eq(1).addClass("un-follow-none");
        pairedElement.children().eq(2).removeClass("un-follow-none");
        pairedElement.children().eq(3).removeClass("un-follow-none");
        pairedElement.attr("data-status", "checked");
    }

    $(document).on("click", ".un-follow-btn", function () {
        $(this).children(".fa-plus-square").toggle();
        $(this).children(".fa-plus-square").next().toggle();
        $(this).children(".fa-check-square").toggle();
        $(this).children(".fa-check-square").next().toggle();
        $(this).toggleClass("btn-secondary");
        $(this).toggleClass("btn-primary");

        var userId = $(this).attr("data-id");
        var userStatus = $(this).attr("data-status");
        var url = "";
        if (userStatus === "checked") {
            url = "/Profile/UnFollowUser?userId=" + userId;
            $(this).attr("data-status", "unchecked");
        }
        else {
            url = "/Profile/FollowUser?userId=" + userId;
            $(this).attr("data-status", "checked");
        }

        $.ajax(
        {
            url: url,
            type: "POST",
            success: function (response) {
            }
        });
    })
})