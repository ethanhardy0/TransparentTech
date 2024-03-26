CREATE PROCEDURE UpdateHashedPassword
    @Password NVARCHAR(255),
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE HashedCredentials
    SET SysPassword = @Password
    WHERE UserID = @UserID;
END;