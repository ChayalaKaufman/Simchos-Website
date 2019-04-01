$(() => {

    $("#add").on('click', function () {
        $("#first-name").val('');
        $("#last-name").val('');
        $("#cell-number").val('');
        $("#initial-deposit").val('');
        $("#date-created").val('');
        $("#add-modal").modal();
        $("#first-name").focus();
    });
    
    $("#add-person").on('click', function () {
        $("#add-modal").modal('hide');
    })
    
    //$("table").on('click', '.history', function () {
    //    const button = $(this);
    //    const id = button.data('id');
    //    ppl = ppl.filter(p => p.id !== id);
    //    fillTable();
    //})

    $(".edit").on('click', function () {
        const button = $(this);
        const id = button.data('id');
        const firstName = button.data('first-name');
        const lastName = button.data('last-name');
        const cellNumber = button.data('cell');
        const dateCreated = button.data('date-created');
        const alwaysInclude = button.data('always-include');

        $("#contributor-id2").val(id);
        $("#first-name2").val(firstName);
        $("#last-name2").val(lastName);
        $("#cell-number2").val(cellNumber);
        $("#date-created2").val(dateCreated);
        //$("#alwaysInclude").val(alwaysInclude);
        $("#edit-modal").modal();
    })


    $("#save-changes").on('click', function () {
        $("#edit-modal").modal('hide');
    })

    $(".deposit").on('click', function () {
        const button = $(this);
        const id = button.data('id');
        const date = button.data('date');

        $("#contributor-id").val(id);
        $("#amount").val('');
        $("#date").val(date);
        $("#deposit-modal").modal();
    })

    $("#save").on('click', function () {
        $("#deposit-modal").modal('hide');
    })

});