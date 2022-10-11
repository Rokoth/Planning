--user
create unique index uidx_user_login 
	on "user"("login") where not is_deleted;

create index idx_user_name
    on "user"("name");

create index idx_user_password
    on "user"("password");
   
--user
create unique index uidx_user_settings_userid
	on user_settings(userid) where not is_deleted;

--project
create unique index uidx_project_parent_id_name 
	on project(parent_id, "name") where not is_deleted;
create unique index uidx_project_parent_id_path_name 
	on project(parent_id, "path") where not is_deleted;

create index idx_project_userid
    on project(userid);

create index idx_project_last_used_date
    on project(last_used_date);

create index idx_project_is_leaf
    on project(is_leaf);

create index idx_project_is_deleted
    on project(is_deleted);


--schedule

create index idx_schedule_project_id
    on schedule(project_id);

create index idx_schedule_begin_date
    on schedule(begin_date);

create index idx_schedule_end_date
    on schedule(end_date);

create index idx_schedule_userid
    on schedule(userid);


--formula
create unique index uidx_formula_name 
	on formula("name") where not is_deleted;