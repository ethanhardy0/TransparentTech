CREATE PROCEDURE InsertUser
    @Username NVARCHAR(50),
    @Email NVARCHAR(100),
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Phone NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SysUser (Username, Email, FirstName, LastName, Phone)
    VALUES (@Username, @Email, @FirstName, @LastName, @Phone);
END;