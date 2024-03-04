# KYC360 REST API

## Introduction

This project implements a REST API for managing Entity data using a mocked database. It provides CRUD endpoints for Creating, Reading, Updating, and Deleting Entities, along with additional features like searching and advanced filtering, timeout and pagination support is also included.
Please checkout the following video for the demo part:
[Video Link](https://drive.google.com/file/d/17fGSc5nAnG_EllW3NMS1Q2hBt773uS3C/view?usp=sharing)

## Getting Started

### Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) installed
- Your favorite code editor (e.g., Visual Studio Code, Visual Studio)

## Bonus Challenges
- Pagination and Sorting
The /api/entities endpoint supports pagination and sorting. Use page and pageSize query parameters for pagination, and sortField and sortOrder for sorting.
Retry and Backoff Mechanism
- Implemented a retry mechanism for database write operations with a backoff strategy.
Log relevant information about each retry attempt.
Adjust parameters for the backoff strategy based on system stability considerations.
