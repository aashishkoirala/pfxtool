---
title: PFX Tool
---
**PFX Tool** is a command line tool that does stuff with and around PFX certificates. Built for .NET Core developers that want to deal with PFX files without depending on PowerShell or certoc. It is a [.NET Core Global Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) that is built to run against the .NET Core 2.2 runtime. Tested on Windows and on Docker Containers based on Windows NanoServer and Alpine Linux. To install:

``` shell
dotnet tool install pfxtool -g
```

The basic usage is as follows:

``` shell
pfxtool <command> <options>
```

The following commands are currently supported:
- **import**: Import certificates and keys from a PFX file to a certificate store.
- **export**: Export certificates and keys from a certificate store to a PFX file.
- **remove**: Remove an existing certificate and keys from a certificate store.
- **list**: List all certificates in a certificate store or in a PFX file.
- **show**: Show details of a certificate in a certificate store or in a PFX file.

For details, run `pfxtool` without any options to get usage instructions.

## Examples

Import *test.pfx* (protected with password *Test123*) into the current user's personal store.

``` shell
pfxtool import --file test.pfx --password Test123 --scope user --store my
```

Export certificate and key with thumbprint *ABCDEF* from the machine's root certificate store into a file *test.pfx*, protecting the private key with password *Test123*.

``` shell
pfxtool export --file test.pfx --password Test123 --scope machine --store root
```