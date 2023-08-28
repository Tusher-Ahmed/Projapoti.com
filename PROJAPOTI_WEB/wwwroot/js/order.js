var dataTable;
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        }
        else {
            if (url.includes("approved")) {
                loadDataTable("approved");
            }
            else {
                if (url.includes("pending")) {
                    loadDataTable("pending");
                }
                else {
                    loadDataTable("all");
                }
            }
        }
    }
    
})
function loadDataTable(status) {
    dataTable = $('#mytable').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll?status="+status
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "25%" },
            { "data": "phoneNumber", "width": "20%" },
            { "data": "applicationUser.email", "width": "20%" },
            { "data": "orderStatus", "width": "10%" },
            { "data": "orderTotal", "width": "10%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <div>
                      <a href="/Admin/Order/Details?orderId=${data}"><i class="bi bi-pencil-square text-success"></i></a>                   
                    </div>
 
                    `
                },
                "width": "10%"
            },
        ]
    });
}
