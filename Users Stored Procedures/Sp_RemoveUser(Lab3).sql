CREATE PROCEDURE RemoveUser
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SysUser
    SET UserStatus = 'Deleted'
    WHERE UserID = @UserID;
END;