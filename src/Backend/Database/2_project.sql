
create table "project" (
    id uuid default uuid_generate_v4(),
    title text not null,
    description text,
    is_done boolean,

    /* Constraints */
    primary key (id)
);

create table "project_user" (
    project_id uuid not null,
    username text not null,

    /* Constraints */
    primary key(project_id, username),
    foreign key (project_id) references "project" (id),
    foreign key (username) references "user" (username)
);