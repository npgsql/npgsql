-- ===================================================================
--
-- This file contains tables that must be loaded in PostgreSQL for the
-- noninteractive tests to run
--
-- ===================================================================

--
-- Used in test_1
--
create table tableA(
field_serial serial,
field_text text,
field_int4 int4,
field_int8 int8,
field_bool bool
) with oids;

create table tableB(
field_serial serial primary key,
field_int2 int2,
field_timestamp timestamp,
field_numeric numeric(11,7)
);

create table tableC(
field_serial serial,
field_date date,
field_time time
);

create table tableD(
field_serial serial,
field_float4 float4,
field_float8 float8
);


create table tableE(
field_serial serial,
field_point point, 
field_box box, 
field_lseg lseg, 
field_path path, 
field_polygon polygon, 
field_circle circle
);

create table tableF(
field_serial serial,
field_bytea bytea
);

create table tableG(
field_serial serial,
field_timestamp_with_timezone timestamp with time zone
);

create table tableH(
field_serial serial,
field_char5 char(5),
field_varchar5 varchar(5)
);

create table tableI(
field_serial serial,
"Field_Case_Sensitive" int4
);

create table tableJ(
field_serial serial,
field_inet inet
);

create table tableK(
field_serial serial,
field_bit bit
);

create table "CaseSensitiveTable"(
field_serial serial
);

create table metadatatest1(
field_serial serial,
field_pk    int4 primary key
);

create table copy1(
t text
);

create table copy2(
field_int4 int4,
field_int8 int8,
field_text text,
field_timestamp timestamp,
field_bool bool
);
