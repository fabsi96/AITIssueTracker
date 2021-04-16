
create table "feature" (
    id uuid default uuid_generate_v4(),
    project_id uuid not null,

    title text not null,
    description text not null,
    startdate timestamp,
    deadline timestamp,
    status varchar(15),

    /* Constraints */
    primary key (id),
    foreign key (project_id) references "project" (id)
);

create table "feature_user" (
    feature_id uuid not null,
    username text not null,

    /* Constraints */
    primary key(feature_id, username),
    foreign key (feature_id) references "feature" (id),
    foreign key (username) references "user" (username)
);
