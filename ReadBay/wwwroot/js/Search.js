var dataTable;

$(document).ready(function () {
    loadDataTable();
});

//Using DataTables.net with Ajax 
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Customer/Search/GetAll"
        },
        "columns": [
            { "data": "title", "width": "10%" },
            { "data": "isbn", "width": "10%" },
            { "data": "author", "width": "10%" },
            { "data": "bookType.name", "width": "10%" },
            { "data": "category.name", "width": "10%" },
            { "data": "price", "width": "10%" },
            { "data": "price50", "width": "10%" },
            { "data": "price100", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Customer/Home/Details/${data}" class="btn btn-primary text-white" style="cursor:pointer">
                                    <i class="bi bi-box-arrow-in-right">Details</i>
                                </a>
                                
                           `;
                }, "width": "20%"
            }
        ]
    });
}

