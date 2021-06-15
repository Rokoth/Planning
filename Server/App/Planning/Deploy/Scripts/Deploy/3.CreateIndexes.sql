--user
create unique index uidx_user_login 
	on "user"("login") where not is_deleted;

create index idx_user_name
    on "user"("name");

create index idx_user_password
    on "user"("password");

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
--project
create unique index uidx_project_parent_id_name 
	on project(parent_id, "name") where not is_deleted;
create unique index uidx_project_parent_id_path_name 
	on project(parent_id, path_name) where not is_deleted;

create index idx_project_userid
    on project(userid);

create index idx_project_last_used_date
    on project(last_used_date);

create index idx_project_is_leaf
    on project(is_leaf);

create index idx_project_is_private
    on project(is_private);

create index idx_project_is_deleted
    on project(is_deleted);


--schedule
create unique index uidx_schedule_userid_order 
	on schedule(userid, "order") where not is_deleted;

create index idx_schedule_project_id
    on schedule(project_id);

create index idx_schedule_begin_date
    on schedule(begin_date);

create index idx_schedule_end_date
    on schedule(end_date);

create index idx_schedule_userid
    on schedule(userid);

create index idx_schedule_order
    on schedule("order");

--formula
create unique index uidx_formula_name 
	on formula("name") where not is_deleted;