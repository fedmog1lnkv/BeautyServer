CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE TABLE "Organizations" (
        "Id" uuid NOT NULL,
        "Name" character varying(50) NOT NULL,
        "Description" character varying(50),
        "Subscription" varchar(10) NOT NULL,
        "Color" character varying(7) NOT NULL,
        "Photo" character varying(2048),
        CONSTRAINT "PK_Organizations" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE TABLE "PhoneChallenges" (
        "Id" uuid NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "PhoneNumber" character varying(20) NOT NULL,
        "VerificationCode" character varying(10) NOT NULL,
        "ExpiredAt" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_PhoneChallenges" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE TABLE "Users" (
        "Id" uuid NOT NULL,
        "Name" character varying(20) NOT NULL,
        "PhoneNumber" varchar(20) NOT NULL,
        CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE TABLE "Services" (
        "Id" uuid NOT NULL,
        "OrganizationId" uuid NOT NULL,
        "Name" character varying(50) NOT NULL,
        "Description" character varying(50),
        "Duration" interval,
        "Price" double precision,
        CONSTRAINT "PK_Services" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Services_Organizations_OrganizationId" FOREIGN KEY ("OrganizationId") REFERENCES "Organizations" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE TABLE "Staffs" (
        "Id" uuid NOT NULL,
        "OrganizationId" uuid NOT NULL,
        "Name" character varying(100) NOT NULL,
        "PhoneNumber" character varying(15) NOT NULL,
        "Role" varchar(10) NOT NULL,
        "CreatedOnUtc" timestamp with time zone NOT NULL,
        "ModifiedOnUtc" timestamp with time zone,
        CONSTRAINT "PK_Staffs" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Staffs_Organizations_OrganizationId" FOREIGN KEY ("OrganizationId") REFERENCES "Organizations" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE TABLE "Venues" (
        "Id" uuid NOT NULL,
        "OrganizationId" uuid NOT NULL,
        "Name" character varying(50) NOT NULL,
        "Description" character varying(150),
        "Color" character varying(7) NOT NULL,
        "Photo" character varying(2048),
        "Latitude" double precision NOT NULL,
        "Longitude" double precision NOT NULL,
        CONSTRAINT "PK_Venues" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Venues_Organizations_OrganizationId" FOREIGN KEY ("OrganizationId") REFERENCES "Organizations" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE TABLE "ServiceStaff" (
        "ServicesId" uuid NOT NULL,
        "StaffsId" uuid NOT NULL,
        CONSTRAINT "PK_ServiceStaff" PRIMARY KEY ("ServicesId", "StaffsId"),
        CONSTRAINT "FK_ServiceStaff_Services_ServicesId" FOREIGN KEY ("ServicesId") REFERENCES "Services" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_ServiceStaff_Staffs_StaffsId" FOREIGN KEY ("StaffsId") REFERENCES "Staffs" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE TABLE "TimeSlots" (
        "Id" uuid NOT NULL,
        "StaffId" uuid NOT NULL,
        "VenueId" uuid NOT NULL,
        "Date" date NOT NULL,
        "Intervals" jsonb NOT NULL,
        CONSTRAINT "PK_TimeSlots" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_TimeSlots_Staffs_StaffId" FOREIGN KEY ("StaffId") REFERENCES "Staffs" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE TABLE "Record" (
        "Id" uuid NOT NULL,
        "UserId" uuid NOT NULL,
        "StaffId" uuid NOT NULL,
        "OrganizationId" uuid NOT NULL,
        "VenueId" uuid NOT NULL,
        "ServiceId" uuid NOT NULL,
        "Status" varchar(10) NOT NULL,
        "Comment" character varying(150),
        "StartTimestamp" timestamp with time zone NOT NULL,
        "EndTimestamp" timestamp with time zone NOT NULL,
        CONSTRAINT "PK_Record" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_Record_Organizations_OrganizationId" FOREIGN KEY ("OrganizationId") REFERENCES "Organizations" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Record_Services_ServiceId" FOREIGN KEY ("ServiceId") REFERENCES "Services" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Record_Staffs_StaffId" FOREIGN KEY ("StaffId") REFERENCES "Staffs" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Record_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_Record_Venues_VenueId" FOREIGN KEY ("VenueId") REFERENCES "Venues" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE TABLE "ServiceVenue" (
        "ServicesId" uuid NOT NULL,
        "VenuesId" uuid NOT NULL,
        CONSTRAINT "PK_ServiceVenue" PRIMARY KEY ("ServicesId", "VenuesId"),
        CONSTRAINT "FK_ServiceVenue_Services_ServicesId" FOREIGN KEY ("ServicesId") REFERENCES "Services" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_ServiceVenue_Venues_VenuesId" FOREIGN KEY ("VenuesId") REFERENCES "Venues" ("Id") ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE INDEX "IX_Record_OrganizationId" ON "Record" ("OrganizationId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE INDEX "IX_Record_ServiceId" ON "Record" ("ServiceId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE INDEX "IX_Record_StaffId" ON "Record" ("StaffId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE INDEX "IX_Record_UserId" ON "Record" ("UserId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE INDEX "IX_Record_VenueId" ON "Record" ("VenueId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE INDEX "IX_Services_OrganizationId" ON "Services" ("OrganizationId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE INDEX "IX_ServiceStaff_StaffsId" ON "ServiceStaff" ("StaffsId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE INDEX "IX_ServiceVenue_VenuesId" ON "ServiceVenue" ("VenuesId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE INDEX "IX_Staffs_OrganizationId" ON "Staffs" ("OrganizationId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE INDEX "IX_TimeSlots_StaffId" ON "TimeSlots" ("StaffId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE UNIQUE INDEX "IX_Users_PhoneNumber" ON "Users" ("PhoneNumber");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    CREATE INDEX "IX_Venues_OrganizationId" ON "Venues" ("OrganizationId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250329054944_InitialCreate') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20250329054944_InitialCreate', '9.0.3');
    END IF;
END $EF$;
COMMIT;

