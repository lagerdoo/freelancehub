# Deploy on OVH Linux VPS with PostgreSQL

This application is prepared for a low-cost OVH Linux VPS deployment with PostgreSQL and Nginx.

## Production architecture

- Ubuntu VPS on OVHcloud
- ASP.NET Core app running behind `systemd`
- Nginx reverse proxy in front of Kestrel
- PostgreSQL on the VPS or managed separately
- Environment variables for secrets and connection strings

## Recommended environment variables

Use environment variables instead of storing production secrets in JSON files.

```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://127.0.0.1:5000
Database__Provider=PostgreSql
ConnectionStrings__PostgreSql=Host=127.0.0.1;Port=5432;Database=freelancehub;Username=freelancehub_app;Password=REPLACE_ME;Pooling=true;Timeout=15;Command Timeout=30
SeedAdmin__Username=admin
SeedAdmin__Email=admin@example.com
SeedAdmin__DisplayName=Site Administrator
SeedAdmin__Password=REPLACE_WITH_STRONG_BOOTSTRAP_PASSWORD
Seeding__EnableDemoContent=false
```

After first successful production bootstrap, remove `SeedAdmin__Password` and manage the admin account through the application/database workflow you prefer.

## Publish

```bash
dotnet publish src/FreelanceHub.Web/FreelanceHub.Web.csproj -c Release -o /var/www/freelancehub/current
```

## Database

Create the PostgreSQL database and a dedicated least-privilege application user first.

Example bootstrap sequence:

```sql
CREATE DATABASE freelancehub;
CREATE USER freelancehub_app WITH ENCRYPTED PASSWORD 'REPLACE_ME';
GRANT CONNECT ON DATABASE freelancehub TO freelancehub_app;
\c freelancehub
GRANT USAGE, CREATE ON SCHEMA public TO freelancehub_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO freelancehub_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO freelancehub_app;
```

Apply migrations from the deployed host:

```bash
dotnet ef database update --project src/FreelanceHub.Infrastructure/FreelanceHub.Infrastructure.csproj --startup-project src/FreelanceHub.Web/FreelanceHub.Web.csproj
```

If `dotnet-ef` is not installed on the server, either install it locally as a tool or run migrations during CI/CD before the service restart.

## Nginx and systemd

Templates are included here:

- `deploy/ovh/freelancehub.service`
- `deploy/ovh/freelancehub.nginx.conf`
- `deploy/ovh/freelancehub.env.example`

Copy and adapt them for your server paths, domain, and credentials.

## OVH VPS hardening baseline

- Update the system immediately after provisioning.
- Use SSH keys instead of password-only access.
- Create a sudo-capable deploy user instead of working permanently as root.
- Restrict exposed ports to `22`, `80`, and `443`.
- Use UFW or iptables and enable the OVHcloud Network Firewall.
- Install Fail2ban for SSH protection.
- Keep automated backups or snapshots enabled.
- Terminate TLS at Nginx with Let’s Encrypt.

## App-specific production notes

- `appsettings.Production.json` is PostgreSQL-first and does not contain secrets.
- The app now fails fast if PostgreSQL is selected without `ConnectionStrings__PostgreSql`.
- Uploaded files are limited to raster image types and validated by signature.
- Security headers, forwarded headers, HSTS, secure cookies, and persistent Data Protection keys are enabled.
- Demo content seeding is disabled in production by default.

## First deployment sequence

1. Provision Ubuntu on the OVH VPS.
2. Update packages and install Nginx, PostgreSQL, and the ASP.NET Core runtime.
3. Create the application directory, deploy user, and environment file.
4. Publish the app into `/var/www/freelancehub/current`.
5. Install the `systemd` service file and enable it.
6. Install the Nginx site config and reload Nginx.
7. Set production environment variables.
8. Apply EF Core migrations.
9. Start the service and verify `/health`.
10. Issue TLS certificates and enable HTTPS.

## Service management commands

```bash
sudo systemctl daemon-reload
sudo systemctl enable freelancehub
sudo systemctl start freelancehub
sudo systemctl restart freelancehub
sudo systemctl status freelancehub
sudo journalctl -u freelancehub -n 200 --no-pager
sudo journalctl -u freelancehub -f
```

For Nginx:

```bash
sudo nginx -t
sudo systemctl reload nginx
sudo systemctl restart nginx
sudo systemctl status nginx
```

## TLS with Let’s Encrypt

Install Certbot and the Nginx plugin:

```bash
sudo apt update
sudo apt install -y certbot python3-certbot-nginx
```

Issue the certificate:

```bash
sudo certbot --nginx -d example.com -d www.example.com
```

Check renewal:

```bash
sudo certbot renew --dry-run
```

## Verification and rollback checks

Verify the application health endpoint:

```bash
curl -I http://127.0.0.1:5000/health
curl -I https://example.com/health
```

Verify PostgreSQL connectivity quickly from the VPS:

```bash
psql "host=127.0.0.1 port=5432 dbname=freelancehub user=freelancehub_app password=REPLACE_ME" -c "select 1;"
```

If startup fails:

- Check `sudo systemctl status freelancehub`
- Check `sudo journalctl -u freelancehub -n 200 --no-pager`
- Check `sudo nginx -t`
- Check `sudo systemctl status nginx`
- Re-verify the environment file and PostgreSQL connection string

Rollback approach:

1. Keep each deployment in a versioned release directory.
2. Repoint the active deployment symlink to the previous release.
3. Restart the app service.
4. Re-check `/health` and service logs.
