$("[name='question_3']").change(function(){
  const q3val = $(this).val();
  if(q3val === "Yes"){
    $("#use-migration").hide();
    $("#import").show();
  }else{
    $("#use-migration").show();
    $("#import").hide();
  }
})