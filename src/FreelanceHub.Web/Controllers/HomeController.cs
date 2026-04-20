using FreelanceHub.Application.Contracts;
using FreelanceHub.Application.Models;
using FreelanceHub.Web.Extensions;
using FreelanceHub.Web.Security;
using FreelanceHub.Web.ViewModels.Public;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Controllers;

[Route("{culture:regex(^fr|en$)}")]
public sealed class HomeController(
    IContentQueryService contentQueryService ,
    ContactFormProtector contactFormProtector ,
    LocalizedContactFormValidator localizedContactFormValidator ,
    ILogger<HomeController> logger) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(string culture , CancellationToken cancellationToken)
    {
        culture = PrepareRequest(culture);
        var snapshot = await contentQueryService.GetHomeSnapshotAsync(cancellationToken);
        SetPublicPageMetadata(snapshot.SiteSetting , culture , snapshot.SiteSetting.Localize(culture , x => x.MetaTitleEn , x => x.MetaTitleFr));

        return View("Landing" , new HomePageViewModel { Snapshot = snapshot });
    }

    [HttpGet("about")]
    public async Task<IActionResult> About(string culture , CancellationToken cancellationToken)
    {
        culture = PrepareRequest(culture);
        var siteSetting = await contentQueryService.GetSiteSettingAsync(cancellationToken);
        var experience = await contentQueryService.GetVisibleExperienceEntriesAsync(cancellationToken);
        SetPublicPageMetadata(siteSetting , culture , culture.IsFrench() ? "A propos" : "About");

        return View(new AboutPageViewModel
        {
            SiteSetting = siteSetting,
            ExperienceEntries = experience
        });
    }

    [HttpGet("services")]
    public async Task<IActionResult> Services(string culture , CancellationToken cancellationToken)
    {
        culture = PrepareRequest(culture);
        var siteSetting = await contentQueryService.GetSiteSettingAsync(cancellationToken);
        SetPublicPageMetadata(siteSetting , culture , culture.IsFrench() ? "Services" : "Services");

        return View(new ServicesPageViewModel
        {
            Services = await contentQueryService.GetPublishedServicesAsync(cancellationToken)
        });
    }

    [HttpGet("projects")]
    public async Task<IActionResult> Projects(string culture , CancellationToken cancellationToken)
    {
        culture = PrepareRequest(culture);
        var siteSetting = await contentQueryService.GetSiteSettingAsync(cancellationToken);
        SetPublicPageMetadata(siteSetting , culture , culture.IsFrench() ? "Projets" : "Projects");

        return View(new ProjectsPageViewModel
        {
            Projects = await contentQueryService.GetPublishedProjectsAsync(false , cancellationToken)
        });
    }

    [HttpGet("experience")]
    public async Task<IActionResult> Experience(string culture , CancellationToken cancellationToken)
    {
        culture = PrepareRequest(culture);
        var siteSetting = await contentQueryService.GetSiteSettingAsync(cancellationToken);
        SetPublicPageMetadata(siteSetting , culture , culture.IsFrench() ? "Experience" : "Experience");

        return View(new ExperiencePageViewModel
        {
            ExperienceEntries = await contentQueryService.GetVisibleExperienceEntriesAsync(cancellationToken)
        });
    }

    [HttpGet("contact")]
    public async Task<IActionResult> Contact(string culture , bool submitted = false , CancellationToken cancellationToken = default)
    {
        culture = PrepareRequest(culture);
        var siteSetting = await contentQueryService.GetSiteSettingAsync(cancellationToken);
        SetPublicPageMetadata(siteSetting , culture , culture.IsFrench() ? "Contact" : "Contact");

        return View(new ContactPageViewModel
        {
            Form = CreateContactFormViewModel() ,
            IsSubmitted = submitted
        });
    }

    [HttpPost("contact")]
    public async Task<IActionResult> Contact(string culture , ContactFormViewModel form , CancellationToken cancellationToken)
    {
        culture = PrepareRequest(culture);
        var siteSetting = await contentQueryService.GetSiteSettingAsync(cancellationToken);
        SetPublicPageMetadata(siteSetting , culture , culture.IsFrench() ? "Contact" : "Contact");

        foreach ( var error in localizedContactFormValidator.Validate(form , culture) )
        {
            ModelState.AddModelError($"Form.{error.Key}" , error.Value);
        }

        if ( !contactFormProtector.TryValidate(HttpContext , form , out var errorCode) )
        {
            ModelState.AddModelError(string.Empty , GetContactSecurityMessage(culture , errorCode));
        }

        if ( !ModelState.IsValid )
        {
            form.SubmissionToken = contactFormProtector.CreateToken();
            return View(new ContactPageViewModel
            {
                Form = form ,
                IsSubmitted = false
            });
        }

        try
        {
            await contentQueryService.SubmitContactMessageAsync(new ContactFormRequest
            {
                Name = form.Name ,
                Email = form.Email ,
                Subject = form.Subject ,
                Message = form.Message
            } , cancellationToken);

            TempData["PublicSuccess"] = culture.IsFrench()
                ? "Votre message a bien ete envoye."
                : "Your message has been sent successfully.";
        }
        catch ( Exception exception )
        {
            logger.LogError(exception , "Failed to submit contact message.");
            ModelState.AddModelError(string.Empty , culture.IsFrench()
                ? "Une erreur est survenue lors de l'envoi du message. Veuillez reessayer."
                : "An error occurred while sending your message. Please try again.");
            form.SubmissionToken = contactFormProtector.CreateToken();

            return View(new ContactPageViewModel
            {
                Form = form ,
                IsSubmitted = false
            });
        }

        return RedirectToAction(nameof(Contact) , new { culture , submitted = true });
    }

    [HttpGet("/error/{statusCode?}")]
    [IgnoreAntiforgeryToken]
    public IActionResult Error(int? statusCode = null)
    {
        ViewData["StatusCode"] = statusCode;
        return View("~/Views/Shared/AppError.cshtml");
    }

    private string PrepareRequest(string culture)
    {
        culture = culture.NormalizeSiteCulture();
        HttpContext.ApplyCulture(culture);
        ViewBag.CurrentCulture = culture;
        return culture;
    }

    private ContactFormViewModel CreateContactFormViewModel()
    {
        return new ContactFormViewModel
        {
            SubmissionToken = contactFormProtector.CreateToken()
        };
    }

    private void SetPublicPageMetadata(FreelanceHub.Domain.Entities.SiteSetting siteSetting , string culture , string pageTitle)
    {
        ViewBag.SiteSetting = siteSetting;
        ViewBag.PageTitle = pageTitle;
        ViewBag.MetaTitle = pageTitle + " | " + siteSetting.SiteName;
        ViewBag.MetaDescription = siteSetting.Localize(culture , x => x.MetaDescriptionEn , x => x.MetaDescriptionFr);
    }

    private static string GetContactSecurityMessage(string culture , string errorCode)
    {
        return (culture.NormalizeSiteCulture() , errorCode) switch
        {
            // ("fr", "rate-limit") => "Trop de tentatives ont ete detectees. Veuillez reessayer plus tard.",
            ("fr", "fast") => "La soumission a ete jugee trop rapide. Veuillez reessayer.",
            ("fr", "expired") => "Le formulaire a expire. Veuillez recharger la page.",
            ("fr", _) => "La soumission du formulaire a ete bloquee pour des raisons de securite.",
            //  ("en", "rate-limit") => "Too many submissions were detected. Please try again later.",
            ("en", "fast") => "The form was submitted too quickly. Please try again.",
            ("en", "expired") => "The form expired. Please refresh the page and try again.",
            _ => "The form submission was blocked for security reasons."
        };
    }
}
