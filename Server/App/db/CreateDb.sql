CREATE DATABASE project
  WITH OWNER = postgres
       ENCODING = 'UTF8'
       TABLESPACE = pg_default
       LC_COLLATE = 'Russian_Russia.1251'
       LC_CTYPE = 'Russian_Russia.1251'
       CONNECTION LIMIT = -1;

CREATE SCHEMA project AUTHORIZATION postgres;
	   
drop extension if exists "uuid-ossp";

create extension "uuid-ossp" schema public;

drop table if exists project.project;

create table project.project
(
     id uuid not null primary key
   , "name" varchar not null
   , path varchar not null
   , parent_id uuid
   , is_project boolean not null default true
   , is_deleted boolean not null default false
   , last_used_date timestamptz
   , version_date timestamptz not null
   , period int not null
   , priority int not null
);

drop table if exists project.project_period;

create table project.project_period
(
     id uuid not null primary key
   , project_id uuid not null
   , version_date timestamptz not null
   , date_begin timestamptz
   , date_end timestamptz
   , is_closed boolean not null default false  
   , is_deleted boolean not null default false
);