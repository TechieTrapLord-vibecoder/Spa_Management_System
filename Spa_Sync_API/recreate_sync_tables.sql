-- Drop existing sync tables and recreate with new schema
-- Run this on MonsterASP database after deploying new API

-- First, drop all existing tables (in correct order due to dependencies)
IF OBJECT_ID('sync_journal_entries', 'U') IS NOT NULL DROP TABLE sync_journal_entries;
IF OBJECT_ID('sync_payrolls', 'U') IS NOT NULL DROP TABLE sync_payrolls;
IF OBJECT_ID('sync_inventories', 'U') IS NOT NULL DROP TABLE sync_inventories;
IF OBJECT_ID('sync_expenses', 'U') IS NOT NULL DROP TABLE sync_expenses;
IF OBJECT_ID('sync_payments', 'U') IS NOT NULL DROP TABLE sync_payments;
IF OBJECT_ID('sync_sales', 'U') IS NOT NULL DROP TABLE sync_sales;
IF OBJECT_ID('sync_appointments', 'U') IS NOT NULL DROP TABLE sync_appointments;
IF OBJECT_ID('sync_products', 'U') IS NOT NULL DROP TABLE sync_products;
IF OBJECT_ID('sync_services', 'U') IS NOT NULL DROP TABLE sync_services;
IF OBJECT_ID('sync_employees', 'U') IS NOT NULL DROP TABLE sync_employees;
IF OBJECT_ID('sync_customers', 'U') IS NOT NULL DROP TABLE sync_customers;
IF OBJECT_ID('sync_persons', 'U') IS NOT NULL DROP TABLE sync_persons;
IF OBJECT_ID('sync_devices', 'U') IS NOT NULL DROP TABLE sync_devices;
IF OBJECT_ID('__EFMigrationsHistory', 'U') IS NOT NULL DROP TABLE __EFMigrationsHistory;

PRINT 'Old tables dropped. EF Core will create new tables on first API run.';
