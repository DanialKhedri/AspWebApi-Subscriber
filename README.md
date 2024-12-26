ASP.NET Core Data Processing API
Overview
This project is an ASP.NET Core Web API that connects to a message broker (such as RabbitMQ, Kafka, or Redis Pub/Sub), processes incoming data, stores it in a SQL Server database, and exposes the data through a REST API with pagination and minimal JWT authentication.

Prerequisites
.NET 9
SQL Server (you can use a local installation or a Docker version)
RabbitMQ / Kafka / Redis Pub/Sub (use a locally installed version for testing)
