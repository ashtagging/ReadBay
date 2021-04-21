var dataTable;

$(document).ready(function () {
    loadDataTable();
});

//Using DataTables.net with Ajax 
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            {      //Lockout Account Button
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        //if true user is currently locked
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                                    <i class="fas fa-lock-open"></i>  Unlock
                                </a>
                            </div>
                           `;
                    }
                    else {
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px;">
                                    <i class="fas fa-lock"></i> Lock
                                </a>
                            </div>
                           `;
                    }

                }, "width": "15%"
            },
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "25%" },           
            { "data": "company.name", "width": "20%" },
            { "data": "role", "width": "10%" },
            { "data": "phoneNumber", "width": "15%" },
            
        ]
    });
}

// Locking and Unlocking Accounts using  Toastr Notifications
function LockUnlock(id) {

    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
            else {
                toastr.error(data.message);
            }
        }
    });
}