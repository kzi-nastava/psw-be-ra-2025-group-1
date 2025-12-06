-- Add TourExecutions table to tours schema
CREATE TABLE IF NOT EXISTS tours."TourExecutions" (
    "Id" bigserial PRIMARY KEY,
    "TouristId" bigint NOT NULL,
    "TourId" bigint NOT NULL,
    "Status" integer NOT NULL,
    "StartTime" timestamp without time zone NOT NULL,
    "EndTime" timestamp without time zone,
    "LastActivity" timestamp without time zone NOT NULL,
    "PercentageCompleted" double precision NOT NULL,
    CONSTRAINT "FK_TourExecutions_Tours" FOREIGN KEY ("TourId") 
        REFERENCES tours."Tour" ("Id") ON DELETE CASCADE
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS "IX_TourExecutions_TouristId" ON tours."TourExecutions" ("TouristId");
CREATE INDEX IF NOT EXISTS "IX_TourExecutions_TourId" ON tours."TourExecutions" ("TourId");
CREATE INDEX IF NOT EXISTS "IX_TourExecutions_Status" ON tours."TourExecutions" ("Status");
CREATE INDEX IF NOT EXISTS "IX_TourExecutions_StartTime" ON tours."TourExecutions" ("StartTime" DESC);

-- Verify creation
SELECT 'TourExecutions table created successfully' as message;
SELECT COUNT(*) as count FROM tours."TourExecutions";
