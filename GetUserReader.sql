CREATE PROCEDURE GetUserReader
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM SysUser
    WHERE Email IS NOT NULL
    AND (UserStatus != 'Deleted' OR UserStatus IS NULL);
END;