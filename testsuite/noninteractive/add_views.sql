-- ===================================================================
--
-- This file contains views that must be loaded in PostgreSQL for the
-- noninteractive tests to run
--
-- ===================================================================


--
-- Used in test_1
--
create view tableA_view as 
select field_text, field_bool
from   tableA;