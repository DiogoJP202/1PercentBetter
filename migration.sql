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
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
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
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(128) NOT NULL,
        [ProviderKey] nvarchar(128) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'00000000000000_CreateIdentitySchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'00000000000000_CreateIdentitySchema', N'8.0.26');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [AvatarUrl] nvarchar(600) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [DisplayName] nvarchar(120) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [MainColor] nvarchar(24) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [OnboardingCompletedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [ThemePreference] nvarchar(40) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE TABLE [Categories] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NULL,
        [Name] nvarchar(80) NOT NULL,
        [Description] nvarchar(240) NULL,
        [Color] nvarchar(24) NOT NULL,
        [Icon] nvarchar(80) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Categories_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE TABLE [DailyCheckIns] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [Date] date NOT NULL,
        [Mood] int NOT NULL,
        [EnergyLevel] int NOT NULL,
        [ProductivityLevel] int NOT NULL,
        [DayScore] int NOT NULL,
        [SmallWin] nvarchar(500) NULL,
        [MainDifficulty] nvarchar(500) NULL,
        [TomorrowAdjustment] nvarchar(500) NULL,
        [Notes] nvarchar(1600) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_DailyCheckIns] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DailyCheckIns_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE TABLE [Identities] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [IdentityStatement] nvarchar(260) NOT NULL,
        [Description] nvarchar(800) NULL,
        [CategoryId] int NULL,
        [Status] int NOT NULL,
        [Color] nvarchar(24) NOT NULL,
        [Icon] nvarchar(80) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Identities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Identities_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Identities_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE TABLE [Goals] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [IdentityId] int NULL,
        [Title] nvarchar(160) NOT NULL,
        [Description] nvarchar(1200) NULL,
        [CategoryId] int NULL,
        [Status] int NOT NULL,
        [Priority] int NOT NULL,
        [StartDate] date NOT NULL,
        [TargetDate] date NULL,
        [Color] nvarchar(24) NOT NULL,
        [Icon] nvarchar(80) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Goals] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Goals_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Goals_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Goals_Identities_IdentityId] FOREIGN KEY ([IdentityId]) REFERENCES [Identities] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE TABLE [Habits] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [GoalId] int NULL,
        [IdentityId] int NULL,
        [Title] nvarchar(160) NOT NULL,
        [Description] nvarchar(1200) NULL,
        [CategoryId] int NULL,
        [FrequencyType] int NOT NULL,
        [DaysOfWeek] nvarchar(80) NULL,
        [SuggestedTime] time NULL,
        [Difficulty] int NOT NULL,
        [TwoMinuteVersion] nvarchar(260) NOT NULL,
        [Trigger] nvarchar(260) NOT NULL,
        [Reward] nvarchar(260) NULL,
        [Status] int NOT NULL,
        [Color] nvarchar(24) NOT NULL,
        [Icon] nvarchar(80) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Habits] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Habits_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Habits_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Habits_Goals_GoalId] FOREIGN KEY ([GoalId]) REFERENCES [Goals] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Habits_Identities_IdentityId] FOREIGN KEY ([IdentityId]) REFERENCES [Identities] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE TABLE [HabitLogs] (
        [Id] int NOT NULL IDENTITY,
        [HabitId] int NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Date] date NOT NULL,
        [Status] int NOT NULL,
        [CompletedAt] datetime2 NULL,
        [Mood] int NULL,
        [EnergyLevel] int NULL,
        [DifficultyFelt] int NULL,
        [Notes] nvarchar(1200) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_HabitLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HabitLogs_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HabitLogs_Habits_HabitId] FOREIGN KEY ([HabitId]) REFERENCES [Habits] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE TABLE [Notes] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [Title] nvarchar(160) NOT NULL,
        [Content] nvarchar(max) NOT NULL,
        [NoteType] int NOT NULL,
        [Tags] nvarchar(500) NULL,
        [GoalId] int NULL,
        [IdentityId] int NULL,
        [HabitId] int NULL,
        [Date] date NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Notes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Notes_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Notes_Goals_GoalId] FOREIGN KEY ([GoalId]) REFERENCES [Goals] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Notes_Habits_HabitId] FOREIGN KEY ([HabitId]) REFERENCES [Habits] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Notes_Identities_IdentityId] FOREIGN KEY ([IdentityId]) REFERENCES [Identities] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Color', N'CreatedAt', N'Description', N'Icon', N'Name', N'UserId') AND [object_id] = OBJECT_ID(N'[Categories]'))
        SET IDENTITY_INSERT [Categories] ON;
    EXEC(N'INSERT INTO [Categories] ([Id], [Color], [CreatedAt], [Description], [Icon], [Name], [UserId])
    VALUES (1, N''#38bdf8'', ''2026-01-01T00:00:00.0000000Z'', N''Code, projetos e aprendizado tecnico.'', N''code-2'', N''Tecnologia'', NULL),
    (2, N''#a78bfa'', ''2026-01-01T00:00:00.0000000Z'', N''Estudos de linguas e pratica diaria.'', N''languages'', N''Idiomas'', NULL),
    (3, N''#22c55e'', ''2026-01-01T00:00:00.0000000Z'', N''Corpo, sono, alimentacao e energia.'', N''heart-pulse'', N''Saude'', NULL),
    (4, N''#f59e0b'', ''2026-01-01T00:00:00.0000000Z'', N''Rotinas de aprendizado e revisao.'', N''book-open'', N''Estudos'', NULL),
    (5, N''#60a5fa'', ''2026-01-01T00:00:00.0000000Z'', N''Carreira, entregas e foco profissional.'', N''briefcase-business'', N''Trabalho'', NULL),
    (6, N''#34d399'', ''2026-01-01T00:00:00.0000000Z'', N''Projetos pessoais e entregas criativas.'', N''rocket'', N''Projetos'', NULL),
    (7, N''#84cc16'', ''2026-01-01T00:00:00.0000000Z'', N''Dinheiro, planejamento e controle.'', N''wallet'', N''Financas'', NULL),
    (8, N''#fb7185'', ''2026-01-01T00:00:00.0000000Z'', N''Organizacao domestica e rotina.'', N''home'', N''Casa'', NULL),
    (9, N''#f472b6'', ''2026-01-01T00:00:00.0000000Z'', N''Relacionamentos e presenca.'', N''users-round'', N''Social'', NULL),
    (10, N''#818cf8'', ''2026-01-01T00:00:00.0000000Z'', N''Foco, ansiedade e clareza.'', N''brain'', N''Mental'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Color', N'CreatedAt', N'Description', N'Icon', N'Name', N'UserId') AND [object_id] = OBJECT_ID(N'[Categories]'))
        SET IDENTITY_INSERT [Categories] OFF;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Categories_UserId_Name] ON [Categories] ([UserId], [Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_DailyCheckIns_UserId_Date] ON [DailyCheckIns] ([UserId], [Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Goals_CategoryId] ON [Goals] ([CategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Goals_IdentityId] ON [Goals] ([IdentityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Goals_UserId_Status] ON [Goals] ([UserId], [Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_HabitLogs_HabitId] ON [HabitLogs] ([HabitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HabitLogs_UserId_HabitId_Date] ON [HabitLogs] ([UserId], [HabitId], [Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Habits_CategoryId] ON [Habits] ([CategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Habits_GoalId] ON [Habits] ([GoalId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Habits_IdentityId] ON [Habits] ([IdentityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Habits_UserId_Status] ON [Habits] ([UserId], [Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Identities_CategoryId] ON [Identities] ([CategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Identities_UserId_Name] ON [Identities] ([UserId], [Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Identities_UserId_Status] ON [Identities] ([UserId], [Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Notes_GoalId] ON [Notes] ([GoalId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Notes_HabitId] ON [Notes] ([HabitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Notes_IdentityId] ON [Notes] ([IdentityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    CREATE INDEX [IX_Notes_UserId_Date] ON [Notes] ([UserId], [Date]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514162755_CreateCoreHabitSchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260514162755_CreateCoreHabitSchema', N'8.0.26');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515183648_AddHabitLocationsAndStacking'
)
BEGIN
    ALTER TABLE [Habits] ADD [LocationId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515183648_AddHabitLocationsAndStacking'
)
BEGIN
    ALTER TABLE [Habits] ADD [StackedAfterHabitId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515183648_AddHabitLocationsAndStacking'
)
BEGIN
    CREATE TABLE [HabitLocations] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [Name] nvarchar(120) NOT NULL,
        [Description] nvarchar(600) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_HabitLocations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HabitLocations_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515183648_AddHabitLocationsAndStacking'
)
BEGIN
    CREATE INDEX [IX_Habits_LocationId] ON [Habits] ([LocationId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515183648_AddHabitLocationsAndStacking'
)
BEGIN
    CREATE INDEX [IX_Habits_StackedAfterHabitId] ON [Habits] ([StackedAfterHabitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515183648_AddHabitLocationsAndStacking'
)
BEGIN
    CREATE UNIQUE INDEX [IX_HabitLocations_UserId_Name] ON [HabitLocations] ([UserId], [Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515183648_AddHabitLocationsAndStacking'
)
BEGIN
    ALTER TABLE [Habits] ADD CONSTRAINT [FK_Habits_HabitLocations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [HabitLocations] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515183648_AddHabitLocationsAndStacking'
)
BEGIN
    ALTER TABLE [Habits] ADD CONSTRAINT [FK_Habits_Habits_StackedAfterHabitId] FOREIGN KEY ([StackedAfterHabitId]) REFERENCES [Habits] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515183648_AddHabitLocationsAndStacking'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260515183648_AddHabitLocationsAndStacking', N'8.0.26');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515185843_AddSimpleHabitsForStacking'
)
BEGIN
    ALTER TABLE [Habits] ADD [StackedAfterSimpleHabitId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515185843_AddSimpleHabitsForStacking'
)
BEGIN
    CREATE TABLE [SimpleHabits] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [Name] nvarchar(160) NOT NULL,
        [ScheduledTime] time NULL,
        [IsActive] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_SimpleHabits] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SimpleHabits_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515185843_AddSimpleHabitsForStacking'
)
BEGIN
    CREATE INDEX [IX_Habits_StackedAfterSimpleHabitId] ON [Habits] ([StackedAfterSimpleHabitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515185843_AddSimpleHabitsForStacking'
)
BEGIN
    CREATE INDEX [IX_SimpleHabits_UserId_IsActive] ON [SimpleHabits] ([UserId], [IsActive]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515185843_AddSimpleHabitsForStacking'
)
BEGIN
    CREATE INDEX [IX_SimpleHabits_UserId_Name_ScheduledTime] ON [SimpleHabits] ([UserId], [Name], [ScheduledTime]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515185843_AddSimpleHabitsForStacking'
)
BEGIN
    ALTER TABLE [Habits] ADD CONSTRAINT [FK_Habits_SimpleHabits_StackedAfterSimpleHabitId] FOREIGN KEY ([StackedAfterSimpleHabitId]) REFERENCES [SimpleHabits] ([Id]) ON DELETE NO ACTION;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515185843_AddSimpleHabitsForStacking'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260515185843_AddSimpleHabitsForStacking', N'8.0.26');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520143732_AddOnboardingTourTracking'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [OnboardingTourCompletedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520143732_AddOnboardingTourTracking'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [OnboardingTourSkippedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520143732_AddOnboardingTourTracking'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [OnboardingTourVersion] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520143732_AddOnboardingTourTracking'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260520143732_AddOnboardingTourTracking', N'8.0.26');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    ALTER TABLE [Notes] ADD [TaskItemId] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    ALTER TABLE [DailyCheckIns] ADD [TaskBlocker] nvarchar(500) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    CREATE TABLE [TaskItems] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [Title] nvarchar(160) NOT NULL,
        [Description] nvarchar(1200) NULL,
        [Notes] nvarchar(2000) NULL,
        [Status] int NOT NULL,
        [Priority] int NOT NULL,
        [TaskDate] date NULL,
        [StartTime] time NULL,
        [EndTime] time NULL,
        [DueDate] date NULL,
        [CategoryId] int NULL,
        [IdentityId] int NULL,
        [GoalId] int NULL,
        [HabitId] int NULL,
        [Color] nvarchar(24) NOT NULL,
        [Icon] nvarchar(80) NOT NULL,
        [ShowOnCalendar] bit NOT NULL,
        [CompletedAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_TaskItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TaskItems_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TaskItems_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_TaskItems_Goals_GoalId] FOREIGN KEY ([GoalId]) REFERENCES [Goals] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_TaskItems_Habits_HabitId] FOREIGN KEY ([HabitId]) REFERENCES [Habits] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_TaskItems_Identities_IdentityId] FOREIGN KEY ([IdentityId]) REFERENCES [Identities] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    CREATE TABLE [TaskTags] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [Name] nvarchar(64) NOT NULL,
        [Color] nvarchar(24) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_TaskTags] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TaskTags_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    CREATE TABLE [TaskItemTags] (
        [TaskItemId] int NOT NULL,
        [TaskTagId] int NOT NULL,
        CONSTRAINT [PK_TaskItemTags] PRIMARY KEY ([TaskItemId], [TaskTagId]),
        CONSTRAINT [FK_TaskItemTags_TaskItems_TaskItemId] FOREIGN KEY ([TaskItemId]) REFERENCES [TaskItems] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TaskItemTags_TaskTags_TaskTagId] FOREIGN KEY ([TaskTagId]) REFERENCES [TaskTags] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    CREATE INDEX [IX_Notes_TaskItemId] ON [Notes] ([TaskItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    CREATE INDEX [IX_TaskItems_CategoryId] ON [TaskItems] ([CategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    CREATE INDEX [IX_TaskItems_GoalId] ON [TaskItems] ([GoalId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    CREATE INDEX [IX_TaskItems_HabitId] ON [TaskItems] ([HabitId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    CREATE INDEX [IX_TaskItems_IdentityId] ON [TaskItems] ([IdentityId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    CREATE INDEX [IX_TaskItems_UserId_DueDate] ON [TaskItems] ([UserId], [DueDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    CREATE INDEX [IX_TaskItems_UserId_Priority] ON [TaskItems] ([UserId], [Priority]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    CREATE INDEX [IX_TaskItems_UserId_Status] ON [TaskItems] ([UserId], [Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    CREATE INDEX [IX_TaskItems_UserId_TaskDate] ON [TaskItems] ([UserId], [TaskDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    CREATE INDEX [IX_TaskItemTags_TaskTagId] ON [TaskItemTags] ([TaskTagId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TaskTags_UserId_Name] ON [TaskTags] ([UserId], [Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    ALTER TABLE [Notes] ADD CONSTRAINT [FK_Notes_TaskItems_TaskItemId] FOREIGN KEY ([TaskItemId]) REFERENCES [TaskItems] ([Id]) ON DELETE SET NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520222939_AddTaskItemsModule'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260520222939_AddTaskItemsModule', N'8.0.26');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520225756_AddHabitEndTimeWindow'
)
BEGIN
    ALTER TABLE [Habits] ADD [EndTime] time NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260520225756_AddHabitEndTimeWindow'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260520225756_AddHabitEndTimeWindow', N'8.0.26');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521045232_AddEmailConfirmedAtToUsers'
)
BEGIN
    ALTER TABLE [AspNetUsers] ADD [EmailConfirmedAt] datetime2 NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521045232_AddEmailConfirmedAtToUsers'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260521045232_AddEmailConfirmedAtToUsers', N'8.0.26');
END;
GO

COMMIT;
GO

