using System.Net.Mail;
using FreelanceHub.Web.Extensions;
using FreelanceHub.Web.ViewModels.Public;

namespace FreelanceHub.Web.Security;

public sealed class LocalizedContactFormValidator
{
    public Dictionary<string, string> Validate(ContactFormViewModel form, string culture)
    {
        culture = culture.NormalizeSiteCulture();
        var errors = new Dictionary<string, string>();

        var name = (form.Name ?? string.Empty).Trim();
        var email = (form.Email ?? string.Empty).Trim();
        var subject = (form.Subject ?? string.Empty).Trim();
        var message = (form.Message ?? string.Empty).Trim();

        if (name.Length is < 2 or > 120)
        {
            errors[nameof(form.Name)] = culture.IsFrench()
                ? "Le nom doit contenir entre 2 et 120 caracteres."
                : "Name must be between 2 and 120 characters.";
        }

        if (string.IsNullOrWhiteSpace(email) || email.Length > 256 || !IsValidEmail(email))
        {
            errors[nameof(form.Email)] = culture.IsFrench()
                ? "Veuillez saisir une adresse email valide."
                : "Please enter a valid email address.";
        }

        if (subject.Length is < 3 or > 140)
        {
            errors[nameof(form.Subject)] = culture.IsFrench()
                ? "Le sujet doit contenir entre 3 et 140 caracteres."
                : "Subject must be between 3 and 140 characters.";
        }

        if (message.Length is < 20 or > 3000)
        {
            errors[nameof(form.Message)] = culture.IsFrench()
                ? "Le message doit contenir entre 20 et 3000 caracteres."
                : "Message must be between 20 and 3000 characters.";
        }

        return errors;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var address = new MailAddress(email);
            var atIndex = email.IndexOf('@');
            return address.Address == email && atIndex > 0 && email.IndexOf('.', atIndex) > atIndex + 1;
        }
        catch
        {
            return false;
        }
    }
}
