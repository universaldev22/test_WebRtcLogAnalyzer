# WebRTC Log Analysis

This project analyzes WebRTC log files and provides aggregated statistics on user activity and error counts. The application parses logs from a WebRTC backend system, processes user JOIN/LEAVE events, counts unique users, and aggregates error types (ERROR, CRITICAL, WARNING).

## Project Design
- The solution uses **ASP.NET Core** for the web API, which exposes an endpoint for log analysis.
- The **Swagger/OpenAPI** integration allows for easy API documentation and testing.
- This project is designed following the **Domain-Driven Design (DDD)** principles. The main reason for this approach is to ensure a clean separation between the core business logic and the infrastructure, improving the system's maintainability and scalability. By focusing on the core domain of log analysis and error aggregation, I was able to clearly define bounded contexts such as the **UserActivityEvent** in the **WebRtcLogAnalyzer.Domain** and the **ILogAnalysisService** in the **WebRtcLogAnalyzer.Application**. This allows the system to be flexible and easily extensible. The use of **entities**, **value objects**, and **domain services** within the **Domain** layer helped to model the problem space accurately and align the solution with real-world requirements. This approach ensures that future enhancements can be made easily without causing significant rework, as new features and requirements can be introduced in isolated layers.




## Assumptions

- The log files are stored in the `logs` directory in the project root.
- The WebRTC log file is named `webrtc_studio.log` and follows a specific log format (timestamps, event types, user IDs).
- The log data will be processed through a web API that exposes the results.

## Key Features

- **Log Parsing**: Parses the WebRTC log file to extract relevant user activity and error data.
- **User Activity**: Tracks JOIN and LEAVE events and calculates the total unique users.
- **Error Aggregation**: Counts different types of errors (ERROR, CRITICAL, WARNING).
- **Web API**: Exposes the aggregated data via an HTTP endpoint `/api/loganalysis`.

## Design Decisions

- The solution uses **ASP.NET Core** for the web API, which exposes an endpoint for log analysis.
- **Serilog** is used for logging the application’s runtime events and errors.
- The **Swagger/OpenAPI** integration allows for easy API documentation and testing.
- The system is designed to process logs asynchronously to handle large log files efficiently.

## Setup Instructions

### 1. Clone the Repository & Install dependencies

Clone the repository to your local machine:

```bash
git clone https://github.com/<your-github-username>/log-analysis.git

cd log-analysis

dotnet restore
```
### 2. Run the Application
```
dotnet run
``` 

### 3. Test

#### 3.1 Swagger Test
```
https://localhost:7234/swagger/index.html
```

#### 3.2 Unit Test
```
dotnet test
```
