# Production schema snapshot

`Main.dacpac` is the whole application schema extracted with SqlPackage, without table data, permissions, or login mappings. It must be generated from the actual database, not inferred from EF models.

Run `bash scripts/refresh-e2e-schema.sh` from the repository after setting `E2E_SCHEMA_SOURCE_CONNECTION_STRING` securely. The script validates the candidate in disposable SQL Server 2025 before replacing the snapshot. Review and commit `Main.dacpac` after a successful refresh. Tests never run this command automatically and need no production credentials.

The initial snapshot is not supplied until source metadata access is available. Missing snapshots fail explicitly rather than silently switching to production or an EF-generated database.


```
read -rs 'E2E_SCHEMA_SOURCE_CONNECTION_STRING?Paste connection string: '
echo
export E2E_SCHEMA_SOURCE_CONNECTION_STRING
```

```
Server=your-server;Database=your-database;User Id=your-user;Password=your-password;Encrypt=True;TrustServerCertificate=True;
```

```
bash scripts/refresh-e2e-schema.sh
unset E2E_SCHEMA_SOURCE_CONNECTION_STRING
```