CREATE TABLE IF NOT EXISTS "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Login" VARCHAR(50) NOT NULL UNIQUE,
    "PasswordHash" TEXT NOT NULL,
    "Role" INTEGER NOT NULL,
    "FullName" VARCHAR(100) NOT NULL,
    "Phone" VARCHAR(20),
    "PassportData" VARCHAR(200)
);

CREATE TABLE IF NOT EXISTS "RoomTypes" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(50) NOT NULL,
    "Description" VARCHAR(200),
    "BasePrice" DECIMAL(10,2) NOT NULL
);

CREATE TABLE IF NOT EXISTS "Rooms" (
    "Id" SERIAL PRIMARY KEY,
    "Number" VARCHAR(10) NOT NULL UNIQUE,
    "RoomTypeId" INTEGER NOT NULL REFERENCES "RoomTypes"("Id"),
    "Floor" INTEGER NOT NULL,
    "PricePerDay" DECIMAL(10,2) NOT NULL,
    "Status" INTEGER NOT NULL,
    "Description" VARCHAR(200)
);

CREATE TABLE IF NOT EXISTS "Services" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Description" VARCHAR(200),
    "Price" DECIMAL(10,2) NOT NULL
);

CREATE TABLE IF NOT EXISTS "Bookings" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id"),
    "RoomId" INTEGER NOT NULL REFERENCES "Rooms"("Id"),
    "CheckIn" TIMESTAMP NOT NULL,
    "CheckOut" TIMESTAMP NOT NULL,
    "TotalAmount" DECIMAL(10,2) NOT NULL,
    "Status" INTEGER NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL
);

CREATE TABLE IF NOT EXISTS "BookingServices" (
    "Id" SERIAL PRIMARY KEY,
    "BookingId" INTEGER NOT NULL REFERENCES "Bookings"("Id"),
    "ServiceId" INTEGER NOT NULL REFERENCES "Services"("Id"),
    "Quantity" INTEGER NOT NULL,
    "TotalPrice" DECIMAL(10,2) NOT NULL
);

CREATE INDEX IF NOT EXISTS "IX_Bookings_UserId" ON "Bookings"("UserId");
CREATE INDEX IF NOT EXISTS "IX_Bookings_RoomId" ON "Bookings"("RoomId");
CREATE INDEX IF NOT EXISTS "IX_Rooms_RoomTypeId" ON "Rooms"("RoomTypeId");