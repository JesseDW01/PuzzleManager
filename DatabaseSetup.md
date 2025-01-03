# Self Signed Certificate

> **Note:** For true “publicly trusted” certificates, you need a real domain name and a public CA (e.g., via Let’s Encrypt or a paid provider). But in local dev scenarios, a self-signed certificate + trusting it locally is usually enough to avoid SSL errors.

## 1 Create or Obtain a Certificate

### A Use PowerShell New-SelfSignedCertificate

- Open Windows PowerShell as Administrator.

Run a command like:

```powershell

New-SelfSignedCertificate `
    -DnsName "localhost" `
    -CertStoreLocation "Cert:\LocalMachine\My" `
    -FriendlyName "SQL Express SSL Cert" `
    -KeyExportPolicy Exportable `
    -Type Custom `
    -KeyUsage DigitalSignature, KeyEncipherment `
    -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.1")


```

- `-DnsName` "localhost": The CN (common name) will be “localhost.”
- `-CertStoreLocation` Cert:\LocalMachine\My: Places the new cert into the LocalMachine “Personal” (My) store.

You can also add `-KeyExportPolicy` Exportable if you need to export the private key.
PowerShell returns a thumbprint if successful. That’s how you identify the cert in Windows.

## 2. Move (or Copy) the Certificate to “Trusted Root” Store

By default, self-signed certs are not trusted. You need to place the certificate in the Trusted Root Certification Authorities store. For example:

1. Open “Manage Computer Certificates” (type “certlm.msc” in the Start menu).
2. Navigate to Certificates (Local Computer) → Personal → Certificates.
   - You should see your newly created certificate (based on the thumbprint or friendly name).
3. Right-click that certificate → Copy.
4. Navigate to Certificates (Local Computer) → Trusted Root Certification Authorities → Certificates.
5. Right-click → Paste.
   - Confirm the prompt to place a copy in the trusted root store.

Now Windows “trusts” that certificate for SSL/TLS connections.

## 3 Configure SQL Server to Use This Certificate

SQL Server (Express or full) needs to be told which certificate to use for encryption.

1. Open SQL Server Configuration Manager (e.g., for SQL 2019/2022, look in “Microsoft SQL Server Tools”).
2. Expand SQL Server Network Configuration → Protocols for SQLEXPRESS (or whichever instance).
3. Right-click Protocols for ... → Properties (or open the “Certificate” tab).
4. In some SQL versions, you’ll see a Certificate drop-down with all server certificates. Pick the new “Local SQL SSL Cert” if it’s visible.
5. Ensure Force Encryption is set to Yes if you want all connections to require encryption.
6. Restart the SQL Server service for changes to take effect.
*(Exact steps vary depending on your SQL Server version. In some, you must set a registry key or a check box for the certificate, then restart.)*
