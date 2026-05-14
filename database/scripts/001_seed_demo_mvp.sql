/*
    1% Better - MVP demo seed

    Banco esperado:
    Server=(localdb)\OnePercentBetterLocalDb;Database=OnePercentBetter

    Usuario demo:
    Email: demo@1better.local
    Senha: Demo@123

    Este script e idempotente para o usuario demo:
    - Cria ou atualiza o usuario demo.
    - Remove dados antigos desse usuario demo.
    - Recria identidades, objetivos, habitos, logs, check-ins e notas.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @UserId nvarchar(450) = N'demo-user-1better';
DECLARE @Email nvarchar(256) = N'demo@1better.local';
DECLARE @NormalizedEmail nvarchar(256) = N'DEMO@1BETTER.LOCAL';
DECLARE @Now datetime2 = SYSUTCDATETIME();
DECLARE @Today date = CONVERT(date, @Now);

IF EXISTS (
    SELECT 1
    FROM AspNetUsers
    WHERE NormalizedEmail = @NormalizedEmail
      AND Id <> @UserId
)
BEGIN
    THROW 51000, 'Ja existe outro usuario com o email demo@1better.local.', 1;
END;

IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = @UserId)
BEGIN
    INSERT INTO AspNetUsers (
        Id,
        UserName,
        NormalizedUserName,
        Email,
        NormalizedEmail,
        EmailConfirmed,
        PasswordHash,
        SecurityStamp,
        ConcurrencyStamp,
        PhoneNumber,
        PhoneNumberConfirmed,
        TwoFactorEnabled,
        LockoutEnd,
        LockoutEnabled,
        AccessFailedCount,
        AvatarUrl,
        CreatedAt,
        DisplayName,
        MainColor,
        OnboardingCompletedAt,
        ThemePreference,
        UpdatedAt
    )
    VALUES (
        @UserId,
        @Email,
        @NormalizedEmail,
        @Email,
        @NormalizedEmail,
        1,
        N'AQAAAAIAAYagAAAAEH19Qtmd90kuouxKa1xtfo89KvzCv+4WkFv1reKz+iQrjg1AyOwv+BrM1pUB9MSIWA==',
        N'50DF4F76-70E2-4C9C-9229-5CF41F775F82',
        N'9C1473BC-6514-4C5C-8DA1-5B90D54A6D7A',
        NULL,
        0,
        0,
        NULL,
        1,
        0,
        NULL,
        DATEADD(day, -21, @Now),
        N'Demo 1% Better',
        N'#22c55e',
        DATEADD(day, -20, @Now),
        N'dark',
        @Now
    );
END
ELSE
BEGIN
    UPDATE AspNetUsers
    SET
        UserName = @Email,
        NormalizedUserName = @NormalizedEmail,
        Email = @Email,
        NormalizedEmail = @NormalizedEmail,
        EmailConfirmed = 1,
        PasswordHash = N'AQAAAAIAAYagAAAAEH19Qtmd90kuouxKa1xtfo89KvzCv+4WkFv1reKz+iQrjg1AyOwv+BrM1pUB9MSIWA==',
        DisplayName = N'Demo 1% Better',
        MainColor = N'#22c55e',
        ThemePreference = N'dark',
        OnboardingCompletedAt = COALESCE(OnboardingCompletedAt, DATEADD(day, -20, @Now)),
        UpdatedAt = @Now
    WHERE Id = @UserId;
END;

DELETE FROM HabitLogs WHERE UserId = @UserId;
DELETE FROM Notes WHERE UserId = @UserId;
DELETE FROM DailyCheckIns WHERE UserId = @UserId;
DELETE FROM Habits WHERE UserId = @UserId;
DELETE FROM Goals WHERE UserId = @UserId;
DELETE FROM Identities WHERE UserId = @UserId;

DECLARE @IdentityDotNet int;
DECLARE @IdentityLanguage int;
DECLARE @IdentityHealth int;

INSERT INTO Identities (
    UserId,
    Name,
    IdentityStatement,
    Description,
    CategoryId,
    Status,
    Color,
    Icon,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    @UserId,
    N'Desenvolvedor .NET consistente',
    N'Eu sou uma pessoa que evolui tecnicamente todos os dias.',
    N'Identidade focada em constancia tecnica, projetos pessoais e aprendizado pratico em ASP.NET Core MVC.',
    1,
    1,
    N'#38bdf8',
    N'code-2',
    DATEADD(day, -20, @Now),
    @Now
);
SET @IdentityDotNet = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO Identities (
    UserId,
    Name,
    IdentityStatement,
    Description,
    CategoryId,
    Status,
    Color,
    Icon,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    @UserId,
    N'Pessoa que aprende idiomas com constancia',
    N'Eu pratico idiomas em pequenos blocos todos os dias.',
    N'Identidade voltada a estudo leve, repeticao e contato diario com ingles.',
    2,
    1,
    N'#a78bfa',
    N'languages',
    DATEADD(day, -18, @Now),
    @Now
);
SET @IdentityLanguage = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO Identities (
    UserId,
    Name,
    IdentityStatement,
    Description,
    CategoryId,
    Status,
    Color,
    Icon,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    @UserId,
    N'Pessoa saudavel e energica',
    N'Eu cuido da minha energia antes de cobrar intensidade.',
    N'Identidade focada em sono, movimento e escolhas pequenas que melhoram o dia.',
    3,
    1,
    N'#22c55e',
    N'heart-pulse',
    DATEADD(day, -14, @Now),
    @Now
);
SET @IdentityHealth = CONVERT(int, SCOPE_IDENTITY());

DECLARE @GoalMvc int;
DECLARE @GoalEnglish int;
DECLARE @GoalEnergy int;

INSERT INTO Goals (
    UserId,
    IdentityId,
    Title,
    Description,
    CategoryId,
    Status,
    Priority,
    StartDate,
    TargetDate,
    Color,
    Icon,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    @UserId,
    @IdentityDotNet,
    N'Evoluir em ASP.NET Core MVC',
    N'Construir um projeto real com MVC, Razor Views, EF Core, SQL Server e dashboard funcional.',
    1,
    1,
    3,
    DATEADD(day, -20, @Today),
    DATEADD(day, 45, @Today),
    N'#38bdf8',
    N'target',
    DATEADD(day, -20, @Now),
    @Now
);
SET @GoalMvc = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO Goals (
    UserId,
    IdentityId,
    Title,
    Description,
    CategoryId,
    Status,
    Priority,
    StartDate,
    TargetDate,
    Color,
    Icon,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    @UserId,
    @IdentityLanguage,
    N'Praticar ingles todos os dias',
    N'Manter contato diario com vocabulario, leitura curta e escuta simples.',
    2,
    1,
    2,
    DATEADD(day, -18, @Today),
    DATEADD(day, 60, @Today),
    N'#a78bfa',
    N'languages',
    DATEADD(day, -18, @Now),
    @Now
);
SET @GoalEnglish = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO Goals (
    UserId,
    IdentityId,
    Title,
    Description,
    CategoryId,
    Status,
    Priority,
    StartDate,
    TargetDate,
    Color,
    Icon,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    @UserId,
    @IdentityHealth,
    N'Melhorar energia e sono',
    N'Dormir melhor, caminhar mais e reduzir dias de baixa energia.',
    3,
    1,
    2,
    DATEADD(day, -14, @Today),
    DATEADD(day, 30, @Today),
    N'#22c55e',
    N'heart-pulse',
    DATEADD(day, -14, @Now),
    @Now
);
SET @GoalEnergy = CONVERT(int, SCOPE_IDENTITY());

DECLARE @HabitStudy int;
DECLARE @HabitCommit int;
DECLARE @HabitEnglish int;
DECLARE @HabitWalk int;
DECLARE @HabitSleep int;

INSERT INTO Habits (
    UserId,
    GoalId,
    IdentityId,
    Title,
    Description,
    CategoryId,
    FrequencyType,
    DaysOfWeek,
    SuggestedTime,
    Difficulty,
    TwoMinuteVersion,
    [Trigger],
    Reward,
    Status,
    Color,
    Icon,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    @UserId,
    @GoalMvc,
    @IdentityDotNet,
    N'Estudar ASP.NET Core por 20 minutos',
    N'Estudo pratico no projeto 1% Better.',
    1,
    1,
    NULL,
    '20:30',
    2,
    N'Abrir o projeto e revisar uma controller.',
    N'Depois do cafe da noite.',
    N'Marcar progresso no dashboard.',
    1,
    N'#38bdf8',
    N'code-2',
    DATEADD(day, -20, @Now),
    @Now
);
SET @HabitStudy = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO Habits (
    UserId,
    GoalId,
    IdentityId,
    Title,
    Description,
    CategoryId,
    FrequencyType,
    DaysOfWeek,
    SuggestedTime,
    Difficulty,
    TwoMinuteVersion,
    [Trigger],
    Reward,
    Status,
    Color,
    Icon,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    @UserId,
    @GoalMvc,
    @IdentityDotNet,
    N'Fazer 1 commit em projeto pessoal',
    N'Pequeno avanco tecnico versionado.',
    1,
    1,
    NULL,
    '21:15',
    3,
    N'Abrir uma issue ou anotar o proximo commit.',
    N'Depois de estudar ASP.NET Core.',
    N'Fechar o dia com progresso visivel.',
    1,
    N'#34d399',
    N'git-commit-horizontal',
    DATEADD(day, -16, @Now),
    @Now
);
SET @HabitCommit = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO Habits (
    UserId,
    GoalId,
    IdentityId,
    Title,
    Description,
    CategoryId,
    FrequencyType,
    DaysOfWeek,
    SuggestedTime,
    Difficulty,
    TwoMinuteVersion,
    [Trigger],
    Reward,
    Status,
    Color,
    Icon,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    @UserId,
    @GoalEnglish,
    @IdentityLanguage,
    N'Revisar 10 palavras em ingles',
    N'Revisao curta de vocabulario e frases uteis.',
    2,
    1,
    NULL,
    '08:30',
    1,
    N'Revisar 3 palavras.',
    N'Depois de escovar os dentes pela manha.',
    N'Ouvir uma musica curta em ingles.',
    1,
    N'#a78bfa',
    N'languages',
    DATEADD(day, -18, @Now),
    @Now
);
SET @HabitEnglish = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO Habits (
    UserId,
    GoalId,
    IdentityId,
    Title,
    Description,
    CategoryId,
    FrequencyType,
    DaysOfWeek,
    SuggestedTime,
    Difficulty,
    TwoMinuteVersion,
    [Trigger],
    Reward,
    Status,
    Color,
    Icon,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    @UserId,
    @GoalEnergy,
    @IdentityHealth,
    N'Caminhar 15 minutos',
    N'Movimento leve para melhorar energia.',
    3,
    1,
    NULL,
    '18:30',
    2,
    N'Calcar o tenis e caminhar por 2 minutos.',
    N'Depois de encerrar o trabalho.',
    N'Tomar banho ouvindo uma playlist.',
    1,
    N'#22c55e',
    N'footprints',
    DATEADD(day, -14, @Now),
    @Now
);
SET @HabitWalk = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO Habits (
    UserId,
    GoalId,
    IdentityId,
    Title,
    Description,
    CategoryId,
    FrequencyType,
    DaysOfWeek,
    SuggestedTime,
    Difficulty,
    TwoMinuteVersion,
    [Trigger],
    Reward,
    Status,
    Color,
    Icon,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    @UserId,
    @GoalEnergy,
    @IdentityHealth,
    N'Dormir antes de 23h',
    N'Rotina simples para reduzir noites muito longas.',
    3,
    1,
    NULL,
    '22:30',
    3,
    N'Deixar o celular carregando longe da cama.',
    N'Depois de escovar os dentes a noite.',
    N'Acordar com mais energia no dia seguinte.',
    1,
    N'#14b8a6',
    N'moon',
    DATEADD(day, -12, @Now),
    @Now
);
SET @HabitSleep = CONVERT(int, SCOPE_IDENTITY());

DECLARE @Offset int = 13;
WHILE @Offset >= 0
BEGIN
    DECLARE @LogDate date = DATEADD(day, -@Offset, @Today);
    DECLARE @StudyStatus int = CASE WHEN @Offset IN (9, 5) THEN 3 WHEN @Offset = 2 THEN 4 ELSE 1 END;
    DECLARE @CommitStatus int = CASE WHEN @Offset IN (12, 8, 4) THEN 2 WHEN @Offset IN (10, 3) THEN 3 ELSE 1 END;
    DECLARE @EnglishStatus int = CASE WHEN @Offset IN (11, 1) THEN 3 ELSE 1 END;
    DECLARE @WalkStatus int = CASE WHEN @Offset IN (13, 7, 6) THEN 2 WHEN @Offset = 5 THEN 3 ELSE 1 END;
    DECLARE @SleepStatus int = CASE WHEN @Offset IN (10, 9, 4) THEN 3 WHEN @Offset = 8 THEN 2 ELSE 1 END;

    INSERT INTO HabitLogs (HabitId, UserId, Date, Status, CompletedAt, Mood, EnergyLevel, DifficultyFelt, Notes, CreatedAt)
    VALUES
    (@HabitStudy, @UserId, @LogDate, @StudyStatus, CASE WHEN @StudyStatus = 1 THEN DATEADD(minute, 30, DATEADD(hour, 20, CONVERT(datetime2, @LogDate))) ELSE NULL END, 4, 4, CASE WHEN @StudyStatus = 1 THEN 2 ELSE 4 END, CASE WHEN @StudyStatus = 1 THEN N'Estudo feito no projeto MVC.' ELSE N'Ficou pesado para o horario.' END, @Now),
    (@HabitCommit, @UserId, @LogDate, @CommitStatus, CASE WHEN @CommitStatus = 1 THEN DATEADD(minute, 15, DATEADD(hour, 21, CONVERT(datetime2, @LogDate))) ELSE NULL END, 4, 3, CASE WHEN @CommitStatus = 1 THEN 3 ELSE 4 END, CASE WHEN @CommitStatus = 1 THEN N'Pequeno commit ou ajuste registrado.' ELSE N'Nao consegui fechar uma alteracao pequena.' END, @Now),
    (@HabitEnglish, @UserId, @LogDate, @EnglishStatus, CASE WHEN @EnglishStatus = 1 THEN DATEADD(minute, 30, DATEADD(hour, 8, CONVERT(datetime2, @LogDate))) ELSE NULL END, 4, 4, CASE WHEN @EnglishStatus = 1 THEN 1 ELSE 3 END, CASE WHEN @EnglishStatus = 1 THEN N'Revisao rapida concluida.' ELSE N'Pulei a revisao pela manha.' END, @Now),
    (@HabitWalk, @UserId, @LogDate, @WalkStatus, CASE WHEN @WalkStatus = 1 THEN DATEADD(minute, 45, DATEADD(hour, 18, CONVERT(datetime2, @LogDate))) ELSE NULL END, 4, 4, CASE WHEN @WalkStatus = 1 THEN 2 ELSE 4 END, CASE WHEN @WalkStatus = 1 THEN N'Caminhada leve feita.' ELSE N'Rotina de fim de tarde apertou.' END, @Now),
    (@HabitSleep, @UserId, @LogDate, @SleepStatus, CASE WHEN @SleepStatus = 1 THEN DATEADD(minute, 45, DATEADD(hour, 22, CONVERT(datetime2, @LogDate))) ELSE NULL END, 3, 3, CASE WHEN @SleepStatus = 1 THEN 2 ELSE 5 END, CASE WHEN @SleepStatus = 1 THEN N'Celular longe da cama.' ELSE N'Dormi tarde por tela demais.' END, @Now);

    SET @Offset -= 1;
END;

DECLARE @CheckOffset int = 9;
WHILE @CheckOffset >= 0
BEGIN
    DECLARE @CheckDate date = DATEADD(day, -@CheckOffset, @Today);

    INSERT INTO DailyCheckIns (
        UserId,
        Date,
        Mood,
        EnergyLevel,
        ProductivityLevel,
        DayScore,
        SmallWin,
        MainDifficulty,
        TomorrowAdjustment,
        Notes,
        CreatedAt,
        UpdatedAt
    )
    VALUES (
        @UserId,
        @CheckDate,
        CASE WHEN @CheckOffset IN (8, 4) THEN 3 WHEN @CheckOffset = 1 THEN 5 ELSE 4 END,
        CASE WHEN @CheckOffset IN (8, 4) THEN 3 ELSE 4 END,
        CASE WHEN @CheckOffset IN (7, 3) THEN 3 ELSE 4 END,
        CASE WHEN @CheckOffset IN (8, 4) THEN 3 WHEN @CheckOffset = 1 THEN 5 ELSE 4 END,
        CASE WHEN @CheckOffset = 0 THEN N'Mantive a versao de 2 minutos mesmo com pouco tempo.' ELSE N'Cumpri pelo menos um habito pequeno.' END,
        CASE WHEN @CheckOffset IN (8, 4) THEN N'Cansaco no fim do dia.' ELSE N'Manter foco depois do trabalho.' END,
        CASE WHEN @CheckOffset IN (8, 4) THEN N'Reduzir o tamanho do estudo noturno.' ELSE N'Deixar o ambiente pronto antes do gatilho.' END,
        N'Check-in demo para visualizar humor, energia, produtividade e ajustes diarios.',
        @Now,
        @Now
    );

    SET @CheckOffset -= 1;
END;

INSERT INTO Notes (
    UserId,
    Title,
    Content,
    NoteType,
    Tags,
    GoalId,
    IdentityId,
    HabitId,
    Date,
    CreatedAt,
    UpdatedAt
)
VALUES
(
    @UserId,
    N'Primeiro insight do projeto',
    N'Quando eu abro o projeto antes de estudar, a friccao cai bastante. A versao de 2 minutos esta funcionando como porta de entrada.',
    2,
    N'tecnologia, foco, aspnet',
    @GoalMvc,
    @IdentityDotNet,
    @HabitStudy,
    DATEADD(day, -7, @Today),
    @Now,
    @Now
),
(
    @UserId,
    N'Dificuldade com horario noturno',
    N'Estudar depois de um dia muito cheio fica instavel. Talvez antecipar o bloco tecnico ou reduzir para revisao de codigo.',
    4,
    N'rotina, ajuste, estudo',
    @GoalMvc,
    @IdentityDotNet,
    @HabitStudy,
    DATEADD(day, -5, @Today),
    @Now,
    @Now
),
(
    @UserId,
    N'Vitoria pequena',
    N'Mesmo sem muito tempo, revisar 3 palavras em ingles manteve a identidade ativa.',
    5,
    N'ingles, consistencia',
    @GoalEnglish,
    @IdentityLanguage,
    @HabitEnglish,
    DATEADD(day, -2, @Today),
    @Now,
    @Now
),
(
    @UserId,
    N'Ajuste de sistema',
    N'Deixar tenis visivel perto da porta aumentou a chance da caminhada acontecer no fim do expediente.',
    6,
    N'saude, ambiente, energia',
    @GoalEnergy,
    @IdentityHealth,
    @HabitWalk,
    @Today,
    @Now,
    @Now
);

COMMIT TRANSACTION;

SELECT
    N'Demo seed aplicado com sucesso.' AS Message,
    @Email AS LoginEmail,
    N'Demo@123' AS LoginPassword,
    @UserId AS UserId;
