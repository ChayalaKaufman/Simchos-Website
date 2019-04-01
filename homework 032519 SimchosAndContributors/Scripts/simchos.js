$(() => {

    $("#add").on('click', function () {
        $("#name").val('');
        $("#date").val('');
        $("#add-modal").modal();
    });

    $("#add-simcha").on('click', function () {
        $("#add-modal").modal('hide');
    })


})