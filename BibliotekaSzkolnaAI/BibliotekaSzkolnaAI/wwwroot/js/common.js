//karuzela ksiazek
window.scrollCarousel = (id, direction, loop = true) => {
    const wrapper = document.getElementById(id);
    if (!wrapper) return;

    const container = wrapper.querySelector('.carousel-container');
    if (!container) return;

    const card = container.querySelector('.book-card');
    if (!card) return;

    const cardWidth = card.offsetWidth + 24; //wartość odstępu (gap)
    const scrollAmount = cardWidth * 1;

    const newScroll = container.scrollLeft + (direction * scrollAmount);

    if (loop) {
        const maxScroll = container.scrollWidth - container.clientWidth;

        if (newScroll > maxScroll) {
            container.scrollTo({ left: 0, behavior: 'smooth' });
        } else if (newScroll < 0) {
            container.scrollTo({ left: maxScroll, behavior: 'smooth' });
        } else {
            container.scrollBy({ left: direction * scrollAmount, behavior: 'smooth' });
        }
    } else {
        container.scrollBy({ left: direction * scrollAmount, behavior: 'smooth' });
    }
};

function scrollToBottom(container) {
    if (container) {
        container.scrollTop = container.scrollHeight;
    }
}

//cache clear
window.clearClientStorage = () => {
    const confirmed = confirm("Czy chcesz wyczyścić pamięć podręczna? To spowoduje odświeżenie strony i pobranie nowych danych z serwera. Ta operacja może znacząco obciążyć twoje urządzenie. Czy mimo to chcesz kontynuować?");
    if (!confirmed) return;

    localStorage.clear();
    sessionStorage.clear();

    const cookies = document.cookie.split(";");
    for (let c of cookies) {
        const eqPos = c.indexOf("=");
        const name = eqPos > -1 ? c.substr(0, eqPos) : c;
        document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=/";
    }

    alert("Dane lokalne zostały wyczyszczone. Strona zostanie odświeżona.");

    location.reload();
};

//tytul ksiazki w tytule karty
window.setTitle = (title) => {
    document.title = title;
}

//toastr
window.ShowToastr = (type, message) => {
    if (type === "success") {
        toastr.success(message, "Sukces", { timeOut: 5000 });
    }
    if (type === "error") {
        toastr.error(message, "Błąd", { timeOut: 5000 });
    }
    if (type === "info") {
        toastr.info(message, "Informacja", { timeOut: 5000 });
    }
    if (type === "warning") {
        toastr.warning(message, "Uwaga", { timeOut: 5000 });
    }
}