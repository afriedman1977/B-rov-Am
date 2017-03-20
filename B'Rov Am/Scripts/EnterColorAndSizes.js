$(function () {
    var x = $("#size-count").val();
    var a = $("#color option:selected").val();
    for (var y = 0; y < x; y++) {        
        $("#color-id-" + y).val(a);
    }

    //$("#color").change(function () {
    //    var b = $("#color option:selected").val(); 
    //    var c = $("#product-id").val();
    //    console.log(c);
    //    $("#choose-sizes").remove();
    //    for (var z = 0; z < x; z++) {           
    //        $("#color-id-" + z).val(b);
    //    }
        //$.get("/Home/EnterColorAndSizes", { id: c, colorId: b }, function (result) {
        //    FillForm(result);
       // });        
       
   // });

    //function FillForm(EnterColorAndSizesViewModel) {
    //    $("#size-div").append(       
    //    '<div class="row" id="choose-sizes">');
    //    EnterColorAndSizesViewModel.CurrentproductColorsize = EnterColorAndSizesViewModel.CollorsSizesForproduct.Find(EnterColorAndSizesViewModel.CollorsSizesForproduct.SizeId == EnterColorAndSizesViewModel.AllSizes[x].SizeId && EnterColorAndSizesViewModel.CollorsSizesForproduct.ColorId == EnterColorAndSizesViewModel.ColorId);
    //    for(var x = 0; x < EnterColorAndSizesViewModel.AllSizes.length; x++){       
       //      if (EnterColorAndSizesViewModel.CollorsSizesForproduct.some(EnterColorAndSizesViewModel.CollorsSizesForproduct.SizeId == EnterColorAndSizesViewModel.AllSizes[x].SizeId && EnterColorAndSizesViewModel.CollorsSizesForproduct.ColorId == EnterColorAndSizesViewModel.ColorId))
       //    {
       //        $("#size-div").append(
       //        + '<div class="col-md-2">'
       //            +' <input type="hidden" name="pcs['+x+'].productId" value="'+EnterColorAndSizesViewModel.CurrentProduct.Id +'" />'
       //            +' <input type="hidden" name="pcs['+x+'].colorId" id="color-id-'+x+'" value="" />'
       //            +' <input type="hidden" name="pcs['+x+'].sizeId" value="'+EnterColorAndSizesViewModel.AllSizes[x].SizeId+'" />'

                   
       //        //EnterColorAndSizesViewModel.CurrentproductColorsize = EnterColorAndSizesViewModel.CollorsSizesForproduct.First(c => c.SizeId == EnterColorAndSizesViewModel.AllSizes[x].SizeId && c.ColorId == EnterColorAndSizesViewModel.ColorId);
           

       //        +' <input type="checkbox" name="pcs['+x+'].included" value="true" checked />'+EnterColorAndSizesViewModel.AllSizes[x].ProductSize+'<span>-------------------</span>'
       //+' </div>'
       //+' <div class="col-md-3">'
       //    +' <input type="text" class="input-sm" name="pcs['+x+'].quantity" value="@Model.CurrentproductColorsize.Quantity">'
       //+' </div>');
    //    }

    //else
    //       {
   //            $("#size-div").append(
   //    ' <div class="col-md-2">'
   //        +' <input type="hidden" name="pcs['+x+'].productId" value="'+EnterColorAndSizesViewModel.CurrentProduct.Id+'" />'
   //         +'<input type="hidden" name="pcs['+x+'].colorId" id="color-id-'+x+'" value="" />'
   //        +' <input type="hidden" name="pcs['+x+'].sizeId" value="'+EnterColorAndSizesViewModel.AllSizes[x].SizeId+'" />'
   //        +' <input type="checkbox" name="pcs['+x+'].included" value="true" />'+ EnterColorAndSizesViewModel.AllSizes[x].ProductSize +'<span>-------------------</span>'
   //     +'</div>'
   //    +' <div class="col-md-3">'
   //         +'<input type="text" class="input-sm" name="pcs['+x+'].quantity" placeholder="Quantity">'
   //    +' </div>');
   //         // }
   //            console.log((x + 1) % 2);
   //if ((x + 1) % 2 === 0){   
   //    $("#size-div").append(
   //    +'<div class="col-md-12">'
   //        +'<hr />'
   //    +' </div>');
   // }
   //      }
   // //     $("#size-div").append(
   // //   + ' </div>'
   // //    );
   // }
});