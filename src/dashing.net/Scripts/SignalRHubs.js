(function ($) {
    $.ajax({
        url: "signalr/hubs",
        dataType: "script",
        async: false,
        cache: true
    });
}(jQuery));