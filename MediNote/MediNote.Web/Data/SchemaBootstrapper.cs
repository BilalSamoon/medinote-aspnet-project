using System;
using System.Linq;
using MediNote.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace MediNote.Web.Data
{
    /// <summary>
    /// By: Camila Esguerra
    ///
    /// Repairs older local databases so the latest app version can still start
    /// without requiring the user to manually drop and recreate the database. Automated if you may lol
    /// </summary>
    public static class SchemaBootstrapper
    {
        public static void EnsureCompatibleSchema(MediNoteDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Database.IsSqlServer())
            {
                EnsureSeedData(context);
                return;
            }

            context.Database.ExecuteSqlRaw(@"
IF COL_LENGTH('Users', 'Email') IS NULL
BEGIN
    ALTER TABLE [Users] ADD [Email] nvarchar(max) NOT NULL CONSTRAINT [DF_Users_Email] DEFAULT N'';
END
");
            context.Database.ExecuteSqlRaw(@"
IF COL_LENGTH('Users', 'FirstName') IS NULL
BEGIN
    ALTER TABLE [Users] ADD [FirstName] nvarchar(max) NOT NULL CONSTRAINT [DF_Users_FirstName] DEFAULT N'';
END

IF COL_LENGTH('Users', 'LastName') IS NULL
BEGIN
    ALTER TABLE [Users] ADD [LastName] nvarchar(max) NOT NULL CONSTRAINT [DF_Users_LastName] DEFAULT N'';
END

IF COL_LENGTH('Users', 'SecurityId') IS NULL
BEGIN
    ALTER TABLE [Users] ADD [SecurityId] nvarchar(max) NOT NULL CONSTRAINT [DF_Users_SecurityId] DEFAULT N'';
END
");


            context.Database.ExecuteSqlRaw(@"
IF COL_LENGTH('Appointments', 'ContactRecipient') IS NULL
BEGIN
    ALTER TABLE [Appointments] ADD [ContactRecipient] nvarchar(max) NOT NULL CONSTRAINT [DF_Appointments_ContactRecipient] DEFAULT N'';
END

IF COL_LENGTH('Appointments', 'NotificationChannel') IS NULL
BEGIN
    ALTER TABLE [Appointments] ADD [NotificationChannel] nvarchar(max) NOT NULL CONSTRAINT [DF_Appointments_NotificationChannel] DEFAULT N'InApp';
END

IF COL_LENGTH('Appointments', 'CreatedAtUtc') IS NULL
BEGIN
    ALTER TABLE [Appointments] ADD [CreatedAtUtc] datetime2 NOT NULL CONSTRAINT [DF_Appointments_CreatedAtUtc] DEFAULT SYSUTCDATETIME();
END

IF COL_LENGTH('Appointments', 'LastUpdatedAtUtc') IS NULL
BEGIN
    ALTER TABLE [Appointments] ADD [LastUpdatedAtUtc] datetime2 NOT NULL CONSTRAINT [DF_Appointments_LastUpdatedAtUtc] DEFAULT SYSUTCDATETIME();
END
");

            context.Database.ExecuteSqlRaw(@"
IF OBJECT_ID(N'[SecurityCodes]', N'U') IS NULL
BEGIN
    CREATE TABLE [SecurityCodes]
    (
        [Id] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Code] nvarchar(max) NOT NULL,
        [Role] nvarchar(max) NOT NULL,
        [IsClaimed] bit NOT NULL
    );
END
");

            context.Database.ExecuteSqlRaw(@"
IF OBJECT_ID(N'[DoctorNotes]', N'U') IS NULL
BEGIN
    CREATE TABLE [DoctorNotes]
    (
        [DoctorNoteId] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [AppointmentId] int NOT NULL,
        [DoctorName] nvarchar(max) NOT NULL,
        [Note] nvarchar(max) NOT NULL,
        [FollowUpInstructions] nvarchar(max) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL
    );
END

IF OBJECT_ID(N'[Prescriptions]', N'U') IS NULL
BEGIN
    CREATE TABLE [Prescriptions]
    (
        [PrescriptionId] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [AppointmentId] int NOT NULL,
        [DoctorName] nvarchar(max) NOT NULL,
        [MedicationName] nvarchar(max) NOT NULL,
        [Dosage] nvarchar(max) NOT NULL,
        [Frequency] nvarchar(max) NOT NULL,
        [Duration] nvarchar(max) NOT NULL,
        [Instructions] nvarchar(max) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL
    );
END

IF OBJECT_ID(N'[NotificationLogs]', N'U') IS NULL
BEGIN
    CREATE TABLE [NotificationLogs]
    (
        [NotificationLogId] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [AppointmentId] int NOT NULL,
        [PatientName] nvarchar(max) NOT NULL,
        [Channel] nvarchar(max) NOT NULL,
        [Type] nvarchar(max) NOT NULL,
        [Recipient] nvarchar(max) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL
    );
END
");

            context.Database.ExecuteSqlRaw(@"
UPDATE [Users]
SET [Email] = CASE
    WHEN ISNULL([Email], N'') = N'' AND ISNULL([Username], N'') <> N'' THEN [Username] + N'@medinote.local'
    WHEN ISNULL([Email], N'') = N'' THEN N'user@medinote.local'
    ELSE [Email]
END;
");
            context.Database.ExecuteSqlRaw(@"
UPDATE [Users]
SET [FirstName] = CASE WHEN ISNULL([FirstName], N'') = N'' THEN [Username] ELSE [FirstName] END,
    [LastName] = CASE WHEN ISNULL([LastName], N'') = N'' THEN [Role] ELSE [LastName] END,
    [SecurityId] = ISNULL([SecurityId], N'');
");


            context.Database.ExecuteSqlRaw(@"
UPDATE [Appointments]
SET [NotificationChannel] = CASE WHEN ISNULL([NotificationChannel], N'') = N'' THEN N'InApp' ELSE [NotificationChannel] END,
    [ContactRecipient] = ISNULL([ContactRecipient], N''),
    [CreatedAtUtc] = CASE WHEN [CreatedAtUtc] = '0001-01-01T00:00:00.0000000' THEN SYSUTCDATETIME() ELSE [CreatedAtUtc] END,
    [LastUpdatedAtUtc] = CASE WHEN [LastUpdatedAtUtc] = '0001-01-01T00:00:00.0000000' THEN SYSUTCDATETIME() ELSE [LastUpdatedAtUtc] END,
    [Status] = CASE WHEN [Status] = N'Rejected' THEN N'Cancelled' ELSE [Status] END;
");

            EnsureSeedData(context);
        }

        private static void EnsureSeedData(MediNoteDbContext context)
        {
            EnsureUser(context, "doctor1", "password123", "Doctor", "DOC123", "John", "Doe", "doctor1@medinote.local");
            EnsureUser(context, "admin1", "adminpassword", "Admin", "ADM123", "Alice", "Admin", "admin1@medinote.local");
            EnsureUser(context, "patient1", "patientpassword", "Patient", string.Empty, "Bob", "Patient", "patient1@medinote.local");

            EnsureSecurityCode(context, "DOC123", "Doctor", true);
            EnsureSecurityCode(context, "ADM123", "Admin", true);
            EnsureSecurityCode(context, "ADM456", "Admin", false);
            EnsureSecurityCode(context, "ADM789", "Admin", false);

            context.SaveChanges();
        }

        private static void EnsureUser(
            MediNoteDbContext context,
            string username,
            string password,
            string role,
            string securityId,
            string firstName,
            string lastName,
            string email)
        {
            var user = context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                context.Users.Add(new User
                {
                    Username = username,
                    Password = password,
                    Role = role,
                    SecurityId = securityId,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                });
                return;
            }

            var changed = false;
            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                user.FirstName = firstName;
                changed = true;
            }
            if (string.IsNullOrWhiteSpace(user.LastName))
            {
                user.LastName = lastName;
                changed = true;
            }
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                user.Email = email;
                changed = true;
            }
            if ((role == "Doctor" || role == "Admin") && string.IsNullOrWhiteSpace(user.SecurityId))
            {
                user.SecurityId = securityId;
                changed = true;
            }

            if (changed)
            {
                context.Users.Update(user);
            }
        }

        private static void EnsureSecurityCode(MediNoteDbContext context, string code, string role, bool isClaimed)
        {
            var existing = context.SecurityCodes.FirstOrDefault(c => c.Code == code);
            if (existing == null)
            {
                context.SecurityCodes.Add(new SecurityCode
                {
                    Code = code,
                    Role = role,
                    IsClaimed = isClaimed
                });
                return;
            }

            var changed = false;
            if (!string.Equals(existing.Role, role, StringComparison.OrdinalIgnoreCase))
            {
                existing.Role = role;
                changed = true;
            }
            if (existing.IsClaimed != isClaimed)
            {
                existing.IsClaimed = isClaimed;
                changed = true;
            }

            if (changed)
            {
                context.SecurityCodes.Update(existing);
            }
        }
    }
}
