/*
    Remove apenas os dados do usuario demo criado em 001_seed_demo_mvp.sql.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @UserId nvarchar(450) = N'demo-user-1better';

DELETE FROM HabitLogs WHERE UserId = @UserId;
DELETE FROM Notes WHERE UserId = @UserId;
DELETE FROM DailyCheckIns WHERE UserId = @UserId;
DELETE FROM Habits WHERE UserId = @UserId;
DELETE FROM Goals WHERE UserId = @UserId;
DELETE FROM Identities WHERE UserId = @UserId;
DELETE FROM AspNetUserTokens WHERE UserId = @UserId;
DELETE FROM AspNetUserLogins WHERE UserId = @UserId;
DELETE FROM AspNetUserRoles WHERE UserId = @UserId;
DELETE FROM AspNetUserClaims WHERE UserId = @UserId;
DELETE FROM AspNetUsers WHERE Id = @UserId;

COMMIT TRANSACTION;

SELECT N'Dados demo removidos.' AS Message;
