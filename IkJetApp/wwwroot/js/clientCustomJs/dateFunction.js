document.addEventListener("DOMContentLoaded", function () {
    var today = new Date().toISOString().split('T')[0];
    document.querySelector('input[name="startDate"]').value = today;
    document.querySelector('input[name="endDate"]').value = today;

    document.querySelector('input[name="startDate"]').addEventListener("change", validateDates);
    document.querySelector('input[name="endDate"]').addEventListener("change", validateDates);

    validateDates(); // İlk açılışta gün sayısını hesapla
});

function validateDates() {
    var startDate = new Date(document.querySelector('input[name="startDate"]').value);
    var endDate = new Date(document.querySelector('input[name="endDate"]').value);
    var today = new Date();
    today.setHours(0, 0, 0, 0); // Sadece tarihi karşılaştır

    if (startDate < today) {
        swal({
            title: 'Hatalı Tarih',
            text: 'Başlangıç tarihi geçmiş bir tarih olamaz.',
            icon: 'error',
            buttons: {
                confirm: {
                    text: "Tamam",
                    className: "btn btn-primary",
                }
            }
        }).then(() => {
            document.querySelector('input[name="startDate"]').value = today.toISOString().split('T')[0];
        });
    }

    if (endDate < startDate) {
        swal({
            title: 'Hatalı Tarih',
            text: 'Bitiş tarihi, başlangıç tarihinden önce olamaz.',
            icon: 'error',
            buttons: {
                confirm: {
                    text: "Tamam",
                    className: "btn btn-primary",
                }
            }
        }).then(() => {
            document.querySelector('input[name="endDate"]').value = startDate.toISOString().split('T')[0];
        });
    }

    // Gün sayısını hesapla ve requestedLeaveDay inputuna yaz
    var timeDiff = Math.abs(endDate.getTime() - startDate.getTime());
    var diffDays = Math.ceil(timeDiff / (1000 * 3600 * 24)) + 1; // Gün sayısını hesapla
    document.querySelector('input[name="requestedLeaveDay"]').value = diffDays;
}


document.querySelector('form').addEventListener('submit', function (event) {
    // İzin tipini kontrol et
    const workOfType = document.querySelector('select[name="workOfType"]').value;

    // Eğer izin tipi seçilmemişse
    if (!workOfType) {
        event.preventDefault(); // Formun gönderilmesini engelle

        swal({
            title: 'Hata!',
            text: 'İzin tipi seçmelisiniz!',
            icon: 'error',
            button: 'Tamam'
        });
        return;
    }

    // Başarı mesajını göster
    swal({
        title: 'Başarı!',
        text: 'Talebiniz yöneticinize iletildi.',
        icon: 'success',
        button: 'Tamam'
    }).then(() => {
        // Mesaj gösterildikten sonra formu gönder ve yönlendir
        event.target.submit();
        window.location.href = '/index'; // Burada yönlendirmek istediğiniz URL'yi belirtin
    });
});