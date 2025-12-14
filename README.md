🎬 WatchMate – Reward-Based Video Advertisement Platform

WatchMate is a package-based video advertisement reward system where users watch ads and earn rewards based on their subscribed package.
The platform controls who can watch which videos, how much reward they earn, and when ads are available.

⚠️ IMPORTANT NOTICE

This repository is for portfolio and interview demonstration purposes only.

❌ Unauthorized reuse
❌ Resale
❌ Commercial or production deployment

is strictly prohibited without written permission from the author.

🎯 Project Purpose

WatchMate is designed to demonstrate:

Controlled reward-based ad viewing

Package-wise content access

Real-world monetization logic

Clean architecture with scalable design

Secure user & reward management

🚀 Key Features
👤 User Management

User registration & authentication

Package-based access control

User reward balance tracking

View history logging

📦 Package Management

Multiple subscription packages

Package-wise:

Daily watch limits

Reward amount per video

Video accessibility rules

Package upgrade/downgrade support

🎥 Video Advertisement Management

Add & manage advertisement videos

Video metadata:

Title

Video URL

Start date & end date

Reward per view

Active/inactive video scheduling

Package-wise video visibility

💰 Reward System

Reward credited after successful video watch

Package-dependent reward calculation

Daily earning limits

Fraud prevention logic (duplicate views)

▶️ Video Player Experience

Facebook Reels–style video player

Next / Previous video navigation

Like & share actions

Watch completion validation before reward

📊 Admin Dashboard

Total views & rewards overview

Package performance analysis

Video performance tracking

User engagement metrics

🧩 Technical Use Cases
Backend (ASP.NET Core Web API)

Clean Architecture implementation

Repository Pattern + Unit of Work

LINQ-based filtering (package, date, status)

Secure reward calculation logic

Transaction-safe reward crediting

Caching for frequently accessed videos

Consistent API response format

Frontend (Angular)

Modular architecture with lazy loading

Custom video player logic

Package-aware UI behavior

API service separation from components

Reactive forms for admin panels

Clean UI & smooth user experience

Database Design

User, Package, Video, Reward tables

Watch history logging

Daily limits & constraints

Relational integrity & indexing

🛠️ Technology Stack
Layer	Technology
Frontend	Angular, TypeScript
Backend	ASP.NET Core Web API
Architecture	Clean Architecture
Patterns	Repository, Unit of Work
Database	SQL Server
Authentication	JWT
Video	URL-based streaming
Tools	LINQ, Background Services
🔐 Security & Control

JWT-based authentication

Package-level authorization

Watch-time validation

Reward manipulation prevention

Admin-only video & package control

📌 Use Case Scenarios

Ad-based earning platforms

Subscription-based reward systems

Video monetization platforms

Learning & engagement reward apps

📎 Disclaimer

WatchMate is a demonstration project, not a live earning platform.
It is built to showcase:

System design

Business logic handling

Secure reward workflows

Scalable architecture practices

👨‍💻 Author

Johir Raihan
Software Developer – ASP.NET Core & Angular
📍 Bangladesh
