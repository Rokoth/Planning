create extension if not exists "uuid-ossp";

create table if not exists "user"(
      id            uuid          not null default uuid_generate_v4() primary key
	, "name"        varchar(100)  not null
	, "description" varchar(1000) null
	, "login"       varchar(100)  not null
	, "password"    bytea         not null
	, version_date  timestamptz   not null default now()
	, is_deleted    boolean       not null
);

create table if not exists "h_user"(
      h_id          bigserial     not null primary key        
    , id            uuid          null
	, "name"        varchar(100)  null
	, "description" varchar(1000) null
	, "login"       varchar(100)  null
	, "password"    bytea         null
	, version_date  timestamptz   null
	, is_deleted    boolean       null
	, change_date   timestamptz   not null default now()
	, "user_id"     varchar       null
);

create table if not exists settings(	 
	  id            int           not null primary key
	, param_name    varchar(100)  not null
	, param_value   varchar(1000) not null	
);

create table if not exists schedule(	 
	  id		   uuid        not null primary key
	, project_id   uuid        not null
	, userid       uuid        not null
	, "order"      bigint      not null
	, begin_date   timestamptz not null
	, end_date	   timestamptz not null	
	, elapsed_time int         not null default 0
	, version_date timestamptz not null
	, is_deleted   boolean     not null default false
);

create table if not exists schedule_cursor(	 
	  id		   uuid        not null primary key
	, schedule_id  uuid        not null
	, userid       uuid        not null	
	, begin_date   timestamptz not null
	, end_date	   timestamptz not null	
	, elapsed_time int         not null default 0
	, version_date timestamptz not null
	, is_deleted   boolean     not null default false
);

create table if not exists project(
	  id             uuid          not null primary key
	, "name"         varchar(100)  not null
	, path_name      varchar(255)  not null
	, "description"  varchar(1000) null
	, "userid"       uuid          not null
	, parent_id      uuid          null
	, "priority"     int           not null
	, "period"       int           not null
	, cycle_period   int           not null
	, add_time       int           not null
	, last_used_date timestamptz   null
	, select_count   int           null
	, is_leaf        bool          not null
	, add_fields     json          null
	, is_private     bool          not null
	, version_date   timestamptz   not null
	, is_deleted     boolean       not null	
);

create table if not exists h_project(
	  h_id           bigserial     not null primary key
	, id             uuid          null
	, "name"         varchar(100)  null
	, path_name      varchar(255)  null
	, "description"  varchar(1000) null
	, "userid"       uuid          null
	, parent_id      uuid          null
	, "priority"     int           null
	, "period"       int           null
	, cycle_period   int           null
	, add_time       int           null
	, last_used_date timestamptz   null
	, select_count   int           null
	, is_leaf        bool          null
	, add_fields     json          null
	, is_private     bool          null
	, version_date   timestamptz   null
	, is_deleted     boolean       null
	, "user_id"      varchar       null
	, change_date    timestamptz   not null
);


create table if not exists h_schedule(	 
      h_id         bigserial       not null primary key
	, id		   uuid            null
	, project_id   uuid            null
	, userid       uuid            null
	, "order"      bigint          null
	, begin_date   timestamptz     null
	, end_date	   timestamptz     null	
	, elapsed_time int             null
	, version_date timestamptz     null
	, is_deleted   boolean         null
	, "user_id"    varchar         null
	, change_date  timestamptz     not null
);

create table if not exists formula(
	  id            uuid          not null default uuid_generate_v4() primary key
	, "name"        varchar(100)  not null
	, "text"        varchar(1000) not null	
	, is_default    boolean       not null
	, version_date  timestamptz   not null default now()
	, is_deleted    boolean       not null
);

create table if not exists h_formula(
      h_id          bigserial     not null primary key
	, id            uuid          null
	, "name"        varchar(100)  null
	, "text"        varchar(1000) null	
	, is_default    boolean       null
	, version_date  timestamptz   null
	, is_deleted    boolean       null
	, "user_id"     varchar       null
	, change_date   timestamptz   not null
);







