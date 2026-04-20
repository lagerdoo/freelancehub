document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".alert").forEach((alert) => {
        window.setTimeout(() => {
            alert.classList.add("fade");
            alert.classList.remove("show");
        }, 4000);
    });

    document.querySelectorAll("[data-bs-toggle='tooltip']").forEach((element) => {
        new bootstrap.Tooltip(element);
    });

    document.querySelectorAll(".clickable-row").forEach((row) => {
        const navigate = () => {
            const href = row.dataset.href;
            if (href) {
                window.location.href = href;
            }
        };

        row.addEventListener("click", (event) => {
            if (event.target.closest("a, button, form, input, textarea, select, label")) {
                return;
            }

            navigate();
        });

        row.addEventListener("keydown", (event) => {
            if (event.key === "Enter" || event.key === " ") {
                event.preventDefault();
                navigate();
            }
        });
    });

    const deleteModalElement = document.getElementById("deleteConfirmModal");
    const confirmDeleteButton = document.getElementById("confirmDeleteButton");
    let pendingDeleteForm = null;

    if (deleteModalElement && confirmDeleteButton) {
        const deleteModal = new bootstrap.Modal(deleteModalElement);
        const deleteMessageElement = document.getElementById("deleteConfirmMessage");

        document.querySelectorAll("[data-delete-confirm='true']").forEach((button) => {
            button.addEventListener("click", (event) => {
                event.preventDefault();
                event.stopPropagation();
                pendingDeleteForm = button.closest("form");
                if (deleteMessageElement) {
                    deleteMessageElement.textContent = button.dataset.deleteMessage || "This item will be permanently deleted.";
                }
                deleteModal.show();
            });
        });

        confirmDeleteButton.addEventListener("click", () => {
            if (pendingDeleteForm) {
                pendingDeleteForm.submit();
            }
        });
    }

    const iconInput = document.getElementById("IconClass");
    const iconPreview = document.getElementById("service-icon-preview");
    if (iconInput && iconPreview) {
        const updateIconPreview = () => {
            iconPreview.className = iconInput.value || "bi bi-grid";
        };

        iconInput.addEventListener("input", updateIconPreview);
        updateIconPreview();
    }

    document.querySelectorAll(".contact-form").forEach((form) => {
        const messages = {
            name: form.dataset.msgName || "Invalid name.",
            email: form.dataset.msgEmail || "Invalid email.",
            subject: form.dataset.msgSubject || "Invalid subject.",
            message: form.dataset.msgMessage || "Invalid message."
        };

        const validators = {
            name: (value) => value.trim().length >= 2 && value.trim().length <= 120,
            email: (value) => /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/.test(value.trim()),
            subject: (value) => value.trim().length >= 3 && value.trim().length <= 140,
            message: (value) => value.trim().length >= 20 && value.trim().length <= 3000
        };

        const setError = (field, message) => {
            const input = form.querySelector(`[data-contact-field='${field}']`);
            const errorElement = form.querySelector(`[data-valmsg-for='Form.${field.charAt(0).toUpperCase() + field.slice(1)}']`);
            if (!input || !errorElement) {
                return true;
            }

            const isValid = !message;
            input.classList.toggle("is-invalid", !isValid);
            errorElement.textContent = message || "";
            return isValid;
        };

        const validateField = (field) => {
            const input = form.querySelector(`[data-contact-field='${field}']`);
            if (!input) {
                return true;
            }

            const isValid = validators[field](input.value);
            return setError(field, isValid ? "" : messages[field]);
        };

        Object.keys(validators).forEach((field) => {
            const input = form.querySelector(`[data-contact-field='${field}']`);
            if (!input) {
                return;
            }

            input.addEventListener("blur", () => validateField(field));
            input.addEventListener("input", () => {
                if (input.classList.contains("is-invalid")) {
                    validateField(field);
                }
            });
        });

        form.addEventListener("submit", (event) => {
            const allValid = Object.keys(validators).every((field) => validateField(field));
            if (!allValid) {
                event.preventDefault();
            }
        });
    });
});
