using FreelanceHub.Application.Contracts;
using FreelanceHub.Application.Models;
using FreelanceHub.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Areas.Admin.Controllers;

[Route("admin/media")]
public sealed class MediaController(IMediaService mediaService) : AdminControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        SetPageTitle("Media library");
        return View(new MediaUploadViewModel
        {
            ExistingMedia = await mediaService.GetMediaAsync(cancellationToken)
        });
    }

    [HttpPost("")]
    public async Task<IActionResult> Index(MediaUploadViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.File is null)
        {
            SetPageTitle("Media library");
            model = new MediaUploadViewModel
            {
                AltTextFr = model.AltTextFr,
                AltTextEn = model.AltTextEn,
                ExistingMedia = await mediaService.GetMediaAsync(cancellationToken)
            };

            return View(model);
        }

        await using var stream = model.File.OpenReadStream();

        await mediaService.UploadAsync(new MediaUploadRequest
        {
            OriginalFileName = model.File.FileName,
            ContentType = model.File.ContentType,
            FileSize = model.File.Length,
            AltTextFr = model.AltTextFr,
            AltTextEn = model.AltTextEn,
            Content = stream
        }, cancellationToken);

        TempData["AdminSuccess"] = "Media uploaded successfully.";
        return RedirectToAction(nameof(Index));
    }
}
