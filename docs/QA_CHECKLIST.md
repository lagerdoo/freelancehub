# QA Checklist

## Functional checks

- Public home page loads successfully at `/fr` and `/en`.
- Navigation links reach Home, About, Services, Projects, Experience, and Contact.
- Language switcher keeps the current page and switches between French and English.
- Homepage CTA buttons navigate to the expected pages.
- Contact form success message appears after a valid submission.
- Contact message is stored and visible in the admin inbox.
- Admin login succeeds with a valid account and fails with invalid credentials.
- Logout ends the admin session and blocks direct access to admin routes.

## Contact form validation and security

- Empty contact form cannot be submitted.
- Invalid email formats are rejected, including `ama@gmail`, missing `@`, or whitespace-only values.
- Name shorter than 2 characters is rejected.
- Subject shorter than 3 characters is rejected.
- Message shorter than 20 characters is rejected.
- Field length limits are enforced for name, email, subject, and message.
- Anti-forgery token is required and invalid token submissions are rejected.
- Honeypot field submissions are blocked.
- Very fast submissions are blocked by the minimum submission delay.
- Repeated submissions from the same client hit the basic anti-spam limit.
- Validation errors are shown client-side without requiring a server round trip.
- Validation errors are enforced again server-side if client-side validation is bypassed.

## UI and typography

- Typography hierarchy looks consistent across hero, section headings, cards, buttons, and footer.
- Headings do not wrap awkwardly on common desktop widths.
- Buttons have consistent spacing, hover states, and focus states.
- Cards and panels keep readable spacing and alignment.
- Brand, navigation, and footer feel visually consistent with the new typography system.
- Public and admin screens both use the intended font pairing and load correctly.

## Responsive checks

- Public pages render correctly on mobile width around `375px`.
- Public pages render correctly on tablet width around `768px`.
- Public pages render correctly on desktop width around `1440px`.
- Hero section remains readable and CTA buttons stay usable on small screens.
- Navigation collapses and expands correctly on mobile.
- Contact form layout stacks correctly on mobile.
- Admin layout remains usable on laptop and tablet widths.
- Admin mobile menu opens, closes, and remains usable without overlapping content.
- Long media filenames render cleanly on desktop and tablet widths without breaking cards or tables.

## Admin content management

- Service create, edit, and delete all work.
- Project create, edit, and delete all work.
- Experience entry create, edit, and delete all work.
- Site settings save successfully and changes appear on the public site.
- Media upload accepts valid raster images and rejects unsupported files.
- Uploaded image paths can be reused in content/settings fields.
- Contact message read/archive status updates persist correctly.
- Contact messages can also be deleted permanently.
- Services and Projects list rows are clickable and open the edit/detail screen.
- Contact message rows are clickable and open the details screen.
- Hover styling is consistent across admin tables.
- Delete actions open a confirmation dialog before submission.
- Back buttons are present on admin detail/edit screens.
- Message status icons display correctly for unread, read, and archived states.
- Service icon preview updates correctly in admin and matches the public Services UI.

## Language checks

- French content fields are displayed on `/fr`.
- English content fields are displayed on `/en`.
- Site settings bilingual fields render correctly in both languages.
- Contact success and security messages match the selected language.
- Contact validation messages are shown in the active site language.
- Server-side contact error messages are shown in the active site language.
- No public route falls back to the wrong language unexpectedly.

## Contact flow end-to-end

- The Send Message button submits successfully when the form is valid.
- Invalid client-side input blocks submission before the request is sent.
- Invalid server-side input returns the same page with localized error messages.
- After a successful submission, the success state is visible and the message appears in admin.

## Production readiness checks

- App starts with `ASPNETCORE_ENVIRONMENT=Production`.
- App starts with `Database__Provider=PostgreSql` and a valid `ConnectionStrings__PostgreSql`.
- App fails fast if PostgreSQL is selected without a configured connection string.
- Security headers are present in production responses.
- Admin authentication cookie is `HttpOnly`, `Secure`, and `SameSite=Lax`.
- Reverse proxy deployment works behind Nginx with forwarded headers.
- Health endpoint `/health` responds successfully.
- Data Protection keys persist across app restarts.
- Demo content seeding is disabled in production unless explicitly enabled.
