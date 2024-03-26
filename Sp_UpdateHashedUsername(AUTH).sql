CREATE PROCEDURE UpdateHashedUsername
    @Username NVARCHAR(50),
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE HashedCredentials
    SET SysUsername = @Username
    WHERE UserID = @UserID;
END;