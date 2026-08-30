# RACEDAY-SYSTEM
The supplied brief identifies the core RaceDay functionality as authentication, user profiles, events, categories, event enrolments and results. The plan below uses those requirements to create a complete relational database and REST API design.
RaceDay Part 1 Planning

This project contains the planning and database design for the RaceDay system.

Documentation

The /docs folder contains:

Entity Relationship Diagram
API Endpoint Plan
SQL Server Database Script
Database Design

The RaceDay database contains six entities:

Roles
Users
Events
Categories
EventEnrollments
Results

The SQL database schema matches the ERD exactly. All primary keys, foreign keys and relationships represented in the ERD are implemented in the SQL Server database script.

Role-Based Design

The system supports three primary roles:

Administrator – manages users, events and system information.
Organizer – creates events, manages categories and records results.
Participant – registers for events and views personal enrolments.
Security Note

Passwords are represented in the database as PasswordHash. In the final application, passwords must never be stored as plain text. Passwords should be securely hashed before being stored in the database.
