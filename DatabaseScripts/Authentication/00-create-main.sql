create table
  `main` (
    `id` int unsigned not null comment 'ID',
    `genre_name` TEXT not null comment 'the name of the genre',
    `description` TEXT null comment 'description of the genre',
    `table_name` TEXT not null comment 'the coresponding table name',
    `visible` BOOLEAN not null comment 'is it normally visible? for tables that may be turned off',
    primary key (`id`)
  );

alter table
  `main`
modify column
  `id` int unsigned not null auto_increment