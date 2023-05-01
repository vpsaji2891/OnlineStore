
$(document).ready(function () {

    ProductLoad();

    productImageUpload();

    if (IsAuthenticated()) {
        $('#btnShowLogin').css('display', 'none');
        $('#btnShowLogout').css('display', 'block');
    }
    else {
        $('#btnShowLogout').css('display', 'none');
        $('#btnShowLogin').css('display', 'block');
    }

    $(".table-list").DataTable({
        columnDefs: [
            { targets: 'no-sort', orderable: false }
        ]
    });

    $('#btnShowLogin').click(function (e) {
        e.preventDefault();
        $('#loginModal').modal('toggle');
    });

    $("#loginModal").on('hide.bs.modal', function () {
        $('#loginError').html('');
    });

    $('#chkRemember').click(function () {
        if ($('#chkRemember').is(':checked')) {
            $('#loginRemember').val(true);
        }
        else {
            $('#loginRemember').val(false);
        }

    });

    $('#loginSubmit').click(function (e) {
        e.preventDefault();

        var user = $('#loginUser').val();
        var password = $('#loginPassword').val();

        if (user != "" && password != "") {
            $.ajax({
                url: LoginUrl,
                type: 'post',
                dataType: 'json',
                data: $('form#loginForm').serialize(),
                success: function (data) {
                    if (data.message != null) {
                        $('#loginError').html(data.message);
                    }
                    if (data.success == true) {
                        $('#loginModal').modal('toggle');

                        if (IsAuthenticated()) {
                            window.location.href = "/";
                        }

                    }
                }
            });
        }
        else {
            $('#loginError').html("Please enter the Login Credentials.!");
        }

    });

    $('#btnSearch').click(function () {
        ProductLoad();
    });

    $('#sortOrder').change(function () {
        ProductLoad();
    });
});

function productImageUpload() {
    const input = document.getElementById("ImgUrl");
    const avatar = document.getElementById("avatar");

    const convertBase64 = (file) => {
        return new Promise((resolve, reject) => {
            const fileReader = new FileReader();
            fileReader.readAsDataURL(file);

            fileReader.onload = () => {
                resolve(fileReader.result);
            };

            fileReader.onerror = (error) => {
                reject(error);
            };
        });
    };

    if ($('#ImgStram').val() != null) {
        avatar.src = $('#ImgStram').val();
    }

    const uploadImage = async (event) => {
        const file = event.target.files[0];
       

            const base64 = await convertBase64(file);
            avatar.src = base64;
            $('#ImgStram').val(base64);
     
    };

    if (input != null) {
        // Allowed file format
        var allowedExtensions = /(\.jpg|\.jpeg|\.png)$/i;

        input.addEventListener("change", (e) => {



            if (!allowedExtensions.exec(input.value)) {

                $('span[data-valmsg-for="ImgUrl"]').text('Invalid file type. Accepted file types are jpeg, jpg & png');
                input.value = '';
                return false;
            }
            else {

                const fileSize = input.files[0].size;
                const maxSize = 1048576 * 1; // 1Mb

                if (fileSize > maxSize) {

                    $('span[data-valmsg-for="ImgUrl"]').text('The maximum allowed file size is 1 Mb');
                    input.value = '';
                    return false;
                }
                uploadImage(e);
                $('span[data-valmsg-for="ImgUrl"]').text('')
            }
        });

    }







}

function IsAuthenticated() {
    var status = false;

    $.ajax({
        url: IsAuthenticatedUrl,
        type: "get",
        async: false,
        dataType: 'json',
        success(result) {
            status = result;
        }
    });
    return status;
}

function AddComment(productId) {
    debugger;

    if (!IsAuthenticated()) {
        $('#loginModal').modal('toggle');
    }
    else {
        var Url = ProductDetailUrl + '?id=' + productId;
        window.location.href = Url;
    }
}

function ProductLoad() {
    debugger;
    var searchData = $('#productSearch').val();
    var sortKey = $('#sortOrder').val();

    var html = ""
    $.ajax({
        url: ProductListUrl, contentType: JSON, async: false, data: { searchData: searchData, sortKey: sortKey }, success: function (result) {
            if (result != null) {

                for (var i = 0; i < result.length; i++) {
                    html = html + '<div class="col-md-3 col-sm-12" >'
                    html = html + '<div class="product-card">'
                    html = html + '<img class="img-thumbnail" src=' + result[i].imgStram + ' alt="" style="width:auto;height:150px;">'
                    html = html + '<h4>' + result[i].productName + ' </h4>' + '<span><small><a class="text-warning" onclick="AddComment(' + result[i].id + ')"  href="#" id=product_' + result[i].id + ' >Add Comment</a></small></span>'
                    html = html + '<p class="price">₹ ' + result[i].productPrice + '</p>'
                    html = html + '<p class="text-success"><small>Category&nbsp;:&nbsp;' + result[i].productCategoryName + ' </small> </p>'
                    html = html + '<p><button> <i class="fa fa-shopping-cart">&nbsp;&nbsp;</i>Add to Cart</button></p>'
                    html = html + '</div>'
                    html = html + '</div>'

                }
                $('#productsList').html(html);
            }
        }
    });

}