
create table "issue" (
    id uuid default uuid_generate_v4(),

    title text not null,
    description text,
    effort_estimation integer,
    issue_type varchar(15) not null,
    status varchar(15) not null,

    /* Constraints */
    primary key (id)
);

create table "project_issue" (
    issue_id uuid not null,
    project_id uuid not null,

    /* Constraints */
    primary key (issue_id),
    foreign key (issue_id) references "issue" (id),
    foreign key (project_id) references "project" (id)
);

create table "feature_issue" (
    issue_id uuid not null,
    feature_id uuid not null,
    
    /* Constraints */
    primary key (issue_id),
    foreign key (issue_id) references "issue" (id),
    foreign key (feature_id) references "feature" (id)
);

create table "issue_user" (
    issue_id uuid not null,
    username text not null,

    /* Constraints */
    primary key (issue_id, username),
    foreign key (issue_id) references "issue" (id),
    foreign key (username) references "user" (username)
);
