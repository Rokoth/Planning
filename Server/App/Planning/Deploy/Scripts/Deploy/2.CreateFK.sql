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








