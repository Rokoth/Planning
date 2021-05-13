--user
create unique index uidx_user_login 
	on "user"("login") where not is_deleted;

create index idx_user_name
    on "user"("name");

create index idx_user_password
    on "user"("password");

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