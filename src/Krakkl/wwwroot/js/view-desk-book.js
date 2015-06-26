$(document).ready(function () {
    $(".glyphicon-asterisk").tooltip();
    $(".glyphicon-question-sign").tooltip();

    CKEDITOR.replace('Synopsis',
        {
            toolbar: 'Basic',
            language: $('#Language_Key').val(),
            filebrowserImageBrowseUrl: '/Image/Browser?bookId=@Model.Key',
            filebrowserImageUploadUrl: '/Image/Upload?bookId=@Model.Key',
            filebrowserImageWindowWidth: '680',
            filebrowserImageWindowHeight: '680'
        });

    var ckeditor = CKEDITOR.instances['Synopsis'];

    ckeditor.on('blur', function (e) {
        var key = $("#Key")[0].value;
        var synopsis = CKEDITOR.instances['Synopsis'].getData();
        var data = { bookKey: key, synopsis: synopsis };
        $.post("/Desk/UpdateSynopsis", data);
    });

    $("#Language_Key").change(function (e) {
        CKEDITOR.instances.Synopsis.destroy();
        CKEDITOR.replace('Synopsis', { toolbar: 'Basic', language: $('#Language_Key').val() });

        var key = $("#Key")[0].value;
        var languageKey = $("#Language_Key").val();
        var data = { bookKey: key, language: languageKey };
        $.post("/Desk/UpdateBookLanguage", data);
    });

    $("#title").blur(function () {
        var key = $("#Key")[0].value;
        var title = $("#title")[0].value;
        var data = { bookKey: key, title: title };
        $.post("/Desk/UpdateBookTitle", data);
    });

    $("#subTitle").blur(function () {
        var key = $("#Key")[0].value;
        var subTitle = $("#subTitle")[0].value;
        var data = { bookKey: key, subTitle: subTitle };
        $.post("/Desk/UpdateBookSubTitle", data);
    });

    $("#seriesTitle").blur(function () {
        var key = $("#Key")[0].value;
        var seriesTitle = $("#seriesTitle")[0].value;
        var data = { bookKey: key, seriesTitle: seriesTitle };
        $.post("/Desk/UpdateBookSeriesTitle", data);
    });

    $("#seriesVolume").blur(function () {
        var key = $("#Key")[0].value;
        var seriesVolume = $("#seriesVolume")[0].value;
        var data = { bookKey: key, seriesVolume: seriesVolume };
        $.post("/Desk/UpdateBookSeriesVolume", data);
    });

    $("#genreKey").change(function (e) {
        var key = $("#Key")[0].value;
        var genreKey = $("#genreKey").val();
        var data = { bookKey: key, genre: genreKey };
        $.post("/Desk/UpdateBookGenre", data);
    });

    $("#Published").change(function (e) {
        var key = $("#Key")[0].value;
        var data = { bookKey: key };
        $.post("/Desk/UpdateBookPublished", data);
    });

    $("#Completed").change(function (e) {
        var key = $("#Key")[0].value;
        var completed = $("#Completed")[0].checked;
        var data = { bookKey: key, completed: completed };
        $.post("/Desk/UpdateBookCompleted", data);
    });

    $("#Abandoned").change(function (e) {
        var key = $("#Key")[0].value;
        var abandoned = $("#Abandoned")[0].checked;
        var data = { bookKey: key, abandoned: abandoned };
        $.post("/Desk/UpdateBookAbandoned", data);
    });

    var input = $('.fileinput').fileinput();
    input.on('change.bs.fileinput', function(e, files) {
        var data = new FormData();
        data.append("UploadedImage", files[0]);
        $.post("/Desk/UploadCoverArt", data);
    });
});
