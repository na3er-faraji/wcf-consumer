# Country Capital Info REST API

A **.NET Core REST API** that provides the capital city of a country by consuming the **CountryInfo SOAP Service**. This project follows **SOLID principles**, **Clean Code practices**, **resiliency patterns**, and **Separation of Concerns (SoC)**.

---

## Table of Contents

- [Features](#features)  
- [Requirements](#requirements)  
- [Installation & Running with Docker](#installation--running-with-docker)  
- [Usage](#usage)  
- [Architecture & Design](#architecture--design)  
- [Resiliency & Logging](#resiliency--logging)  
- [Error Handling](#error-handling)  

---

## Features

### SOAP Integration
- Consumes the `CapitalCity` method from the [CountryInfo SOAP Service](http://webservices.oorsprong.org/websamples.countryinfo/CountryInfoService.wso?WSDL).

### Resiliency & Decoupled API Calls
- Uses a fully **decoupled `ResilientApiExecutor`** that allows calling **any API**, any number of times, with **customizable retry policies and timeouts**.
- Handles retries, timeouts, and error propagation automatically.
- Fully **reusable across services**.

### Caching
- Implements a **separate cache layer** via a **`Cacheable` attribute**.
- Automatically caches API responses.
- **Decoupled** from business logic.

### Logging
- `ResilientApiExecutor` logs **both requests and responses** comprehensively.
- Provides **full observability** into API calls.

### REST Endpoint
- `GET /countries/{isoCode}/capital`  
- Returns a JSON object containing both the country code and capital:

```json
{
  "countryCode": "US",
  "capital": "Washington"
}
```

## Requirements
- Docker and Docker Compose installed
- Internet connection (to access the SOAP service)

## Installation & Running with Docker
Clone the repository:

```bash
git clone https://github.com/na3er-faraji/wcf-consumer.git
cd wcf-consumer
```

Build and start the API using Docker Compose:

```bash
docker-compose up --build
```

 The API will be available at http://localhost:8080/swagger

### To stop the API:

```bash
docker-compose down
```

## Usage
Endpoint
bash
Copy code
GET /countries/{isoCode}/capital
Path Parameter:
isoCode – ISO 2-letter country code (e.g., US, FR, DE).

Response Example:

```bash
{
  "countryCode": "US",
  "capital": "Washington"
}
```

## Architecture & Design

- Controller Layer: Handles REST requests (CountryController).

- Use Case / Handler Layer: GetCapitalHandler processes GetCapitalQuery.

- Service Layer: CountryService uses ResilientApiExecutor to call APIs and returns structured results.

- SOAP Client Layer: Wraps SOAP calls to CapitalCity.

- Resiliency & Caching: ResilientApiExecutor for retries/timeouts/logging; Cacheable attribute handles caching separately.

### Design Principles:

- SOLID: Single responsibility, dependency injection.

- Separation of Concerns (SoC)

- Reusability: ResilientApiExecutor for any API.

- Clean Code

## Resiliency & Logging

- Retry Policy: Up to 3 retries with exponential backoff using Polly.

- Timeouts: Prevent long waits for SOAP calls.

- Logging: Requests and responses fully logged (excluding sensitive data).

## Error Handling

- Graceful handling of SOAP service failures.

- Differentiates between client (400-level) and server (500-level) errors.

Examples:

```bash
{
  "error": "Country not found"
}
```
- NotFoundException → 404 Not Found

- ServiceUnavailableException → 503 Service Unavailable

- Generic exceptions → 500 Internal Server Error