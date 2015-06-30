$(document).ready(function () {
    $(".glyphicon-asterisk").tooltip();
    $(".glyphicon-question-sign").tooltip();

    var key = $("#Key")[0].value;

    CKEDITOR.replace('Synopsis',
        {
            toolbar: 'Basic',
            language: $('#Language_Key').val(),
            filebrowserImageBrowseUrl: '/Image/Browser?bookKey=' + key,
            filebrowserImageUploadUrl: '/Image/Upload?bookKey' + key,
            filebrowserImageWindowWidth: '680',
            filebrowserImageWindowHeight: '680'
        });

    var ckeditor = CKEDITOR.instances['Synopsis'];

    ckeditor.on('blur', function (e) {
        var synopsis = CKEDITOR.instances['Synopsis'].getData();
        var data = { bookKey: key, synopsis: synopsis };
        $.post("/Desk/UpdateSynopsis", data);
    });

    $("#Language_Key").change(function (e) {
        CKEDITOR.instances.Synopsis.destroy();
        CKEDITOR.replace('Synopsis',
        {
            toolbar: 'Basic',
            language: $('#Language_Key').val(),
            filebrowserImageBrowseUrl: '/Image/Browser?bookKey=' + key,
            filebrowserImageUploadUrl: '/Image/Upload?bookKey' + key,
            filebrowserImageWindowWidth: '680',
            filebrowserImageWindowHeight: '680'
        });

        var languageKey = $("#Language_Key").val();
        var data = { bookKey: key, language: languageKey };
        $.post("/Desk/UpdateBookLanguage", data);
    });

    $("#title").blur(function () {
        var title = $("#title")[0].value;
        var data = { bookKey: key, title: title };
        $.post("/Desk/UpdateBookTitle", data);
    });

    $("#subTitle").blur(function () {
        var subTitle = $("#subTitle")[0].value;
        var data = { bookKey: key, subTitle: subTitle };
        $.post("/Desk/UpdateBookSubTitle", data);
    });

    $("#seriesTitle").blur(function () {
        var seriesTitle = $("#seriesTitle")[0].value;
        var data = { bookKey: key, seriesTitle: seriesTitle };
        $.post("/Desk/UpdateBookSeriesTitle", data);
    });

    $("#seriesVolume").blur(function () {
        var seriesVolume = $("#seriesVolume")[0].value;
        var data = { bookKey: key, seriesVolume: seriesVolume };
        $.post("/Desk/UpdateBookSeriesVolume", data);
    });

    $("#genreKey").change(function (e) {
        var genreKey = $("#genreKey").val();
        var data = { bookKey: key, genre: genreKey };
        $.post("/Desk/UpdateBookGenre", data);
    });

    $("#Published").change(function (e) {
        var data = { bookKey: key };
        $.post("/Desk/UpdateBookPublished", data);
    });

    $("#Completed").change(function (e) {
        var data = { bookKey: key };
        $.post("/Desk/UpdateBookCompleted", data);
    });

    $("#Abandoned").change(function (e) {
        var abandoned = $("#Abandoned")[0].checked;
        var data = { bookKey: key, abandoned: abandoned };
        $.post("/Desk/UpdateBookAbandoned", data);
    });

    var input = $('.fileinput').fileinput();
    input.on('change.bs.fileinput', function(e, files) {
        var formData = new FormData();
        formData.append("UploadedImage", files);

        $.ajax({
            url: '/Desk/UploadCoverArt?bookKey=' + key,
            type: 'POST',
            data: formData,
            cache: false,
            contentType: false,
            processData: false
        });
    });
});
