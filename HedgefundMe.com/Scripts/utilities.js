function validateSearch() {if (document.getElementById("search").value == '') return false;return true;}
function confirmDelete() { var answer = confirm('Delete all checked?'); return answer; }
function confirmRemove() { var answer = confirm('Remove all members checked?'); return answer; }
function confirmAdd() { var answer = confirm('Add all members checked?'); return answer; }
function confirmAddAll() { var answer = confirm('Add all users to this role?'); return answer; }
function confirmRemoveAll() { var answer = confirm('Remove all users, but Admin, from this role?'); return answer; }
 
function checkUncheckAll() {
    $("input[name = deleteInputs]").each(function () {
        if ($('input[name=checkuncheckall]')[0].checked == 1) { this.setAttribute('checked', true); }
        else { this.checked = false; }
    });
}
function validatePageNum() {
    var x = $("#pagenum").val();
    
    if (!isNaN(parseFloat(x)) && isFinite(x)) {
        return;
    } else {
        $("#pagenum").val('1');
    }
}
