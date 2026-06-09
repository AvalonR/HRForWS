# Swagger Verification

## Purpose

This document verifies that Swagger is enabled and working in the HR System Web Services project.

## Verification Steps

1. The project was restored using:

```bash
dotnet restore
```

2. The project was built using:

```bash
dotnet build
```

3. The project was run using:

```bash
dotnet run
```

4. The Swagger UI was accessed at:

```text
http://localhost:5065/swagger
```

## Result

Swagger opened successfully and displayed the available API endpoint:

```text
GET /weatherforecast
```

## Evidence

A screenshot of the Swagger page was saved in:

```text
docs/screenshots/swagger-home.png
```

## Notes

At this stage, the project contains the default ASP.NET Core endpoint. More HR system endpoints will be added in later features.