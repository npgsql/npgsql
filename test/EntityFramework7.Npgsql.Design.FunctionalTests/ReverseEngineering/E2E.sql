DROP TABLE IF EXISTS "AllDataTypes";
CREATE TABLE "AllDataTypes" (
	"AllDataTypesID"    serial    PRIMARY KEY,

	"smallintColumn"    smallint            NOT NULL,
	"intColumn"         int                 NOT NULL,
	"bigintColumn"      bigint              NOT NULL,
	"realColumn"        real                NULL,
	"doubleColumn"      double precision    NOT NULL,
	"decimalColumn"     decimal             NOT NULL,
	"numericColumn"     numeric             NOT NULL,
	"moneyColumn"       money               NOT NULL,
					  	 	   	            
	"charColumn"        char                NULL,
	"textColumn"        text                NULL,
	"varcharColumn"     varchar             NULL,
					  	 	   	            
	"dateColumn"        date                NOT NULL,
	"timestampColumn"   timestamp           NULL,
	"timestampTzColumn" timestamptz         NULL,
	"timeColumn"        time                NULL,
								            
	"byteaColumn"       bytea               NULL,
	"boolColumn"        boolean             NOT NULL,
	"uuidColumn"        uuid                NULL
);

DROP TABLE IF EXISTS "PropertyConfiguration";
CREATE TABLE "PropertyConfiguration" (
	"PropertyConfigurationID" serial PRIMARY KEY,
--	"PropertyConfigurationID" tinyint IDENTITY(1, 1) PRIMARY KEY, -- tests error message about tinyint identity columns
	"WithDateDefaultExpression" timestamp NOT NULL DEFAULT (now()),
	"WithDateFixedDefault" timestamp NOT NULL DEFAULT ('2015-10-20 11:00:00'),
	"WithDateNullDefault" timestamp NULL DEFAULT (NULL),
--	"WithGuidDefaultExpression" "uniqueidentifier" NOT NULL DEFAULT (newsequentialid()),
	"WithVarcharNullDefaultValue" varchar NULL DEFAULT (NULL),
	"WithDefaultValue" int NOT NULL DEFAULT ((-1)),
	"WithNullDefaultValue" smallint NULL DEFAULT (NULL),
	"WithMoneyDefaultValue" money NOT NULL DEFAULT ((0.00)),
	"A" int NOT NULL,
	"B" int NOT NULL
);

CREATE INDEX "Test_PropertyConfiguration_Index"
	ON "PropertyConfiguration" ("A", "B");

DROP TABLE IF EXISTS "Test Spaces Keywords Table";
CREATE TABLE "Test Spaces Keywords Table" (
	"Test Spaces Keywords TableID" int PRIMARY KEY,
	"abstract" int NOT NULL,
	"class" int NULL,
	"volatile" int NOT NULL,
	"Spaces In Column" int NULL,
	"Tabs	In	Column" int NOT NULL,
	"@AtSymbolAtStartOfColumn" int NULL,
	"@Multiple@At@Symbols@In@Column" int NOT NULL,
	"Commas,In,Column" int NULL,
	"$Dollar$Sign$Column" int NOT NULL,
	"!Exclamation!Mark!Column" int NULL,
	"""Double""Quotes""Column" int NULL,
	"\Backslashes\In\Column" int NULL
);

DROP TABLE IF EXISTS "SelfReferencing";
CREATE TABLE "SelfReferencing" (
	"SelfReferencingID" int PRIMARY KEY,
	"Name" text NOT NULL,
	"Description" text NOT NULL,
	"SelfReferenceFK" int NULL,
	CONSTRAINT "FK_SelfReferencing" FOREIGN KEY 
	(
		"SelfReferenceFK"
	) REFERENCES "SelfReferencing" (
		"SelfReferencingID"
	)
);

DROP TABLE IF EXISTS "OneToManyPrincipal";
CREATE TABLE "OneToManyPrincipal" (
	"OneToManyPrincipalID1" int,
	"OneToManyPrincipalID2" int,
	"Other" text NOT NULL,
	CONSTRAINT "PK_OneToManyPrincipal" PRIMARY KEY
	(
		"OneToManyPrincipalID1", "OneToManyPrincipalID2"
	)
);

DROP TABLE IF EXISTS "OneToManyDependent";
CREATE TABLE "OneToManyDependent" (
	"OneToManyDependentID1" int,
	"OneToManyDependentID2" int,
	"SomeDependentEndColumn" text NOT NULL,
	"OneToManyDependentFK2" int NULL, -- deliberately put FK columns in other order to make sure we get correct order in key
	"OneToManyDependentFK1" int NULL,
	CONSTRAINT "PK_OneToManyDependent" PRIMARY KEY
	(
		"OneToManyDependentID1", "OneToManyDependentID2"
	),
	CONSTRAINT "FK_OneToManyDependent" FOREIGN KEY 
	(
		"OneToManyDependentFK1", "OneToManyDependentFK2"
	) REFERENCES "OneToManyPrincipal" (
		"OneToManyPrincipalID1", "OneToManyPrincipalID2"
	)
);

DROP TABLE IF EXISTS "OneToOnePrincipal";
CREATE TABLE "OneToOnePrincipal" (
	"OneToOnePrincipalID1" int,
	"OneToOnePrincipalID2" int,
	"SomeOneToOnePrincipalColumn" text NOT NULL,
	CONSTRAINT "PK_OneToOnePrincipal" PRIMARY KEY
	(
		"OneToOnePrincipalID1", "OneToOnePrincipalID2"
	)
);

DROP TABLE IF EXISTS "OneToOneDependent";
CREATE TABLE "OneToOneDependent" (
	"OneToOneDependentID1" int,
	"OneToOneDependentID2" int,
	"SomeDependentEndColumn" text NOT NULL,
	CONSTRAINT "PK_OneToOneDependent" PRIMARY KEY
	(
		"OneToOneDependentID1", "OneToOneDependentID2"
	),
	CONSTRAINT "FK_OneToOneDependent" FOREIGN KEY 
	(
		"OneToOneDependentID1", "OneToOneDependentID2"
	) REFERENCES "OneToOnePrincipal" (
		"OneToOnePrincipalID1", "OneToOnePrincipalID2"
	)
);

DROP TABLE IF EXISTS "OneToOneSeparateFKPrincipal";
CREATE TABLE "OneToOneSeparateFKPrincipal" (
	"OneToOneSeparateFKPrincipalID1" int,
	"OneToOneSeparateFKPrincipalID2" int,
	"SomeOneToOneSeparateFKPrincipalColumn" text NOT NULL,
	CONSTRAINT "PK_OneToOneSeparateFKPrincipal" PRIMARY KEY
	(
		"OneToOneSeparateFKPrincipalID1", "OneToOneSeparateFKPrincipalID2"
	)
);

DROP TABLE IF EXISTS "OneToOneSeparateFKDependent";
CREATE TABLE "OneToOneSeparateFKDependent" (
	"OneToOneSeparateFKDependentID1" int,
	"OneToOneSeparateFKDependentID2" int,
	"SomeDependentEndColumn" text NOT NULL,
	"OneToOneSeparateFKDependentFK1" int NULL,
	"OneToOneSeparateFKDependentFK2" int NULL,
	CONSTRAINT "PK_OneToOneSeparateFKDependent" PRIMARY KEY
	(
		"OneToOneSeparateFKDependentID1", "OneToOneSeparateFKDependentID2"
	),
	CONSTRAINT "FK_OneToOneSeparateFKDependent" FOREIGN KEY 
	(
		"OneToOneSeparateFKDependentFK1", "OneToOneSeparateFKDependentFK2"
	) REFERENCES "OneToOneSeparateFKPrincipal" (
		"OneToOneSeparateFKPrincipalID1", "OneToOneSeparateFKPrincipalID2"
	),
	CONSTRAINT "UK_OneToOneSeparateFKDependent" UNIQUE
	(
		"OneToOneSeparateFKDependentFK1", "OneToOneSeparateFKDependentFK2"
	)
);

DROP TABLE IF EXISTS "OneToOneFKToUniqueKeyPrincipal";
CREATE TABLE "OneToOneFKToUniqueKeyPrincipal" (
	"OneToOneFKToUniqueKeyPrincipalID1" int,
	"OneToOneFKToUniqueKeyPrincipalID2" int,
	"SomePrincipalColumn" text NOT NULL,
	"OneToOneFKToUniqueKeyPrincipalUniqueKey1" int NOT NULL,
	"OneToOneFKToUniqueKeyPrincipalUniqueKey2" int NOT NULL,
	CONSTRAINT "PK_OneToOneFKToUniqueKeyPrincipal" PRIMARY KEY
	(
		"OneToOneFKToUniqueKeyPrincipalID1", "OneToOneFKToUniqueKeyPrincipalID2"
	),
	CONSTRAINT "UK_OneToOneFKToUniqueKeyPrincipal" UNIQUE
	(
		"OneToOneFKToUniqueKeyPrincipalUniqueKey1", "OneToOneFKToUniqueKeyPrincipalUniqueKey2"
	)
);

DROP TABLE IF EXISTS "OneToOneFKToUniqueKeyDependent";
CREATE TABLE "OneToOneFKToUniqueKeyDependent" (
	"OneToOneFKToUniqueKeyDependentID1" int,
	"OneToOneFKToUniqueKeyDependentID2" int,
	"SomeColumn" text NOT NULL,
	"OneToOneFKToUniqueKeyDependentFK1" int NULL,
	"OneToOneFKToUniqueKeyDependentFK2" int NULL,
	CONSTRAINT "PK_OneToOneFKToUniqueKeyDependent" PRIMARY KEY
	(
		"OneToOneFKToUniqueKeyDependentID1", "OneToOneFKToUniqueKeyDependentID2"
	),
	CONSTRAINT "FK_OneToOneFKToUniqueKeyDependent" FOREIGN KEY 
	(
		"OneToOneFKToUniqueKeyDependentFK1", "OneToOneFKToUniqueKeyDependentFK2"
	) REFERENCES "OneToOneFKToUniqueKeyPrincipal" (
		"OneToOneFKToUniqueKeyPrincipalUniqueKey1", "OneToOneFKToUniqueKeyPrincipalUniqueKey2"
	),
	CONSTRAINT "UK_OneToOneFKToUniqueKeyDependent" UNIQUE
	(
		"OneToOneFKToUniqueKeyDependentFK1", "OneToOneFKToUniqueKeyDependentFK2"
	)
);

DROP TABLE IF EXISTS "FilteredOut";
CREATE TABLE "FilteredOut" (
	"FilteredOutID" int PRIMARY KEY,
	"Unused1" text NOT NULL,
	"Unused2" int NOT NULL
);