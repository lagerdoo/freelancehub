# CV Import

## Purpose

The CV import feature is a private admin-only workflow designed to speed up template personalization.
It helps a new user upload a resume, review extracted suggestions, and selectively apply those suggestions to the website content.

## Supported format

- V1 supports text-based `PDF` files only.
- Scanned PDFs and OCR-based extraction are not supported.
- Uploaded CV files are stored privately under `App_Data/CvImports` and are not exposed through `wwwroot`.

## Import flow

1. Open `Admin > CV import`.
2. Upload a PDF resume.
3. The app stores the file privately and extracts readable text from the document.
4. The parser generates suggestions for:
   - site/profile summary
   - contact/professional info
   - skills
   - experience entries
   - suggested services
   - project suggestions when the text appears structured enough
5. Review the generated suggestions on the import review screen.
6. Edit any values before saving.
7. Choose which sections/items to apply.
8. Apply the selected changes.

## Save behavior

- Site/profile suggestions update the site settings only after confirmation.
- Imported services are saved as `unpublished` drafts.
- Imported projects are saved as `unpublished` drafts.
- Imported experience entries are saved as `hidden` entries.
- Nothing is published automatically.
- The uploaded CV file can be deleted from the review flow or from the imports list.

## Current limitations

- No OCR.
- No DOCX import in V1.
- French and English fields are initially populated from the same extracted text when no translation is available.
- Project extraction is intentionally conservative and may return no project suggestions.
- Extraction is heuristic-based and should be reviewed manually before applying.

## How to test

1. Start the app and sign into the admin area.
2. Go to `/admin/cv-import`.
3. Upload a text-based PDF with:
   - a visible name/header
   - a summary/profile section
   - a skills section
   - at least one experience entry with years
4. Confirm the review page shows extracted suggestions.
5. Edit one profile field, one service, and one experience entry.
6. Apply only the selected items.
7. Verify:
   - updated profile content appears in `Admin > Settings`
   - imported services appear in `Admin > Services` as drafts
   - imported experience entries appear in `Admin > Experience` as hidden
   - imported projects, if selected, appear in `Admin > Projects` as drafts
8. Test `Delete uploaded CV after apply`.
9. Return to `Admin > CV import` and verify the import status updates accordingly.
