$(function () {
    $("#add-product").click(function () {
        $("#submit-add").show();
        $("#submit-edit").hide();
        $(".modal-title").text("Add Product");
        $("#add-product-modal").modal();
    });

    $(".edit").click(function () {
        var id = $(this).closest('tr').data('product-id');
        $.get("/Home/GetProductById?id=" + id, function (result) {
            $("#style-number").val(result.StyleNumber);
            $("#brand").val(result.Brand);
            $("#description").val(result.Description);
            $("#price").val(result.Price);
            $("#product-id").val(result.Id);
            var x = $("#product-id").val();
            console.log("product id" + x);
            console.log("result id" + result.Id);
            $('#category').val(result.CategoryId).change();
            $("#submit-add").hide();
            $("#submit-edit").show();
            $("#add-product-modal").modal();
        });
    });

    $(".delete").click(function () {
        var product = $(this).closest('tr').find('td:eq(0)').text();       
        if (!confirm("Do you want to delete " + product)) {
            return false;
        }        
    });

    $(".listen").click(function () {
        var description = $(this).closest('tr').find('td:eq(2)').text();
        responsiveVoice.speak(description);
    });

    //$(".details").click(function () {
    //    $("#all-details").modal();
    //})
   

    //$("#submit-add").click(function () {
    //    var style = $("#style-number").val();
    //    var brand = $("#brand").val();
    //    var description = $("#description").val();
    //    //var color = $("#color option:selected").text();
    //    //var size = $("#size option:selected").text();
    //    var price = $("#price").val();
    //    var categoryId = $("#category option:selected").val();
    //    //var colorId = $("#color option:selected").val();
    //    //var sizeId = $("#size option:selected").val();
    //    console.log(style);
    //    console.log(brand);
    //    console.log(description);
    //    //console.log(color);
    //    //console.log(size);
    //    console.log(price);
    //    console.log(categoryId);
    //    //console.log(colorId);
    //    //console.log(sizeId);
    //    //$.post("/Home/AddProduct", { styleNumber: style, brand: brand, description: description, color: color, size: size, price: price, categoryId: categoryId, colorId: colorId, sizeId: sizeId }, function (result) {
    //    $.post("/Home/AddProduct", { styleNumber: style, brand: brand, description: description, price: price, categoryId: categoryId }, function (result) {
    //        $("#add-product-modal").modal('hide');
    //        $("#add-color-size-modal").modal();
    //        $(".modal-title").text("Add Colors and Sizes for " + result.StyleNumber);
    //    });

    //});
});