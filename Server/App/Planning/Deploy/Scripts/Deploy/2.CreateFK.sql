--schedule
alter table schedule 
add constraint fk_schedule_user_id 
	foreign key(userid) 
		references "user"(id) 
		on delete no action on update no action;

alter table schedule 
add constraint fk_schedule_project_id 
	foreign key(project_id) 
		references project(id) 
		on delete no action on update no action;



--project
alter table project 
add constraint fk_project_user_id 
	foreign key(userid) 
		references "user"(id) 
		on delete no action on update no action;

alter table project 
add constraint fk_project_parent_id 
	foreign key(parent_id) 
		references project(id) 
		on delete no action on update no action;


--user
alter table "user" 
add constraint fk_user_formula_id 
	foreign key(formula_id) 
		references formula(id) 
		on delete no action on update no action;


