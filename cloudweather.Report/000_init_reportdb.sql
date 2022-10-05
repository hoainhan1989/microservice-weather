CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;


DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20221004095904_inital-migration') THEN
    CREATE TABLE "weather-report" (
        "Id" uuid NOT NULL,
        "CreatedOn" timestamp with time zone NOT NULL,
        "AverageHighF" numeric NOT NULL,
        "AverageLowF" numeric NOT NULL,
        "RainfallTotalInches" numeric NOT NULL,
        "SnowTotalInches" numeric NOT NULL,
        "ZipCode" text NOT NULL,
        CONSTRAINT "PK_weather-report" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20221004095904_inital-migration') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20221004095904_inital-migration', '6.0.3');
    END IF;
END $EF$;
COMMIT;

