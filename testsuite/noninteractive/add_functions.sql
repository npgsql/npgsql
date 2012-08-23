-- ===================================================================
--
-- This file contains functions  that must be loaded in PostgreSQL for
-- the noninteractive tests to run
--
-- ===================================================================


--
-- Used in test_1
--
create function funcA() returns int as '
     select 0;

' language 'sql';


create function funcB() returns setof tablea as ' 
select * from tablea;
' language 'sql';


create function funcC() returns int8 as '
select count(*) from tablea;
' language 'sql';


create function funcC(int4) returns int8 as '
select count(*) from tablea where field_int4 = $1;
' language 'sql';

create function ambiguousParameterType(int2, int4, int8, text, varchar(10), char(5)) returns int4 as ' select 4 as result; ' language 'sql';

create function testreturnrecord() returns record as 'select 4 ,5' language 'sql' ;

create function testreturnsetofrecord() returns setof record as 'values (8,9), (6,7)' language 'sql';

CREATE OR REPLACE FUNCTION testmultcurfunc() RETURNS SETOF refcursor AS 'DECLARE ref1 refcursor; ref2 refcursor; BEGIN OPEN ref1 FOR SELECT 1; RETURN NEXT ref1; OPEN ref2 FOR SELECT 2; RETURN next ref2; RETURN; END;' LANGUAGE 'plpgsql';

CREATE OR REPLACE FUNCTION funcwaits() returns integer as 
' 
declare t integer;
begin

t := 0;

while t < 100000000 loop
t := t + 1;
end loop;

return t;
end;
'


LANGUAGE 'plpgsql';


create function testreturnrefcursor() returns refcursor as 
'
declare ref refcursor;
begin
	open ref for select * from tablea;
	return ref;
end;

' language 'plpgsql' volatile called on null input security invoker;


create function testreturnvoid() returns void as
'
begin
	insert into tablea(field_text) values (''testvoid'');
	return;
end;
' language 'plpgsql';

create function "FunctionCaseSensitive"(int4, text) returns int4 as
$BODY$
begin
	return 0;
end
$BODY$
language 'plpgsql';

create function testtimestamptzparameter(timestamptz) returns refcursor as
$BODY$
declare ref refcursor;
begin
	open ref for select * from tablea;
	return ref;
end
$BODY$
language 'plpgsql' volatile called on null input security invoker;

create or replace function testoutparameter(x int, y int, out sum int, out product int) returns record as 'select $1 + $2 , $1 * $2' language 'sql' ;

create or replace function testoutparameter2(x int, y int, out sum int, out product int) as 'select $1 + $2, $1 * $2' language 'sql';

create or replace function testreturnrecordresultset(a int4, b int4) returns table (a int4, b int4) as
$BODY$
begin
return query
select 1, 2;
end;
$BODY$
language 'plpgsql' 


