

//document.querySelector('form').addEventListener('submit', function (event) {
//    event.preventDefault(); // Formun gönderilmesini engelle

//    swal({
//        title: 'Başarı!',
//        text: 'Talebiniz yöneticinize iletildi.',
//        icon: 'success',
//        button: 'Tamam'
//    }).then(() => {
//        // Mesaj gösterildikten sonra formu gönder ve yönlendir
//        event.target.submit();
//        window.location.href = '/Personnel/WorkOff/Index'; // Burada yönlendirmek istediğiniz URL'yi belirtin
//    });
//});

//function setupFormSubmission(formSelector, redirectUrl) {
//    const form = document.querySelector(formSelector);
//    if (!form) {
//        console.warn(`Form ${formSelector} bulunamadı.`);
//        return;
//    }

//    console.log(`Form ${formSelector} bulundu ve ayarlandı.`);

//    form.addEventListener('submit', function (event) {
//        event.preventDefault(); // Formun gönderilmesini engelle

//        swal({
//            title: 'Başarı!',
//            text: 'Talebiniz yöneticinize iletildi.',
//            icon: 'success',
//            button: 'Tamam'
//        }).then(() => {
//            // Mesaj gösterildikten sonra formu gönder ve yönlendir
//            form.submit(); // event.target.submit() yerine form.submit() kullanın
//            window.location.href = redirectUrl; // Yönlendirme URL'sini kullan
//        });
//    });
//}

//document.addEventListener('DOMContentLoaded', function () {
//    setupFormSubmission('form#formWorkOffCreate', '/Personnel/WorkOff/Index');
//    setupFormSubmission('form#formWorkOffEdit', '/Personnel/WorkOff/Index');
//    setupFormSubmission('form#formExpenseCreate', '/Personnel/Expense/Index');
//    setupFormSubmission('form#formExpenseEdit', '/Personnel/Expense/Index');
//    setupFormSubmission('form#formPrepaymentCreate', '/Personnel/Prepayment/Index');
//    setupFormSubmission('form#formPrepaymentEdit', '/Personnel/Prepayment/Index');
//});

function setupFormSubmission(formSelector, redirectUrl) {
    const form = document.querySelector(formSelector);
    if (!form) {
        console.warn(`Form ${formSelector} bulunamadı.`);
        return;
    }

    form.addEventListener('submit', function (event) {
        event.preventDefault(); // Formun gönderilmesini engelle

        swal({
            title: 'Başarı!',
            text: 'Talebiniz yöneticinize iletildi.',
            icon: 'success',
            button: 'Tamam'
        }).then(() => {
            // Mesaj gösterildikten sonra formu gönder ve yönlendir
            form.submit(); // Formu yeniden gönderir
            setTimeout(() => {
                window.location.href = redirectUrl; // Yönlendirme URL'sini kullan
            }, 100); // Yönlendirmeyi biraz geciktir
        });
    });
}

document.addEventListener('DOMContentLoaded', function () {
    setupFormSubmission('form#formWorkOffCreate', '/Personnel/WorkOff/Index');
    setupFormSubmission('form#formWorkOffEdit', '/Personnel/WorkOff/Index');
    setupFormSubmission('form#formExpenseCreate', '/Personnel/Expense/Index');
    setupFormSubmission('form#formExpenseEdit', '/Personnel/Expense/Index');
    setupFormSubmission('form#formPrepaymentCreate', '/Personnel/Prepayment/Index');
    setupFormSubmission('form#formPrepaymentEdit', '/Personnel/Prepayment/Index');
});

