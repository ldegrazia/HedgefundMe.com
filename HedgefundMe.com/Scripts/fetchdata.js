function fetchData(id) {
    $.ajax({
        type: "POST",
        url: "/Home/GetRankingData",
        data: { id: id },
        dataType: "json",
        success: function (response) {
            debugger;
            alert(response);
        },
        error: function (error) {
            alert(error);
        }
    });
}