-- init.sql

create table if not exists Brand (
    Id serial primary key,
    Name varchar(30) not null unique
);

create table if not exists Category (
    Id serial primary key,
    Name varchar(30) not null unique
);

create table if not exists Shoe (
    Id serial primary key,
    Name varchar(30) not null unique,
    Price integer not null,
    Size integer not null,
    Color varchar(30) not null,
    Quantity integer not null,
    BrandId integer not null,
    CategoryId integer not null,
    foreign key (BrandId) references Brand (Id),
    foreign key (CategoryId) references Category (Id)
);

create table if not exists Role (
    Id serial primary key,
    Name varchar(30) not null unique
);

create table if not exists Client (
    Id serial primary key,
    FirstName varchar(30) not null,
    LastName varchar(30) not null,
    Username varchar(30) not null unique,
    Password varchar(30) not null,
    Email varchar(30) not null unique,
    Phone varchar(30) not null unique,
    RoleId integer not null,
    foreign key (RoleId) references Role (Id)
);

insert into Client (FirstName, LastName, Username, Password, Email, Phone, RoleId)
values ('Dima', 'Oshuev', 'dimza', 'root', 'dimza@gmail.com', '+79229315106', 1),
       ('Egot', 'Ivonin', 'egar', '1234', 'egar@gmail.com', '+79229415612', 2);

create table if not exists Cart (
    Id serial primary key,
    ClientId integer not null,
    foreign key (ClientId) references Client (Id)
);

insert into Cart (ClientId) values (2);

create table if not exists CartItem (
    Id serial primary key,
    CartId integer not null,
    ShoeId integer not null,
    Quantity integer not null,
    foreign key (CartId) references Cart (Id),
    foreign key (ShoeId) references Shoe (Id)
);

insert into CartItem(CartId, ShoeId, Quantity) values (1, 2, 1);


create table if not exists PickupPoint (
    Id serial primary key,
    City varchar(30) not null unique,
    Address varchar(30) not null unique
);

create table if not exists "Order" (
    Id serial primary key,
    ClientId integer not null,
    OrderDate date not null,
    PickupPointId integer not null,
    foreign key (ClientId) references Client (Id),
    foreign key (PickupPointId) references PickupPoint (Id)
);

insert into "Order" (ClientId, OrderDate, PickupPointId)
values (1, '01-06-2024', 2);

create table if not exists OrderItem (
    Id serial primary key,
    OrderId integer not null,
    ShoeId integer not null,
    Quantity integer not null,
    Price integer not null,
    IsPickedUp bool not null,
    foreign key (OrderId) references "Order" (Id),
    foreign key (ShoeId) references Shoe (Id)
);

insert into OrderItem (OrderId, ShoeId, Quantity, Price, IsPickedUp)
values (1, 1, 1, 100, 'false');

insert into Brand (Name)
values ('Adidas'), ('Puma'), ('Nike'), ('Columbia'), ('New Balance'), ('Crocs');

insert into Category (Name)
values ('Сапоги'), ('Ботинки'), ('Туфли'), ('Домашняя обувь'), ('Пляжная обувь'), ('Спортивная обувь');

insert into Shoe (Name, Price, Size, Color, Quantity, BrandId, CategoryId)
values ('Ozelia', 9699, 45, 'Черный', 23, 1, 6),
       ('Precision Vi', 11899, 43, 'Бело-зеленый', 11, 3, 6),
       ('Sabo', 4500, 39, 'Белый', 34, 6, 5),
       ('Firecamp', 17000, 46, 'Черно-красный', 18, 4, 2);

insert into Role (Name)
values ('admin'), ('user'), ('moderator');

insert into PickupPoint (City, Address)
values ('Киров', 'пр. Октябрьский 11а'),
       ('Советск', 'ул. Ленина 7'),
       ('Санкт - Перербург', 'пр. Маршала Брюхера 87а');

select * from Brand;
select * from Category;
select * from Shoe;
select * from Role;
select * from Client;
select * from Cart;
select * from PickupPoint;
select * from CartItem;
select * from "Order";
select * from OrderItem;
