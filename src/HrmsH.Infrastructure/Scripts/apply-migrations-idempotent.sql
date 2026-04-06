IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [Appointments] (
        [Id] int NOT NULL IDENTITY,
        [PatientId] int NOT NULL,
        [DoctorId] int NULL,
        [DepartmentId] int NULL,
        [ScheduledStart] datetime2 NOT NULL,
        [ScheduledEnd] datetime2 NULL,
        [Status] int NOT NULL,
        [Reason] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] int NOT NULL IDENTITY,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [Facilities] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Code] nvarchar(50) NULL,
        [Address] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Facilities] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [Invoices] (
        [Id] int NOT NULL IDENTITY,
        [InvoiceNumber] nvarchar(50) NOT NULL,
        [PatientId] int NOT NULL,
        [InvoiceDate] datetime2 NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [PaidAmount] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Invoices] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [Patients] (
        [Id] int NOT NULL IDENTITY,
        [MedicalRecordNumber] nvarchar(50) NOT NULL,
        [FullName] nvarchar(200) NOT NULL,
        [DateOfBirth] datetime2 NULL,
        [Gender] int NOT NULL,
        [Phone] nvarchar(50) NULL,
        [Email] nvarchar(200) NULL,
        [Address] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Patients] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [Products] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(50) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [GenericName] nvarchar(200) NULL,
        [Strength] nvarchar(100) NULL,
        [Unit] nvarchar(50) NULL,
        [DefaultSalePrice] decimal(18,2) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [ServiceItems] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(50) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ServiceItems] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [StaffMembers] (
        [Id] int NOT NULL IDENTITY,
        [FullName] nvarchar(200) NOT NULL,
        [StaffType] int NOT NULL,
        [Phone] nvarchar(50) NULL,
        [Email] nvarchar(200) NULL,
        [UserId] int NULL,
        [DepartmentId] int NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_StaffMembers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [StockMovements] (
        [Id] int NOT NULL IDENTITY,
        [ProductId] int NOT NULL,
        [StockBatchId] int NULL,
        [Type] int NOT NULL,
        [Quantity] int NOT NULL,
        [Reason] nvarchar(500) NULL,
        [MovementDate] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_StockMovements] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] int NOT NULL,
        [RoleId] int NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] int NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [Departments] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Code] nvarchar(50) NULL,
        [FacilityId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Departments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Departments_Facilities_FacilityId] FOREIGN KEY ([FacilityId]) REFERENCES [Facilities] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [InvoiceItems] (
        [Id] int NOT NULL IDENTITY,
        [InvoiceId] int NOT NULL,
        [ServiceItemId] int NULL,
        [ProductId] int NULL,
        [Description] nvarchar(500) NOT NULL,
        [UnitPrice] decimal(18,2) NOT NULL,
        [Quantity] decimal(18,2) NOT NULL,
        [LineTotal] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_InvoiceItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InvoiceItems_Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [Payments] (
        [Id] int NOT NULL IDENTITY,
        [InvoiceId] int NOT NULL,
        [PaymentDate] datetime2 NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [Method] nvarchar(50) NULL,
        [Reference] nvarchar(200) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payments_Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [Visits] (
        [Id] int NOT NULL IDENTITY,
        [PatientId] int NOT NULL,
        [DoctorId] int NULL,
        [VisitDate] datetime2 NOT NULL,
        [ChiefComplaint] nvarchar(500) NULL,
        [Notes] nvarchar(max) NULL,
        [Diagnosis] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Visits] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Visits_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [StockBatches] (
        [Id] int NOT NULL IDENTITY,
        [ProductId] int NOT NULL,
        [BatchNumber] nvarchar(100) NULL,
        [ExpiryDate] datetime2 NULL,
        [QuantityOnHand] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_StockBatches] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StockBatches_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE TABLE [DoctorProfiles] (
        [Id] int NOT NULL IDENTITY,
        [StaffMemberId] int NOT NULL,
        [Specialty] nvarchar(200) NULL,
        [LicenseNumber] nvarchar(100) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_DoctorProfiles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DoctorProfiles_StaffMembers_StaffMemberId] FOREIGN KEY ([StaffMemberId]) REFERENCES [StaffMembers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Appointments_ScheduledStart] ON [Appointments] ([ScheduledStart]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Departments_FacilityId] ON [Departments] ([FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DoctorProfiles_StaffMemberId] ON [DoctorProfiles] ([StaffMemberId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_InvoiceItems_InvoiceId] ON [InvoiceItems] ([InvoiceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Invoices_InvoiceNumber] ON [Invoices] ([InvoiceNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Patients_MedicalRecordNumber] ON [Patients] ([MedicalRecordNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Payments_InvoiceId] ON [Payments] ([InvoiceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Products_Code] ON [Products] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ServiceItems_Code] ON [ServiceItems] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StockBatches_ExpiryDate] ON [StockBatches] ([ExpiryDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StockBatches_ProductId] ON [StockBatches] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_StockMovements_MovementDate] ON [StockMovements] ([MovementDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Visits_PatientId] ON [Visits] ([PatientId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304092952_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260304092952_InitialCreate', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304122602_AddMenuManagement'
)
BEGIN
    CREATE TABLE [Menus] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [MenuKey] nvarchar(100) NOT NULL,
        [Url] nvarchar(500) NULL,
        [ParentId] int NULL,
        [DisplayOrder] int NOT NULL,
        [Icon] nvarchar(100) NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Menus] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Menus_Menus_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [Menus] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304122602_AddMenuManagement'
)
BEGIN
    CREATE TABLE [RoleMenus] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] int NOT NULL,
        [MenuId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_RoleMenus] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RoleMenus_Menus_MenuId] FOREIGN KEY ([MenuId]) REFERENCES [Menus] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304122602_AddMenuManagement'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Menus_MenuKey] ON [Menus] ([MenuKey]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304122602_AddMenuManagement'
)
BEGIN
    CREATE INDEX [IX_Menus_ParentId] ON [Menus] ([ParentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304122602_AddMenuManagement'
)
BEGIN
    CREATE INDEX [IX_RoleMenus_MenuId] ON [RoleMenus] ([MenuId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304122602_AddMenuManagement'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RoleMenus_RoleId_MenuId] ON [RoleMenus] ([RoleId], [MenuId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260304122602_AddMenuManagement'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260304122602_AddMenuManagement', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309100302_UpdateLanguage'
)
BEGIN
    CREATE TABLE [Languages] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(10) NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [IsDefault] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Languages] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309100302_UpdateLanguage'
)
BEGIN
    CREATE TABLE [Translations] (
        [Id] int NOT NULL IDENTITY,
        [LanguageCode] nvarchar(10) NOT NULL,
        [Key] nvarchar(200) NOT NULL,
        [Value] nvarchar(1000) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Translations] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309100302_UpdateLanguage'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Languages_Code] ON [Languages] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309100302_UpdateLanguage'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Translations_LanguageCode_Key] ON [Translations] ([LanguageCode], [Key]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309100302_UpdateLanguage'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260309100302_UpdateLanguage', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309112324_AddAuditLogs'
)
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] int NOT NULL IDENTITY,
        [EntityType] nvarchar(200) NOT NULL,
        [EntityId] int NOT NULL,
        [Action] nvarchar(50) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [UserIdInt] int NULL,
        [PatientId] int NULL,
        [Description] nvarchar(2000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309112324_AddAuditLogs'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_CreatedAt] ON [AuditLogs] ([CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309112324_AddAuditLogs'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_EntityId] ON [AuditLogs] ([EntityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309112324_AddAuditLogs'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_EntityType] ON [AuditLogs] ([EntityType]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309112324_AddAuditLogs'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_PatientId] ON [AuditLogs] ([PatientId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260309112324_AddAuditLogs'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260309112324_AddAuditLogs', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310120043_AddVisitServices'
)
BEGIN
    CREATE TABLE [VisitServices] (
        [Id] int NOT NULL IDENTITY,
        [VisitId] int NOT NULL,
        [ServiceItemId] int NOT NULL,
        [Quantity] int NOT NULL DEFAULT 1,
        [UnitPrice] decimal(18,2) NOT NULL,
        [Notes] nvarchar(max) NULL,
        [IsBilled] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_VisitServices] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_VisitServices_Visits_VisitId] FOREIGN KEY ([VisitId]) REFERENCES [Visits] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310120043_AddVisitServices'
)
BEGIN
    CREATE INDEX [IX_VisitServices_VisitId] ON [VisitServices] ([VisitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310120043_AddVisitServices'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260310120043_AddVisitServices', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310132459_AddServiceAssignments'
)
BEGIN
    CREATE TABLE [DepartmentServices] (
        [Id] int NOT NULL IDENTITY,
        [DepartmentId] int NOT NULL,
        [ServiceItemId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_DepartmentServices] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DepartmentServices_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_DepartmentServices_ServiceItems_ServiceItemId] FOREIGN KEY ([ServiceItemId]) REFERENCES [ServiceItems] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310132459_AddServiceAssignments'
)
BEGIN
    CREATE TABLE [DoctorServices] (
        [Id] int NOT NULL IDENTITY,
        [StaffMemberId] int NOT NULL,
        [ServiceItemId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_DoctorServices] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DoctorServices_ServiceItems_ServiceItemId] FOREIGN KEY ([ServiceItemId]) REFERENCES [ServiceItems] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_DoctorServices_StaffMembers_StaffMemberId] FOREIGN KEY ([StaffMemberId]) REFERENCES [StaffMembers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310132459_AddServiceAssignments'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DepartmentServices_DepartmentId_ServiceItemId] ON [DepartmentServices] ([DepartmentId], [ServiceItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310132459_AddServiceAssignments'
)
BEGIN
    CREATE INDEX [IX_DepartmentServices_ServiceItemId] ON [DepartmentServices] ([ServiceItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310132459_AddServiceAssignments'
)
BEGIN
    CREATE INDEX [IX_DoctorServices_ServiceItemId] ON [DoctorServices] ([ServiceItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310132459_AddServiceAssignments'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DoctorServices_StaffMemberId_ServiceItemId] ON [DoctorServices] ([StaffMemberId], [ServiceItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310132459_AddServiceAssignments'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260310132459_AddServiceAssignments', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310154751_AddUserNotificationReads'
)
BEGIN
    CREATE TABLE [UserNotificationReads] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [NotificationType] nvarchar(64) NOT NULL,
        [NotificationKey] nvarchar(128) NOT NULL,
        [ReadAt] datetime2 NOT NULL,
        CONSTRAINT [PK_UserNotificationReads] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310154751_AddUserNotificationReads'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserNotificationReads_UserId_NotificationType_NotificationKey] ON [UserNotificationReads] ([UserId], [NotificationType], [NotificationKey]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260310154751_AddUserNotificationReads'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260310154751_AddUserNotificationReads', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260313103255_AddPrescriptions'
)
BEGIN
    CREATE TABLE [Prescriptions] (
        [Id] int NOT NULL IDENTITY,
        [VisitId] int NOT NULL,
        [PatientId] int NOT NULL,
        [DoctorId] int NULL,
        [Notes] nvarchar(2000) NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Prescriptions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Prescriptions_StaffMembers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [StaffMembers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Prescriptions_Visits_VisitId] FOREIGN KEY ([VisitId]) REFERENCES [Visits] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260313103255_AddPrescriptions'
)
BEGIN
    CREATE TABLE [PrescriptionItems] (
        [Id] int NOT NULL IDENTITY,
        [PrescriptionId] int NOT NULL,
        [ProductId] int NOT NULL,
        [ProductName] nvarchar(256) NOT NULL,
        [Dosage] nvarchar(256) NULL,
        [Frequency] nvarchar(256) NULL,
        [Duration] nvarchar(256) NULL,
        [Quantity] int NOT NULL,
        [Instructions] nvarchar(1000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_PrescriptionItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PrescriptionItems_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PrescriptionItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260313103255_AddPrescriptions'
)
BEGIN
    CREATE INDEX [IX_PrescriptionItems_PrescriptionId] ON [PrescriptionItems] ([PrescriptionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260313103255_AddPrescriptions'
)
BEGIN
    CREATE INDEX [IX_PrescriptionItems_ProductId] ON [PrescriptionItems] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260313103255_AddPrescriptions'
)
BEGIN
    CREATE INDEX [IX_Prescriptions_DoctorId] ON [Prescriptions] ([DoctorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260313103255_AddPrescriptions'
)
BEGIN
    CREATE INDEX [IX_Prescriptions_PatientId] ON [Prescriptions] ([PatientId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260313103255_AddPrescriptions'
)
BEGIN
    CREATE INDEX [IX_Prescriptions_VisitId] ON [Prescriptions] ([VisitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260313103255_AddPrescriptions'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260313103255_AddPrescriptions', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260313144020_AddPatientClinicalDetails'
)
BEGIN
    ALTER TABLE [Patients] ADD [Allergies] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260313144020_AddPatientClinicalDetails'
)
BEGIN
    ALTER TABLE [Patients] ADD [BloodGroup] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260313144020_AddPatientClinicalDetails'
)
BEGIN
    ALTER TABLE [Patients] ADD [ChronicConditions] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260313144020_AddPatientClinicalDetails'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260313144020_AddPatientClinicalDetails', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260317131113_AddDoctorRevenueRules'
)
BEGIN
    CREATE TABLE [DoctorRevenueRules] (
        [Id] int NOT NULL IDENTITY,
        [DoctorId] int NULL,
        [DepartmentId] int NULL,
        [ServiceItemId] int NULL,
        [MinVisitsPerDay] int NOT NULL,
        [MaxVisitsPerDay] int NULL,
        [DoctorSharePercent] decimal(5,2) NOT NULL,
        [HospitalSharePercent] decimal(5,2) NOT NULL,
        [IsActive] bit NOT NULL,
        [ValidFrom] datetime2 NULL,
        [ValidTo] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_DoctorRevenueRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DoctorRevenueRules_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DoctorRevenueRules_ServiceItems_ServiceItemId] FOREIGN KEY ([ServiceItemId]) REFERENCES [ServiceItems] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DoctorRevenueRules_StaffMembers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [StaffMembers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260317131113_AddDoctorRevenueRules'
)
BEGIN
    CREATE INDEX [IX_DoctorRevenueRules_DepartmentId] ON [DoctorRevenueRules] ([DepartmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260317131113_AddDoctorRevenueRules'
)
BEGIN
    CREATE INDEX [IX_DoctorRevenueRules_DoctorId] ON [DoctorRevenueRules] ([DoctorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260317131113_AddDoctorRevenueRules'
)
BEGIN
    CREATE INDEX [IX_DoctorRevenueRules_ServiceItemId] ON [DoctorRevenueRules] ([ServiceItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260317131113_AddDoctorRevenueRules'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260317131113_AddDoctorRevenueRules', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260318130932_AddDoctorRevenueShares'
)
BEGIN
    CREATE TABLE [DoctorRevenueShares] (
        [Id] int NOT NULL IDENTITY,
        [DoctorId] int NOT NULL,
        [InvoiceId] int NOT NULL,
        [VisitId] int NULL,
        [Date] datetime2 NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [DoctorAmount] decimal(18,2) NOT NULL,
        [HospitalAmount] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_DoctorRevenueShares] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DoctorRevenueShares_Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_DoctorRevenueShares_StaffMembers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [StaffMembers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DoctorRevenueShares_Visits_VisitId] FOREIGN KEY ([VisitId]) REFERENCES [Visits] ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260318130932_AddDoctorRevenueShares'
)
BEGIN
    CREATE INDEX [IX_DoctorRevenueShares_DoctorId] ON [DoctorRevenueShares] ([DoctorId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260318130932_AddDoctorRevenueShares'
)
BEGIN
    CREATE INDEX [IX_DoctorRevenueShares_InvoiceId] ON [DoctorRevenueShares] ([InvoiceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260318130932_AddDoctorRevenueShares'
)
BEGIN
    CREATE INDEX [IX_DoctorRevenueShares_VisitId] ON [DoctorRevenueShares] ([VisitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260318130932_AddDoctorRevenueShares'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260318130932_AddDoctorRevenueShares', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319083524_AddInstallments'
)
BEGIN
    ALTER TABLE [Payments] ADD [InstallmentItemId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319083524_AddInstallments'
)
BEGIN
    CREATE TABLE [InstallmentPlans] (
        [Id] int NOT NULL IDENTITY,
        [InvoiceId] int NOT NULL,
        [PatientId] int NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_InstallmentPlans] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InstallmentPlans_Invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319083524_AddInstallments'
)
BEGIN
    CREATE TABLE [InstallmentItems] (
        [Id] int NOT NULL IDENTITY,
        [InstallmentPlanId] int NOT NULL,
        [DueDate] datetime2 NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [PaidAmount] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_InstallmentItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_InstallmentItems_InstallmentPlans_InstallmentPlanId] FOREIGN KEY ([InstallmentPlanId]) REFERENCES [InstallmentPlans] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319083524_AddInstallments'
)
BEGIN
    CREATE INDEX [IX_Payments_InstallmentItemId] ON [Payments] ([InstallmentItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319083524_AddInstallments'
)
BEGIN
    CREATE INDEX [IX_InstallmentItems_InstallmentPlanId] ON [InstallmentItems] ([InstallmentPlanId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319083524_AddInstallments'
)
BEGIN
    CREATE INDEX [IX_InstallmentPlans_InvoiceId] ON [InstallmentPlans] ([InvoiceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319083524_AddInstallments'
)
BEGIN
    ALTER TABLE [Payments] ADD CONSTRAINT [FK_Payments_InstallmentItems_InstallmentItemId] FOREIGN KEY ([InstallmentItemId]) REFERENCES [InstallmentItems] ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260319083524_AddInstallments'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260319083524_AddInstallments', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323084623_AddDiagnosticsModule'
)
BEGIN
    CREATE TABLE [DiagnosticTests] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(50) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Type] int NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_DiagnosticTests] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323084623_AddDiagnosticsModule'
)
BEGIN
    CREATE TABLE [DiagnosticOrders] (
        [Id] int NOT NULL IDENTITY,
        [PatientId] int NOT NULL,
        [VisitId] int NULL,
        [DoctorId] int NULL,
        [DiagnosticTestId] int NOT NULL,
        [OrderedAt] datetime2 NOT NULL,
        [ResultValue] nvarchar(200) NULL,
        [ResultNotes] nvarchar(2000) NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_DiagnosticOrders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DiagnosticOrders_DiagnosticTests_DiagnosticTestId] FOREIGN KEY ([DiagnosticTestId]) REFERENCES [DiagnosticTests] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323084623_AddDiagnosticsModule'
)
BEGIN
    CREATE INDEX [IX_DiagnosticOrders_DiagnosticTestId] ON [DiagnosticOrders] ([DiagnosticTestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323084623_AddDiagnosticsModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DiagnosticTests_Code] ON [DiagnosticTests] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323084623_AddDiagnosticsModule'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260323084623_AddDiagnosticsModule', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    ALTER TABLE [DiagnosticTests] ADD [ContrastRequired] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    ALTER TABLE [DiagnosticTests] ADD [Description] nvarchar(2000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    ALTER TABLE [DiagnosticTests] ADD [DurationMinutes] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    ALTER TABLE [DiagnosticTests] ADD [ImagingCategoryId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    ALTER TABLE [DiagnosticTests] ADD [PriorPreparationRequired] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    ALTER TABLE [DiagnosticOrders] ADD [ClinicalIndication] nvarchar(2000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    ALTER TABLE [DiagnosticOrders] ADD [Priority] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    CREATE TABLE [ImagingCategories] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(50) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ImagingCategories] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    CREATE TABLE [RadiologyAppointments] (
        [Id] int NOT NULL IDENTITY,
        [DiagnosticOrderId] int NOT NULL,
        [AppointmentDate] datetime2 NOT NULL,
        [Room] nvarchar(100) NOT NULL,
        [Equipment] nvarchar(200) NOT NULL,
        [RadiologistId] int NULL,
        [TechnicianId] int NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_RadiologyAppointments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RadiologyAppointments_DiagnosticOrders_DiagnosticOrderId] FOREIGN KEY ([DiagnosticOrderId]) REFERENCES [DiagnosticOrders] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    CREATE TABLE [RadiologyStudies] (
        [Id] int NOT NULL IDENTITY,
        [RadiologyAppointmentId] int NOT NULL,
        [ExecutionDate] datetime2 NOT NULL,
        [EquipmentUsed] nvarchar(200) NOT NULL,
        [TechnicianId] int NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_RadiologyStudies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RadiologyStudies_RadiologyAppointments_RadiologyAppointmentId] FOREIGN KEY ([RadiologyAppointmentId]) REFERENCES [RadiologyAppointments] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    CREATE TABLE [RadiologyAttachments] (
        [Id] int NOT NULL IDENTITY,
        [RadiologyStudyId] int NOT NULL,
        [FilePathOrUrl] nvarchar(2000) NOT NULL,
        [FileType] nvarchar(100) NOT NULL,
        [FileSizeBytes] bigint NOT NULL,
        [ThumbnailPath] nvarchar(2000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_RadiologyAttachments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RadiologyAttachments_RadiologyStudies_RadiologyStudyId] FOREIGN KEY ([RadiologyStudyId]) REFERENCES [RadiologyStudies] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    CREATE TABLE [RadiologyReports] (
        [Id] int NOT NULL IDENTITY,
        [RadiologyStudyId] int NOT NULL,
        [Findings] nvarchar(4000) NOT NULL,
        [Conclusion] nvarchar(3000) NOT NULL,
        [Recommendations] nvarchar(3000) NULL,
        [ReportedById] int NOT NULL,
        [ValidatedById] int NULL,
        [ReportDate] datetime2 NOT NULL,
        [ValidatedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_RadiologyReports] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RadiologyReports_RadiologyStudies_RadiologyStudyId] FOREIGN KEY ([RadiologyStudyId]) REFERENCES [RadiologyStudies] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    CREATE INDEX [IX_DiagnosticTests_ImagingCategoryId] ON [DiagnosticTests] ([ImagingCategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ImagingCategories_Code] ON [ImagingCategories] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    CREATE INDEX [IX_RadiologyAppointments_DiagnosticOrderId] ON [RadiologyAppointments] ([DiagnosticOrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    CREATE INDEX [IX_RadiologyAttachments_RadiologyStudyId] ON [RadiologyAttachments] ([RadiologyStudyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    CREATE INDEX [IX_RadiologyReports_RadiologyStudyId] ON [RadiologyReports] ([RadiologyStudyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    CREATE INDEX [IX_RadiologyStudies_RadiologyAppointmentId] ON [RadiologyStudies] ([RadiologyAppointmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    ALTER TABLE [DiagnosticTests] ADD CONSTRAINT [FK_DiagnosticTests_ImagingCategories_ImagingCategoryId] FOREIGN KEY ([ImagingCategoryId]) REFERENCES [ImagingCategories] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323121304_AddRadiologyPhase1'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260323121304_AddRadiologyPhase1', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323131627_AddLaboratoryWorkflow'
)
BEGIN
    CREATE TABLE [LaboratoryOrders] (
        [Id] int NOT NULL IDENTITY,
        [PatientId] int NOT NULL,
        [VisitId] int NULL,
        [ReferringDoctorId] int NULL,
        [OrderedAt] datetime2 NOT NULL,
        [Priority] int NOT NULL,
        [ClinicalIndication] nvarchar(2000) NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [IsPaid] bit NOT NULL,
        [PaidAt] datetime2 NULL,
        [PaymentMethod] nvarchar(100) NULL,
        [ValidatedById] int NULL,
        [ValidatedAt] datetime2 NULL,
        [DeliveredAt] datetime2 NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_LaboratoryOrders] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323131627_AddLaboratoryWorkflow'
)
BEGIN
    CREATE TABLE [LaboratoryOrderItems] (
        [Id] int NOT NULL IDENTITY,
        [LaboratoryOrderId] int NOT NULL,
        [DiagnosticTestId] int NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_LaboratoryOrderItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LaboratoryOrderItems_DiagnosticTests_DiagnosticTestId] FOREIGN KEY ([DiagnosticTestId]) REFERENCES [DiagnosticTests] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_LaboratoryOrderItems_LaboratoryOrders_LaboratoryOrderId] FOREIGN KEY ([LaboratoryOrderId]) REFERENCES [LaboratoryOrders] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323131627_AddLaboratoryWorkflow'
)
BEGIN
    CREATE TABLE [LaboratorySamples] (
        [Id] int NOT NULL IDENTITY,
        [LaboratoryOrderId] int NOT NULL,
        [SampleType] nvarchar(100) NOT NULL,
        [CollectedAt] datetime2 NOT NULL,
        [CollectedById] int NOT NULL,
        [SampleBarcode] nvarchar(100) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_LaboratorySamples] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LaboratorySamples_LaboratoryOrders_LaboratoryOrderId] FOREIGN KEY ([LaboratoryOrderId]) REFERENCES [LaboratoryOrders] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323131627_AddLaboratoryWorkflow'
)
BEGIN
    CREATE TABLE [LaboratoryResults] (
        [Id] int NOT NULL IDENTITY,
        [LaboratoryOrderItemId] int NOT NULL,
        [LaboratorySampleId] int NOT NULL,
        [Value] nvarchar(200) NOT NULL,
        [Unit] nvarchar(50) NULL,
        [ReferenceRange] nvarchar(100) NULL,
        [Flag] int NOT NULL,
        [EnteredById] int NOT NULL,
        [EnteredAt] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_LaboratoryResults] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LaboratoryResults_LaboratoryOrderItems_LaboratoryOrderItemId] FOREIGN KEY ([LaboratoryOrderItemId]) REFERENCES [LaboratoryOrderItems] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_LaboratoryResults_LaboratorySamples_LaboratorySampleId] FOREIGN KEY ([LaboratorySampleId]) REFERENCES [LaboratorySamples] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323131627_AddLaboratoryWorkflow'
)
BEGIN
    CREATE INDEX [IX_LaboratoryOrderItems_DiagnosticTestId] ON [LaboratoryOrderItems] ([DiagnosticTestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323131627_AddLaboratoryWorkflow'
)
BEGIN
    CREATE INDEX [IX_LaboratoryOrderItems_LaboratoryOrderId] ON [LaboratoryOrderItems] ([LaboratoryOrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323131627_AddLaboratoryWorkflow'
)
BEGIN
    CREATE INDEX [IX_LaboratoryResults_LaboratoryOrderItemId] ON [LaboratoryResults] ([LaboratoryOrderItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323131627_AddLaboratoryWorkflow'
)
BEGIN
    CREATE INDEX [IX_LaboratoryResults_LaboratorySampleId] ON [LaboratoryResults] ([LaboratorySampleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323131627_AddLaboratoryWorkflow'
)
BEGIN
    CREATE INDEX [IX_LaboratorySamples_LaboratoryOrderId] ON [LaboratorySamples] ([LaboratoryOrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323131627_AddLaboratoryWorkflow'
)
BEGIN
    CREATE UNIQUE INDEX [IX_LaboratorySamples_SampleBarcode] ON [LaboratorySamples] ([SampleBarcode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323131627_AddLaboratoryWorkflow'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260323131627_AddLaboratoryWorkflow', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323133414_OptimizeRadiologyOrderNaming'
)
BEGIN
    ALTER TABLE [RadiologyAppointments] DROP CONSTRAINT [FK_RadiologyAppointments_DiagnosticOrders_DiagnosticOrderId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323133414_OptimizeRadiologyOrderNaming'
)
BEGIN
    ALTER TABLE [DiagnosticOrders] DROP CONSTRAINT [PK_DiagnosticOrders];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323133414_OptimizeRadiologyOrderNaming'
)
BEGIN
    EXEC sp_rename N'[DiagnosticOrders]', N'RadiologyOrders';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323133414_OptimizeRadiologyOrderNaming'
)
BEGIN
    EXEC sp_rename N'[RadiologyOrders].[IX_DiagnosticOrders_DiagnosticTestId]', N'IX_RadiologyOrders_DiagnosticTestId', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323133414_OptimizeRadiologyOrderNaming'
)
BEGIN
    EXEC sp_rename N'[RadiologyAppointments].[DiagnosticOrderId]', N'RadiologyOrderId', N'COLUMN';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323133414_OptimizeRadiologyOrderNaming'
)
BEGIN
    EXEC sp_rename N'[RadiologyAppointments].[IX_RadiologyAppointments_DiagnosticOrderId]', N'IX_RadiologyAppointments_RadiologyOrderId', N'INDEX';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323133414_OptimizeRadiologyOrderNaming'
)
BEGIN
    ALTER TABLE [RadiologyOrders] ADD CONSTRAINT [PK_RadiologyOrders] PRIMARY KEY ([Id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323133414_OptimizeRadiologyOrderNaming'
)
BEGIN
    ALTER TABLE [RadiologyAppointments] ADD CONSTRAINT [FK_RadiologyAppointments_RadiologyOrders_RadiologyOrderId] FOREIGN KEY ([RadiologyOrderId]) REFERENCES [RadiologyOrders] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260323133414_OptimizeRadiologyOrderNaming'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260323133414_OptimizeRadiologyOrderNaming', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324093001_RemoveRadiologyModule'
)
BEGIN
    ALTER TABLE [DiagnosticTests] DROP CONSTRAINT [FK_DiagnosticTests_ImagingCategories_ImagingCategoryId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324093001_RemoveRadiologyModule'
)
BEGIN
    DROP TABLE [ImagingCategories];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324093001_RemoveRadiologyModule'
)
BEGIN
    DROP TABLE [RadiologyAttachments];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324093001_RemoveRadiologyModule'
)
BEGIN
    DROP TABLE [RadiologyReports];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324093001_RemoveRadiologyModule'
)
BEGIN
    DROP TABLE [RadiologyStudies];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324093001_RemoveRadiologyModule'
)
BEGIN
    DROP TABLE [RadiologyAppointments];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324093001_RemoveRadiologyModule'
)
BEGIN
    DROP TABLE [RadiologyOrders];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324093001_RemoveRadiologyModule'
)
BEGIN
    DROP INDEX [IX_DiagnosticTests_ImagingCategoryId] ON [DiagnosticTests];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324093001_RemoveRadiologyModule'
)
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DiagnosticTests]') AND [c].[name] = N'ContrastRequired');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [DiagnosticTests] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [DiagnosticTests] DROP COLUMN [ContrastRequired];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324093001_RemoveRadiologyModule'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DiagnosticTests]') AND [c].[name] = N'Description');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [DiagnosticTests] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [DiagnosticTests] DROP COLUMN [Description];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324093001_RemoveRadiologyModule'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DiagnosticTests]') AND [c].[name] = N'DurationMinutes');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [DiagnosticTests] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [DiagnosticTests] DROP COLUMN [DurationMinutes];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324093001_RemoveRadiologyModule'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DiagnosticTests]') AND [c].[name] = N'ImagingCategoryId');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [DiagnosticTests] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [DiagnosticTests] DROP COLUMN [ImagingCategoryId];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324093001_RemoveRadiologyModule'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DiagnosticTests]') AND [c].[name] = N'PriorPreparationRequired');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [DiagnosticTests] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [DiagnosticTests] DROP COLUMN [PriorPreparationRequired];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324093001_RemoveRadiologyModule'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260324093001_RemoveRadiologyModule', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324151535_AddLaboratoryItemsToInvoices'
)
BEGIN
    ALTER TABLE [LaboratoryOrderItems] ADD [BilledAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324151535_AddLaboratoryItemsToInvoices'
)
BEGIN
    ALTER TABLE [LaboratoryOrderItems] ADD [IsBilled] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324151535_AddLaboratoryItemsToInvoices'
)
BEGIN
    ALTER TABLE [InvoiceItems] ADD [LaboratoryOrderItemId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324151535_AddLaboratoryItemsToInvoices'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_InvoiceItems_LaboratoryOrderItemId] ON [InvoiceItems] ([LaboratoryOrderItemId]) WHERE [LaboratoryOrderItemId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324151535_AddLaboratoryItemsToInvoices'
)
BEGIN
    ALTER TABLE [InvoiceItems] ADD CONSTRAINT [FK_InvoiceItems_LaboratoryOrderItems_LaboratoryOrderItemId] FOREIGN KEY ([LaboratoryOrderItemId]) REFERENCES [LaboratoryOrderItems] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324151535_AddLaboratoryItemsToInvoices'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260324151535_AddLaboratoryItemsToInvoices', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324155927_AddPrescriptionItemBilling'
)
BEGIN
    ALTER TABLE [PrescriptionItems] ADD [IsBilled] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324155927_AddPrescriptionItemBilling'
)
BEGIN
    ALTER TABLE [InvoiceItems] ADD [PrescriptionItemId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324155927_AddPrescriptionItemBilling'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_InvoiceItems_PrescriptionItemId] ON [InvoiceItems] ([PrescriptionItemId]) WHERE [PrescriptionItemId] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324155927_AddPrescriptionItemBilling'
)
BEGIN
    ALTER TABLE [InvoiceItems] ADD CONSTRAINT [FK_InvoiceItems_PrescriptionItems_PrescriptionItemId] FOREIGN KEY ([PrescriptionItemId]) REFERENCES [PrescriptionItems] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260324155927_AddPrescriptionItemBilling'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260324155927_AddPrescriptionItemBilling', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325102447_PharmacyCogsAndPurchases'
)
BEGIN
    ALTER TABLE [StockBatches] ADD [UnitCost] decimal(18,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325102447_PharmacyCogsAndPurchases'
)
BEGIN
    ALTER TABLE [InvoiceItems] ADD [LineCost] decimal(18,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325102447_PharmacyCogsAndPurchases'
)
BEGIN
    ALTER TABLE [InvoiceItems] ADD [UnitCost] decimal(18,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325102447_PharmacyCogsAndPurchases'
)
BEGIN
    CREATE TABLE [PharmacyPurchaseInvoices] (
        [Id] int NOT NULL IDENTITY,
        [InvoiceNumber] nvarchar(max) NOT NULL,
        [InvoiceDate] datetime2 NOT NULL,
        [SupplierName] nvarchar(max) NULL,
        [SupplierReference] nvarchar(max) NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [PaidAmount] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_PharmacyPurchaseInvoices] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325102447_PharmacyCogsAndPurchases'
)
BEGIN
    CREATE TABLE [PharmacyPurchaseInvoiceItems] (
        [Id] int NOT NULL IDENTITY,
        [PharmacyPurchaseInvoiceId] int NOT NULL,
        [ProductId] int NOT NULL,
        [BatchNumber] nvarchar(max) NULL,
        [ExpiryDate] datetime2 NOT NULL,
        [Quantity] int NOT NULL,
        [UnitPurchasePrice] decimal(18,2) NOT NULL,
        [LineTotal] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_PharmacyPurchaseInvoiceItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PharmacyPurchaseInvoiceItems_PharmacyPurchaseInvoices_PharmacyPurchaseInvoiceId] FOREIGN KEY ([PharmacyPurchaseInvoiceId]) REFERENCES [PharmacyPurchaseInvoices] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PharmacyPurchaseInvoiceItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325102447_PharmacyCogsAndPurchases'
)
BEGIN
    CREATE INDEX [IX_PharmacyPurchaseInvoiceItems_PharmacyPurchaseInvoiceId] ON [PharmacyPurchaseInvoiceItems] ([PharmacyPurchaseInvoiceId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325102447_PharmacyCogsAndPurchases'
)
BEGIN
    CREATE INDEX [IX_PharmacyPurchaseInvoiceItems_ProductId] ON [PharmacyPurchaseInvoiceItems] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325102447_PharmacyCogsAndPurchases'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260325102447_PharmacyCogsAndPurchases', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325130057_DoctorVisitSettings'
)
BEGIN
    CREATE TABLE [DoctorVisitSettings] (
        [Id] int NOT NULL IDENTITY,
        [StaffMemberId] int NOT NULL,
        [MinVisitDurationMinutes] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_DoctorVisitSettings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DoctorVisitSettings_StaffMembers_StaffMemberId] FOREIGN KEY ([StaffMemberId]) REFERENCES [StaffMembers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325130057_DoctorVisitSettings'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DoctorVisitSettings_StaffMemberId] ON [DoctorVisitSettings] ([StaffMemberId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325130057_DoctorVisitSettings'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260325130057_DoctorVisitSettings', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325135057_DoctorWeeklyScheduleDays'
)
BEGIN
    CREATE TABLE [DoctorWeeklyScheduleDays] (
        [Id] int NOT NULL IDENTITY,
        [StaffMemberId] int NOT NULL,
        [DayOfWeek] int NOT NULL,
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_DoctorWeeklyScheduleDays] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DoctorWeeklyScheduleDays_StaffMembers_StaffMemberId] FOREIGN KEY ([StaffMemberId]) REFERENCES [StaffMembers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325135057_DoctorWeeklyScheduleDays'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DoctorWeeklyScheduleDays_StaffMemberId_DayOfWeek] ON [DoctorWeeklyScheduleDays] ([StaffMemberId], [DayOfWeek]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260325135057_DoctorWeeklyScheduleDays'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260325135057_DoctorWeeklyScheduleDays', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    ALTER TABLE [Visits] ADD [FacilityId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    ALTER TABLE [StockMovements] ADD [FacilityId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PharmacyPurchaseInvoices]') AND [c].[name] = N'SupplierReference');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [PharmacyPurchaseInvoices] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [PharmacyPurchaseInvoices] ALTER COLUMN [SupplierReference] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PharmacyPurchaseInvoices]') AND [c].[name] = N'SupplierName');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [PharmacyPurchaseInvoices] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [PharmacyPurchaseInvoices] ALTER COLUMN [SupplierName] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PharmacyPurchaseInvoices]') AND [c].[name] = N'InvoiceNumber');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [PharmacyPurchaseInvoices] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [PharmacyPurchaseInvoices] ALTER COLUMN [InvoiceNumber] nvarchar(50) NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    ALTER TABLE [PharmacyPurchaseInvoices] ADD [FacilityId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    ALTER TABLE [Payments] ADD [FacilityId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    ALTER TABLE [LaboratoryOrders] ADD [FacilityId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    ALTER TABLE [Invoices] ADD [FacilityId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    ALTER TABLE [InstallmentPlans] ADD [FacilityId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    ALTER TABLE [InstallmentItems] ADD [FacilityId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    ALTER TABLE [Appointments] ADD [FacilityId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    CREATE TABLE [StaffFacilityAssignments] (
        [Id] int NOT NULL IDENTITY,
        [StaffMemberId] int NOT NULL,
        [FacilityId] int NOT NULL,
        [DepartmentId] int NULL,
        [IsPrimary] bit NOT NULL,
        [FromDate] datetime2 NULL,
        [ToDate] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_StaffFacilityAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_StaffFacilityAssignments_StaffMembers_StaffMemberId] FOREIGN KEY ([StaffMemberId]) REFERENCES [StaffMembers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    UPDATE a
    SET a.FacilityId = d.FacilityId
    FROM Appointments a
    INNER JOIN Departments d ON d.Id = a.DepartmentId
    WHERE a.FacilityId IS NULL AND a.DepartmentId IS NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    UPDATE v
    SET v.FacilityId = d.FacilityId
    FROM Visits v
    INNER JOIN StaffMembers s ON s.Id = v.DoctorId
    INNER JOIN Departments d ON d.Id = s.DepartmentId
    WHERE v.FacilityId IS NULL AND v.DoctorId IS NOT NULL AND s.DepartmentId IS NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    UPDATE i
    SET i.FacilityId = lo.FacilityId
    FROM Invoices i
    INNER JOIN InvoiceItems ii ON ii.InvoiceId = i.Id
    INNER JOIN LaboratoryOrderItems loi ON loi.Id = ii.LaboratoryOrderItemId
    INNER JOIN LaboratoryOrders lo ON lo.Id = loi.LaboratoryOrderId
    WHERE i.FacilityId IS NULL AND lo.FacilityId IS NOT NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    UPDATE p SET p.FacilityId = i.FacilityId
    FROM Payments p
    INNER JOIN Invoices i ON i.Id = p.InvoiceId
    WHERE p.FacilityId IS NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    UPDATE ip SET ip.FacilityId = i.FacilityId
    FROM InstallmentPlans ip
    INNER JOIN Invoices i ON i.Id = ip.InvoiceId
    WHERE ip.FacilityId IS NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    UPDATE ii SET ii.FacilityId = ip.FacilityId
    FROM InstallmentItems ii
    INNER JOIN InstallmentPlans ip ON ip.Id = ii.InstallmentPlanId
    WHERE ii.FacilityId IS NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    CREATE INDEX [IX_Visits_FacilityId] ON [Visits] ([FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    CREATE INDEX [IX_StockMovements_FacilityId] ON [StockMovements] ([FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    CREATE INDEX [IX_PharmacyPurchaseInvoices_FacilityId] ON [PharmacyPurchaseInvoices] ([FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PharmacyPurchaseInvoices_InvoiceNumber] ON [PharmacyPurchaseInvoices] ([InvoiceNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    CREATE INDEX [IX_Payments_FacilityId] ON [Payments] ([FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    CREATE INDEX [IX_LaboratoryOrders_FacilityId] ON [LaboratoryOrders] ([FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    CREATE INDEX [IX_Invoices_FacilityId] ON [Invoices] ([FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    CREATE INDEX [IX_InstallmentPlans_FacilityId] ON [InstallmentPlans] ([FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    CREATE INDEX [IX_InstallmentItems_FacilityId] ON [InstallmentItems] ([FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    CREATE INDEX [IX_Appointments_FacilityId] ON [Appointments] ([FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    CREATE UNIQUE INDEX [IX_StaffFacilityAssignments_StaffMemberId_FacilityId] ON [StaffFacilityAssignments] ([StaffMemberId], [FacilityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326131130_AddMultiFacilitySupport'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260326131130_AddMultiFacilitySupport', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326135204_AddFacilityParentHierarchy'
)
BEGIN
    ALTER TABLE [Facilities] ADD [ParentId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326135204_AddFacilityParentHierarchy'
)
BEGIN
    CREATE INDEX [IX_Facilities_ParentId] ON [Facilities] ([ParentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326135204_AddFacilityParentHierarchy'
)
BEGIN
    ALTER TABLE [Facilities] ADD CONSTRAINT [FK_Facilities_Facilities_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [Facilities] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326135204_AddFacilityParentHierarchy'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260326135204_AddFacilityParentHierarchy', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326154050_AddHospitalTenantScope'
)
BEGIN
    CREATE TABLE [Hospitals] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Code] nvarchar(50) NULL,
        [Address] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Hospitals] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326154050_AddHospitalTenantScope'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Hospitals_Code] ON [Hospitals] ([Code]) WHERE [Code] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326154050_AddHospitalTenantScope'
)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Hospitals WHERE Code = 'DEFAULT-HOSP')
    BEGIN
        INSERT INTO Hospitals (Name, Code, Address, CreatedAt, CreatedBy, IsDeleted)
        VALUES ('Default Hospital', 'DEFAULT-HOSP', 'Main', SYSUTCDATETIME(), 'migration', 0);
    END
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326154050_AddHospitalTenantScope'
)
BEGIN
    ALTER TABLE [Facilities] ADD [HospitalId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326154050_AddHospitalTenantScope'
)
BEGIN
    DECLARE @defaultHospitalId INT = (SELECT TOP 1 Id FROM Hospitals WHERE Code = 'DEFAULT-HOSP');
    UPDATE Facilities SET HospitalId = @defaultHospitalId WHERE HospitalId = 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326154050_AddHospitalTenantScope'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [HospitalId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326154050_AddHospitalTenantScope'
)
BEGIN
    CREATE INDEX [IX_Facilities_HospitalId] ON [Facilities] ([HospitalId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326154050_AddHospitalTenantScope'
)
BEGIN
    ALTER TABLE [Facilities] ADD CONSTRAINT [FK_Facilities_Hospitals_HospitalId] FOREIGN KEY ([HospitalId]) REFERENCES [Hospitals] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326154050_AddHospitalTenantScope'
)
BEGIN
    DECLARE @defaultHospitalId2 INT = (SELECT TOP 1 Id FROM Hospitals WHERE Code = 'DEFAULT-HOSP');
    UPDATE U
    SET U.HospitalId = @defaultHospitalId2
    FROM AspNetUsers U
    WHERE U.HospitalId IS NULL
      AND EXISTS (
          SELECT 1
          FROM AspNetUserRoles UR
          INNER JOIN AspNetRoles R ON R.Id = UR.RoleId
          WHERE UR.UserId = U.Id
            AND R.Name <> 'SuperAdmin'
      );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260326154050_AddHospitalTenantScope'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260326154050_AddHospitalTenantScope', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327083450_AddUserFacilityScope'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [FacilityId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260327083450_AddUserFacilityScope'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260327083450_AddUserFacilityScope', N'8.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    DROP INDEX [IX_Visits_PatientId] ON [Visits];
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Visits]') AND [c].[name] = N'Diagnosis');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Visits] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [Visits] ALTER COLUMN [Diagnosis] nvarchar(2000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Visits]') AND [c].[name] = N'ChiefComplaint');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Visits] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [Visits] ALTER COLUMN [ChiefComplaint] nvarchar(2000) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    ALTER TABLE [Visits] ADD [ClinicalDataJson] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    ALTER TABLE [Visits] ADD [VisitFormTemplate] nvarchar(32) NOT NULL DEFAULT N'GENERAL';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    ALTER TABLE [Patients] ADD [ParentGuardianName] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    ALTER TABLE [Patients] ADD [PediatricGjtl] decimal(9,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    ALTER TABLE [Patients] ADD [PediatricMtl] decimal(9,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    ALTER TABLE [Patients] ADD [PediatricPkl] decimal(9,2) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    ALTER TABLE [Patients] ADD [PriorAbortion] bit NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    ALTER TABLE [Patients] ADD [PriorLiveBirth] bit NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    CREATE INDEX [IX_Visits_DoctorId_VisitDate] ON [Visits] ([DoctorId], [VisitDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    CREATE INDEX [IX_Visits_PatientId_VisitDate] ON [Visits] ([PatientId], [VisitDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    CREATE INDEX [IX_Visits_VisitFormTemplate] ON [Visits] ([VisitFormTemplate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406211644_VisitDepartmentClinicalData'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260406211644_VisitDepartmentClinicalData', N'8.0.11');
END;
GO

COMMIT;
GO

